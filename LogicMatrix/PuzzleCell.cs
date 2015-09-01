using System;
using System.Collections.Generic;
using System.Text;

namespace LogicMatrix
{
    public class PuzzleCell
    {
        public bool[] m_bValues;        // True if the icon is still in the cell, false if it is not
        public int m_iFinalIcon;

        public PuzzleCell(int iSize)
        {
            m_bValues = new bool[iSize];
            Reset();
        }

        public PuzzleCell(PuzzleCell Other)
        {
            m_bValues = new bool[Other.m_bValues.Length];
            for (int i = 0; i < m_bValues.Length; i++)
            {
                m_bValues[i] = Other.m_bValues[i];
            }
            m_iFinalIcon = Other.m_iFinalIcon;
        }

        public int GetRemainingIcon()
        {
            int iRemaining = -1;
            int iCount = 0;
            for (int i = 0; i < m_bValues.Length; i++)
            {
                if (m_bValues[i])
                {
                    iCount++;
                    iRemaining = i;
                }
            }
            if (iCount > 1)
                iRemaining = -1;
            return iRemaining;
        }

        public void Reset()
        {
            for (int i = 0; i < m_bValues.Length; i++)
            {
                m_bValues[i] = true;
            }
            m_iFinalIcon = -1;
        }
    }
}
