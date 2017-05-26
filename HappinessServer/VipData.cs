using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HappinessNetwork;

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
        
        static int[] s_VipLevels = { 50, 100, 250, 500, 1000, 2500, 10000, 500000, 250000, 1000000 };

        public static VipDataArgs Create(int vipPoints)
        {
            int level = 0;
            while (level < s_VipLevels.Length && vipPoints >= s_VipLevels[level])
            {
                level++;
            }

            VipDataArgs vda = new VipDataArgs();
            vda.Level = level;
            vda.Progress = s_VipLevels[level] - vipPoints;
            switch (level)
            {
                default:
                case 0:
                    vda.Hints = 2;
                    vda.MegaHints = 0;
                    vda.UndoSize = 3;
                    break;
                case 1:
                    vda.Hints = 3;
                    vda.MegaHints = 1;
                    vda.UndoSize = 5;
                    break;
                case 2:
                    vda.Hints = 5;
                    vda.MegaHints = 3;
                    vda.UndoSize = 10;
                    break;
                case 3:
                    vda.Hints = 8;
                    vda.MegaHints = 3;
                    vda.UndoSize = 15;
                    break;
                case 4:
                    vda.Hints = 12;
                    vda.MegaHints = 4;
                    vda.UndoSize = 25;
                    break;
                case 5:
                    vda.Hints = 17;
                    vda.MegaHints = 5;
                    vda.UndoSize = 40;
                    break;
                case 6:
                    vda.Hints = 25;
                    vda.MegaHints = 6;
                    vda.UndoSize = -1;
                    break;
                case 7:
                    vda.Hints = 40;
                    vda.MegaHints = 10;
                    vda.UndoSize = -1;
                    break;
                case 8:
                    vda.Hints = 80;
                    vda.MegaHints = 15;
                    vda.UndoSize = -1;
                    break;
                case 9:
                    vda.Hints = -1;
                    vda.MegaHints = 20;
                    vda.UndoSize = -1;
                    break;
                case 10:
                    vda.Hints = -1;
                    vda.MegaHints = -1;
                    vda.UndoSize = -1;
                    break;
            }
            return vda;
        }
    }
}
