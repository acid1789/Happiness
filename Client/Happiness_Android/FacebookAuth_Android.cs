using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Happiness_Android
{
    class FacebookAuth_Android : Happiness.FacebookAuth
    {
        public FacebookAuth_Android()
        {
            _instance = this;
        }

        public override string[] DoAuth()
        {
            throw new NotImplementedException();
        }
    }
}