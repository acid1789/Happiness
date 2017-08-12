using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class SocketArg : EventArgs
    {
        public NetworkCore.Socket Socket;

        public SocketArg(NetworkCore.Socket s)
        {
            Socket = s;
        }
    }

    public class ListenThread
    {
        Thread _theThread;
        int _port;

        public event EventHandler<SocketArg> OnConnectionAccepted;

        public ListenThread(int port)
        {
            _port = port;
        }

        public void Destroy()
        {
            if (_theThread != null)
            {
                _theThread.Abort();
                _theThread = null;
            }
        }

        public void Start()
        {
            if (_theThread == null)
            {
                _theThread = new Thread(new ThreadStart(ListenThreadFunc));
                _theThread.Name = "Listen Thread";
                _theThread.Start();
            }
        }
        
        void ListenThreadFunc()
        {
            NetworkCore.Listener listener = new NetworkCore.Listener();
            listener.OnNewConnection += Listener_OnNewConnection;
            listener.Listen(_port);            
        }

        private void Listener_OnNewConnection(NetworkCore.Socket obj)
        {
            OnConnectionAccepted?.Invoke(this, new SocketArg(obj));
        }
    }
}
