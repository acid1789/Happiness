﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HappinessNetwork;

namespace Happiness
{
    public class HubScene : Scene
    {
        Tower[] m_Towers;

        UILabel m_LevelLabel;
        UILabel m_Level;
        UIProgressBar m_ExpBar;

        UIButton m_ResetTutorial;
        UIButton m_Options;
        UIButton m_SignOut;
        UIButton m_Exit;
        UIButton m_BuyCoins;

        FloorSelectDialog m_FloorSelect;

        SoundDialog m_SoundDialog;
        Options m_OptionsDialog;
        BuyCoinsModal m_CoinsDialog;

        public HubScene(Happiness game) : base(game)
        {
            InputController.IC.OnClick += IC_OnClick;
            InputController.IC.OnDragBegin += IC_OnDragBegin;
            InputController.IC.OnDrag += IC_OnDrag;
            InputController.IC.OnScroll += IC_OnScroll;

            NetworkManager nm = NetworkManager.Net;

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
            m_Towers[0] = new Tower(3, game.m_GameInfo.GameData.TowerFloors[0], new Rectangle(leftX, towerTop, towerSize, towerSize), Assets.Towers[0]);
            m_Towers[1] = new Tower(4, game.m_GameInfo.GameData.TowerFloors[1], new Rectangle(midX, towerTop, towerSize, towerSize), Assets.Towers[1]);
            m_Towers[2] = new Tower(5, game.m_GameInfo.GameData.TowerFloors[2], new Rectangle(rightX, towerTop, towerSize, towerSize), Assets.Towers[2]);
            m_Towers[3] = new Tower(6, game.m_GameInfo.GameData.TowerFloors[3], new Rectangle(leftX, centerY, towerSize, towerSize), Assets.Towers[3]);
            m_Towers[4] = new Tower(7, game.m_GameInfo.GameData.TowerFloors[4], new Rectangle(midX, centerY, towerSize, towerSize), Assets.Towers[0]);
            m_Towers[5] = new Tower(8, game.m_GameInfo.GameData.TowerFloors[5], new Rectangle(rightX, centerY, towerSize, towerSize), Assets.Towers[0]);

            // Level/Exp display
            int expBarWidth = (int)(Constants.HubScene_ExpBarWidth * Game.ScreenWidth);
            int expBarHeight = (int)(Constants.HubScene_ExpBarHeight * Game.ScreenHeight);
            int marginLeftRight = (int)(Constants.HubScene_MarginLeftRight * Game.ScreenWidth);
            int levelY = (int)(Constants.HubScene_MarginTopBottom * Game.ScreenHeight);                        
            m_LevelLabel = new UILabel("Level: ", marginLeftRight, levelY, Color.Goldenrod, Assets.HelpFont, UILabel.XMode.Left);
            m_Level = new UILabel(game.m_GameInfo.GameData.Level.ToString(), marginLeftRight + m_LevelLabel.Width, levelY, Color.White, Assets.HelpFont, UILabel.XMode.Left);
            m_ExpBar = new UIProgressBar(new Rectangle(marginLeftRight, levelY + m_Level.Height, expBarWidth, expBarHeight));
            m_ExpBar.ProgressColor = Color.Yellow;
            m_ExpBar.Progress = (float)game.m_GameInfo.GameData.Exp / Balance.ExpForNextLevel(game.m_GameInfo.GameData.Level);

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

            game.ValidateVIPSettings();
        }

        public override void Shutdown()
        {
            base.Shutdown();

            // Remove input handlers
            InputController.IC.OnClick -= IC_OnClick;
            InputController.IC.OnDrag -= IC_OnDrag;
            InputController.IC.OnDragBegin -= IC_OnDragBegin;
        }

        void SetupTutorial()
        {
            Game.Tutorial.Load(Game.m_GameInfo.GameData.Tutorial);
            int centerX = Game.ScreenWidth >> 1;
            int towerSize = (int)(Constants.HubScene_TowerSize * Game.ScreenHeight);
            int towerTop = (int)(Constants.HubScene_TowerAreaTop * Game.ScreenHeight);
            int leftX = centerX - towerSize - towerSize;
            int tutorialWidth = (int)(Constants.HubScene_TutorialWidth * Game.ScreenWidth);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.ClickTower, new Vector2(leftX, towerTop + (towerSize >> 1)), 0, new Rectangle(leftX - (tutorialWidth + 5), towerTop + (towerSize >> 1) + (Game.Tutorial.ArrowHeight >> 1), tutorialWidth, 0), "Tap the 3x3 tower to get started.", TutorialSystem.TutorialPiece.None, m_Towers[0].Rect);
            Game.Tutorial.TriggerPiece(TutorialSystem.TutorialPiece.ClickTower);
        }

        #region InputHandlers
        private void IC_OnClick(object sender, DragArgs e)
        {
            if( e.Abort )
                return;

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
                    Happiness.Game.ResetTutorial();
                    SetupTutorial();
                }
                if (m_Options.Click(e.CurrentX, e.CurrentY))
                {
                    m_OptionsDialog = new Options(Game);
                }
                if (m_BuyCoins.Click(e.CurrentX, e.CurrentY))
                {
                    m_CoinsDialog = new BuyCoinsModal();
                }
                if ( m_Exit.Click(e.CurrentX, e.CurrentY) )
                    Happiness.Game.Exit();
                if (m_SignOut.Click(e.CurrentX, e.CurrentY))
                {
                    // Nuke local game data
                    GameInfoValidator.Instance.DeleteLocalFile();

                    // Goto startup scene
                    Game.GotoScene(new StartupScene(Game));
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
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if ( m_FloorSelect != null )
                m_FloorSelect.Update(gameTime);
            if(m_OptionsDialog != null )
                m_OptionsDialog.Update(gameTime);
            if (m_CoinsDialog != null)
                m_CoinsDialog.Update(gameTime);
        }
                
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (m_CoinsDialog != null)
            {
                m_CoinsDialog.Draw(spriteBatch);
                return;
            }

            foreach( Tower t in m_Towers )
                t.Draw(spriteBatch);

            m_LevelLabel.Draw(spriteBatch);
            m_Level.Draw(spriteBatch);
            m_ExpBar.Draw(spriteBatch);
            m_ResetTutorial.Draw(spriteBatch);
            m_Options.Draw(spriteBatch);
            m_BuyCoins.Draw(spriteBatch);
            m_Exit.Draw(spriteBatch);
            m_SignOut.Draw(spriteBatch);


            if ( m_FloorSelect != null )
                m_FloorSelect.Draw(spriteBatch);

            if(m_SoundDialog != null )
                m_SoundDialog.Draw(spriteBatch);
            if( m_OptionsDialog != null )
                m_OptionsDialog.Draw(spriteBatch);
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
