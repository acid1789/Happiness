using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkCore;
using ServerCore;

namespace HappinessServer
{
    class Program
    {
        static HappinessServer _server;

        static void Main(string[] args)
        {
            ServerArgs sargs = new ServerArgs(args);

            LogThread.AlwaysPrintToConsole = true;
            _server = new HappinessServer(sargs.ListenPort, sargs.DBString, sargs.HostAddress, sargs.HostPort);

            _server.Run();

            LogThread.GetLog().Shutdown();
        }
    }
}
