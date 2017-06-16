using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness
{
    class DisplayNameDialog
    {
        Rectangle m_Rect;

        string m_szTitle;
        Vector2 m_vTitlePosition;

        string m_szSecondaryText;
        Vector2 m_vSecondaryPosition;

        UIInputField m_Input;

        UIButton[] m_Buttons;

        public DisplayNameDialog(int screenWidth, int screenHeight)
        {
            int centerScreenX = screenWidth >> 1;
            int centerScreenY = screenHeight >> 1;

            int width = (int)(Constants.DisplayNameDialog_Width * screenWidth);
            int height = (int)(Constants.DisplayNameDialog_Height * screenHeight);
            int topMargin = (int)(Constants.DisplayNameDialog_MarginTopBottom * screenHeight);
            int lineSpace = (int)(Constants.DisplayNameDialog_LineSpace * screenHeight);
            int inputWidth = (int)(Constants.DisplayNameDialog_InputWidth * screenWidth);
            int inputHeight = (int)(Constants.DisplayNameDialog_InputHeight * screenHeight);
            int buttonWidth = (int)(Constants.DisplayNameDialog_ButtonWidth * screenWidth);
            int buttonHeight= (int)(Constants.DisplayNameDialog_ButtonHeight * screenHeight);
            
            m_Rect = new Rectangle(centerScreenX - (width >> 1), centerScreenY - (height >> 1), width, height);

            m_szTitle = "Choose a display name";
            Vector2 size = Assets.DialogFont.MeasureString(m_szTitle);
            m_vTitlePosition = new Vector2(centerScreenX - (size.X * 0.5f), m_Rect.Top + topMargin);

            m_szSecondaryText = "Pick a name for other players to see you as";
            m_vSecondaryPosition.Y = m_vTitlePosition.Y + size.Y + lineSpace;
            size = Assets.HelpFont.MeasureString(m_szSecondaryText);
            m_vSecondaryPosition.X = centerScreenX - (size.X * 0.5f);

            m_Input = new UIInputField(new Rectangle(centerScreenX - (inputWidth >> 1), (int)(m_vSecondaryPosition.Y + size.Y + lineSpace + lineSpace), inputWidth, inputHeight));
            m_Input.Focused = true;

            int buttonY = m_Input.Rect.Bottom + lineSpace;
            height = (buttonY + buttonHeight + topMargin) - m_Rect.Top;
            m_Buttons = new UIButton[2];
            m_Buttons[0] = new UIButton(0, "Done", Assets.HelpFont, new Rectangle(centerScreenX + (buttonWidth >> 1), buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);
            m_Buttons[1] = new UIButton(1, "Cancel", Assets.HelpFont, new Rectangle(centerScreenX - ((buttonWidth >> 1) + buttonWidth), buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);
            
        }

        // Returns false if dialog is done
        public MessageBoxResult HandleClick(int x, int y)
        {
            if( m_Input.Rect.Contains(x, y) )
                m_Input.Focused = true;

            foreach (UIButton b in m_Buttons)
            {
                if (b.Click(x, y))
                {
                    if( b.ButtonID == 0 )
                        return MessageBoxResult.OK;
                    else
                        return MessageBoxResult.Cancel;
                }
            }

            return MessageBoxResult.NoResult;
        }

        public void HandleKey(KeyArgs key)
        {
            if( m_Input.Focused )
                m_Input.HandleKey(key);
        }

        public void Update(double deltaTime)
        {
            if (m_Input.Focused)
                m_Input.Update(deltaTime);

            m_Buttons[0].Enabled = (m_Input.Text != null && m_Input.Text.Length > 0);
        }

        public void Draw(Renderer sb)
        {
            // Draw frame
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);

            // Draw text
            Happiness.ShadowString(sb, Assets.DialogFont, m_szTitle, m_vTitlePosition, Color.Goldenrod);
            Happiness.ShadowString(sb, Assets.HelpFont, m_szSecondaryText, m_vSecondaryPosition, Color.LightGray);

            // Draw input field
            m_Input.Draw(sb);

            // Draw buttons
            foreach( UIButton b in m_Buttons )
                b.Draw(sb);
        }

        #region Accessors
        public string Name
        {
            get { return m_Input.Text; }
        }
        #endregion
    }
}
