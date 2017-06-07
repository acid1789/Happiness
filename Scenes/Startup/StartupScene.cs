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
        DisplayNameDialog m_DisplayNameDialog;

        Rectangle m_LogoRectangle;
        string m_szCreditLine;
        string m_szMusicCreditLine;
        string m_szArtistCreditLine;
        Vector2 m_vCreditPosition;
        Vector2 m_vMusicCreditPosition;
        Vector2 m_vArtistCreditPosition;

        GameInfoValidator m_GIV;
        Rectangle m_WaitRect;
        Point m_WaitTextCenter;
        string m_szWaitText;
        
        public StartupScene(Happiness game) : base(game)
        {
            m_Step = SignInStep.CachedDataCheck;
            
            //LoadStartupDetails();
            m_GIV = GameInfoValidator.Instance;
            m_GIV.BeginLoadFromDisk();

            game.SoundManager.PlayMainMenuMusic();
            game.SoundManager.PlaySound(SoundManager.SEInst.Happiness);

            InputController.IC.OnClick += IC_OnClick;
            InputController.IC.OnKeyUp += IC_OnKeyUp;

            int logoSize = (int)(Constants.Startup_LogoSize * game.ScreenHeight);
            m_LogoRectangle = new Rectangle(0, 0, logoSize, logoSize);

            m_szCreditLine = "A logic puzzle game by Ron O'Hara";
            m_szMusicCreditLine = "Muisc by Ronald Jenkees (www.ronaldjenkees.com)";
            m_szArtistCreditLine = "Artwork by: <your name here!> (Artist Wanted)";

            int creditX = (int)(Constants.Startup_CreditX * game.ScreenWidth);
            int creditY = (int)(Constants.Startup_CreditY * game.ScreenHeight);
            int musicY = (int)(Constants.Startup_MusicCreditY * game.ScreenHeight);
            m_vCreditPosition.X = creditX;
            m_vCreditPosition.Y = game.ScreenHeight - creditY;
            m_vMusicCreditPosition.Y = game.ScreenHeight - musicY;
            m_vMusicCreditPosition.X = creditX + 20;

            m_vArtistCreditPosition.X = game.ScreenWidth / 2;
            m_vArtistCreditPosition.Y = game.ScreenHeight - musicY;


            int waitIconSize = (int)(Constants.Startup_WaitIconSize * game.ScreenWidth);
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
                case GameInfoValidator.LoadStatus.OAuthFailed:
                    ShowSignInDialog();
                    m_SignInDialog.Status = "Failed to authenticate!";
                    break;
                case GameInfoValidator.LoadStatus.Loading:
                    m_szWaitText = "Loading";
                    break;
                case GameInfoValidator.LoadStatus.FetchingFromServer:
                    m_szWaitText = "Fetching From Server";
                    break;                    
                case GameInfoValidator.LoadStatus.ServerDeniedAccess:
                    ShowSignInDialog();
                    m_SignInDialog.Status = "Incorrect email or password.";
                    break;
                case GameInfoValidator.LoadStatus.ServerFetchComplete:
                    m_szWaitText = "Fetched from server";
                    Game.TheGameInfo = m_GIV.m_GameInfo;
                    GotoHubScene();
                    break;
                case GameInfoValidator.LoadStatus.ServerUnreachable:
                case GameInfoValidator.LoadStatus.NoFile:
                    ShowSignInDialog();
                    break;
            }


            if (m_SignInDialog != null)
                m_SignInDialog.Update(gameTime);           
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
            Happiness.ShadowString(spriteBatch, Assets.DialogFont, m_szArtistCreditLine, m_vArtistCreditPosition, Color.LightGray);

            // Show sign in dialog
            if (m_SignInDialog != null)
                m_SignInDialog.Draw(spriteBatch);
            
            if (m_szWaitText != null)
            {
                Assets.WaitIcon.Draw(spriteBatch, m_WaitRect, Color.White);
                Happiness.ShadowString(spriteBatch, Assets.HelpFont, m_szWaitText, Happiness.CenterText(m_WaitTextCenter, m_szWaitText, Assets.HelpFont), Color.White);
            }
        }

        void ShowSignInDialog()
        {
            m_szWaitText = null;
            m_GIV.Reset();
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
            m_szWaitText = "Signing In...";

            switch (m_SignInDialog.AuthType)
            {
                case SignInDialog.SignInType.Email: DoEmailSignIn(); break;
                case SignInDialog.SignInType.Google: DoGoogleSignIn(); break;
                case SignInDialog.SignInType.Facebook: DoFacebookSignIn(); break;
            }
            m_SignInDialog = null;
        }

        void DoEmailSignIn()
        {
            bool createMode = m_SignInDialog.EmailCreate;
            string email = m_SignInDialog.Email;
            string password = m_SignInDialog.Password;
            
            m_GIV.RequestFromServer(email, password, createMode);
        }

        void DoGoogleSignIn()
        {
            m_GIV.StartOAuth();
            string[] credentials = GoogleAuth.DoAuth();
            m_GIV.FinishOAuth(credentials[0], credentials[1], true);
        }

        void DoFacebookSignIn()
        {
            m_GIV.StartOAuth();
            string[] credentials = FacebookAuth.DoAuth();
            m_GIV.FinishOAuth(credentials[0], credentials[1], false);
        }
        #endregion
    }
}
