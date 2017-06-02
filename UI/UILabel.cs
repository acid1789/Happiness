using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    class UILabel
    {
        public enum XMode
        {
            Left,
            Center,
            Right
        }

        string m_szText;
        Vector2 m_vPosition;
        Color m_Color;
        SpriteFont m_Font;
        int m_X;
        XMode m_XMode;

        bool m_bHidden;

        public UILabel(string text, int x, int y, Color color, SpriteFont font, XMode mode)
        {
            m_bHidden = false;

            m_szText = text;            
            m_Color = color;
            m_Font = font;

            m_XMode = mode;
            m_X = x;
            m_vPosition = new Vector2(x, y);
            DoLayout();
        }

        void DoLayout()
        {
            if (m_XMode == XMode.Left)
                m_vPosition = new Vector2(m_X, m_vPosition.Y);
            else if (m_XMode == XMode.Center)
            {
                Vector2 size = m_Font.MeasureString(m_szText);
                m_vPosition.X = m_X - (size.X * 0.5f);
            }
            else
            {
                Vector2 size = m_Font.MeasureString(m_szText);
                m_vPosition.X = m_X - size.X;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if( !m_bHidden )
                Happiness.ShadowString(sb, m_Font, m_szText, m_vPosition, m_Color);
        }

        #region Accessors
        public Color Color
        {
            get { return m_Color; }
            set { m_Color = value; }
        }

        public int Width
        {
            get { return (int)m_Font.MeasureString(m_szText).X; }
        }

        public int Height
        {
            get { return (int)m_Font.MeasureString(m_szText).Y; }
        }

        public bool Hidden
        {
            get { return m_bHidden; }
            set { m_bHidden = value; }
        }

        public SpriteFont Font
        {
            get { return m_Font; }
            set { m_Font = value; }
        }

        public string Text
        {
            get { return m_szText; }
            set { m_szText = value; DoLayout(); }
        }

        public int PositionX
        {
            get { return (int)m_vPosition.X; }
            set { m_X = value; DoLayout(); }
        }

        public int PositionY
        {
            get { return (int)m_vPosition.Y; }
            set { m_vPosition.Y = value; }                
        }
        #endregion
    }
}
