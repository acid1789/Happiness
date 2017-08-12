using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Plus.v1;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Services;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Gms.Common.Apis;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;
using Android.Gms.Common;

namespace Happiness_Android
{
    class GoogleAuthHandler : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        #region GoogleApiClient.IConnectionCallbacks
        public void OnConnected(Bundle connectionHint)
        {
        }

        public void OnConnectionSuspended(int cause)
        {
        }
        #endregion

        #region GoogleApiClient.IOnConnectionFailedListener
        public void OnConnectionFailed(ConnectionResult result)
        {
        }
        #endregion
    }

    class GoogleAuth_Android : Happiness.GoogleAuth
    {
        string m_szEmail;
        string m_szId;

        bool m_WaitingForAuth;

        GoogleApiClient m_GoogleApiClient;
        GoogleAuthHandler m_AuthHandler;
        
        public GoogleAuth_Android()
        {
            _instance = this;
        }

        public override void BeginAuth()
        {
            Activity1.Instance.RegisterActivityResultHandler(Activity1.RC_SIGN_IN, OnSignInResult);

            GoogleSignInOptions gso = new GoogleSignInOptions.Builder()
                .RequestEmail()
                .RequestIdToken("225487862431-rktk7eoinh6v8t9nu5f1pt5snp6rd0nq.apps.googleusercontent.com")
                .Build();

            m_AuthHandler = new GoogleAuthHandler();
            m_GoogleApiClient = new GoogleApiClient.Builder(Application.Context)
                .AddConnectionCallbacks(m_AuthHandler)
                .AddOnConnectionFailedListener(m_AuthHandler)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                .Build();

            m_GoogleApiClient.Connect();
            Intent signInIntent = Auth.GoogleSignInApi.GetSignInIntent(m_GoogleApiClient);
            Activity1.Instance.StartActivityForResult(signInIntent, Activity1.RC_SIGN_IN);
            m_WaitingForAuth = true;
        }

        public override string[] FinishAuth()
        {          
            return m_WaitingForAuth ? null : new string[] { m_szEmail, m_szId };
        }    

        void OnSignInResult(Result resultCode, Intent data)
        {
            Activity1.Instance.UnRegisterActivityResultHandler(Activity1.RC_SIGN_IN, OnSignInResult);
            if (resultCode == Result.Ok)
            {
                GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                GoogleSignInAccount acct = result.SignInAccount;
                ((GoogleAuth_Android)_instance).m_szEmail = acct.Email;
                ((GoogleAuth_Android)_instance).m_szId = acct.IdToken;                
            }
            m_WaitingForAuth = false;
        }
        
    }
   
}