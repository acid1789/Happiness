using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using NetworkCore;

namespace ServerCore
{
    public abstract class GameServer : ServerBase
    {
        GlobalServerManager _gs;
        AuthStringManager _authManager;

        public GameServer(int listenPort, string dbConnectionString, string globalAddress, int globalPort)
            : base(listenPort, dbConnectionString)
        {
            _authManager = new AuthStringManager();

            // Start the global server manager
            _gs = new GlobalServerManager(globalAddress, globalPort);
            _gs.OnAccountInfoResponse += _gs_OnAccountInfoResponse;
            _gs.OnCurrencyUpdate += _gs_OnCurrencyUpdate;
            _gs.Start();

            ListenThread.OnConnectionAccepted += new EventHandler<SocketArg>(lt_OnConnectionAccepted);            
        }

        public abstract GameClient CreateClient(Socket s);
    
        void lt_OnConnectionAccepted(object sender, SocketArg e)
        {
            GameClient client = CreateClient(e.Socket);

            client.OnAccountRequest += new EventHandler<AccountRequestArgs>(client_OnCredentialsRequest);
            client.OnChatMessage += new EventHandler<ChatMessageArgs>(client_OnChatMessage);

            InputThread.AddConnection(client);
        }

        public virtual void NewAuthorizedClient(GameClient client)
        {
            // Get the chat info
            //FetchChatInfo(client);

            _authManager.RegisterAuthString(client.AuthString, client.AccountId, client.HardCurrency, client.Vip, client.DisplayName);
        }

        #region Server Tasks
        public void FetchChatInfo(GameClient client)
        {
            GSTask t = new GSTask();
            t.Type = (int)GSTask.GSTType.ChatBlockList_Fetch;
            t.Client = client;
            TaskProcessor.AddTask(t);
        }
        #endregion

        #region Client Event Handlers
        void client_OnCredentialsRequest(object sender, AccountRequestArgs e)
        {
            GSTask t = new GSTask();
            t.Type = (int)GSTask.GSTType.CredentialsRequest;
            t.Client = (GameClient)sender;
            t.Args = e;

            TaskProcessor.AddTask(t);
        }

        void client_OnChatMessage(object sender, ChatMessageArgs e)
        {
            GSTask t = new GSTask();
            t.Type = (int)GSTask.GSTType.ChatMessage;
            t.Client = (GameClient)sender;
            t.Args = e;

            TaskProcessor.AddTask(t);
        }
        #endregion

        #region Global Server Event Handlers
        void _gs_OnAccountInfoResponse(AccountInfoResponseArgs e)
        {
            GSTask t = new GSTask();
            t.Type = (int)GSTask.GSTType.AccountInfoResponse;
            t.Client = null;
            t.Args = e;

            TaskProcessor.AddTask(t);
        }

        private void _gs_OnCurrencyUpdate(object sender, CurrencyUpdateArgs e)
        {
            GSTask t = new GSTask();
            t.Type = (int)GSTask.GSTType.CurrencyUpdate;
            t.Client = null;
            t.Args = e;

            TaskProcessor.AddTask(t);
        }
        #endregion

        #region Accessors
        public GlobalServerManager GlobalServer
        {
            get { return _gs; }
        }

        public AuthStringManager AuthManager
        {
            get { return _authManager; }
        }
        #endregion
    }
}
