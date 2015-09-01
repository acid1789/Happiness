using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Happiness
{
    public class DisplayVerticalClue
    {
        public Rectangle[] m_aDisplayRects;
        public Rectangle[] m_aOverlayRects;
        public Rectangle m_rBounds;

        public DisplayVerticalClue(int iX, int iY, int iIconSize)
        {
            m_aDisplayRects = new Rectangle[3];

            m_aDisplayRects[0] = new Rectangle(iX, iY, iIconSize, iIconSize);
            m_aDisplayRects[1] = new Rectangle(iX, iY + iIconSize, iIconSize, iIconSize);
            m_aDisplayRects[2] = new Rectangle(iX, iY + iIconSize + iIconSize, iIconSize, iIconSize);

            m_aOverlayRects = new Rectangle[2];
            m_aOverlayRects[0] = new Rectangle(iX, iY + (iIconSize / 2), iIconSize, iIconSize);
            m_aOverlayRects[1] = new Rectangle(iX, iY + (iIconSize / 2) + iIconSize, iIconSize, iIconSize);

            m_rBounds = new Rectangle(iX, iY, iIconSize, iIconSize * 3);
        }
    }
}
