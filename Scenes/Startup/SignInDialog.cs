using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Happiness
{
    class SignInDialog
    {
        Rectangle m_Rect;
        int m_iCenterDialogX;
        bool m_bInputEnabled;

        string m_szTitleText;
        Vector2 m_vTitlePosition;

        string m_szEmailText;
        UIInputField m_Email;
        Vector2 m_vEmailLabelPosition;

        string m_szPasswordText;
        UIInputField m_Password;
        Vector2 m_vPasswordLabelPosition;

        UICheckbox m_RememberMe;

        UIButton[] m_AuthButtons;
        UIButton[] m_DialogButtons;

        string m_szStatusText;
        float m_fStatusTextY;
        Color m_StatusColor;

        public event EventHandler OnSignIn;
        public event EventHandler OnExit;
        public event EventHandler OnSkip;


        public SignInDialog(int screenWidth, int screenHeight)
        {
            m_iCenterDialogX = (screenWidth >> 1);
            int centerY = (screenHeight >> 1);

            int width = (int)(Constants.SignInDialog_Width * screenWidth);
            int height = (int)(Constants.SignInDialog_Height * screenHeight);
            int left = m_iCenterDialogX - (width >> 1);
            int top = centerY - (height >> 1);
            m_Rect = new Rectangle(left, top, width, height);


            m_szTitleText = "Sign In";
            Vector2 titleSize = Assets.MenuFont.MeasureString(m_szTitleText);
            m_vTitlePosition = new Vector2(m_iCenterDialogX - (titleSize.X * 0.5f), m_Rect.Top + 20);

            int inputFieldWidth = width - 250;
            int inputFieldLeft = m_Rect.Left + 150;
            m_szEmailText = "Email:";
            Vector2 emailSize = Assets.HelpFont.MeasureString(m_szEmailText);
            m_vEmailLabelPosition = new Vector2(inputFieldLeft - (emailSize.X + 2), m_Rect.Top + 100);
            m_Email = new UIInputField(new Rectangle(inputFieldLeft, m_Rect.Top + 100, inputFieldWidth, (int)emailSize.Y));

            m_szPasswordText = "Password:";
            Vector2 passSize = Assets.HelpFont.MeasureString(m_szPasswordText);
            m_vPasswordLabelPosition = new Vector2(inputFieldLeft - (passSize.X + 2), m_Email.Rect.Bottom + 20);
            m_Password = new UIInputField(new Rectangle(inputFieldLeft, m_Email.Rect.Bottom + 20, inputFieldWidth, (int)passSize.Y), true);

            m_RememberMe = new UICheckbox("Remember Me", inputFieldLeft, m_Password.Rect.Bottom + 20, screenHeight);

            int authY = m_RememberMe.Rect.Bottom + 10;
            int authSize = 60;
            m_AuthButtons = new UIButton[2];
            m_AuthButtons[0] = new UIButton(0, null, null, new Rectangle(m_iCenterDialogX - 100, authY, authSize, authSize), Assets.Facebook);
            m_AuthButtons[1] = new UIButton(1, null, null, new Rectangle(m_iCenterDialogX + 40, authY, authSize, authSize), Assets.Google);

            m_StatusColor = Color.White;
            m_fStatusTextY = m_AuthButtons[0].Rect.Bottom + 5;

            int btnY = m_Rect.Bottom - 80;
            m_DialogButtons = new UIButton[3];
            m_DialogButtons[0] = new UIButton(0, "Sign In", Assets.DialogFont, new Rectangle(m_iCenterDialogX - 100, btnY, 200, 50), Assets.ScrollBar);
            m_DialogButtons[1] = new UIButton(1, "Exit", Assets.DialogFont, new Rectangle(m_Rect.Left + 40, btnY, 80, 50), Assets.ScrollBar);
            m_DialogButtons[2] = new UIButton(2, "Skip", Assets.DialogFont, new Rectangle(m_Rect.Right - 120, btnY, 80, 50), Assets.ScrollBar);
        }

        #region Input
        public void HandleClick(int x, int y)
        {
            m_Email.Focused = false;
            m_Password.Focused = false;
            if (m_Email.Rect.Contains(x, y))
                m_Email.Focused = true;
            else if (m_Password.Rect.Contains(x, y))
                m_Password.Focused = true;

            m_RememberMe.HandleClick(x, y);

            foreach (UIButton b in m_DialogButtons)
            {
                if (b.Click(x, y))
                {
                    HandleDialogButton(b.ButtonID);
                }
            }
        }

        void HandleDialogButton(int buttonID)
        {
            switch (buttonID)
            {
                case 0: // Sign In
                    if( OnSignIn != null )
                        OnSignIn(this, null);
                    break;
                case 1: // Exit
                    if( OnExit != null )
                        OnExit(this, null);
                    break;
                case 2:
                    if( OnSkip != null )
                        OnSkip(this, null);
                    break;
            }
        }

        public void HandleKeyUp(KeyArgs key)
        {
            UIInputField focused = null;
            if( m_Email.Focused  )
                focused = m_Email;
            else if( m_Password.Focused )
                focused = m_Password;
            if (focused != null)
            {
                if (key.Key == Keys.Enter)
                {
                }
                else if (key.Key == Keys.Tab)
                {
                    if (m_Email.Focused)
                    {
                        m_Email.Focused = false;
                        m_Password.Focused = true;
                    }
                        
                }
                else
                {
                    focused.HandleKey(key);
                }
            }
        }
        #endregion

        public void Update(GameTime gt)
        {
            m_Email.Update(gt);
            m_Password.Update(gt);
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw frame & background
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);

            // Draw the title
            Happiness.ShadowString(sb, Assets.MenuFont, m_szTitleText, m_vTitlePosition, Color.Goldenrod);

            // Draw email field
            m_Email.Draw(sb);
            Happiness.ShadowString(sb, Assets.HelpFont, m_szEmailText, m_vEmailLabelPosition, Color.LightGray);

            // Draw password field
            m_Password.Draw(sb);
            Happiness.ShadowString(sb, Assets.HelpFont, m_szPasswordText, m_vPasswordLabelPosition, Color.LightGray);

            // Draw remember me box
            m_RememberMe.Draw(sb);

            // Draw auth buttons
            foreach( UIButton b in m_AuthButtons )
                b.Draw(sb);

            // Draw status text
            if (m_szStatusText != null)
            {
                Vector2 size = Assets.DialogFont.MeasureString(m_szStatusText);
                Vector2 pos = new Vector2(m_iCenterDialogX - (size.X * 0.5f), m_fStatusTextY);
                Happiness.ShadowString(sb, Assets.DialogFont, m_szStatusText, pos, m_StatusColor);
            }

            // Draw dialog buttons
            foreach( UIButton b in m_DialogButtons )
                b.Draw(sb);
        }

        #region Accessors
        public bool RememberMe
        {
            get { return m_RememberMe.Checked; }
            set { m_RememberMe.Checked = value; }
        }

        public string Status
        {
            get { return m_szStatusText; }
            set { m_szStatusText = value; }
        }

        public Color StatusColor
        {
            get { return m_StatusColor; }
            set { m_StatusColor = value; }
        }

        public bool InputEnabled
        {
            get { return m_bInputEnabled; }
            set
            {
                m_bInputEnabled = value;
                m_Email.Enabled = value;
                m_Password.Enabled = value;
                m_RememberMe.Enabled = value;
                foreach( UIButton b in m_AuthButtons )
                    b.Enabled = value;

                m_DialogButtons[0].Text = value ? "Sign In" : "Cancel";
            }
        }

        public string Email
        {
            get { return m_Email.Text; }
            set { m_Email.Text = value; }
        }

        public string Password
        {
            get { return m_Password.Text; }
            set { m_Password.Text = value; }
        }
        #endregion
    }
}
