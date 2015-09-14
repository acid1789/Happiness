using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public enum MessageBoxButtons
    {
        OK,
        YesNo,
        BuyCoinsCancel,
    }

    public enum MessageBoxResult
    {
        OK,
        Yes,
        No,
        BuyCoins,
        Cancel,

        NoResult
    }


    public class MessageBox
    {
        int m_iContext;
        List<string> m_Lines;
        UIButton[] m_Buttons;

        Rectangle m_Rect;
        int m_iMarginTopBottom;
        int m_iMarginLeftRight;
        int m_iTextLineHeight;
        int m_iCenterDialogX;

        string m_szCheckboxText;
        int m_iCheckboxSize;
        bool m_bCheckboxChecked;
        Rectangle m_CheckboxRect;
        Vector2 m_CheckboxTextLocation;

        Color m_TextColor;

        #region Initialization
        public MessageBox(string message, MessageBoxButtons buttons, int context, int screenWidth, int screenHeight, string checkboxText = null)
        {
            m_iContext = context;
            m_TextColor = Color.White;

            m_szCheckboxText = checkboxText;
            m_iCheckboxSize = (int)(Constants.MessageBox_CheckboxSize * screenHeight);

            m_iMarginLeftRight = (int)(Constants.MessageBox_LeftRightMargin * screenWidth);
            m_iMarginTopBottom = (int)(Constants.MessageBox_TopBottomMargin * screenHeight);

            int checkboxHeight = checkboxText == null ? 0 : m_iMarginTopBottom + m_iCheckboxSize;

            int width = (int)(Constants.MessageBox_Width * screenWidth);            
            SetupLines(width - (m_iMarginLeftRight * 2), message);

            int buttonWidth = (int)(Constants.MessageBox_ButtonWidth * screenWidth);
            int buttonHeight = (int)(Constants.MessageBox_ButtonHeight * screenHeight);

            int lineSpace = (int)(Constants.MessageBox_LineSpace * screenHeight);
            Vector2 testTextSize = Assets.DialogFont.MeasureString("TEST");
            m_iTextLineHeight = (int)testTextSize.Y + lineSpace;
            
            int dialogHeight = (m_iMarginTopBottom + m_iMarginTopBottom) + (m_iTextLineHeight * m_Lines.Count) + m_iMarginTopBottom + buttonHeight + checkboxHeight;

            int halfScreenW = screenWidth >> 1;
            int halfScreenH = screenHeight >> 1;
            int halfDialogW = width >> 1;
            int halfDialogH = dialogHeight >> 1;

            m_iCenterDialogX = halfScreenW;

            m_Rect = new Rectangle(halfScreenW - halfDialogW, halfScreenH - halfDialogH, width, dialogHeight);
            
            int buttonY = (m_Rect.Bottom - m_iMarginTopBottom) - buttonHeight;
            int halfButtonWidth = buttonWidth >> 1;
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    m_Buttons = new UIButton[1];
                    m_Buttons[0] = new UIButton((int)MessageBoxResult.OK, "OK", Assets.HelpFont, new Rectangle(halfScreenW - halfButtonWidth, buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);
                    break;
                case MessageBoxButtons.YesNo:
                    m_Buttons = new UIButton[2];
                    m_Buttons[0] = new UIButton((int)MessageBoxResult.Yes, "Yes", Assets.HelpFont, new Rectangle(halfScreenW - (halfButtonWidth + buttonWidth), buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);
                    m_Buttons[1] = new UIButton((int)MessageBoxResult.No, "No", Assets.HelpFont, new Rectangle(halfScreenW + halfButtonWidth, buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);
                    break;
                case MessageBoxButtons.BuyCoinsCancel:
                    m_Buttons = new UIButton[2];
                    m_Buttons[0] = new UIButton((int)MessageBoxResult.BuyCoins, "Buy Coins", Assets.HelpFont, new Rectangle(halfScreenW - (halfButtonWidth + buttonWidth), buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);
                    m_Buttons[1] = new UIButton((int)MessageBoxResult.Cancel, "Cancel", Assets.HelpFont, new Rectangle(halfScreenW + halfButtonWidth, buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);
                    break;
            }

            if (m_szCheckboxText != null)
            {
                Vector2 textSize = Assets.HelpFont.MeasureString(m_szCheckboxText);
                int checkWidth = m_iCheckboxSize + 5 + (int)textSize.X;
                int checkLeft = m_iCenterDialogX - (checkWidth >> 1);
                m_CheckboxRect = new Rectangle(checkLeft, buttonY - (m_iMarginTopBottom + m_iCheckboxSize), checkWidth, m_iCheckboxSize);
                m_CheckboxTextLocation = new Vector2(checkLeft + m_iCheckboxSize + 5, (m_CheckboxRect.Top + (m_iCheckboxSize >> 1)) - (textSize.Y / 2));
            }
        }

        void SetupLines(int width, string message)
        {
            m_Lines = new List<string>();

            // Do all requested line breaks
            int start = 0;
            int idx = message.IndexOf('\n');
            while (idx >= 0)
            {
                string line = message.Substring(start, idx - start);
                start = idx + 1;
                m_Lines.Add(line);

                idx = message.IndexOf('\n', start);
            }
            m_Lines.Add(message.Substring(start, message.Length - start));

            // Now wrap all lines that are to long
            string[] longLines = m_Lines.ToArray();
            m_Lines = new List<string>();
            foreach (string line in longLines)
            {
                Vector2 size = Assets.DialogFont.MeasureString(line);
                if (size.X > width)
                {
                    // this line is to long, wrap it
                    string[] words = line.Split(' ');
                    string smallLine = "";
                    foreach (string word in words)
                    {
                        string testLine = smallLine + " " + word;
                        size = Assets.DialogFont.MeasureString(testLine);
                        if (size.X > width)
                        {
                            // Adding this word doesnt fit, add a line break right before this word
                            m_Lines.Add(smallLine);
                            smallLine = word;
                        }
                        else
                        {
                            // This line still fits, keep going
                            smallLine = testLine;
                        }
                    }
                    m_Lines.Add(smallLine);
                }
                else
                {
                    // this line fits, just add it
                    m_Lines.Add(line);
                }
            }
        }
        #endregion

        public MessageBoxResult HandleClick(int x, int y)
        {
            if (m_szCheckboxText != null)
            {
                if (m_CheckboxRect.Contains(x, y))
                {
                    m_bCheckboxChecked = !m_bCheckboxChecked;
                }
            }

            foreach (UIButton b in m_Buttons)
            {
                if (b.Click(x, y))
                {
                    return (MessageBoxResult)b.ButtonID;
                }
            }

            return MessageBoxResult.NoResult;
        }

        #region Drawing
        public void Draw(SpriteBatch sb)
        {
            // Draw dialog frame
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);

            // Draw text lines
            int iY = m_Rect.Top + m_iMarginTopBottom;
            foreach (string line in m_Lines)
            {
                Vector2 strSize = Assets.DialogFont.MeasureString(line);

                sb.DrawString(Assets.DialogFont, line, new Vector2(m_iCenterDialogX - (strSize.X * 0.5f), iY), m_TextColor);
                iY += m_iTextLineHeight;
            }

            // Draw Checkbox
            if (m_szCheckboxText != null)
            {
                sb.Draw(Assets.CheckBox, new Rectangle(m_CheckboxRect.Left, m_CheckboxRect.Top, m_iCheckboxSize, m_iCheckboxSize), Color.White);
                if( m_bCheckboxChecked )
                    sb.Draw(Assets.Check, new Rectangle(m_CheckboxRect.Left, m_CheckboxRect.Top, m_iCheckboxSize, m_iCheckboxSize), Color.White);

                sb.DrawString(Assets.HelpFont, m_szCheckboxText, m_CheckboxTextLocation, Color.LightGray);
            }

            // Draw buttons
            foreach (UIButton b in m_Buttons)
            {
                b.Draw(sb);
            }
        }
        #endregion

        #region Accessors
        public Color TextColor
        {
            get { return m_TextColor; }
            set { m_TextColor = value; }
        }

        public int Context
        {
            get { return m_iContext; }
        }

        public bool Checkbox
        {
            get { return m_bCheckboxChecked; }
            set { m_bCheckboxChecked = value; }
        }
        #endregion
    }
}
