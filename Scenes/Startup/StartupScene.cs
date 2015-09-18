using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Happiness
{
    public class StartupScene : Scene
    {
        enum SignInStep
        {
            InputCredentials,
            Connecting,
            WaitingForSignIn,
            WaitingForGameData,
            Finished,
            GettingDisplayName,
        }

        SignInStep m_Step;
        SignInDialog m_SignInDialog;
        MessageBox m_MessageBox;
        DisplayNameDialog m_DisplayNameDialog;

        Rectangle m_LogoRectangle;
        string m_szCreditLine;
        string m_szMusicCreditLine;
        Vector2 m_vCreditPosition;
        Vector2 m_vMusicCreditPosition;

        public StartupScene(Happiness game) : base(game)
        {
            m_Step = SignInStep.InputCredentials;
            m_SignInDialog = new SignInDialog(game.ScreenWidth, game.ScreenHeight);
            m_SignInDialog.OnSignIn += M_SignInDialog_OnSignIn;
            m_SignInDialog.OnExit += M_SignInDialog_OnExit;
            m_SignInDialog.OnSkip += M_SignInDialog_OnSkip;

            LoadStartupDetails();

            InputController.IC.OnClick += IC_OnClick;
            InputController.IC.OnKeyUp += IC_OnKeyUp;

            int logoSize = (int)(Constants.Startup_LogoSize * game.ScreenHeight);
            m_LogoRectangle = new Rectangle(0, 0, logoSize, logoSize);

            m_szCreditLine = "A logic puzzle game by Ron O'Hara";
            m_szMusicCreditLine = "Muisc by Ronald Jenkees (www.ronaldjenkees.com)";

            int creditX = (int)(Constants.Startup_CreditX * game.ScreenWidth);
            int creditY = (int)(Constants.Startup_CreditY * game.ScreenHeight);
            int musicY = (int)(Constants.Startup_MusicCreditY * game.ScreenHeight);
            m_vCreditPosition.X = creditX;
            m_vCreditPosition.Y = game.ScreenHeight - creditY;
            m_vMusicCreditPosition.Y = game.ScreenHeight - musicY;
            m_vMusicCreditPosition.X = creditX + 20;
        }

        public override void Shutdown()
        {
            base.Shutdown();

            InputController.IC.OnClick -= IC_OnClick;
            InputController.IC.OnKeyUp -= IC_OnKeyUp;
        }

        #region Inupt
        private void IC_OnClick(object sender, DragArgs e)
        {
            if (m_MessageBox != null)
            {
                MessageBoxResult res = m_MessageBox.HandleClick(e.CurrentX, e.CurrentY);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        m_DisplayNameDialog = new DisplayNameDialog(Game.ScreenWidth, Game.ScreenHeight);
                        m_Step = SignInStep.GettingDisplayName;
                        m_SignInDialog.Status = null;                        
                        m_MessageBox = null;
                        break;
                    case MessageBoxResult.No:
                        m_MessageBox = null;
                        m_SignInDialog.Status = null;
                        m_SignInDialog.InputEnabled = true;
                        m_Step = SignInStep.InputCredentials;
                        break;
                }
            }
            else if (m_DisplayNameDialog != null)
            {
                MessageBoxResult res = m_DisplayNameDialog.HandleClick(e.CurrentX, e.CurrentY);
                if (res != MessageBoxResult.NoResult)
                {
                    if (res == MessageBoxResult.OK)
                    {
                        NetworkManager.Net.SignIn(m_SignInDialog.Email, m_SignInDialog.Password, m_DisplayNameDialog.Name);
                        m_SignInDialog.Status = "Creating Account...";
                        m_SignInDialog.StatusColor = Color.Green;
                        m_Step = SignInStep.WaitingForSignIn;
                    }
                    m_DisplayNameDialog = null;
                }
            }
            else if (m_SignInDialog != null)
                m_SignInDialog.HandleClick(e.CurrentX, e.CurrentY);
        }

        private void IC_OnKeyUp(object sender, KeyArgs e)
        {
            if( m_SignInDialog != null )
                m_SignInDialog.HandleKeyUp(e);
            if(m_DisplayNameDialog != null )
                m_DisplayNameDialog.HandleKey(e);
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if( m_SignInDialog != null )
                m_SignInDialog.Update(gameTime);

            if(m_DisplayNameDialog != null )
                m_DisplayNameDialog.Update(gameTime);

            switch (m_Step)
            {
                case SignInStep.InputCredentials:
                    break;
                case SignInStep.Connecting:
                    if (NetworkManager.Net.Connected)
                    {
                        // Done connecting, now sign in
                        NetworkManager.Net.SignIn(m_SignInDialog.Email, m_SignInDialog.Password, null);
                        m_SignInDialog.Status = "Signing In...";
                        m_Step = SignInStep.WaitingForSignIn;
                    }
                    break;
                case SignInStep.WaitingForSignIn:
                    if (NetworkManager.Net.SignInState != NetworkManager.SignInStatus.CredentialsSent)
                    {
                        switch (NetworkManager.Net.SignInState)
                        {
                            case NetworkManager.SignInStatus.InvalidAccount:
                                if (m_MessageBox == null)
                                {
                                    string message = string.Format("An account for email address '{0}' does not exist.\nWould you like to create one now?", m_SignInDialog.Email);
                                    m_MessageBox = new MessageBox(message, MessageBoxButtons.YesNo, (int)NetworkManager.SignInStatus.InvalidAccount, Game.ScreenWidth, Game.ScreenHeight);
                                }
                                break;
                            case NetworkManager.SignInStatus.InvalidPassword:
                                m_SignInDialog.Status = "Invalid Account or Password";
                                m_SignInDialog.StatusColor = Color.Red;
                                m_SignInDialog.InputEnabled = true;
                                m_Step = SignInStep.InputCredentials;
                                break;
                            case NetworkManager.SignInStatus.SignedIn:
                                NetworkManager.Net.FetchGameData();
                                m_SignInDialog.StatusColor = Color.Green;
                                m_SignInDialog.Status = "Waiting for game data...";
                                m_Step = SignInStep.WaitingForGameData;
                                break;
                        }
                    }
                    break;
                case SignInStep.WaitingForGameData:
                    if (NetworkManager.Net.GameData != null)
                    {
                        m_Step = SignInStep.Finished;
                        SaveStartupDetails();
                        GotoHubScene();
                    }
                    break;
                case SignInStep.Finished:
                default:
                    break;
            }
        }

        void GotoHubScene()
        {
            Game.GotoScene(new HubScene(Game));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw background
            spriteBatch.Draw(Assets.Background, new Rectangle(0, 0, Game.ScreenWidth, Game.ScreenHeight), Color.White);

            // Draw Logo
            spriteBatch.Draw(Assets.Logo, m_LogoRectangle, Color.White);

            // Draw Credits
            Happiness.ShadowString(spriteBatch, Assets.MenuFont, m_szCreditLine, m_vCreditPosition, Color.Goldenrod);
            Happiness.ShadowString(spriteBatch, Assets.DialogFont, m_szMusicCreditLine, m_vMusicCreditPosition, Color.LightGray);

            if (m_Step == SignInStep.Finished)
            {
            }
            else
            {
                // Show sign in dialog
                if(m_SignInDialog != null )
                    m_SignInDialog.Draw(spriteBatch);
            }

            if(m_DisplayNameDialog != null )
                m_DisplayNameDialog.Draw(spriteBatch);

            if (m_MessageBox != null )
                m_MessageBox.Draw(spriteBatch);
        }

        #region Startup Details
        static string s_StartupFile = "startup.info";
        void LoadStartupDetails()
        {
            if (File.Exists(s_StartupFile))
            {
                FileStream fs = File.OpenRead(s_StartupFile);
                BinaryReader br = new BinaryReader(fs);

                m_SignInDialog.Email = br.ReadString();
                m_SignInDialog.Password = br.ReadString();
                m_SignInDialog.RememberMe = br.ReadBoolean();

                br.Close();
            }
        }

        void SaveStartupDetails()
        {
            FileStream fs = File.Open(s_StartupFile, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            if (m_SignInDialog.RememberMe)
            {
                bw.Write(m_SignInDialog.Email);
                bw.Write(m_SignInDialog.Password);
            }
            else
            {
                bw.Write("");
                bw.Write("");
            }
            bw.Write(m_SignInDialog.RememberMe);

            bw.Close();
        }
        #endregion

        #region Dialog Events
        private void M_SignInDialog_OnExit(object sender, EventArgs e)
        {
            Game.ExitGame();
        }

        private void M_SignInDialog_OnSignIn(object sender, EventArgs e)
        {
            if (m_Step == SignInStep.InputCredentials)
            {
                if (m_SignInDialog.Email == null || m_SignInDialog.Email.Length <= 0)
                {
                    m_SignInDialog.StatusColor = Color.Red;
                    m_SignInDialog.Status = "Please enter an email address.";
                }
                else if (m_SignInDialog.Password == null || m_SignInDialog.Password.Length <= 0)
                {
                    m_SignInDialog.StatusColor = Color.Red;
                    m_SignInDialog.Status = "Please enter a password.";
                }
                else
                {
                    m_SignInDialog.InputEnabled = false;
                    m_SignInDialog.Status = "Connecting to server...";
                    m_SignInDialog.StatusColor = Color.Green;
                    m_Step = SignInStep.Connecting;

                    NetworkManager.Net.Connect("127.0.0.1", 1255);
                }
            }
            else
            {
                // Cancel
            }
        }

        private void M_SignInDialog_OnSkip(object sender, EventArgs e)
        {
            // Disable networking
            NetworkManager.Net.Disabled = true;

            // Load static gamedata
            NetworkManager.Net.LoadStaticData();   
            
            GotoHubScene();         
        }
        #endregion
    }
}
