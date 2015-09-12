using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NetworkCore;

namespace Happiness
{
    class NetworkLog : LogInterface
    {
        public NetworkLog() : base()
        {
        }

        public override void Log(LogMessageType type, bool logToConsole, string message)
        {
            Debug.WriteLine(message);
        }
    }
}
