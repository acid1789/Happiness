using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetworkCore;

namespace ServerCore
{
    public class GSTask : Task
    {
        public enum GSTType
        {
            CredentialsRequest,
            AccountInfoResponse,
            ChatBlockList_Fetch,
            ChatBlockList_Process,
            ChatInfo_Process,
            ChatMessage,
            FlagAccount,
            SpendCoins_Global,
            CurrencyUpdate,

            Last
        }

        public GSTask() { }
        public GSTask(GSTType type) : base((int)type)
        {
        }
        
        public GameClient Client
        {
            get { return (GameClient)_client; }
            set { _client = value; }
        }
    }

    public class GSTaskProcessor : TaskProcessor
    {
        protected GameServer _server;

        public GSTaskProcessor(GameServer server)
            : base()
        {
            _server = server;

            _taskHandlers[(int)GSTask.GSTType.CredentialsRequest] = CredentialsRequestHandler;
            _taskHandlers[(int)GSTask.GSTType.AccountInfoResponse] = AccountInfoResponseHandler;
            _taskHandlers[(int)GSTask.GSTType.ChatBlockList_Fetch] = ChatBlockListFetchHandler;
            _taskHandlers[(int)GSTask.GSTType.ChatBlockList_Process] = ChatBlockListProcessHandler;
            _taskHandlers[(int)GSTask.GSTType.ChatInfo_Process] = ChatInfoProcessHandler;
            _taskHandlers[(int)GSTask.GSTType.ChatMessage] = ChatMessageHandler;
            _taskHandlers[(int)GSTask.GSTType.SpendCoins_Global] = SpendCoinsGlobalHandler;
            _taskHandlers[(int)GSTask.GSTType.CurrencyUpdate] = CurrencyUpdateHandler;
        }

        #region Task Handlers
        void CredentialsRequestHandler(Task t)
        {
            GSTask task = (GSTask)t;
            // Forward credentials request onto the global server
            AccountRequestArgs args = (AccountRequestArgs)task.Args;
            _server.GlobalServer.RequestAccountInfo(task.Client.SessionKey, args.Email, args.Password, args.DisplayName, args.OAuthMode);
        }

        void AccountInfoResponseHandler(Task t)
        {
            GSTask task = (GSTask)t;
            AccountInfoResponseArgs args = (AccountInfoResponseArgs)task.Args;
            GameClient client = (GameClient)_server.InputThread.FindClient(args.ClientKey);
            if (args.AccountId < 0)
            {
                // Account doesnt exist
                client.SendAccountResponse(-1, null);
            }
            else if (args.DisplayName == null || args.DisplayName.Length <= 0)
            {
                // Account exists but password is wrong
                client.SendAccountResponse(args.AccountId, null);
            }
            else if (client.PendingAuthTask != null)
            {
                // Cache this auth string for later
                _server.AuthManager.RegisterAuthString(args.AuthString, args.AccountId, args.HardCurrency, args.Vip, args.DisplayName);

                // Add the task to process this
                GSTask authTask = (GSTask)client.PendingAuthTask;
                
                AddTask(authTask);
            }
            else
            {                
                // Store stuff
                client.AccountId = args.AccountId;
                client.HardCurrency = args.HardCurrency;
                client.Vip = args.Vip;
                client.DisplayName = args.DisplayName;
                client.AuthString = args.AuthString;

                // Let the server do new client stuff
                _server.NewAuthorizedClient(client);
            }
        }

        void ChatBlockListFetchHandler(Task t)
        {
            GSTask task = (GSTask)t;

            // Fetch the chat block list from the database
            task.Type = (int)GSTask.GSTType.ChatBlockList_Process;
            DBQuery query = AddDBQuery(string.Format("SELECT * FROM chat_blocks WHERE blocker={0};", task.Client.AccountId), task);
        }

        void ChatBlockListProcessHandler(Task t)
        {
            GSTask task = (GSTask)t;

            // Add all blocked users to the block list
            task.Client.BlockList.Clear();
            DBQuery q = task.Query;
            if (q != null && q.Rows != null && q.Rows.Count > 0)
            {
                foreach (object[] row in q.Rows)
                {
                    uint blocked = (uint)row[2];
                    task.Client.BlockList.Add(blocked);
                }
            }

            // Fetch the chat channel info
            task.Type = (int)GSTask.GSTType.ChatInfo_Process;
            DBQuery query = AddDBQuery(string.Format("SELECT * FROM chat_info WHERE account_id={0};", task.Client.AccountId), task);
        }

        void ChatInfoProcessHandler(Task t)
        {
            GSTask task = (GSTask)t;

            DBQuery q = task.Query;
            if (q != null && q.Rows != null && q.Rows.Count > 0) 
            {
                object[] row = q.Rows[0];
                task.Client.ChatChannels = (uint)row[1];
            }
            else 
            {
                // No channels for this user, add them to the global channel by default
                task.Type = -1;     // Set this to less than zero so no task happens after the database query finishes
                task.Client.ChatChannels = 1;
                DBQuery query = AddDBQuery(string.Format("INSERT INTO chat_info SET account_id={0},channels={1};", task.Client.AccountId, task.Client.ChatChannels), task);
            }
            task.Client.SendChatChannels();
                       
        }

        void ChatMessageHandler(Task t)
        {
            GSTask task = (GSTask)t;

            ChatMessageArgs args = (ChatMessageArgs)task.Args;

            // Fetch client list
            Connection[] clients = _server.InputThread.Clients;

            // Send message to clients
            foreach( GameClient gc in clients )
            {
                if( gc.IsInChannel(args.Channel) )
                {
                    if( !gc.IsChatBlocked(task.Client.AccountId) )
                    {
                        gc.SendChat(args.Channel, args.Message, task.Client.DisplayName);
                    }
                }
            }
        }

        void SpendCoinsGlobalHandler(Task t)
        {
            GSTask task = (GSTask)t;

            int amount = (int)task.Args;
            ulong serverSpendID = (ulong)task.Query.Rows[0][0];

            _server.GlobalServer.SpendCoins(task.Client.AccountId, amount, serverSpendID);
        }

        void CurrencyUpdateHandler(Task t)
        {
            CurrencyUpdateArgs args = (CurrencyUpdateArgs)t.Args;

            _server.AuthManager.UpdateAccount(args.AccountId, args.NewCurrency, args.NewVIP);

            // Find the client
            GameClient client = _server.InputThread.FindClientByID(args.AccountId);
            if (client != null)
            {
                // Tell the client
                client.CurrencyUpdate(args.NewCurrency, args.NewVIP);
            }
        }
        #endregion

    }
}
