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

        public void RegisterAuthString(string authString, int accountId, int hardCurrency, int vip)
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
            _accounts[authString] = aai;    
        }

        public AuthAccountInfo FindAccount(string authString)
        {
            if( _accounts.ContainsKey(authString) )
                return _accounts[authString];
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


        public class AuthAccountInfo
        {
            public int AccountID;
            public int HardCurrency;
            public int Vip;
            public long Timestamp;
        }
    }
}
