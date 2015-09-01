using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Happiness
{
    public class DisplayRow
    {
        public DisplayCell[] m_aCells;
        public Rectangle m_rBounds;

        public DisplayRow(int iSize, Rectangle rBounds, int iColWidth, int iColSpacerWidth)
        {
            m_rBounds = rBounds;
            m_aCells = new DisplayCell[iSize];

            int iX = m_rBounds.X;
            for (int iCol = 0; iCol < iSize; iCol++)
            {
                m_aCells[iCol] = new DisplayCell(iSize, new Rectangle(iX, m_rBounds.Top, iColWidth - iColSpacerWidth, m_rBounds.Height));
                iX += iColWidth;
            }
        }
    }
}