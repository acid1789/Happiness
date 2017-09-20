using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;

using System.Collections.Generic;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

using Plugin.InAppBilling;

namespace Happiness_Android
{
    [Activity(Label = "Happiness"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = Android.Content.PM.LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.SensorLandscape
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
    {
        public const int RC_SIGN_IN = 1;

        public HappinessAndroidGame TheGame { get; private set; }
        public ICallbackManager CallbackManager { get; private set; }

        public delegate void OnActivityResultDelegate(Result resultCode, Intent data);
        Dictionary<int, List<OnActivityResultDelegate>> m_ActivityResultHandlers;

        public static Activity1 Instance { get; set; }
        protected override void OnCreate(Bundle bundle)
        {
            Instance = this;
            base.OnCreate(bundle);
            FacebookSdk.SdkInitialize(this);
            CallbackManager = CallbackManagerFactory.Create();
            TheGame = new HappinessAndroidGame();
            SetContentView((View)TheGame.Services.GetService(typeof(View)));
            TheGame.Run();
        }

        public void RegisterActivityResultHandler(int requestCode, OnActivityResultDelegate handler)
        {
            if (m_ActivityResultHandlers == null)
                m_ActivityResultHandlers = new Dictionary<int, List<OnActivityResultDelegate>>();
            if (!m_ActivityResultHandlers.ContainsKey(requestCode))
                m_ActivityResultHandlers[requestCode] = new List<OnActivityResultDelegate>();

            if (!m_ActivityResultHandlers[requestCode].Contains(handler))
                m_ActivityResultHandlers[requestCode].Add(handler);
        }

        public void UnRegisterActivityResultHandler(int requestCode, OnActivityResultDelegate handler)
        {
            if (m_ActivityResultHandlers != null && m_ActivityResultHandlers.ContainsKey(requestCode))
            {
                List<OnActivityResultDelegate> handlers = m_ActivityResultHandlers[requestCode];
                if (handlers.Contains(handler))
                    handlers.Remove(handler);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            CallbackManager.OnActivityResult(requestCode, (int)resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);

            if (m_ActivityResultHandlers != null && m_ActivityResultHandlers.ContainsKey(requestCode))
            {
                OnActivityResultDelegate[] handlers = m_ActivityResultHandlers[requestCode].ToArray();
                foreach (OnActivityResultDelegate handler in handlers)
                {
                    handler.Invoke(resultCode, data);
                }
            }
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            VirtualKeyboard_Android.Instance.HandleEvent(e.Action, e.KeyCode, (e.Modifiers & MetaKeyStates.ShiftMask) != 0, e.Characters);
            return base.DispatchKeyEvent(e);
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            return base.OnKeyDown(keyCode, e);
        }

        public override bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            System.Console.WriteLine(keyCode.ToString());
            return base.OnKeyUp(keyCode, e);
        }        
    }
}

