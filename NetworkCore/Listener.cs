using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sockets.Plugin;

namespace NetworkCore
{
    public class Listener
    {
        TcpSocketListener _listener;

        public event Action<Socket> OnNewConnection;

        public Listener()
        {
            _listener = new TcpSocketListener(10);
            _listener.ConnectionReceived += _listener_ConnectionReceived;
        }

        public void Listen(int port)
        {
            _listener.StartListeningAsync(port);
        }

        private void _listener_ConnectionReceived(object sender, Sockets.Plugin.Abstractions.TcpSocketListenerConnectEventArgs e)
        {
            TcpSocketClient client = (TcpSocketClient)e.SocketClient;
            Socket ns = new Socket(client);
            OnNewConnection?.Invoke(ns);
        }
    }
}
