using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using HappinessNetwork;

namespace Happiness
{
    public class StartupScene : Scene
    {
        enum SignInStep
        {
            CachedDataCheck,
            EmailSignIn
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

        GameInfoValidator m_GIV;
        GameInfo m_GameInfo;
        Rectangle m_WaitRect;
        Point m_WaitTextCenter;
        string m_szWaitText;
        
        public StartupScene(Happiness game) : base(game)
        {
            m_Step = SignInStep.CachedDataCheck;
            
            //LoadStartupDetails();
            m_GIV = new GameInfoValidator();
            m_GIV.BeginLoadFromDisk();

            game.SoundManager.PlayMainMenuMusic();

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


            int waitIconSize = 200;
            m_WaitRect = new Rectangle((Game.ScreenWidth / 2) - (waitIconSize / 2), (Game.ScreenHeight / 2) - (waitIconSize / 2), waitIconSize, waitIconSize);
            m_WaitTextCenter = new Point(Game.ScreenWidth / 2, m_WaitRect.Bottom + 10);
            m_szWaitText = "Initializing";
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
            if (m_SignInDialog != null)
                m_SignInDialog.HandleClick(e.CurrentX, e.CurrentY);
        }

        private void IC_OnKeyUp(object sender, KeyArgs e)
        {
            if (m_SignInDialog != null)
                m_SignInDialog.HandleKeyUp(e);
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (m_GIV.Status)
            {
                case GameInfoValidator.LoadStatus.Idle:
                    m_szWaitText = null;
                    break;
                case GameInfoValidator.LoadStatus.Loading:
                    m_szWaitText = "Loading";
                    break;
                case GameInfoValidator.LoadStatus.FetchingFromServer:
                    m_szWaitText = "Fetching From Server";
                    break;
                case GameInfoValidator.LoadStatus.ServerDeniedAccess:
                    m_szWaitText = "Server said fuck off";
                    break;
                case GameInfoValidator.LoadStatus.ServerFetchComplete:
                    m_szWaitText = "Fetched from server";
                    break;
                case GameInfoValidator.LoadStatus.NoFile:
                    m_szWaitText = null;
                    ShowSignInDialog();
                    m_GIV.Reset();
                    break;
            }


            if (m_SignInDialog != null)
                m_SignInDialog.Update(gameTime);
            /*
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
                        */
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

            // Show sign in dialog
            if (m_SignInDialog != null)
                m_SignInDialog.Draw(spriteBatch);

            /*
                            if(m_DisplayNameDialog != null )
                                m_DisplayNameDialog.Draw(spriteBatch);

                            if (m_MessageBox != null )
                                m_MessageBox.Draw(spriteBatch);
                            */

            if (m_szWaitText != null)
            {
                Assets.WaitIcon.Draw(spriteBatch, m_WaitRect, Color.White);
                Happiness.ShadowString(spriteBatch, Assets.HelpFont, m_szWaitText, Happiness.CenterText(m_WaitTextCenter, m_szWaitText, Assets.HelpFont), Color.White);
            }
        }

        void ShowSignInDialog()
        {
            m_SignInDialog = new SignInDialog(Game.ScreenWidth, Game.ScreenHeight);
            m_SignInDialog.OnSignIn += M_SignInDialog_OnSignIn;
            m_SignInDialog.OnExit += M_SignInDialog_OnExit;
        }
        
        #region Dialog Events
        private void M_SignInDialog_OnExit(object sender, EventArgs e)
        {
            Game.ExitGame();
        }

        private void M_SignInDialog_OnSignIn(object sender, EventArgs e)
        {
            bool createMode = m_SignInDialog.EmailCreate;
            string email = m_SignInDialog.Email;
            string password = m_SignInDialog.Password;

            m_SignInDialog = null;
            m_szWaitText = "Signing In...";


            M_SignInDialog_OnSkip(null, null);
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
