using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HappinessNetwork;

namespace Happiness
{
    public class HubScene : Scene
    {
        enum MessageBoxContext
        {
            ResetTutorial,
            ExitGame,
            SignOut,
        }

        Tower[] m_Towers;

        UILabel m_LevelLabel;
        UILabel m_Level;
        UIProgressBar m_ExpBar;

        UIButton m_ResetTutorial;
        UIButton m_Options;
        UIButton m_SignOut;
        UIButton m_Exit;
        UIButton m_BuyCoins;
        UICoinsDisplay m_Coins;
        UIVIPDisplay m_VIP;

        FloorSelectDialog m_FloorSelect;

        SoundDialog m_SoundDialog;
        Options m_OptionsDialog;
        BuyCoinsModal m_CoinsDialog;
        VIPDialog m_VipDialog;

        MessageBox m_MessageBox;

        public HubScene(Happiness game) : base(game)
        {
            InputController.IC.OnClick += IC_OnClick;
            InputController.IC.OnDragBegin += IC_OnDragBegin;
            InputController.IC.OnDrag += IC_OnDrag;
            InputController.IC.OnScroll += IC_OnScroll;
            InputController.IC.OnKeyDown += IC_OnKeyDown;
            
            SoundManager.Inst.PlayMainMenuMusic();

            //m_SoundDialog = new SoundDialog(game.ScreenWidth, game.ScreenHeight, game);
            //game.SoundManager.StopMusic();

            // Setup Towers
            int centerX = Game.ScreenWidth >> 1;
            int centerY = Game.ScreenHeight >> 1;
            int towerSize = (int)(Constants.HubScene_TowerSize * Game.ScreenHeight);
            int towerTop = (int)(Constants.HubScene_TowerAreaTop * Game.ScreenHeight);
            int leftX = centerX - towerSize - towerSize;
            int midX = centerX - (towerSize >> 1);
            int rightX = centerX + towerSize;
            m_Towers = new Tower[6];
            m_Towers[0] = new Tower(3, game.TheGameInfo.GameData.TowerFloors[0], new Rectangle(leftX, towerTop, towerSize, towerSize), Assets.Towers[0]);
            m_Towers[1] = new Tower(4, game.TheGameInfo.GameData.TowerFloors[1], new Rectangle(midX, towerTop, towerSize, towerSize), Assets.Towers[1]);
            m_Towers[2] = new Tower(5, game.TheGameInfo.GameData.TowerFloors[2], new Rectangle(rightX, towerTop, towerSize, towerSize), Assets.Towers[2]);
            m_Towers[3] = new Tower(6, game.TheGameInfo.GameData.TowerFloors[3], new Rectangle(leftX, centerY, towerSize, towerSize), Assets.Towers[3]);
            m_Towers[4] = new Tower(7, game.TheGameInfo.GameData.TowerFloors[4], new Rectangle(midX, centerY, towerSize, towerSize), Assets.Towers[0]);
            m_Towers[5] = new Tower(8, game.TheGameInfo.GameData.TowerFloors[5], new Rectangle(rightX, centerY, towerSize, towerSize), Assets.Towers[0]);

            // Level/Exp display
            int expBarWidth = (int)(Constants.HubScene_ExpBarWidth * Game.ScreenWidth);
            int expBarHeight = (int)(Constants.HubScene_ExpBarHeight * Game.ScreenHeight);
            int marginLeftRight = (int)(Constants.HubScene_MarginLeftRight * Game.ScreenWidth);
            int levelY = (int)(Constants.HubScene_MarginTopBottom * Game.ScreenHeight);                        
            m_LevelLabel = new UILabel("Level: ", marginLeftRight, levelY, Color.Goldenrod, Assets.HelpFont, UILabel.XMode.Left);
            m_Level = new UILabel(game.TheGameInfo.GameData.Level.ToString(), marginLeftRight + m_LevelLabel.Width, levelY, Color.White, Assets.HelpFont, UILabel.XMode.Left);
            m_ExpBar = new UIProgressBar(new Rectangle(marginLeftRight, levelY + m_Level.Height, expBarWidth, expBarHeight));
            m_ExpBar.ProgressColor = Color.Yellow;
            m_ExpBar.Progress = (float)game.TheGameInfo.GameData.Exp / Balance.ExpForNextLevel(game.TheGameInfo.GameData.Level);

            SetupTutorial();

            int buttonWidth = (int)(Constants.HubScene_ButtonWidth * Game.ScreenWidth);
            int buttonHeight = (int)(Constants.HubScene_ButtonHeight * Game.ScreenHeight);
            int buttonY = Game.ScreenHeight - levelY - buttonHeight;
            m_ResetTutorial = new UIButton(0, "Reset Tutorial", Assets.HelpFont, new Rectangle(marginLeftRight, buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);
            m_Options = new UIButton(0, "Options", Assets.HelpFont, new Rectangle((marginLeftRight * 2) + buttonWidth, buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);
            m_BuyCoins = new UIButton(0, "Buy Coins", Assets.HelpFont, new Rectangle((marginLeftRight * 3) + (buttonWidth * 2), buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);

            int buttonRight = Game.ScreenWidth - marginLeftRight - buttonWidth;
            m_Exit = new UIButton(0, "Exit", Assets.HelpFont, new Rectangle(buttonRight, buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);
            m_SignOut = new UIButton(0, "Sign Out", Assets.HelpFont, new Rectangle(buttonRight - marginLeftRight - buttonWidth, buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);

            int startY = (int)(Constants.HelpPanel_Height * Game.ScreenHeight);
            int coinsWidth = (int)(Constants.HubScene_CoinsWidth * Game.ScreenWidth);
            int coinsLeft = Game.ScreenWidth - (coinsWidth + marginLeftRight);
            m_Coins = new UICoinsDisplay(startY, coinsLeft, levelY, coinsWidth);
            m_Coins.SetCoins(Game.TheGameInfo.HardCurrency);
            Game.OnCurrencyChange += Game_OnCurrencyChange;
            Game.OnVipDataChange += Game_OnVipDataChange;

            int vipTop = (levelY * 2) + m_Coins.Height;
            m_VIP = new UIVIPDisplay(coinsLeft + marginLeftRight, vipTop, coinsWidth - marginLeftRight);

            game.ValidateVIPSettings();
        }

        public override void Shutdown()
        {
            base.Shutdown();

            // Remove input handlers
            InputController.IC.OnClick -= IC_OnClick;
            InputController.IC.OnDrag -= IC_OnDrag;
            InputController.IC.OnDragBegin -= IC_OnDragBegin;
            InputController.IC.OnKeyDown -= IC_OnKeyDown;

            Happiness.Game.OnCurrencyChange -= Game_OnCurrencyChange;
            Happiness.Game.OnVipDataChange -= Game_OnVipDataChange;
        }

        void SetupTutorial()
        {
            Game.Tutorial.Load(Game.TheGameInfo.GameData.Tutorial);
            int centerX = Game.ScreenWidth >> 1;
            int towerSize = (int)(Constants.HubScene_TowerSize * Game.ScreenHeight);
            int towerTop = (int)(Constants.HubScene_TowerAreaTop * Game.ScreenHeight);
            int leftX = centerX - towerSize - towerSize;
            int tutorialWidth = (int)(Constants.HubScene_TutorialWidth * Game.ScreenWidth);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.ClickTower, new Vector2(leftX, towerTop + (towerSize >> 1)), 0, new Rectangle(leftX - (tutorialWidth + 5), towerTop + (towerSize >> 1) + (Game.Tutorial.ArrowHeight >> 1), tutorialWidth, 0), "Tap the 3x3 tower to get started.", TutorialSystem.TutorialPiece.None, m_Towers[0].Rect);
            Game.Tutorial.TriggerPiece(TutorialSystem.TutorialPiece.ClickTower);
        }

        private void Game_OnCurrencyChange(int hardCurrency)
        {
            m_Coins.SetCoins(hardCurrency);
        }

        private void Game_OnVipDataChange(int obj)
        {
            m_VIP.UpdateLevel();
        }


        #region InputHandlers
        private void IC_OnClick(object sender, DragArgs e)
        {
            if( e.Abort )
                return;

            if (m_MessageBox != null)
            {
                MessageBoxResult result = m_MessageBox.HandleClick(e.CurrentX, e.CurrentY);
                if (result != MessageBoxResult.NoResult)
                {
                    DoMessageBoxResult(result);
                }
                return;
            }

            if (m_SoundDialog != null)
            {
                m_SoundDialog.HandleClick(e.CurrentX, e.CurrentY);
                return;
            }

            if (m_OptionsDialog != null)
            {
                if( m_OptionsDialog.HandleClick(e.CurrentX, e.CurrentY) )
                    return;

                // Options exited
                m_OptionsDialog = null;
            }

            if (m_CoinsDialog != null)
            {
                if (m_CoinsDialog.HandleClick(e.CurrentX, e.CurrentY))
                    return;
                m_CoinsDialog = null;
            }

            if (m_VipDialog != null)
            {
                if(!m_VipDialog.HandleClick(e.CurrentX, e.CurrentY))
                    m_VipDialog = null;
                return;
            }

            if (m_FloorSelect != null)
            {
                if (!m_FloorSelect.HandleClick(e.CurrentX, e.CurrentY))
                    m_FloorSelect = null;
            }            
            else
            {
                foreach (Tower t in m_Towers)
                {
                    if (t.Click(e.CurrentX, e.CurrentY))
                    {
                        //ActivateTower(t);
                        SoundManager.Inst.PlaySound(SoundManager.SEInst.MenuAccept);
                        Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.ClickTower);
                        m_FloorSelect = new FloorSelectDialog(t.Size - 3, Game.ScreenWidth, Game.ScreenHeight, Game);
                        break;
                    }
                }

                if (m_ResetTutorial.Click(e.CurrentX, e.CurrentY))
                {
                    m_MessageBox = new MessageBox("Are you sure you want to restart the tutorial?", MessageBoxButtons.YesNo, (int)MessageBoxContext.ResetTutorial, Game.ScreenWidth, Game.ScreenHeight);                    
                }
                if (m_Options.Click(e.CurrentX, e.CurrentY))
                {
                    m_OptionsDialog = new Options(Game);
                }
                if (m_BuyCoins.Click(e.CurrentX, e.CurrentY) ||
                    m_Coins.HandleClick(e.CurrentX, e.CurrentY))
                {
                    m_CoinsDialog = new BuyCoinsModal();
                }
                if (m_Exit.Click(e.CurrentX, e.CurrentY))
                    DoExit();                
                if (m_SignOut.Click(e.CurrentX, e.CurrentY))
                    m_MessageBox = new MessageBox("Are you sure you want to sign out?", MessageBoxButtons.YesNo, (int)MessageBoxContext.SignOut, Game.ScreenWidth, Game.ScreenHeight);                
                if (m_VIP.HandleClick(e.CurrentX, e.CurrentY))
                {
                    m_VipDialog = new VIPDialog();
                }
            }
        }

        private void IC_OnDrag(object sender, DragArgs e)
        {
            if( m_FloorSelect != null )
                m_FloorSelect.Drag(e);

            if( m_OptionsDialog != null )
                m_OptionsDialog.Drag(e);

            if(m_CoinsDialog != null )
                m_CoinsDialog.Drag(e);
        }

        private void IC_OnDragBegin(object sender, DragArgs e)
        {
            if( m_FloorSelect != null )
                m_FloorSelect.DragBegin(e);
            if( m_CoinsDialog != null )
                m_CoinsDialog.DragBegin(e);
        }

        private void IC_OnScroll(int delta)
        {
            if( m_FloorSelect != null )
                m_FloorSelect.Scroll(delta);
            if( m_CoinsDialog != null )
                m_CoinsDialog.Scroll(delta);
        }

        private void IC_OnKeyDown(object sender, KeyArgs e)
        {
            if (m_FloorSelect != null)
            {
                if (!m_FloorSelect.OnKeyDown(e))
                    m_FloorSelect = null;
            }
            else if (m_CoinsDialog != null)
            {
                if (!m_CoinsDialog.OnKeyDown(e))
                    m_CoinsDialog = null;
            }
            else
            {
                if (e.Key == Keys.Escape)
                    DoExit();
            }
        }
        #endregion

        void DoExit()
        {
            m_MessageBox = new MessageBox("Are you sure you want to exit the game?", MessageBoxButtons.YesNo, (int)MessageBoxContext.ExitGame, Game.ScreenWidth, Game.ScreenHeight);
        }

        void DoMessageBoxResult(MessageBoxResult result)
        {
            MessageBoxContext context = (MessageBoxContext)m_MessageBox.Context;
            switch (context)
            {
                case MessageBoxContext.ResetTutorial:
                    if (result == MessageBoxResult.Yes)
                    {
                        Happiness.Game.ResetTutorial();
                        SetupTutorial();
                    }
                    break;
                case MessageBoxContext.ExitGame:
                    if (result == MessageBoxResult.Yes)
                        Happiness.Game.ExitGame();
                    break;
                case MessageBoxContext.SignOut:
                    if (result == MessageBoxResult.Yes)
                    {
                        // Nuke local game data
                        GameInfoValidator.Instance.DeleteLocalFile();

                        // Goto startup scene
                        Game.GotoScene(new StartupScene(Game));
                    }
                    break;
            }

            m_MessageBox = null;
        }


        public override void Update(double gameTime)
        {
            base.Update(gameTime);
            
            if ( m_FloorSelect != null )
                m_FloorSelect.Update(gameTime);
            if(m_OptionsDialog != null )
                m_OptionsDialog.Update(gameTime);
            if (m_CoinsDialog != null)
                m_CoinsDialog.Update(gameTime);
        }
                
        public override void Draw(Renderer spriteBatch)
        {
            if (m_CoinsDialog != null)
            {
                m_CoinsDialog.Draw(spriteBatch);
                return;
            }
            if (m_VipDialog != null)
            {
                m_VipDialog.Draw(spriteBatch);
                return;
            }

            foreach ( Tower t in m_Towers )
                t.Draw(spriteBatch);

            m_LevelLabel.Draw(spriteBatch);
            m_Level.Draw(spriteBatch);
            m_ExpBar.Draw(spriteBatch);
            m_ResetTutorial.Draw(spriteBatch);
            m_Options.Draw(spriteBatch);
            m_BuyCoins.Draw(spriteBatch);
            m_Exit.Draw(spriteBatch);
            m_SignOut.Draw(spriteBatch);
            m_Coins.Draw(spriteBatch);
            m_VIP.Draw(spriteBatch);


            if ( m_FloorSelect != null )
                m_FloorSelect.Draw(spriteBatch);

            if(m_SoundDialog != null )
                m_SoundDialog.Draw(spriteBatch);
            if( m_OptionsDialog != null )
                m_OptionsDialog.Draw(spriteBatch);

            if(m_MessageBox != null )
                m_MessageBox.Draw(spriteBatch);
        }

        void ActivateTower(Tower t)
        {
            if (t.Locked)
                ShowLockedMessage();
            else
            {
                // Launch the puzzle
                GameScene gs = new GameScene(Game);
                gs.Initialize(t.Floor, t.Size, true);
                Game.GotoScene(gs);
            }
        }

        void ShowLockedMessage()
        {
        }
    }
}
