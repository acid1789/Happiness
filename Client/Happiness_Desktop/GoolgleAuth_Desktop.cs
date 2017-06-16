using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Plus.v1;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Services;

namespace Happiness_Desktop
{
    public class GoolgleAuth_Desktop : Happiness.GoogleAuth
    {
        string m_szSecrets;
        string m_szEmail;
        string m_szId;

        public GoolgleAuth_Desktop()
        {
            _instance = this;
        }

        public override string[] DoAuth()
        {
            Console.WriteLine("Google Auth");
            m_szEmail = null;
            try
            {
                Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
            return new string[] { m_szEmail, m_szId };
        }

        async Task Run()
        {
#if DEBUG
            m_szSecrets = Environment.GetEnvironmentVariable("HAPPINESS_GOOGLE_SECRET");
#endif
            GoogleClientSecrets secrets = GoogleClientSecrets.Load(m_szSecrets.ToStream());
            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets.Secrets, new[] { PlusService.Scope.UserinfoEmail }, "user", CancellationToken.None);

            PlusService service = new PlusService(new BaseClientService.Initializer() { HttpClientInitializer = credential, ApplicationName = "Happiness" });
            Person p = await service.People.Get("me").ExecuteAsync();
            m_szEmail = p.Emails.FirstOrDefault().Value;
            m_szId = p.Id;
            Console.WriteLine("Email: " + m_szEmail + " ID: " + p.Id);
        }
    }

    static class String
    {
        public static Stream ToStream(this string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
