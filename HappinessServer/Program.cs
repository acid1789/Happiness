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
            LogThread.AlwaysPrintToConsole = true;
            _server = new HappinessServer(1255, "server=127.0.0.1;uid=Happiness;pwd=hgame;database=happiness;", "127.0.0.1", 1789);

            _server.Run();

            LogThread.GetLog().Shutdown();
        }
    }
}
