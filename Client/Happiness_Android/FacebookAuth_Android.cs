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
using Org.Json;

using Xamarin.Facebook;
using Xamarin.Facebook.Login;


namespace Happiness_Android
{
    public class GraphCBHandler : Java.Lang.Object, GraphRequest.IGraphJSONObjectCallback
    {
        FacebookAuth_Android _fba;
        public GraphCBHandler(FacebookAuth_Android fba)
        {
            _fba = fba;
        }
        public void OnCompleted(JSONObject p0, GraphResponse p1)
        {
            try
            {
                _fba.OnFinished(true, p0.GetString("email"));
            }
            catch (JSONException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    public class FBCallbackHandler : Java.Lang.Object, IFacebookCallback
    {
        FacebookAuth_Android _fba;
        GraphCBHandler _graphCBHandler;
        public FBCallbackHandler(FacebookAuth_Android fba)
        {
            _fba = fba;
        }

        public void OnCancel()
        {
            _fba.OnFinished(false, null);
        }

        public void OnError(FacebookException p0)
        {
            _fba.OnFinished(false, p0.ToString());
        }

        public void OnSuccess(Java.Lang.Object p0)
        {
            // Facebook Email address            
            LoginResult loginResult = (LoginResult)p0;
            _graphCBHandler = new GraphCBHandler(_fba);
            _fba.SetAccessToken(loginResult.AccessToken.Token);
            GraphRequest request = GraphRequest.NewMeRequest(loginResult.AccessToken, _graphCBHandler);

            Bundle parameters = new Bundle();
            parameters.PutString("fields", "email");
            request.Parameters = (parameters);
            request.ExecuteAsync();
        }
    }

    public class FacebookAuth_Android : Happiness.FacebookAuth
    {
        string m_szEmail;
        string m_szId;

        bool m_WaitingForAuth;
        FBCallbackHandler m_FBCBH;

        public FacebookAuth_Android()
        {
            _instance = this;
            
            m_FBCBH = new FBCallbackHandler(this);
            LoginManager.Instance.RegisterCallback(Activity1.Instance.CallbackManager, m_FBCBH);
        }

        public override void BeginAuth()
        {
            if (!m_WaitingForAuth)
            {
                m_WaitingForAuth = true;
                LoginManager.Instance.LogInWithReadPermissions(Activity1.Instance, new string[] { "email" });
            }
        }

        public override string[] FinishAuth()
        {
            return m_WaitingForAuth ? null : new string[] { m_szEmail, m_szId };
        }

        public void OnFinished(bool success, string s0)
        {
            if (success)
            {
                m_szEmail = s0;
            }
            m_WaitingForAuth = false;
        }

        public void SetAccessToken(string accessToken)
        {
            m_szId = accessToken;
        }
    }
}