using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness
{
    public abstract class FacebookAuth
    {
        protected static FacebookAuth _instance;
        public static FacebookAuth Instance { get { return _instance; } }

        public abstract string[] DoAuth();
    }
}
