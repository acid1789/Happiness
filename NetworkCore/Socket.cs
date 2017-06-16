using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sockets.Plugin;

namespace NetworkCore
{
    public class Socket
    {
        TcpSocketClient _client;
        bool _connected;

        public Socket()
        {
            _connected = false;
        }

        public bool Connect(string address, int port)
        {
            if (_connected)
                Close();

            _client = new TcpSocketClient();
            _connected = false;
            _client.ConnectAsync(address, port).Wait();
            _client.ReadStream.ReadTimeout = 10;
            _connected = true;
            return _connected;
        }

        public void Close()
        {
            if (_client != null)
                _client.DisconnectAsync().Wait();
            _client = null;
            _connected = false;
        }

        public void Send(byte[] buffer)
        {
            _client.WriteStream.Write(buffer, 0, buffer.Length);
            _client.WriteStream.Flush();
        }

        public int Receive(byte[] buffer)
        {
            int bytesRead = 0;
            try
            {
                bytesRead = _client.ReadStream.Read(buffer, 0, buffer.Length);
            }
            catch (Exception) { }
            return bytesRead;
        }

        public bool Connected { get { return _connected; } }
        public string RemoteEndPoint { get { return _client.RemoteAddress + ":" + _client.RemotePort; } }
    }
}
