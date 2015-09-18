using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class Tower
    {
        int m_iSize;
        string m_szSize;
        Vector2 m_vSizePosition;

        int m_iFloor;
        string m_szFloor;
        Vector2 m_vFloorPosition;

        Rectangle m_Rect;
        Texture2D m_Icon;

        bool m_bLocked;

        public Tower(int size, int floor, Rectangle rect, Texture2D icon)
        {
            m_Rect = rect;
            m_Icon = icon;
            m_bLocked = floor <= 0;

            int iconCenterX = m_Rect.Left + (m_Rect.Width >> 1);
            
            m_iSize = size;
            m_szSize = string.Format("{0} x {0}", m_iSize);
            Vector2 strSize = Assets.DialogFont.MeasureString(m_szSize);
            m_vSizePosition.X = iconCenterX - (strSize.X / 2);
            m_vSizePosition.Y = m_Rect.Bottom;

            m_iFloor = floor;
            m_szFloor = (floor != 0) ? string.Format("Floor: {0:n0}", floor) : string.Format("Unlocks at level {0}", HappinessNetwork.Balance.UnlockThreshold(size - 4));
            m_vFloorPosition.Y = m_vSizePosition.Y + strSize.Y;
            strSize = Assets.DialogFont.MeasureString(m_szFloor);
            m_vFloorPosition.X = iconCenterX - (strSize.X / 2);
        }

        public bool Click(int x, int y)
        {
            return ( m_Rect.Contains(x, y) );
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw the icon
            sb.Draw(m_Icon, m_Rect, m_bLocked ? Color.DarkGray : Color.White);
            if( m_bLocked )
                sb.Draw(Assets.NotOverlay, m_Rect, Color.White);
            
            Happiness.ShadowString(sb, Assets.DialogFont, m_szSize, m_vSizePosition, Color.Goldenrod);
            Happiness.ShadowString(sb, Assets.DialogFont, m_szFloor, m_vFloorPosition, Color.White);
        }

        void DrawShadowedString(SpriteBatch sb, SpriteFont font, string str, Vector2 position, Color color)
        {
            sb.DrawString(font, str, new Vector2(position.X + 2, position.Y + 2), Color.Black);
            sb.DrawString(font, str, position, color);
        }

        #region Accessors
        public bool Locked
        {
            get { return m_bLocked; }
        }

        public int Floor
        {
            get { return m_iFloor; }
        }

        public int Size
        {
            get { return m_iSize; }
        }
        #endregion
    }
}
