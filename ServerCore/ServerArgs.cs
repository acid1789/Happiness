using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerCore
{
    public class ServerArgs
    {
        public int ListenPort;
        public string DBString;
        public string HostAddress;
        public int HostPort;

        public ServerArgs(string[] args)
        {
            foreach (string arg in args)
            {
                try
                {
                    int equals = arg.IndexOf('=');
                    string key = arg.Substring(0, equals);
                    string value = arg.Substring(equals + 1);

                    switch (key)
                    {
                        case "listen": ListenPort = int.Parse(value); break;
                        case "db": DBString = value; break;
                        case "host": HostAddress = value; break;
                        case "host_port": HostPort = int.Parse(value); break;
                        default: Console.WriteLine("Unknown argument: " + arg); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Invalid arg: {0}\n{1}", arg, ex.ToString());
                }
            }
        }
    }
}
