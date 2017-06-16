using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness
{
    public abstract class PurchaseSystem
    {
        protected static PurchaseSystem _instance;
        public static PurchaseSystem Instance { get { return _instance; } }

        public abstract void Display(int account, int productId);
    }
}
