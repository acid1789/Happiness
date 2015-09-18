using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HappinessNetwork
{
    public static class Balance
    {
        static double[] s_ParTimes = new double[] { 1f * 60, 5f * 60, 10f * 60, 30 * 60, 45 * 60, 60 * 60 };
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
}
