using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicMatrix
{
    public class Hint
    {
        private Clue theClue;
        private bool m_bSetFinalIcon;
        private int m_iRow;
        private int m_iCol;
        private int m_iIcon;

        public Hint()
        {
        }

        public bool Init(Puzzle P, Clue C)
        {
            theClue = C;
            if (!theClue.GetHintAction(P, out m_bSetFinalIcon, out m_iRow, out m_iCol, out m_iIcon))
                return false;

            return true;
        }

        public bool ShouldHide(Puzzle P)
        {
            if ((P.m_Rows[m_iRow].m_Cells[m_iCol].m_iFinalIcon == m_iIcon && m_bSetFinalIcon) ||
                (!P.m_Rows[m_iRow].m_Cells[m_iCol].m_bValues[m_iIcon] && !m_bSetFinalIcon))
            {
                return true;
            }
            return false;
        }

        public bool ShouldDraw(int iRow, int iCol, int iIcon)
        {
            return (iRow == m_iRow && iCol == m_iCol && iIcon == m_iIcon); 
        }

        public bool ShouldDraw(Clue C)
        {
            return (C == theClue);
        }
    }
}
