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
        None,
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
        FormattedLine[] m_Lines;
        UIButton[] m_Buttons;

        Rectangle m_Rect;
        int m_iMarginTopBottom;
        int m_iMarginLeftRight;
        int m_iTextLineHeight;
        int m_iCenterDialogX;

        UICheckbox m_CheckBox;

        Color m_TextColor;

        #region Initialization
        public MessageBox(string message, MessageBoxButtons buttons, int context, int screenWidth, int screenHeight, string checkboxText = null)
        {
            m_iContext = context;
            m_TextColor = Color.White;
            
            int iCheckboxSize = (int)(Constants.MessageBox_CheckboxSize * screenHeight);

            m_iMarginLeftRight = (int)(Constants.MessageBox_LeftRightMargin * screenWidth);
            m_iMarginTopBottom = (int)(Constants.MessageBox_TopBottomMargin * screenHeight);

            int checkboxHeight = checkboxText == null ? 0 : m_iMarginTopBottom + iCheckboxSize;

            int width = (int)(Constants.MessageBox_Width * screenWidth);            
            m_Lines = Happiness.FormatLines(width - (m_iMarginLeftRight * 2), message, Assets.DialogFont);

            int buttonWidth = (int)(Constants.MessageBox_ButtonWidth * screenWidth);
            int buttonHeight = (int)(Constants.MessageBox_ButtonHeight * screenHeight);

            int lineSpace = (int)(Constants.MessageBox_LineSpace * screenHeight);
            Vector2 testTextSize = Assets.DialogFont.MeasureString("TEST");
            m_iTextLineHeight = (int)testTextSize.Y + lineSpace;
            
            int buttonAreaHeight = buttons == MessageBoxButtons.None ? 0 : m_iMarginTopBottom + buttonHeight;
            int dialogHeight = (m_iMarginTopBottom + m_iMarginTopBottom) + (m_iTextLineHeight * m_Lines.Length) + buttonAreaHeight + checkboxHeight;

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
                case MessageBoxButtons.None:
                    break;
            }

            if (checkboxText != null)
            {
                m_CheckBox = new UICheckbox(checkboxText, 0, buttonY - (m_iMarginTopBottom + iCheckboxSize), screenHeight);
                m_CheckBox.Left = m_iCenterDialogX - (m_CheckBox.Rect.Width >> 1);
            }
        }        
        #endregion

        public MessageBoxResult HandleClick(int x, int y)
        {
            if (m_CheckBox != null)
            {
                m_CheckBox.HandleClick(x, y);
            }

            if (m_Buttons != null)
            {
                foreach (UIButton b in m_Buttons)
                {
                    if (b.Click(x, y))
                    {
                        return (MessageBoxResult)b.ButtonID;
                    }
                }
            }

            return MessageBoxResult.NoResult;
        }

        public Rectangle GetButtonRect(int button)
        {
            return m_Buttons[button].Rect;
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
            foreach (FormattedLine line in m_Lines)
            {
                Vector2 strSize = line.Size(Assets.DialogFont);
                
                line.Draw(sb, Assets.DialogFont, new Vector2(m_iCenterDialogX - (strSize.X * 0.5f), iY), m_TextColor);
                iY += m_iTextLineHeight;
            }

            // Draw Checkbox
            if (m_CheckBox != null)
            {
                m_CheckBox.Draw(sb);
            }

            // Draw buttons
            if (m_Buttons != null)
            {
                foreach (UIButton b in m_Buttons)
                {
                    b.Draw(sb);
                }
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
            get { return m_CheckBox != null ? m_CheckBox.Checked : false; }
            set { m_CheckBox.Checked = value; }
        }
        #endregion
    }
}
