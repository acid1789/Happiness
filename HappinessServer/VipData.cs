using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HappinessNetwork;
using NetworkCore;

namespace HappinessServer
{
    class VipData
    {
        // $1 = 16 credits
        // Hint = 2 credits
        // Mega Hint = 16 credits

        // $1 = 10 Vip Points
        // $5 = 55 Vip Points
        // $10 = 125 Vip Points
        // $25 = 350 Vip Points
        // $50 = 750 Vip Points
        // $100 = 1500 Vip Points
        // $1000 = 20000 Vip Points

        // Vip Level 1 - 50 Vip Points, $5
        // Vip Level 2 - 100 Vip Points, $10
        // Vip Level 3 - 250 Vip Points, $25
        // Vip Level 4 - 500 Vip Points, $50
        // Vip Level 5 - 1000 Vip Points, $100
        // Vip Level 6 - 2500 Vip Points, $250
        // Vip Level 7 - 10000 Vip Points, $1,000
        // Vip Level 8 - 50000 Vip Points, $5,000
        // Vip Level 9 - 250000 Vip Points, $25,000
        // Vip Level 10 - 1000000 Vip Points, $100,000

        // Disable Timer - Vip 1
        // Disable Vip Exp Bonus = Vip 2
        // Error Detector = Vip 4
        // Super Error Detector = Vip 8
        

        public static VipDataArgs Create(int vipPoints)
        {
            int level = 0;
            while (level < VIPLevels.Levels.Length && vipPoints >= VIPLevels.Levels[level])
            {
                level++;
            }

            VipDataArgs vda = new VipDataArgs();
            vda.Level = level;
            vda.Points = vipPoints;

            VIPDetails details = VIPDetails.Levels[level];
            vda.Hints = details.Hints;
            vda.MegaHints = details.MegaHints;
            vda.UndoSize = details.UndoSize;
            vda.ExpBonus = details.ExpBonus;
            return vda;
        }
    }
}
