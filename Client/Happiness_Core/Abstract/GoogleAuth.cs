using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness
{
    public abstract class GoogleAuth
    {
        protected static GoogleAuth _instance;
        public static GoogleAuth Instance { get { return _instance; } }

        public abstract string[] DoAuth();
    }
}
