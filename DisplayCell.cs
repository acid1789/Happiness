using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Happiness
{
    public class DisplayCell
    {
        public Rectangle[] m_aDisplayRects;
        public Rectangle m_rBounds;
        public Rectangle m_rFinal;

        public DisplayCell(int iSize, Rectangle rBounds)
        {
            m_aDisplayRects = new Rectangle[iSize];
            m_rBounds = rBounds;

            switch (iSize)
            {
                case 3:
                    Create3x3();
                    break;
                case 4:
                    Create4x4();
                    break;
                case 5:
                    Create5x5();
                    break;
                case 6:
                    Create6x6();
                    break;
                case 7:
                    Create7x7();
                    break;
                case 8:
                    Create8x8();
                    break;
            }
        }

        private void Create3x3()
        {
            int iIconSize = m_rBounds.Width / 2;

            int iX = m_rBounds.X;
            for (int i = 0; i < 2; i++)
            {
                m_aDisplayRects[i] = new Rectangle(iX, m_rBounds.Y, iIconSize, iIconSize);
                iX += iIconSize;
            }

            iX = m_rBounds.X + (iIconSize / 2);
            int iY = m_rBounds.Y + iIconSize;
            m_aDisplayRects[2] = new Rectangle(iX, iY, iIconSize, iIconSize);

            m_rFinal = new Rectangle(m_rBounds.X, m_rBounds.Y, iIconSize * 2, iIconSize * 2); ;
        }

        private void Create4x4()
        {
            int iIconSize = m_rBounds.Width / 2;

            int iX = m_rBounds.X;
            for (int i = 0; i < 2; i++)
            {
                m_aDisplayRects[i] = new Rectangle(iX, m_rBounds.Y, iIconSize, iIconSize);
                iX += iIconSize;
            }

            iX = m_rBounds.X;
            int iY = m_rBounds.Y + iIconSize;
            for (int i = 2; i < 4; i++)
            {
                m_aDisplayRects[i] = new Rectangle(iX, iY, iIconSize, iIconSize);
                iX += iIconSize;
            }

            m_rFinal = new Rectangle(m_rBounds.X, m_rBounds.Y, iIconSize * 2, iIconSize * 2);
        }

        private void Create5x5()
        {
            int iIconSize = m_rBounds.Width / 3;

            int iX = m_rBounds.X;
            for (int i = 0; i < 3; i++)
            {
                m_aDisplayRects[i] = new Rectangle(iX, m_rBounds.Y, iIconSize, iIconSize);
                iX += iIconSize;
            }

            iX = m_rBounds.X +  (iIconSize / 3);
            int iY = m_rBounds.Y + iIconSize;
            for (int i = 3; i < 5; i++)
            {
                m_aDisplayRects[i] = new Rectangle(iX, iY, iIconSize, iIconSize);
                iX += iIconSize + (iIconSize / 3);
            }

            iX = m_rBounds.X + (iIconSize / 2);
            m_rFinal = new Rectangle(iX, m_rBounds.Y, iIconSize * 2, iIconSize * 2);
        }

        private void Create6x6()
        {
            int iIconSize = m_rBounds.Width / 3;

            int iX = m_rBounds.X;
            for (int i = 0; i < 3; i++)
            {
                m_aDisplayRects[i] = new Rectangle(iX, m_rBounds.Y, iIconSize, iIconSize);
                iX += iIconSize;
            }

            iX = m_rBounds.X;
            int iY = m_rBounds.Y + iIconSize;
            for (int i = 3; i < 6; i++)
            {
                m_aDisplayRects[i] = new Rectangle(iX, iY, iIconSize, iIconSize);
                iX += iIconSize;
            }

            iX = m_rBounds.X + (iIconSize / 2);
            m_rFinal = new Rectangle(iX, m_rBounds.Y, iIconSize * 2, iIconSize * 2);
        }

        private void Create7x7()
        {
            int iIconSize = m_rBounds.Width / 4;

            int iX = m_rBounds.X;
            for (int i = 0; i < 4; i++)
            {
                m_aDisplayRects[i] = new Rectangle(iX, m_rBounds.Y, iIconSize, iIconSize);
                iX += iIconSize;
            }

            iX = m_rBounds.X + (iIconSize / 2);
            int iY = m_rBounds.Y + iIconSize;
            for (int i = 4; i < 7; i++)
            {
                m_aDisplayRects[i] = new Rectangle(iX, iY, iIconSize, iIconSize);
                iX += iIconSize;
            }

            iX = m_rBounds.X + iIconSize;
            m_rFinal = new Rectangle(iX, m_rBounds.Y, iIconSize * 2, iIconSize * 2);
        }

        private void Create8x8()
        {
            int iIconSize = m_rBounds.Width / 4;

            int iX = m_rBounds.X;
            for (int i = 0; i < 4; i++)
            {
                m_aDisplayRects[i] = new Rectangle(iX, m_rBounds.Y, iIconSize, iIconSize);
                iX += iIconSize;
            }

            iX = m_rBounds.X;
            int iY = m_rBounds.Y + iIconSize;
            for (int i = 4; i < 8; i++)
            {
                m_aDisplayRects[i] = new Rectangle(iX, iY, iIconSize, iIconSize);
                iX += iIconSize;
            }

            iX = m_rBounds.X + iIconSize;
            m_rFinal = new Rectangle(iX, m_rBounds.Y, iIconSize * 2, iIconSize * 2);
        }
    }
}
