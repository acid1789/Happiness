using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            Finished
        }

        SignInStep m_Step;
        SignInDialog m_SignInDialog;
        MessageBox m_MessageBox;

        public StartupScene(Happiness game) : base(game)
        {
            m_Step = SignInStep.InputCredentials;
            m_SignInDialog = new SignInDialog(game.ScreenWidth, game.ScreenHeight);
            m_SignInDialog.OnSignIn += M_SignInDialog_OnSignIn;
            m_SignInDialog.OnExit += M_SignInDialog_OnExit;
            m_SignInDialog.OnSkip += M_SignInDialog_OnSkip;

            InputController.IC.OnClick += IC_OnClick;
            InputController.IC.OnKeyUp += IC_OnKeyUp;            
        }

        public override void Shutdown()
        {
            base.Shutdown();

            InputController.IC.OnClick -= IC_OnClick;
            InputController.IC.OnKeyUp -= IC_OnKeyUp;
        }

        private void IC_OnClick(object sender, DragArgs e)
        {
            if (m_MessageBox != null)
            {
                MessageBoxResult res = m_MessageBox.HandleClick(e.CurrentX, e.CurrentY);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        NetworkManager.Net.SignIn(m_SignInDialog.Email, m_SignInDialog.Password, "HU_" + ((ushort)DateTime.Now.Ticks).ToString());
                        m_SignInDialog.Status = "Creating Account...";
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
            else if ( m_SignInDialog != null )
                m_SignInDialog.HandleClick(e.CurrentX, e.CurrentY);
        }

        private void IC_OnKeyUp(object sender, KeyArgs e)
        {
            if( m_SignInDialog != null )
                m_SignInDialog.HandleKeyUp(e);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if( m_SignInDialog != null )
                m_SignInDialog.Update(gameTime);
            
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

            if (m_Step == SignInStep.Finished)
            {
            }
            else
            {
                // Show sign in dialog
                if(m_SignInDialog != null )
                    m_SignInDialog.Draw(spriteBatch);
            }

            if(m_MessageBox != null )
                m_MessageBox.Draw(spriteBatch);
        }

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
