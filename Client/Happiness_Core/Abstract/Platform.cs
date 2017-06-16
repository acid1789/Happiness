using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness
{
    public abstract class Platform
    {
        protected static Platform _instance;
        public static Platform Instance { get { return _instance; } }

        public abstract void ExitApp();

        public abstract Vector2 RotateAroundAxis_Z(float rotation);

        public abstract bool ApplicationIsActivated();
    }
}
