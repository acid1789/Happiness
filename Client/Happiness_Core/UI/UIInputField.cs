using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness
{
    public class UIInputField
    {
        Rectangle m_InputRect;
        bool m_bFocused;
        bool m_bMaskedText;
        bool m_bShowCursor;
        bool m_bEnabled;
        double m_dfCursorBlinkTime;

        string m_szInputText;
        string m_szMaskedText;

        UIFrame m_FocusFrame;

        public UIInputField(Rectangle rect, bool masked = false)
        {
            m_bEnabled = true;
            m_dfCursorBlinkTime = 0;
            m_InputRect = rect;
            m_bFocused = false;
            m_bMaskedText = masked;
            m_szInputText = "";
            m_szMaskedText = "";

            m_FocusFrame = new UIFrame(2, m_InputRect);
        }

        #region Input
        public void AppendString(string str)
        {
            // For now just put this at the end, this really needs to get placed wherever the cursor is
            Text = m_szInputText + str;
        }

        public void HandleKey(KeyArgs key)
        {
            if (key.Key == Keys.Backspace)
            {
                if (m_szInputText.Length > 0)
                {
                    m_szInputText = m_szInputText.Substring(0, m_szInputText.Length - 1);
                    m_szMaskedText = m_szMaskedText.Substring(0, m_szInputText.Length);
                }
            }
            else
            {
                char c = KeyToAscii(key);
                if (c != 0)
                {
                    m_szInputText += c;
                    m_szMaskedText += "*";
                }
            }
        }

        char KeyToAscii(KeyArgs key)
        {
            int keyInt = (int)key.Key;
            if (key.Key >= Keys.Num0 && key.Key <= Keys.Num9)
            {
                keyInt -= (int)Keys.Num0;
                if (key.Shift)
                    return Constants.Ascii0to9Shift[keyInt];
                else
                {
                    keyInt += '0';
                    return (char)keyInt;
                }
            }
            else if (key.Key >= Keys.A && key.Key <= Keys.Z)
            {
                keyInt -= (int)Keys.A;
                keyInt += key.Shift ? 'A' : 'a';
                return (char)keyInt;
            }
            else if (key.Key == Keys.OemPeriod)
            {
                return (key.Shift) ? '>' : '.';
            }
            else if (key.Key == Keys.OemQuestion)
                return (key.Shift) ? '?' : '/';
            else if (key.Key == Keys.OemComma)
                return (key.Shift) ? '<' : ',';
            else if (key.Key == Keys.OemPipe)
                return key.Shift ? '|' : '\\';
            else if (key.Key == Keys.OemCloseBrackets)
                return key.Shift ? '}' : ']';
            else if (key.Key == Keys.OemOpenBrackets)
                return key.Shift ? '{' : '[';
            else if (key.Key == Keys.OemPlus)
                return key.Shift ? '+' : '=';
            else if (key.Key == Keys.OemMinus)
                return key.Shift ? '_' : '-';
            else if (key.Key == Keys.OemTilde)
                return key.Shift ? '~' : '`';
            else if (key.Key == Keys.Slash)
                return key.Shift ? '?' : '/';
            else if (key.Key == Keys.Backslash)
                return '\\';
            else if (key.Key == Keys.Equals)
                return key.Shift ? '+' : '=';
            else if (key.Key == Keys.Apostrophe)
                return key.Shift ? '\"' : '\'';
            else if (key.Key == Keys.Semicolon)
                return key.Shift ? ':' : ';';
            else
            {
                System.Diagnostics.Debug.WriteLine("KeyToAscii: " + key.Key.ToString());
            }

            return (char)0;
        }
        #endregion

        public void Update(double gt)
        {
            if (m_bFocused)
            {
                m_dfCursorBlinkTime -= gt;
                if (m_dfCursorBlinkTime <= 0)
                {
                    m_bShowCursor = !m_bShowCursor;
                    m_dfCursorBlinkTime = 0.5;
                }
            }
        }

        #region Drawing
        public void Draw(Renderer sb)
        {
            // Draw the box
            sb.Draw(Assets.TransGray, m_InputRect, m_bEnabled ? Color.White : Color.Gray);

            // Draw the text
            Happiness.ShadowString(sb, Assets.HelpFont, m_bMaskedText ? m_szMaskedText : m_szInputText, new Vector2(m_InputRect.Left + 4, m_InputRect.Top), m_bEnabled ? Color.White : Color.Gray);

            // Draw the cursor
            if (m_bShowCursor && m_bFocused && m_bEnabled)
            {
                Vector2 textSize = Assets.HelpFont.MeasureString(m_bMaskedText ? m_szMaskedText : m_szInputText);
                int cursorLeft = m_InputRect.Left + 4 + (int)textSize.X;
                sb.Draw(Assets.GoldBarVertical, new Rectangle(cursorLeft, m_InputRect.Top + 2, 2, m_InputRect.Height - 4), Color.White);
            }

            // Draw outline
            if (m_bFocused && m_bEnabled)
            {
                m_FocusFrame.Draw(sb);
            }

        }        
        #endregion

        #region Accessors
        public Rectangle Rect
        {
            get { return m_InputRect; }
        }

        public bool Focused
        {
            get { return m_bFocused; }
            set { m_bFocused = value; }
        }

        public bool Enabled
        {
            get { return m_bEnabled; }
            set { m_bEnabled = value; }
        }

        public string Text
        {
            get { return m_szInputText; }
            set
            {
                m_szInputText = value;
                m_szMaskedText = "";
                for (int i = 0; i < m_szInputText.Length; i++)
                    m_szMaskedText += "*";
            }
        }
        #endregion
    }
}
