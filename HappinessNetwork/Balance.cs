using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HappinessNetwork
{
    public static class Balance
    {
        static double[] s_ParTimes = new double[] { 2f * 60, 5f * 60, 10f * 60, 30 * 60, 45 * 60, 60 * 60 };
        static double[] s_BaseExpValues = new double[] { 200, 600, 1000, 1600, 2800, 4400  };
        static double[] s_TimeBonusExpValues = new double[] { 400, 1200, 2000, 3200, 5600, 8800 };
        static int[] s_TowerUnlockThresholds = new int[] { 3, 25, 60, 150, 500 };

        public static double ParTime(int towerIndex)
        {
            return s_ParTimes[towerIndex];
        }

        public static double BaseExp(int towerIndex)
        {
            return s_BaseExpValues[towerIndex];
        }

        public static double BonusExp(int towerIndex, double seconds)
        {
            double timePercent = Math.Min(Math.Max(seconds / Balance.ParTime(towerIndex), 0), 1);
            double bonus = s_TimeBonusExpValues[towerIndex] - (s_TimeBonusExpValues[towerIndex] * timePercent);
            return bonus;
        }

        public static int ExpForNextLevel(int level)
        {
            return (int)(Math.Log(level + 3) * 1000);
        }

        public static int UnlockThreshold(int tower)
        {
            return s_TowerUnlockThresholds[tower];
        }

    }

    public class VIPDetails
    {
        public int Hints;
        public int MegaHints;
        public int UndoSize;
        public float ExpBonus;

        public static VIPDetails[] Levels =
        {
            new VIPDetails { Hints = 2, MegaHints = 0, UndoSize = 3, ExpBonus = 1 },        // 0
            new VIPDetails { Hints = 3, MegaHints = 1, UndoSize = 5, ExpBonus = 1.1f },     // 1
            new VIPDetails { Hints = 5, MegaHints = 3, UndoSize = 10, ExpBonus = 1.25f },   // 2
            new VIPDetails { Hints = 8, MegaHints = 3, UndoSize = 15, ExpBonus = 1.6f },    // 3
            new VIPDetails { Hints = 12, MegaHints = 4, UndoSize = 25, ExpBonus = 2 },      // 4
            new VIPDetails { Hints = 17, MegaHints = 5, UndoSize = 40, ExpBonus = 2.5f },   // 5
            new VIPDetails { Hints = 25, MegaHints = 6, UndoSize = -1, ExpBonus = 3.2f },   // 6
            new VIPDetails { Hints = 40, MegaHints = 10, UndoSize = -1, ExpBonus = 4.25f }, // 7
            new VIPDetails { Hints = 80, MegaHints = 15, UndoSize = -1, ExpBonus = 6 },     // 8
            new VIPDetails { Hints = -1, MegaHints = 20, UndoSize = -1, ExpBonus = 8.75f }, // 9
            new VIPDetails { Hints = -1, MegaHints = -1, UndoSize = -1, ExpBonus = 12 }     // 10
        };
    }
}
