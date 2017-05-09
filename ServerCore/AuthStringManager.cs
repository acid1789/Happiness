using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkCore;

namespace ServerCore
{
    public class AuthStringManager
    {
        const int MAX_CACHED_ACCOUNTS = 10000;
        Dictionary<string, AuthAccountInfo> _accounts;

        public AuthStringManager()
        {
            _accounts = new Dictionary<string, AuthAccountInfo>();
        }

        public void RegisterAuthString(string authString, int accountId, int hardCurrency, int vip, string displayName)
        {
            if (_accounts.Count > MAX_CACHED_ACCOUNTS)
            {
                int preDump = _accounts.Count;
                // Dump accounts that are a day old or more
                DumpAccounts(24 * 60 * 60);

                LogInterface.Log(string.Format("Dumping cached accounts: ({0}) -> ({1})", preDump, _accounts.Count), LogInterface.LogMessageType.System, true);
            }

            AuthAccountInfo aai = new AuthAccountInfo();
            aai.AccountID = accountId;
            aai.HardCurrency = hardCurrency;
            aai.Vip = vip;
            aai.Timestamp = DateTime.Now.Ticks;
            aai.DisplayName = displayName;
            _accounts[authString] = aai;    
        }

        public AuthAccountInfo FindAccount(string authString)
        {
            string ascii = AuthStringToAscii(authString);
            LogThread.Log("Checking auth cache for: " + ascii, NetworkCore.LogInterface.LogMessageType.Normal);
            if (_accounts.ContainsKey(authString))
            {
                AuthAccountInfo aai = _accounts[authString];
                aai.Timestamp = DateTime.Now.Ticks;
                LogThread.Log("Authstring found for account: " + aai.AccountID, NetworkCore.LogInterface.LogMessageType.Normal);
                return aai;
            }

            LogThread.Log(string.Format("Authstring {0} not found in the local cache", ascii), NetworkCore.LogInterface.LogMessageType.System);
            return null;
        }

        void DumpAccounts(double secondsOld)
        {
            List<string> toDump = new List<string>();
            foreach (KeyValuePair<string, AuthAccountInfo> kvp in _accounts)
            {
                DateTime stamp = new DateTime(kvp.Value.Timestamp);
                if( (DateTime.Now - stamp).TotalSeconds >= secondsOld )
                    toDump.Add(kvp.Key);
            }

            foreach( string key in toDump )
                _accounts.Remove(key);
        }


        public static string AuthStringToAscii(string authString)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(authString);
            string str = "";
            foreach( byte b in bytes )
                str += b.ToString("X2");
            return str;
        }

        public class AuthAccountInfo
        {
            public int AccountID;
            public int HardCurrency;
            public int Vip;
            public long Timestamp;
            public string DisplayName;
        }
    }
}
