using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Happiness_Desktop
{
    public class FacebookAuth_Desktop : Happiness.FacebookAuth
    {
        static string s_szSecrets;
        static string s_szEmail;
        static string s_szId;

        public FacebookAuth_Desktop()
        {
            _instance = this;
        }

        public override void BeginAuth()
        {
            throw new NotImplementedException();
        }

        public override string[] FinishAuth()
        {
            throw new NotImplementedException();
        }

        /*
        public override string[] DoAuth()
        {
            s_szEmail = null;
            s_szId = null;

#if DEBUG
            s_szSecrets = Environment.GetEnvironmentVariable("HAPPINESS_FACEBOOK_APPID");
#endif

            Process process = new Process();
            process.StartInfo.FileName = "FacebookAuthenticator.exe";
            process.StartInfo.Arguments = s_szSecrets;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            return new string[] { s_szEmail, s_szId };
        }

        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine(outLine.Data);
            if (outLine.Data != null && outLine.Data.StartsWith("Credentials="))
            {
                string result = outLine.Data.Substring(12);
                string[] parts = result.Split(new string[] { "$?$" }, StringSplitOptions.None);
                if (parts.Length == 3)
                {
                    s_szId = parts[1];
                    s_szEmail = parts[2];
                }
            }
        }*/
    }
}
