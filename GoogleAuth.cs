using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Plus.v1;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Services;

namespace Happiness
{
    class GoogleAuth
    {
        static string s_szEmail;
        static string s_szId;

        public static string[] DoAuth()
        {
            Console.WriteLine("Google Auth");
            s_szEmail = null;
            try
            {
                new GoogleAuth().Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
            return new string[] { s_szEmail, s_szId };
        }
        
        async Task Run()
        {
            string s_Secrets = Environment.GetEnvironmentVariable("HAPPINESS_GOOGLE_SECRET");
            GoogleClientSecrets secrets = GoogleClientSecrets.Load(s_Secrets.ToStream());            
            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets.Secrets, new[] { PlusService.Scope.UserinfoEmail }, "user", CancellationToken.None);

            PlusService service = new PlusService(new BaseClientService.Initializer() { HttpClientInitializer = credential, ApplicationName = "Happiness" });
            Person p = await service.People.Get("me").ExecuteAsync();
            s_szEmail = p.Emails.FirstOrDefault().Value;
            s_szId = p.Id;
            Console.WriteLine("Email: " + s_szEmail + " ID: " + p.Id);            
        }
    }
}
