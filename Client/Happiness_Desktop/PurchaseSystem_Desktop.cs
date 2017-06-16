using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness_Desktop
{
    public class PurchaseSystem_Desktop : Happiness.PurchaseSystem
    {
        public PurchaseSystem_Desktop()
        {
            _instance = this;
        }

        public override void Display(int account, int productId)
        {
#if DEBUG
            string host = "localhost:8080";
#else
            string host = "www.ronzgames.com/braintree";
#endif
            string url = string.Format("http://{0}/purchase?uid={1}&pid={2}", host, account, productId);
            System.Diagnostics.Process.Start(url);
        }
    }
}
