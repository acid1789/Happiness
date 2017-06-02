using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkCore
{
    public class VIPLevels
    {
        static int[] s_VipLevels = { 50, 100, 250, 500, 1000, 2500, 10000, 500000, 250000, 1000000 };

        public static int[] Levels { get { return s_VipLevels; } }
    }
}
