using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using ServerCore;
using System.Security.Cryptography;

namespace GlobalServer
{
    class GlobalTask : Task
    {
        public enum GlobalType
        {
            AccountInfoRequest,
            AccountInfoProcess,
            SpendCoins,
            SpendCoins_Process,
            AuthStringRequest,
            AuthStringProcess,
        }

        public GlobalClient Client
        {
            get { return (GlobalClient)_client; }
            set { _client = value; }
        }
    }

    class PendingQuery
    {
        public enum QueryType
        {
            AccountInfo
        }

        QueryType _type;
        GSTask _task;

        public PendingQuery(QueryType type, GSTask task)
        {
            _type = type;
            _task = task;
        }

        public QueryType Type
        {
            get { return _type; }
        }

        public GSTask Task
        {
            get { return _task; }
        }
    }

    class GlobalTaskProcessor : TaskProcessor
    {
        public GlobalTaskProcessor()
            : base()
        {
            _taskHandlers[(int)GlobalTask.GlobalType.AccountInfoRequest] = AccountInfoRequestHandler;
            _taskHandlers[(int)GlobalTask.GlobalType.AccountInfoProcess] = AccountInfoProcessHandler;
            _taskHandlers[(int)GlobalTask.GlobalType.SpendCoins] = SpendCoinsHandler;
            _taskHandlers[(int)GlobalTask.GlobalType.SpendCoins_Process] = SpendCoins_ProcessHandler;
            _taskHandlers[(int)GlobalTask.GlobalType.AuthStringRequest] = AuthStringRequestHandler;
            _taskHandlers[(int)GlobalTask.GlobalType.AuthStringProcess] = AuthStringProcessHandler;
        }

        bool ValidPassword(string pwIn, int oAuthMode, string pwDB, string googleId, string facebookId)
        {
            switch (oAuthMode)
            {
                case 1: // Google
                    return pwIn == googleId;
                case 2: // Facebook
                    return pwIn == facebookId;
            }
            return pwIn == pwDB;
        }

        #region Task Handlers
        void AccountInfoRequestHandler(Task t)
        {
            GlobalTask task = (GlobalTask)t;
            // Fetch account from the database
            AccountInfoRequestArgs args = (AccountInfoRequestArgs)task.Args;
            DBQuery q = AddDBQuery(string.Format("SELECT * FROM accounts WHERE email=\"{0}\";", args.Email), task);
            
            task.Type = (int)GlobalTask.GlobalType.AccountInfoProcess;
        }

        void AccountInfoProcessHandler(Task t)
        {
            GlobalTask task = (GlobalTask)t;
            int accountId = -1;
            string displayName = "";
            int hardCurrency = 0;
            string authString = "";
            int vip = 0;
            
            AccountInfoRequestArgs args = (AccountInfoRequestArgs)task.Args;
            bool sendAccountInfo = true;

            if (task.Query.Rows.Count > 0)
            {
                // 0: account_id
                // 1: email
                // 2: password
                // 3: display name
                // 4: hard_currency
                // 5: auth_string
                // 6: vip
                // 7: google_id
                // 8: facebook_id

                // Found the account, check the password
                object[] row = task.Query.Rows[0];
                accountId = (int)row[0];
                string pw = row[2].ToString();
                string google_id = (row[7] is DBNull) ? null : row[7].ToString();
                string facebook_id = (row[8] is DBNull) ? null : row[8].ToString();
                if( ValidPassword(args.Password, args.OAuthMode, pw, google_id, facebook_id) )
                {
                    // password match
                    displayName = row[3].ToString();
                    hardCurrency = (int)row[4];
                    vip = (int)row[6];

                    if (row[5] is DBNull)
                    {
                        // Auth string doesnt exist, generate it now
                        authString = GenerateAuthString((string)row[1], pw, displayName, accountId);

                        // Store it in the database
                        DBQuery q = AddDBQuery(string.Format("UPDATE accounts SET auth_string=\"{0}\" WHERE account_id={1};", authString, accountId), null);
                    }
                    else
                        authString = (string)row[5];
                }
                else
                {
                    // password mismatch - displayName stays empty but accountId is filled in
                }
            }
            else
            {
                // Account does not exist
                if (args.DisplayName != null)
                {
                    // This is actually a request to create the account.
                    sendAccountInfo = false;

                    task.Type = (int)GlobalTask.GlobalType.AccountInfoRequest;
                    switch (args.OAuthMode)
                    {
                        default:
                            AddDBQuery(string.Format("INSERT INTO accounts SET email=\"{0}\",password=\"{1}\",display_name=\"{2}\",hard_currency={3},vip={4};", args.Email, args.Password, args.DisplayName, 0, 0), task);
                            break;
                        case 1: // Google
                            AddDBQuery(string.Format("INSERT INTO accounts SET email=\"{0}\",display_name=\"{1}\",hard_currency={2},vip={3},google_id={4};", args.Email, args.DisplayName, 0, 0,args.Password), task);
                            break;
                        case 2: // Facebook
                            AddDBQuery(string.Format("INSERT INTO accounts SET email=\"{0}\",display_name=\"{1}\",hard_currency={2},vip={3},facebook_id={4};", args.Email, args.DisplayName, 0, 0, args.Password), task);
                            break;
                    }
                }
            }
            if(sendAccountInfo )
                task.Client.SendAccountInfo(args.ClientKey, accountId, displayName, hardCurrency, vip, authString); 
        }

        void SpendCoinsHandler(Task t)
        {
            GlobalSpendCoinArgs args = (GlobalSpendCoinArgs)t.Args;
            string sql = string.Format("SELECT hard_currency FROM accounts WHERE account_id={0};", args.AccountId);
            t.Type = (int)GlobalTask.GlobalType.SpendCoins_Process;
            AddDBQuery(sql, t);
        }

        void SpendCoins_ProcessHandler(Task t)
        {
            GlobalTask task = (GlobalTask)t;
            GlobalSpendCoinArgs args = (GlobalSpendCoinArgs)t.Args;
            int currency = (int)t.Query.Rows[0][0];
            int before = currency;
            
            currency -= args.Amount;
            if (currency < 0)
            {
                // Spent more than they had!?
                currency = 0;
            }

            // Store transaction in the database
            string sql = string.Format("INSERT INTO transactions SET account_id={0}, amount={1}, before_t={2}, after_t={3}, server_record={4}, timestamp={5};", args.AccountId, -args.Amount, before, currency, args.ServerRecord, DateTime.Now.Ticks);
            AddDBQuery(sql, null, false);

            // Store currency in the database
            sql = string.Format("UPDATE accounts SET hard_currency={0} WHERE account_id={1};", currency, args.AccountId);
            AddDBQuery(sql, null, false);

            // Tell the client about it
            task.Client.HardCurrencyUpdate(args.AccountId, currency);
        }

        void AuthStringRequestHandler(Task t)
        {
            GlobalTask task = (GlobalTask)t;
            task.Type = (int)GlobalTask.GlobalType.AuthStringProcess;
            object[] args = (object[])task.Args;
            DBQuery q = AddDBQuery(string.Format("SELECT * FROM accounts WHERE auth_string=\"{0}\";", (string)args[0]), task);
        }

        void AuthStringProcessHandler(Task t)
        {
            GlobalTask task = (GlobalTask)t;

            int accountId = -1;
            string displayName = "";
            int hardCurrency = 0;
            object[] args = (object[])task.Args;
            string authString = (string)args[0];
            int vip = 0;

            if (task.Query.Rows.Count > 0)
            {
                if (task.Query.Rows.Count > 1)
                {
                    // This matched more than one record? log this error but just associate with the first one
                    string errorMessage = string.Format("AuthString {0} matches multiple account records!", authString);
                    NetworkCore.LogInterface.Log(errorMessage, NetworkCore.LogInterface.LogMessageType.Security, true);
                }

                object[] row = task.Query.Rows[0];
                accountId = (int)row[0];
                displayName = row[3].ToString();
                hardCurrency = (int)row[4];
                vip = (int)row[6];
            }
            else
            {
                // No accounts matching the given auth string!

                // Log the error
                string errorMessage = string.Format("Unknown auth string '{0}' recieved from: {1}", authString, task.Client.IPAddress);
                NetworkCore.LogInterface.Log(errorMessage, NetworkCore.LogInterface.LogMessageType.Security, true);

                // Deny access
                accountId = -1;
            }
            task.Client.SendAccountInfo((uint)args[1], accountId, displayName, hardCurrency, vip, authString);
        }
        #endregion

        string GenerateAuthString(string email, string password, string displayName, int accountId)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(email);
            bw.Write(password);
            bw.Write(displayName);
            bw.Write(accountId);

            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(ms.ToArray());
            bw.Close();
            
            string authString = Encoding.UTF8.GetString(hash);
            return authString;
        }
    }
}
