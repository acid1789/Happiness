using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness
{
    public class UICheckbox
    {
        public enum XMode
        {
            Left,
            Center,
            Right
        }

        bool m_bEnabled;
        bool m_bChecked;
        XMode m_XMode;
        int m_iCheckboxSize;

        string m_szText;       
        Rectangle m_Rect;
        Vector2 m_TextLocation;              

        Color m_TextColor;
        Color m_TextDisabledColor;

        public UICheckbox(string text, int x, int top, int screenHeight, XMode xmode = XMode.Left)
        {
            m_bEnabled = true;
            m_XMode = xmode;
            m_szText = text;
            m_TextColor = Color.LightGray;
            m_TextDisabledColor = Color.Gray;

            m_iCheckboxSize = (int)(Constants.MessageBox_CheckboxSize * screenHeight);

            Layout(x, top);
        }

        void Layout(int x, int top)
        {
            Vector2 textSize = Assets.HelpFont.MeasureString(m_szText);
            int checkWidth = m_iCheckboxSize + 5 + (int)textSize.X;
            int left = x;
            switch (m_XMode)
            {
                case XMode.Left:
                    left = x;
                    break;
                case XMode.Center:
                    left = x - (checkWidth >> 1);
                    break;
                case XMode.Right:
                    left = x - checkWidth;
                    break;
            }
            m_Rect = new Rectangle(left, top, checkWidth, m_iCheckboxSize);
            m_TextLocation = new Vector2(left + m_iCheckboxSize + 5, (m_Rect.Top + (m_iCheckboxSize >> 1)) - (textSize.Y / 2));
        }

        public void HandleClick(int x, int y)
        {
            if (m_bEnabled && m_Rect.Contains(x, y))
            {
                m_bChecked = !m_bChecked;
                Happiness.Game.SoundManager.PlaySound(SoundManager.SEInst.MenuAccept);
            }
        }

        public void HandleKey(KeyArgs key)
        {
            if( m_bEnabled && key.Key == Keys.Space )
                m_bChecked = !m_bChecked;
        }

        public void Draw(Renderer sb)
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
            set { Layout(value, m_Rect.Top); }
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
