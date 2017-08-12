using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Auth;

namespace Happiness_Android
{
    class FacebookAuth_Android : Happiness.FacebookAuth
    {
        string m_szEmail;
        string m_szId;

        public FacebookAuth_Android()
        {
            _instance = this;

            //ButtonFacebook_Clicked(null, null);
        }

        public override string[] DoAuth()
        {
            m_szEmail = null;
            try
            {
                ShowWebView().Wait();                
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

        private Task<bool> ShowWebView()
        {
            var tcs = new TaskCompletionSource<bool>();

            var auth = new OAuth2Authenticator(
                clientId: "286939585084672",
                scope: "email",
                authorizeUrl: new Uri("http://www.google.com"), // new Uri("https://www.facebook.com/v2.9/dialog/oauth"),
                redirectUrl: new Uri("fb286939585084672://authorize"),
                getUsernameAsync: null,
                isUsingNativeUI: true)
            { AllowCancel = true };
            
            auth.Completed += (sender, eventArgs) =>
            {
                if (eventArgs.IsAuthenticated)
                {                    
                    //authenticationResponseValues = eventArgs.Account.Properties;                    
                    tcs.SetResult(true);
                }

            };

            auth.Error +=
                (s, ea) =>
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Error = ").AppendLine($"{ea.Message}");

                    Console.WriteLine("Authentication Error: " + sb.ToString());
                    return;
                };

            var intent = auth.GetUI(Application.Context);
            intent.SetFlags(ActivityFlags.NewTask);

            Application.Context.StartActivity(intent);

            return tcs.Task;
        }

        #region sample
        string fb_app_id = "1889013594699403";
        OAuth2Authenticator authenticator;
        bool native_ui = true;

        protected void ButtonFacebook_Clicked(object sender, EventArgs e)
        {
            authenticator
                 = new Xamarin.Auth.OAuth2Authenticator
                 (
                     clientId:
                         new Func<string>
                            (
                                () =>
                                {
                                    string retval_client_id = "oops something is wrong!";

                                    retval_client_id = fb_app_id;
                                    return retval_client_id;
                                }
                            ).Invoke(),
                     //clientSecret: null,   // null or ""
                     authorizeUrl:
                         new Func<Uri>
                            (
                                () =>
                                {
                                    string uri = null;
                                    if (native_ui)
                                    {
                                        uri = "https://www.facebook.com/v2.9/dialog/oauth";
                                    }
                                    else
                                    {
                                        // old
                                        uri = "https://m.facebook.com/dialog/oauth/";
                                    }
                                    return new Uri(uri);
                                }
                            ).Invoke(),
                     //accessTokenUrl: new Uri("https://www.googleapis.com/oauth2/v4/token"),
                     redirectUrl:
                         new Func<Uri>
                            (
                                () =>
                                {
                                    string uri = null;
                                    if (native_ui)
                                    {
                                        uri =
                                            //"fb1889013594699403://localhost/path"
                                            //"fb1889013594699403://xamarin.com"
                                            $"fb{fb_app_id}://authorize"
                                            ;
                                    }
                                    else
                                    {
                                        uri =
                                            //"https://localhost/path"
                                            $"fb{fb_app_id}://authorize"
                                            ;
                                    }
                                    return new Uri(uri);
                                }
                            ).Invoke(),
                     scope: "", // "basic", "email",
                     getUsernameAsync: null,
                     isUsingNativeUI: native_ui
                 )
                 {
                     AllowCancel = true,
                 };

            authenticator.Completed +=
                (s, ea) =>
                {
                    StringBuilder sb = new StringBuilder();

                    if (ea.Account != null && ea.Account.Properties != null)
                    {
                        sb.Append("Token = ").AppendLine($"{ea.Account.Properties["access_token"]}");
                    }
                    else
                    {
                        sb.Append("Not authenticated ").AppendLine($"Account.Properties does not exist");
                    }

                    Console.WriteLine("Authentication Results: " + sb.ToString());

                    return;
                };

            authenticator.Error +=
                (s, ea) =>
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Error = ").AppendLine($"{ea.Message}");

                    Console.WriteLine("Authentication Error: " + sb.ToString());
                    return;
                };

            // after initialization (creation and event subscribing) exposing local object 
            var intent = authenticator.GetUI(Application.Context);
            intent.SetFlags(ActivityFlags.NewTask);

            Application.Context.StartActivity(intent);

            return;
        }

        #endregion
    }
}