using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class UICheckbox
    {
        bool m_bEnabled;
        bool m_bChecked;
        int m_iCheckboxSize;

        string m_szText;       
        Rectangle m_Rect;
        Vector2 m_TextLocation;              

        Color m_TextColor;
        Color m_TextDisabledColor;

        public UICheckbox(string text, int left, int top, int screenHeight)
        {
            m_bEnabled = true;
            m_szText = text;
            m_TextColor = Color.LightGray;
            m_TextDisabledColor = Color.Gray;

            m_iCheckboxSize = (int)(Constants.MessageBox_CheckboxSize * screenHeight);

            Vector2 textSize = Assets.HelpFont.MeasureString(m_szText);
            int checkWidth = m_iCheckboxSize + 5 + (int)textSize.X;
            m_Rect = new Rectangle(left, top, checkWidth, m_iCheckboxSize);
            m_TextLocation = new Vector2(left + m_iCheckboxSize + 5, (m_Rect.Top + (m_iCheckboxSize >> 1)) - (textSize.Y / 2));
        }

        public void HandleClick(int x, int y)
        {
            if( m_bEnabled && m_Rect.Contains(x, y) )
                m_bChecked = !m_bChecked;
        }

        public void HandleKey(KeyArgs key)
        {
            if( m_bEnabled && key.Key == Microsoft.Xna.Framework.Input.Keys.Space )
                m_bChecked = !m_bChecked;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Assets.CheckBox, new Rectangle(m_Rect.Left, m_Rect.Top, m_iCheckboxSize, m_iCheckboxSize), m_bEnabled ? Color.White : Color.Gray);
            if (m_bChecked)
                sb.Draw(Assets.Check, new Rectangle(m_Rect.Left, m_Rect.Top, m_iCheckboxSize, m_iCheckboxSize), m_bEnabled ? Color.White : Color.Gray);

            Happiness.ShadowString(sb, Assets.HelpFont, m_szText, m_TextLocation, m_bEnabled ? m_TextColor : m_TextDisabledColor);
        }

        #region Accessors
        public bool Checked
        {
            get { return m_bChecked; }
            set { m_bChecked = value; }
        }

        public Color TextColor
        {
            get { return m_TextColor; }
            set { m_TextColor = value; }
        }

        public int Left
        {
            get { return m_Rect.Left; }
            set { m_Rect = new Rectangle(value, m_Rect.Top, m_Rect.Width, m_Rect.Height); }
        }

        public Rectangle Rect
        {
            get { return m_Rect; }
        }

        public bool Enabled
        {
            get { return m_bEnabled; }
            set { m_bEnabled = value; }
        }
        #endregion
    }
}
