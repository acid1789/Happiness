using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SecretsInjector
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == "set")
            {
                string google = Environment.GetEnvironmentVariable("HAPPINESS_GOOGLE_SECRET");
                string facebook = Environment.GetEnvironmentVariable("HAPPINESS_FACEBOOK_APPID");

                SetFile(args[1] + "/GoogleAuth.cs", google);
                SetFile(args[1] + "/FacebookAuth.cs", facebook);
            }
            else if( args[0] == "clear" )
            {
                ClearFile(args[1] + "/GoogleAuth.cs");
                ClearFile(args[1] + "/FacebookAuth.cs");
            }
        }

        static void SetFile(string file, string secret)
        {
            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("static string s_szSecrets"))
                {
                    lines[i] = "        static string s_szSecrets = \"" + secret + "\";";
                    break;
                }
            }
            File.WriteAllLines(file, lines);
        }

        static void ClearFile(string file)
        {
            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("static string s_szSecrets"))
                {
                    lines[i] = "        static string s_szSecrets;";
                    break;
                }
            }
            File.WriteAllLines(file, lines);
        }
    }
}
