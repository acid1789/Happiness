using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ServerCore;
using NetworkCore;

namespace GlobalServer
{
    class GlobalServer
    {
        static ServerBase _server;
        static WebServer _webServer;
        
        static void Main(string[] args)
        {
            ServerArgs sargs = new ServerArgs(args);
            
            _server = new ServerBase(sargs.ListenPort, sargs.DBString);
            _server.TaskProcessor = new GlobalTaskProcessor();
            _server.DatabaseSetup();
#if DEBUG
            LogThread.AlwaysPrintToConsole = true;
#endif
            _webServer = new WebServer(null, "http://localhost:8080/purchase/", "http://localhost:8080/checkout/");
            _webServer.Run();

            Marketplace m = new Marketplace(_server);
            _server.TaskProcessor.AddTask(new GlobalTask(GlobalTask.GlobalType.Products_Fetch));

            _server.ListenThread.OnConnectionAccepted += new EventHandler<SocketArg>(lt_OnConnectionAccepted);

            _server.Run();
        }
        
        static void lt_OnConnectionAccepted(object sender, SocketArg e)
        {
            GlobalClient gclient = new GlobalClient(e.Socket);

            gclient.OnAccountInfoRequest += new EventHandler<AccountInfoRequestArgs>(gclient_OnAccountInfoRequest);
            gclient.OnSpendCoins += Gclient_OnSpendCoins;
            gclient.OnAuthStringRequest += Gclient_OnAuthStringRequest;

            _server.InputThread.AddConnection(gclient);

            // Send the products to the new client
            if(Marketplace.Instance.Products != null )
                gclient.SendProducts(Marketplace.Instance.Products);
        }

        static void gclient_OnAccountInfoRequest(object sender, AccountInfoRequestArgs e)
        {
            GlobalTask gst = new GlobalTask();
            gst.Type = (int)GlobalTask.GlobalType.AccountInfoRequest;
            gst.Client = (GlobalClient)sender;
            gst.Args = e;

            _server.TaskProcessor.AddTask(gst);
        }

        private static void Gclient_OnSpendCoins(object sender, GlobalSpendCoinArgs e)
        {
            GlobalTask gst = new GlobalTask();
            gst.Type = (int)GlobalTask.GlobalType.SpendCoins;
            gst.Client = (GlobalClient)sender;
            gst.Args = e;

            _server.TaskProcessor.AddTask(gst);
        }

        private static void Gclient_OnAuthStringRequest(GlobalClient arg1, string arg2, uint clientKey)
        {
            GlobalTask gt = new GlobalTask();
            gt.Type = (int)GlobalTask.GlobalType.AuthStringRequest;
            gt.Client = arg1;
            gt.Args = new object[] { arg2, clientKey };
            _server.TaskProcessor.AddTask(gt);
        }

#region Accessors
        public static DatabaseThread Database
        {
            get { return _server.Database; }
        }

        public static ServerBase Server
        {
            get { return _server; }
        }
#endregion
    }
}
