using System;
using System.Collections.Generic;
using System.Text;

namespace LogicMatrix
{
    public class PuzzleRow
    {
        public PuzzleCell[] m_Cells;

        public PuzzleRow(int iSize)
        {
            m_Cells = new PuzzleCell[iSize];
            for (int i = 0; i < iSize; i++)
            {
                m_Cells[i] = new PuzzleCell(iSize);
            }
        }

        public PuzzleRow(PuzzleRow Other)
        {
            m_Cells = new PuzzleCell[Other.m_Cells.Length];
            for (int i = 0; i < m_Cells.Length; i++)
            {
                m_Cells[i] = new PuzzleCell(Other.m_Cells[i]);
            }
        }

        public void Reset()
        {
            for (int i = 0; i < m_Cells.Length; i++)
            {
                m_Cells[i].Reset();
            }
        }

        public void Reset(int iColumn)
        {
            int iFinal = m_Cells[iColumn].m_iFinalIcon;
            m_Cells[iColumn].Reset();

            for (int i = 0; i < m_Cells.Length; i++)
            {
                if (i == iColumn)
                    continue;

                if (m_Cells[i].m_iFinalIcon >= 0)
                {
                    m_Cells[iColumn].m_bValues[m_Cells[i].m_iFinalIcon] = false;
                }
                else if( iFinal >= 0 )
                {
                    m_Cells[i].m_bValues[iFinal] = true;
                }
            }

            m_Cells[iColumn].m_iFinalIcon = m_Cells[iColumn].GetRemainingIcon();            
        }

        public bool IsCompleted()
        {
            for (int i = 0; i < m_Cells.Length; i++)
            {
                int iFinal = m_Cells[i].m_iFinalIcon;
                if (iFinal < 0)
                    return false;
            }
            return true;
        }
    }
}
