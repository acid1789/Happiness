using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    class UIButton
    {
        Rectangle m_Rect;
        Texture2D m_MainTexture;
        Texture2D m_DisabledTexture;
        Color m_MainColor;
        Color m_DisabledColor;
        int m_ButtonID;

        string m_Text;
        Vector2 m_TextPosition;
        SpriteFont m_Font;
        Color m_TextColor;
        Color m_TextColorDisabled;

        string m_UnderText;
        Vector2 m_UnderTextPosition;
        SpriteFont m_UnderTextFont;
        Color m_UnderTextColor;
        Color m_UnderTextColorDisabled;

        bool m_bDisabled;
        
        public event EventHandler OnClick;

        public UIButton(int buttonID, string text, SpriteFont font, Rectangle rect, Texture2D main)
        {
            m_Rect = rect;

            m_MainTexture = main;
            m_MainColor = Color.White;
            m_DisabledTexture = main;
            m_DisabledColor = Color.DarkGray;
            

            m_ButtonID = buttonID;
            m_Text = text;
            m_Font = font;
            ResizeText();
            m_TextColor = Color.Black;
            m_TextColorDisabled = Color.Gray;

            m_UnderTextFont = Assets.HelpFont;
            m_UnderTextColor = m_TextColor;
            m_UnderTextColorDisabled = m_TextColorDisabled;
            ResizeUnderText();

            m_bDisabled = false;
        }

        public bool Click(int x, int y)
        {
            if (!m_bDisabled && m_Rect.Contains(x, y))
            {
                if( OnClick != null )
                    OnClick(m_ButtonID, null);
                return true;
            }
            return false;
        }

        void ResizeText()
        {
            Vector2 textSize = m_Font.MeasureString(m_Text);

            int halfWidth = m_Rect.Width >> 1;
            int halfHeight = m_Rect.Height >> 1;

            m_TextPosition.X = m_Rect.Left + halfWidth - (textSize.X * 0.5f);
            m_TextPosition.Y = m_Rect.Top + halfHeight - (textSize.Y * 0.5f);
        }

        void ResizeUnderText()
        {
            if (m_UnderText != null)
            {
                Vector2 textSize = m_UnderTextFont.MeasureString(m_UnderText);

                int halfWidth = m_Rect.Width >> 1;

                m_UnderTextPosition.X = m_Rect.Left + halfWidth - (textSize.X * 0.5f);
                m_UnderTextPosition.Y = m_Rect.Bottom;
            }
        }

        public void Draw(SpriteBatch sb)
        {        
            sb.Draw(m_bDisabled ? m_DisabledTexture : m_MainTexture, m_Rect, m_bDisabled ? m_DisabledColor : m_MainColor);            
            sb.DrawString(m_Font, m_Text, m_TextPosition, m_bDisabled ? m_TextColorDisabled : m_TextColor);   
            
            if( m_UnderText != null )
                sb.DrawString(m_UnderTextFont, m_UnderText, m_UnderTextPosition, m_bDisabled ? m_UnderTextColorDisabled : m_UnderTextColor); 
        }

        #region Accessors
        public Rectangle Rect
        {
            get { return m_Rect; }
        }

        public int ButtonID
        {
            get { return m_ButtonID; }
        }

        public bool Enabled
        {
            get { return !m_bDisabled; }
            set { m_bDisabled = !value; }
        }

        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; ResizeText(); }
        }

        public string UnderText
        {
            get { return m_UnderText; }
            set { m_UnderText = value; ResizeUnderText(); }
        }

        public Color TextColor
        {
            get { return m_TextColor; }
            set { m_TextColor = value; }
        }

        public Color TextColorDisabled
        {
            get { return m_TextColorDisabled; }
            set { m_TextColorDisabled = value; }
        }

        public Color UnderTextColor
        {
            get { return m_UnderTextColor; }
            set { m_UnderTextColor = value; }
        }

        public Color UnderTextColorDisabled
        {
            get { return m_UnderTextColorDisabled; }
            set { m_UnderTextColorDisabled = value; }
        }

        public Texture2D DisabledTexture
        {
            get { return m_DisabledTexture; }
            set { m_DisabledTexture = value; }
        }

        public Color MainColor
        {
            get { return m_MainColor; }
            set { m_MainColor = value; }        
        }

        public Color DisabledColor
        {
            get { return m_DisabledColor; }
            set { m_DisabledColor = value; }
        }
        #endregion
    }
}
