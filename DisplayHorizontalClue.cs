using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace Happiness
{
    public class DisplayHorizontalClue
    {
        public Rectangle[] m_aDisplayRects;
        public Rectangle m_rBounds;

        public DisplayHorizontalClue(int iX, int iY, int iIconSize)
        {
            m_aDisplayRects = new Rectangle[3];

            m_aDisplayRects[0] = new Rectangle(iX, iY, iIconSize, iIconSize);
            m_aDisplayRects[1] = new Rectangle(iX + iIconSize, iY, iIconSize, iIconSize);
            m_aDisplayRects[2] = new Rectangle(iX + iIconSize + iIconSize, iY, iIconSize, iIconSize);

            m_rBounds = new Rectangle(iX, iY, iIconSize * 3, iIconSize);
        }
    }
}
