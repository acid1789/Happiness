using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NetworkCore;

namespace ServerCore
{
    public class GlobalServerManager
    {
        Thread _theThread;
        GlobalClient _gc;
        string _address;
        int _port;
        
        public event Action<AccountInfoResponseArgs> OnAccountInfoResponse;
        public event EventHandler<CurrencyUpdateArgs> OnCurrencyUpdate;
        public event Action<GlobalProduct[]> OnGlobalProductInfo;

        public GlobalServerManager(string address, int port)
        {
            _address = address;
            _port = port;
        }

        public void Start()
        {
            if (_theThread == null)
            {
                _theThread = new Thread(new ThreadStart(GlobalServerManagerThread));
                _theThread.Name = "Global Server Manager";
                _theThread.Start();
            }
        }

        void GlobalServerManagerThread()
        {
            while (true)
            {
                try
                {
                    if (_gc == null || !_gc.Connected)
                    {
                        _gc = new GlobalClient();
                        _gc.OnAccountInfoResponse += OnAccountInfoResponse;
                        _gc.OnCurrencyUpdate += OnCurrencyUpdate;
                        _gc.OnGlobalProductInfo += OnGlobalProductInfo;

                        _gc.Connect(_address, _port);
                        if (_gc.Connected)
                            LogThread.Log("Connected to global server ", LogThread.LogMessageType.Normal, true);
                    }
                    else
                    {
                        _gc.Update();
                    }
                }
                catch (Exception ex)
                {
                    LogThread.Log(ex.ToString(), LogThread.LogMessageType.Error, true);
                }

                Thread.Sleep(1000);
            }
        }

        public void RequestAccountInfo(uint clientKey, string email, string password, string displayName, int oauthMode)
        {
            _gc.RequestAccountInfo(clientKey, email, password, displayName, oauthMode);
        }

        public void FetchAuthString(string authString, uint clientKey)
        {
            _gc.RequestAccountInfoFromAuthString(authString, clientKey);
        }

        public void SpendCoins(int accountId, int amount, ulong gameServerSpendRecord)
        {
            _gc.SpendCoins(accountId, amount, gameServerSpendRecord);
        }
    }
}
