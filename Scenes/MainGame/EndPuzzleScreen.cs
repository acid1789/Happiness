using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HappinessNetwork;

namespace Happiness
{
    public class EndPuzzleScreen
    {
        enum AnimStep
        {
            Start,
            YourTime,
            BaseExp,
            BonusExp,
            TotalExp,
            LevelUp,
            Finish
        }

        Rectangle m_Rect;
        bool m_bSuccess;
        AnimStep m_AnimStep;
        double m_AnimTimeRemaining;
        bool m_bLevelUnlocked;
        
        int m_iTotalExp;
        int m_iExpStep;
        int m_iCenterX;
        int m_iTower;

        UILabel m_Title;
        UILabel m_Success;
        UIButton[] m_Buttons;

        UILabel m_TimeLabel;
        UILabel m_Time;
        UILabel m_ParLabel;
        UILabel m_Par;

        UILabel m_ExpBaseLabel;
        UILabel m_ExpBonusLabel;
        UILabel m_ExpTotalLabel;
        UILabel m_ExpBase;
        UILabel m_ExpBonus;
        UILabel m_ExpTotal;
        Rectangle m_ScoreTotalBar;

        UILabel m_LevelLabel;
        UILabel m_Level;
        UIProgressBar m_ExpBar;
        UILabel m_ExpPoints;

        UILabel m_Unlock;

        Happiness m_Game;

        public event EventHandler OnNextPuzzle;
        public event EventHandler OnMainMenu;
        public event EventHandler OnRestartPuzzle;
        
        public EndPuzzleScreen(bool success, int puzzleSize, double seconds, int screenWidth, int screenHeight, Happiness game)
        {
            m_bLevelUnlocked = false;
            m_bSuccess = success;
            m_Game = game;

            m_iCenterX = screenWidth >> 1;
            int width = (int)(Constants.EndScreen_Width * screenWidth);
            int height = (int)(Constants.EndScreen_Height * screenHeight);
            int margin = (int)(Constants.EndScreen_MarginTopBottom * screenHeight);

            // rectangle
            m_Rect = new Rectangle(m_iCenterX - (width >> 1), (screenHeight >> 1) - (height >> 1), width, height);

            // Title
            int iTitleY = m_Rect.Top + margin;
            m_Title = new UILabel("Puzzle Complete!", m_iCenterX, iTitleY, Color.Goldenrod, Assets.DialogFont, UILabel.XMode.Center);

            // Success
            int iSuccessY = iTitleY + (int)(Constants.EndScreen_SuccessGap * screenHeight);
            m_Success = new UILabel(success ? "SUCCESS!" : "INCORRECT", m_iCenterX, iSuccessY, success ? Color.Green : Color.Red, Assets.MenuFont, UILabel.XMode.Center);

            // Buttons
            int buttonWidth = (int)(Constants.EndScreen_ButtonWidth * screenWidth);
            int buttonHeight = (int)(Constants.EndScreen_ButtonHeight * screenHeight);
            int buttonSpace = (int)(Constants.EndScreen_ButtonSpace * screenWidth);
            int buttonY = m_Rect.Bottom - (margin + buttonHeight);
            int halfButtonWidth = (buttonWidth >> 1);
            
            m_Buttons = new UIButton[3];
            m_Buttons[0] = new UIButton(0, "Next Puzzle", Assets.HelpFont, new Rectangle(m_iCenterX + halfButtonWidth + buttonSpace, buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);
            m_Buttons[1] = new UIButton(1, "Main Menu", Assets.HelpFont, new Rectangle(m_iCenterX - halfButtonWidth, buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);
            m_Buttons[2] = new UIButton(2, "Restart Puzzle", Assets.HelpFont, new Rectangle(m_iCenterX - (halfButtonWidth + buttonSpace + buttonWidth), buttonY, buttonWidth, buttonHeight), Assets.ScrollBar);

            m_Buttons[0].Enabled = false;
            m_Buttons[1].Enabled = false;
            m_Buttons[2].Enabled = false;

            // Your time
            m_iTower = puzzleSize - 3;
            double parSeconds = Balance.ParTime(m_iTower);
            int quarterWidth = (width >> 2);
            int leftTimeX = m_iCenterX - quarterWidth;
            int rightTimeX = m_iCenterX + quarterWidth;
            int timeGap = (int)(Constants.EndScreen_TimeGap * screenHeight);
            int timeLabelY = iSuccessY + (int)(Constants.EndScreen_TimeLabelGap * screenHeight);
            m_TimeLabel = new UILabel("Your Time", leftTimeX, timeLabelY, Color.Goldenrod, Assets.DialogFont, UILabel.XMode.Center);
            m_Time = new UILabel(Happiness.TimeString(seconds), leftTimeX, timeLabelY + timeGap, (success && (seconds <= parSeconds)) ? Color.Green : Color.Red, Assets.HelpFont, UILabel.XMode.Center);

            m_ParLabel = new UILabel("Par Time", rightTimeX, timeLabelY, Color.Goldenrod, Assets.DialogFont, UILabel.XMode.Center);
            m_Par = new UILabel(Happiness.TimeString(parSeconds), rightTimeX, timeLabelY + timeGap, Color.LightGray, Assets.HelpFont, UILabel.XMode.Center);
            
            m_Time.Hidden = true;

            // Scores
            double baseExp = success ? Balance.BaseExp(m_iTower) : 0;
            double bonusExp = success ? Balance.BonusExp(m_iTower, seconds) : 0;
            double totalExp = baseExp + bonusExp;
            m_iTotalExp = (int)totalExp;
            m_iExpStep = (int)((float)m_iTotalExp * 0.05f);

            int scoreCenterGap = (int)(Constants.EndScreen_ScoreCenterGap * screenWidth);
            int scoreSpace = (int)(Constants.EndScreen_ScoreSpace * screenHeight);
            int scoreBarWidth = (int)(Constants.EndScreen_ScoreBarWidth * screenWidth);
            int scoreBarHeight = (int)(Constants.EndScreen_ScoreBarHeight * screenHeight);
            int scoreL = m_iCenterX - scoreCenterGap;
            int scoreR = m_iCenterX + (scoreCenterGap * 3);
            int iScoreY = timeLabelY + timeGap + (int)(Constants.EndScreen_TimeScoreGap * screenHeight);
            m_ExpBaseLabel = new UILabel("Completion Exp:", scoreL, iScoreY, Color.Goldenrod, Assets.HelpFont, UILabel.XMode.Right);
            m_ExpBase = new UILabel(((int)baseExp).ToString(), scoreR, iScoreY, (baseExp > 0) ? Color.Green : Color.Gray, Assets.HelpFont, UILabel.XMode.Right);
            iScoreY += scoreSpace;
            m_ExpBonusLabel = new UILabel("Bonus Exp:", scoreL, iScoreY, Color.Goldenrod, Assets.HelpFont, UILabel.XMode.Right);
            m_ExpBonus = new UILabel(((int)bonusExp).ToString(), scoreR, iScoreY, (bonusExp > 0) ? Color.Green : Color.Gray, Assets.HelpFont, UILabel.XMode.Right);

            m_ScoreTotalBar = new Rectangle(m_iCenterX, iScoreY + scoreSpace, scoreBarWidth, scoreBarHeight);

            iScoreY += scoreSpace + scoreBarHeight + scoreBarHeight;
            m_ExpTotalLabel = new UILabel("Total Exp:", scoreL, iScoreY, Color.Goldenrod, Assets.HelpFont, UILabel.XMode.Right);
            m_ExpTotal = new UILabel(((int)totalExp).ToString(), scoreR, iScoreY, (totalExp > 0) ? Color.Green : Color.Gray, Assets.HelpFont, UILabel.XMode.Right);

            m_ExpBase.Hidden = true;
            m_ExpBonus.Hidden = true;
            m_ExpTotal.Hidden = true;

            // Level up
            int expBarWidth = (int)(Constants.EndScreen_ExpBarWidth * screenWidth);
            int expBarHeight = (int)(Constants.EndScreen_ExpBarHeight * screenHeight);
            int expBarLeft = m_iCenterX - (expBarWidth >> 1);
            int levelY = iScoreY + scoreSpace + (int)(Constants.EndScreen_ScoreLevelGap * screenHeight);            
            m_LevelLabel = new UILabel("Level: ", expBarLeft, levelY, Color.Goldenrod, Assets.HelpFont, UILabel.XMode.Left);
            m_Level = new UILabel(game.m_GameInfo.GameData.Level.ToString(), expBarLeft + m_LevelLabel.Width, levelY, Color.White, Assets.HelpFont, UILabel.XMode.Left);
            m_ExpBar = new UIProgressBar(new Rectangle(expBarLeft, levelY + m_Level.Height, expBarWidth, expBarHeight));
            m_ExpBar.ProgressColor = Color.Yellow;            
            
            SetupExpDisplay();

            // Unlock
            int iUnlockY = m_ExpBar.Rect.Bottom + (int)(Constants.EndScreen_LevelUnlockGap * screenHeight);
            m_Unlock = new UILabel(string.Format("Tower {0} Unlocked!", m_iTower + 2), m_iCenterX, iUnlockY, Color.Yellow, Assets.MenuFont, UILabel.XMode.Center);
            m_Unlock.Hidden = true;

            m_AnimStep = AnimStep.Start;
            m_AnimTimeRemaining = 1.0f;

            if (!game.Tutorial.IsPieceSetup(TutorialSystem.TutorialPiece.EndScreen1))
            {
                int instRW = (screenWidth - 20) - (m_ScoreTotalBar.Right + 30);
                Rectangle instRect = new Rectangle(m_ScoreTotalBar.Right + 30, m_ExpBonus.PositionY + game.Tutorial.ArrowHeight, instRW, 0);
                game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.EndScreen1, new Vector2(m_ScoreTotalBar.Right + 10, m_ExpBonus.PositionY), Constants.ArrowLeft, instRect,
                    "When you complete a puzzle you gain a set amount of experience points based on the puzzle difficulty.\n\nYou also get a bonus amount of experience points that varies depending on the amount of time it took you to finish the puzzle.", TutorialSystem.TutorialPiece.EndScreen2, Rectangle.Empty, true);

                game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.EndScreen2, new Vector2(m_iCenterX, m_ExpBar.Rect.Top - 5), Constants.ArrowDown, instRect,
                    "As you gain experience, you will increase in levels.\n\nAs you increase in levels, you will unlock the larger towers for harder puzzles.", TutorialSystem.TutorialPiece.EndScreen3, Rectangle.Empty, true);

                instRect.Y -= 10;
                game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.EndScreen3, new Vector2(m_Buttons[0].Rect.Center.X, m_Buttons[0].Rect.Top - 5), Constants.ArrowDown, instRect,
                    "Tap the Next Puzzle button to move on to the second puzzle.", TutorialSystem.TutorialPiece.Puzzle2, m_Buttons[0].Rect);

                game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.EndScreen4, new Vector2(m_Buttons[0].Rect.Center.X, m_Buttons[0].Rect.Top - 5), Constants.ArrowDown, instRect,
                    "Tap the Next Puzzle button to move on to the next puzzle.", TutorialSystem.TutorialPiece.Horizontal_NextTo, m_Buttons[0].Rect);
            }
            game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Puzzle2);
        }
        
        public bool HandleClick(int iX, int iY)
        {
            foreach (UIButton b in m_Buttons)
            {
                if (b.Click(iX, iY))
                {
                    switch (b.ButtonID)
                    {
                        case 0:
                            m_Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.EndScreen3);
                            m_Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.EndScreen4);
                            if ( OnNextPuzzle != null )
                                OnNextPuzzle(this, null);
                            break;
                        case 1:
                            if( OnMainMenu != null )
                                OnMainMenu(this, null);
                            break;
                        case 2:
                            if( OnRestartPuzzle != null )
                                OnRestartPuzzle(this, null);
                            break;
                    }
                    return false;
                }
            }
            return true;
        }

        public void Update(GameTime gt)
        {
            if (m_AnimStep != AnimStep.Finish)
            {
                m_AnimTimeRemaining -= gt.ElapsedGameTime.TotalSeconds;
                if (m_AnimTimeRemaining <= 0)
                {
                    NextAnimStep();
                }
                else if (m_AnimStep == AnimStep.LevelUp)
                {
                    UpdateLevelUp(gt);
                }
            }
        }

        void NextAnimStep()
        {
            int nextStep = (int)m_AnimStep + 1;
            m_AnimStep = (AnimStep)nextStep;
            switch (m_AnimStep)
            {
                case AnimStep.YourTime:
                    m_Time.Hidden = false;
                    break;
                case AnimStep.BaseExp:
                    m_ExpBase.Hidden = false;
                    break;
                case AnimStep.BonusExp:
                    m_ExpBonus.Hidden = false;
                    break;
                case AnimStep.TotalExp:
                    m_ExpTotal.Hidden = false;
                    break;
                case AnimStep.LevelUp:
                    break;
                case AnimStep.Finish:
                    if( m_bLevelUnlocked )
                        m_Unlock.Hidden = false;
                    m_Buttons[0].Enabled = m_bSuccess;
                    m_Buttons[1].Enabled = true;
                    m_Buttons[2].Enabled = true;
                    break;
            }
            m_AnimTimeRemaining = 0.5f;
        }

        void UpdateLevelUp(GameTime gt)
        {
            m_AnimTimeRemaining = 10000;
            if (m_iTotalExp > 0)
            {
                int expAdj = Math.Min(m_iTotalExp, m_iExpStep);
                m_Game.m_GameInfo.GameData.Exp += expAdj;
                m_iTotalExp -= expAdj;

                SetupExpDisplay();
            }
            else
            {
                m_AnimTimeRemaining = 0;
            }
        }

        void SetupExpDisplay()
        {
            int expForNextLevel = Balance.ExpForNextLevel(m_Game.m_GameInfo.GameData.Level);
            while (m_Game.m_GameInfo.GameData.Exp >= expForNextLevel)
            {
                m_Game.m_GameInfo.GameData.Exp -= expForNextLevel;
                m_Game.m_GameInfo.GameData.Level++;
                expForNextLevel = Balance.ExpForNextLevel(m_Game.m_GameInfo.GameData.Level);
                m_Level.Text = m_Game.m_GameInfo.GameData.Level.ToString();
                m_Level.Color = Color.Yellow;
                m_Level.Font = Assets.DialogFont;
                m_Level.PositionY -= 4;

                m_bLevelUnlocked = m_iTower < (m_Game.m_GameInfo.GameData.TowerFloors.Length - 1) && m_Game.m_GameInfo.GameData.Level >= Balance.UnlockThreshold(m_iTower) && m_Game.m_GameInfo.GameData.TowerFloors[m_iTower + 1] == 0;
                if( m_bLevelUnlocked )
                    m_Game.m_GameInfo.GameData.TowerFloors[m_iTower + 1] = 1;
            }
            m_ExpBar.Progress = (float)m_Game.m_GameInfo.GameData.Exp / (float)expForNextLevel;
            string expString = string.Format("{0:N0} / {1:N0}", m_Game.m_GameInfo.GameData.Exp, expForNextLevel);
            m_ExpPoints = new UILabel(expString, m_iCenterX, m_ExpBar.Rect.Bottom, Color.LightGray, Assets.HelpFont, UILabel.XMode.Center);
        }

        public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            // Draw frame
            spriteBatch.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            spriteBatch.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            spriteBatch.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);

            // Draw title
            m_Title.Draw(spriteBatch);

            // Draw Success/Failure
            m_Success.Draw(spriteBatch);

            // Draw Time
            m_TimeLabel.Draw(spriteBatch);
            m_Time.Draw(spriteBatch);

            // Draw Par Time
            m_ParLabel.Draw(spriteBatch);
            m_Par.Draw(spriteBatch);

            // Draw Exp
            m_ExpBaseLabel.Draw(spriteBatch);
            m_ExpBase.Draw(spriteBatch);
            m_ExpBonusLabel.Draw(spriteBatch);
            m_ExpBonus.Draw(spriteBatch);
            m_ExpTotal.Draw(spriteBatch);
            m_ExpTotalLabel.Draw(spriteBatch);
            spriteBatch.Draw(Assets.GoldBarHorizontal, m_ScoreTotalBar, Color.White);

            // Draw level up
            m_LevelLabel.Draw(spriteBatch);
            m_Level.Draw(spriteBatch);
            m_ExpBar.Draw(spriteBatch);
            m_ExpPoints.Draw(spriteBatch);

            // Draw unlock
            m_Unlock.Draw(spriteBatch);

            // Draw Buttons
            foreach ( UIButton b in m_Buttons )
                b.Draw(spriteBatch);           
        }     
    }
}
