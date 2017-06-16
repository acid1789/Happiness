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
    class PurchaseSystem_Android : Happiness.PurchaseSystem
    {
        public PurchaseSystem_Android()
        {
            _instance = this;
        }

        public override void Display(int account, int productId)
        {
            throw new NotImplementedException();
        }
    }
}