using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness
{
    class SignInDialog
    {
        enum Mode
        {
            SignInButtons,
            SigningIn,
            EmailInput
        }

        public enum SignInType
        {
            Email,
            Google,
            Facebook,

            None
        }

        Rectangle m_Rect;
        int m_iCenterDialogX;
        int m_iMargin;
        bool m_bInputEnabled;

        string m_szTitleText;
        Vector2 m_vTitlePosition;

        string m_szAccountStatusText;
        Vector2 m_vAccountStatusPosition;
        UIButton m_AccountStatusButton;

        string m_szEmailText;
        UIInputField m_Email;
        Vector2 m_vEmailLabelPosition;

        string m_szPasswordText;
        UIInputField m_Password;
        Vector2 m_vPasswordLabelPosition;

        string m_szPasswordText2;
        UIInputField m_Password2;
        Vector2 m_vPassword2LabelPosition;
        
        UIButton[] m_DialogButtons;
        
        string m_szStatusText;
        Color m_StatusColor;
        Vector2 m_vStatusPosition;
        int m_iStatusOffsetY;

        SignInType m_SignInType;
        Mode m_Mode;
        UIButton[] m_SignInButtons;
        bool m_bEmailCreate = true;

        public event EventHandler OnSignIn;
        public event EventHandler OnExit;


        public SignInDialog(int screenWidth, int screenHeight)
        {
            m_Mode = Mode.SignInButtons;

            m_iCenterDialogX = (screenWidth >> 1);
            int centerY = (screenHeight >> 1);

            int width = (int)(Constants.SignInDialog_Width * screenWidth);
            int height = (int)(Constants.SignInDialog_Height * screenHeight);
            int left = m_iCenterDialogX - (width >> 1);
            int top = centerY - (height >> 1);
            m_Rect = new Rectangle(left, top, width, height);

            m_iMargin = (int)(Constants.SignInDialog_Margin * screenHeight);
            m_iStatusOffsetY = (int)(Constants.SignInDialog_StatusOffsetY * screenHeight);

            SetTitle("Sign In");

            int iconSize = (int)(Constants.SignInDialog_AuthSize * screenHeight);
            int buttonWidth = (int)(Constants.SignInDialog_IconButtonWidth * screenWidth);
            int iconX = (int)(Constants.SignInDialog_IconMarginLeftRight * screenWidth);
            int iconY = (int)(Constants.SignInDialog_IconMarginTopBottom * screenHeight);
            int buttonMargin = (int)(Constants.SignInDialog_IconButtonGap * screenHeight);
            int buttonHeight = iconSize + (iconY * 2);
            int buttonLeft = m_iCenterDialogX - (buttonWidth / 2);

            int buttonsHeight = (buttonHeight * 4) + (buttonMargin * 3);
            int buttonsTop = (centerY - (buttonsHeight / 2)) + 30;


            m_SignInButtons = new UIButton[4];
            int buttonTop = buttonsTop;
            m_SignInButtons[0] = new IconButton(0, new Rectangle(buttonLeft, buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar,
                                                    new Rectangle(buttonLeft + iconX, buttonTop + iconY, iconSize, iconSize), Assets.Facebook,
                                                    "Facebook\nSign In", Assets.DialogFont);
            buttonTop += buttonHeight + buttonMargin;
            m_SignInButtons[1] = new IconButton(1, new Rectangle(buttonLeft, buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar,
                                                    new Rectangle(buttonLeft + iconX, buttonTop + iconY, iconSize, iconSize), Assets.Google,
                                                    "Google\nSign In", Assets.DialogFont);
            buttonTop += buttonHeight + buttonMargin;
            m_SignInButtons[2] = new IconButton(2, new Rectangle(buttonLeft, buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar,
                                                    new Rectangle(buttonLeft + iconX, buttonTop + iconY, iconSize, iconSize), Assets.Email,
                                                    "Email\nSign In", Assets.DialogFont);
            buttonTop += buttonHeight + buttonMargin;
            m_SignInButtons[3] = new UIButton(3, "Exit", Assets.DialogFont, new Rectangle(buttonLeft, buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar);

            
            int inputFieldWidth = (int)(Constants.SignInDialog_InputWidth * screenWidth);
            int inputFieldLeft = m_Rect.Left + (int)(Constants.SignInDialog_InputLeft * screenWidth);
            int inputFieldTop = m_Rect.Top + (int)(Constants.SignInDialog_InputTop * screenHeight);
            m_szEmailText = "Email:";
            Vector2 emailSize = Assets.HelpFont.MeasureString(m_szEmailText);
            m_vEmailLabelPosition = new Vector2(inputFieldLeft - (emailSize.X + 2), inputFieldTop);
            m_Email = new UIInputField(new Rectangle(inputFieldLeft, inputFieldTop, inputFieldWidth, (int)emailSize.Y));

            int passwordY = m_Email.Rect.Bottom + m_iMargin;
            m_szPasswordText = "Password:";
            Vector2 passSize = Assets.HelpFont.MeasureString(m_szPasswordText);
            m_vPasswordLabelPosition = new Vector2(inputFieldLeft - (passSize.X + 2), passwordY);
            m_Password = new UIInputField(new Rectangle(inputFieldLeft, passwordY, inputFieldWidth, (int)passSize.Y), true);

            passwordY = m_Password.Rect.Bottom + m_iMargin;
            m_szPasswordText2 = "Confirm Password:";
            passSize = Assets.HelpFont.MeasureString(m_szPasswordText2);
            m_vPassword2LabelPosition = new Vector2(inputFieldLeft - (passSize.X + 2), passwordY);
            m_Password2 = new UIInputField(new Rectangle(inputFieldLeft, passwordY, inputFieldWidth, (int)passSize.Y), true);


            m_StatusColor = Color.White;

            int btnY = m_Rect.Bottom - (int)(Constants.SignInDialog_ButtonGap * screenHeight);
            int btnLargeW = (int)(Constants.SignInDialog_ButtonWidthLarge * screenWidth);
            int btnSmallW = (int)(Constants.SignInDialog_ButtonWidthSmall * screenWidth);
            int btnH = (int)(Constants.SignInDialog_ButtonHeight * screenHeight);
            m_DialogButtons = new UIButton[2];
            m_DialogButtons[0] = new UIButton(0, "Sign In", Assets.DialogFont, new Rectangle(m_iCenterDialogX + 20, btnY, btnLargeW, btnH), Assets.ScrollBar);
            m_DialogButtons[1] = new UIButton(1, "Cancel", Assets.DialogFont, new Rectangle(m_iCenterDialogX - 20 - btnLargeW, btnY, btnLargeW, btnH), Assets.ScrollBar);

            SetupAccountStatusMode(true);

            m_Email.Focused = true;

            m_bInputEnabled = true;
            int waitSize = (int)(Constants.SignInDialog_WaitIconSize * screenHeight);
            int waitSpace = (int)(Constants.SignInDialog_WaitIconSpace * screenHeight);
            //m_WaitRect = new Rectangle(m_DialogButtons[2].Rect.Right - waitSize, m_DialogButtons[2].Rect.Top - (waitSpace + waitSize), waitSize, waitSize);
            
        }

        #region Input
        public void HandleClick(int x, int y)
        {
            switch (m_Mode)
            {
                case Mode.SignInButtons:
                    foreach (UIButton b in m_SignInButtons)
                    {
                        if (b.Click(x, y))
                            HandleSignInButton(b.ButtonID);
                    }
                    break;
                case Mode.EmailInput:
                    m_Email.Focused = false;
                    m_Password.Focused = false;
                    m_Password2.Focused = false;
                    if (m_Email.Rect.Contains(x, y))
                        m_Email.Focused = true;
                    else if (m_Password.Rect.Contains(x, y))
                        m_Password.Focused = true;
                    else if( m_bEmailCreate && m_Password2.Rect.Contains(x, y) )
                        m_Password2.Focused = true;

                    if( m_Email.Focused || m_Password.Focused || m_Password2.Focused )
                        Happiness.Game.SoundManager.PlaySound(SoundManager.SEInst.MenuNavigate);

                    if (m_AccountStatusButton.Click(x, y))
                    {
                        SetupAccountStatusMode(!m_bEmailCreate);
                    }

                    foreach (UIButton b in m_DialogButtons)
                    {
                        if (b.Click(x, y))
                        {
                            HandleDialogButton(b.ButtonID);
                        }
                    }

                    break;
            }
        }

        void HandleSignInButton(int buttonID)
        {
            switch (buttonID)
            {
                default:
                case 0: // Facebook
                    m_SignInType = SignInType.Facebook;
                    if (OnSignIn != null)
                        OnSignIn(this, null);
                    break;
                case 1: // Google
                    m_SignInType = SignInType.Google;
                    if ( OnSignIn != null )
                        OnSignIn(this, null);
                    break;
                case 2: // Email
                    m_Mode = Mode.EmailInput;
                    SetupAccountStatusMode(m_bEmailCreate);
                    break;
                case 3: // Exit
                    if (OnExit != null)
                        OnExit(this, null);
                    break;
            }
        }

        void HandleDialogButton(int buttonID)
        {
            switch (buttonID)
            {
                case 0: // Sign In
                    m_SignInType = SignInType.Email;
                    if (OnSignIn != null)
                        OnSignIn(this, null);
                    break;
                case 1: // Cancel
                    m_Mode = Mode.SignInButtons;
                    SetTitle("Sign In");
                    break;
            }
        }

        public void HandleKeyUp(KeyArgs key)
        {
            UIInputField focused = null;
            if (m_Email.Focused)
                focused = m_Email;
            else if (m_Password.Focused)
                focused = m_Password;
            else if (m_Password2.Focused )
                focused = m_Password2;
            if (focused != null)
            {
                if (key.Key == Keys.Enter)
                {
                    if (OnSignIn != null)
                        OnSignIn(this, null);
                }
                else if (key.Key == Keys.Tab)
                {
                    if (m_Email.Focused)
                    {
                        m_Email.Focused = false;
                        m_Password.Focused = true;
                        m_Password2.Focused = false;
                    }
                    else if (m_Password.Focused)
                    {
                        if (m_bEmailCreate)
                        {
                            m_Email.Focused = false;
                            m_Password.Focused = false;
                            m_Password2.Focused = true;
                        }
                        else
                        {
                            m_Email.Focused = true;
                            m_Password.Focused = false;
                            m_Password2.Focused = false;
                        }
                    }
                    else if (m_Password2.Focused)
                    {
                        m_Email.Focused = true;
                        m_Password.Focused = false;
                        m_Password2.Focused = false;
                    }
                    Happiness.Game.SoundManager.PlaySound(SoundManager.SEInst.MenuNavigate);
                }
                else
                {
                    focused.HandleKey(key);
                }
            }
        }
        #endregion

        void SetTitle(string title)
        {
            m_szTitleText = title;
            Vector2 titleSize = Assets.MenuFont.MeasureString(m_szTitleText);
            m_vTitlePosition = new Vector2(m_iCenterDialogX - (titleSize.X * 0.5f), m_Rect.Top + m_iMargin);
        }

        void SetupAccountStatusMode(bool createMode)
        {
            m_bEmailCreate = createMode;
            SetTitle(m_bEmailCreate ? "Create Account" : "Sign In");
            m_szAccountStatusText = m_bEmailCreate ? "Already have an account?" : "New User?";
            Vector2 size = Assets.DialogFont.MeasureString(m_szAccountStatusText);
            m_vAccountStatusPosition = new Vector2(m_iCenterDialogX - (size.X * 0.5f), (m_bEmailCreate ? m_Password2.Rect.Bottom : m_Password.Rect.Bottom) + m_iMargin);


            int asWidth = 200;
            int asHeight = 32;
            int asTop = (int)(m_vAccountStatusPosition.Y + size.Y + m_iMargin + 1);
            int asLeft = m_iCenterDialogX - (asWidth / 2);
            m_AccountStatusButton = new UIButton(0, m_bEmailCreate ? "Sign In" : "Create Account", Assets.DialogFont, new Rectangle(asLeft, asTop,  asWidth, asHeight), Assets.ScrollBar);

            m_DialogButtons[0].Text = m_bEmailCreate ? "Create" : "Sign In";
        }

        public void Update(double gt)
        {
            if (m_Mode == Mode.EmailInput)
            {
                m_Email.Update(gt);
                m_Password.Update(gt);

                if( m_bEmailCreate )
                    m_Password2.Update(gt);
            }
        }

        public void Draw(Renderer sb)
        {
            // Draw frame & background
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);

            // Draw the title
            Happiness.ShadowString(sb, Assets.MenuFont, m_szTitleText, m_vTitlePosition, Color.Goldenrod);

            if( m_szStatusText != null )
                Happiness.ShadowString(sb, Assets.HelpFont, m_szStatusText, m_vStatusPosition, Color.Red);

            switch (m_Mode)
            {
                case Mode.SignInButtons:
                    foreach (UIButton button in m_SignInButtons)
                    {
                        button.Draw(sb);
                    }
                    break;
                case Mode.EmailInput:
                    DrawEmailInput(sb);
                    break;
            }
        }

        void DrawEmailInput(Renderer sb)
        {
            // Draw email field
            m_Email.Draw(sb);
            Happiness.ShadowString(sb, Assets.HelpFont, m_szEmailText, m_vEmailLabelPosition, Color.LightGray);

            // Draw password field
            m_Password.Draw(sb);
            Happiness.ShadowString(sb, Assets.HelpFont, m_szPasswordText, m_vPasswordLabelPosition, Color.LightGray);

            // Draw confirm password field
            if (m_bEmailCreate)
            {
                m_Password2.Draw(sb);
                Happiness.ShadowString(sb, Assets.HelpFont, m_szPasswordText2, m_vPassword2LabelPosition, Color.LightGray);
            }
            

            Happiness.ShadowString(sb, Assets.DialogFont, m_szAccountStatusText, m_vAccountStatusPosition, Color.Goldenrod);
            m_AccountStatusButton.Draw(sb);

            foreach( UIButton b in m_DialogButtons )
                b.Draw(sb);
        }        

        #region Accessors

        public string Status
        {
            get { return m_szStatusText; }
            set
            {
                m_szStatusText = value;
                Vector2 size = Assets.HelpFont.MeasureString(m_szStatusText);
                m_vStatusPosition = new Vector2(m_iCenterDialogX - (size.X * 0.5f), m_Rect.Top + m_iMargin + m_iStatusOffsetY);
            }
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

        public bool EmailCreate
        {
            get { return m_bEmailCreate; }
        }

        public SignInType AuthType
        {
            get { return m_SignInType; }
        }
        #endregion
    }
}
