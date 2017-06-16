using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkCore;
using HappinessNetwork;

namespace Happiness
{
    class VIPDialog
    {
        Rectangle m_Rect;

        UILabel m_LevelLabel;
        UILabel m_Level;
        UIProgressBar m_ExpBar;
        UILabel m_ExpText;

        UIFrame m_LevelsAreaFrame;
        UIButton m_BtnLeft;
        UIButton m_BtnRight;
        UILabel m_DetailsLevelLabel;
        UILabel m_DetailsRequiredPoints;
        UILabel m_Hints;
        UILabel m_HintsLabel;
        UILabel m_MegaHints;
        UILabel m_MegaHintsLabel;
        UILabel m_UndoSize;
        UILabel m_UndoSizeLabel;
        UILabel m_ExpBonus;
        UILabel m_ExpBonusLabel;
        UILabel m_DisableTimer;
        UILabel m_DisableExpBonus;
        UILabel m_ErrorDetector;
        UILabel m_ErrorDetector2;

        

        UIButton m_Close;

        int m_DetailsLevel;

        public VIPDialog()
        {
            Happiness game = Happiness.Game;
            int screenWidth = game.ScreenWidth;
            int screenHeight = game.ScreenHeight;

            int width = (int)(Constants.VIPDialog_Width * screenWidth);
            int height = (int)(Constants.VIPDialog_Height * screenHeight);

            int centerDialogX = (screenWidth >> 1);
            int left = centerDialogX - (width >> 1);
            int centerY = (screenHeight >> 1);
            int top = centerY - (height >> 1);
            m_Rect = new Rectangle(left, top, width, height);


            // Level/Exp display
            int expBarWidth = (int)(Constants.VIPDialog_ExpBarWidth * game.ScreenWidth);
            int expBarHeight = (int)(Constants.VIPDialog_ExpBarHeight * game.ScreenHeight);
            int marginTopBottom = (int)(Constants.VIPDialog_MarginTopBottom * game.ScreenWidth);


            int expBarLeft = centerDialogX - (expBarWidth >> 1);
            m_LevelLabel = new UILabel("Level: ", expBarLeft, marginTopBottom, Color.Goldenrod, Assets.DialogFont, UILabel.XMode.Left);
            m_Level = new UILabel(game.TheGameInfo.VipData.Level.ToString(), expBarLeft + m_LevelLabel.Width, top + marginTopBottom, Color.White, Assets.MenuFont, UILabel.XMode.Left);
            int xpBarTop = top + marginTopBottom + m_Level.Height;
            m_LevelLabel.PositionY = (xpBarTop - m_LevelLabel.Height) - 6;
            m_ExpBar = new UIProgressBar(new Rectangle(expBarLeft, xpBarTop, expBarWidth, expBarHeight));
            m_ExpBar.ProgressColor = Color.DarkBlue;
            m_ExpBar.Progress = (game.TheGameInfo.VipData.Level >= 10) ? 1.0f : (float)game.TheGameInfo.VipData.Points / (float)VIPLevels.Levels[game.TheGameInfo.VipData.Level];


            string expStr = (game.TheGameInfo.VipData.Level >= 10) ? game.TheGameInfo.VipData.Points.ToString("n0") : string.Format("{0} / {1}", game.TheGameInfo.VipData.Points.ToString("n0"), VIPLevels.Levels[game.TheGameInfo.VipData.Level].ToString("n0"));
            m_ExpText = new UILabel(expStr, centerDialogX, xpBarTop + expBarHeight, Color.WhiteSmoke, Assets.HelpFont, UILabel.XMode.Center);
            
            int buttonWidth = (int)(Constants.BuyCreditsDialog_ButtonWidth * screenWidth);
            int buttonHeight = (int)(Constants.BuyCreditsDialog_ButtonHeight * screenHeight);
            int centerButtonLeft = centerDialogX - (buttonWidth >> 1);
            int closeTop = (top + height) - (buttonHeight + marginTopBottom);
            m_Close = new UIButton(0, "Close", Assets.DialogFont, new Rectangle(centerButtonLeft, closeTop, buttonWidth, buttonHeight), Assets.ScrollBar);

            int levelsAreaTop = xpBarTop + expBarHeight + m_ExpText.Height + marginTopBottom;
            int levelsAreaWidth = (int)(Constants.VIPDialog_LevelsAreaWidth * screenWidth);
            int levelsAreaHeight = (closeTop - marginTopBottom) - levelsAreaTop;
            m_LevelsAreaFrame = new UIFrame(5, new Rectangle(centerDialogX - (levelsAreaWidth >> 1), levelsAreaTop, levelsAreaWidth, levelsAreaHeight));

            int lrButtonSpace = (int)(Constants.VIPDialog_LRButtonSpace * screenWidth);
            int lrButtonWidth = (int)(Constants.VIPDialog_LRButtonWidth * screenWidth);
            int lrButtonHeight = (int)(Constants.VIPDialog_LRButtonHeight * screenHeight);
            int lrButtonTop = (levelsAreaTop + (levelsAreaHeight >> 1)) - (lrButtonHeight >> 1);
            m_BtnLeft = new UIButton(0, "<", Assets.MenuFont, new Rectangle(m_LevelsAreaFrame.Rect.Left - (lrButtonWidth + lrButtonSpace), lrButtonTop, lrButtonWidth, lrButtonHeight), Assets.ScrollBar);
            m_BtnRight = new UIButton(1, ">", Assets.MenuFont, new Rectangle(m_LevelsAreaFrame.Rect.Right + lrButtonSpace, lrButtonTop, lrButtonWidth, lrButtonHeight), Assets.ScrollBar);

            int lineSpace = (int)(Constants.VIPDialog_LineSpace * screenHeight);

            int rowY = levelsAreaTop + (lineSpace * 2);
            m_DetailsLevelLabel = new UILabel("VIP Level: 0", m_LevelsAreaFrame.Rect.Left + lrButtonSpace, rowY, Color.White, Assets.HelpFont, UILabel.XMode.Left);
            m_DetailsRequiredPoints = new UILabel("Points Required: 50000", m_LevelsAreaFrame.Rect.Right - lrButtonSpace, rowY, Color.White, Assets.HelpFont, UILabel.XMode.Right);
            
            rowY += m_DetailsLevelLabel.Height + (lineSpace * 3);            
            m_HintsLabel = new UILabel("Hints (per puzzle): ", centerDialogX, rowY, Color.Goldenrod, Assets.DialogFont, UILabel.XMode.Right);
            m_Hints = new UILabel("0", centerDialogX, rowY, Color.Yellow, Assets.DialogFont, UILabel.XMode.Left);

            rowY += m_HintsLabel.Height + lineSpace;
            m_MegaHintsLabel = new UILabel("Mega Hints (per puzzle): ", centerDialogX, rowY, Color.Goldenrod, Assets.DialogFont, UILabel.XMode.Right);
            m_MegaHints = new UILabel("0", centerDialogX, rowY, Color.Yellow, Assets.DialogFont, UILabel.XMode.Left);

            rowY += m_HintsLabel.Height + lineSpace;
            m_UndoSizeLabel = new UILabel("Undo Size: ", centerDialogX, rowY, Color.Goldenrod, Assets.DialogFont, UILabel.XMode.Right);
            m_UndoSize = new UILabel("0", centerDialogX, rowY, Color.Yellow, Assets.DialogFont, UILabel.XMode.Left);

            rowY += m_HintsLabel.Height + lineSpace;
            m_ExpBonusLabel = new UILabel("Experience Bonus: ", centerDialogX, rowY, Color.Goldenrod, Assets.DialogFont, UILabel.XMode.Right);
            m_ExpBonus = new UILabel("1.5", centerDialogX, rowY, Color.Yellow, Assets.DialogFont, UILabel.XMode.Left);

            rowY += m_HintsLabel.Height + lineSpace;
            m_DisableTimer = new UILabel("Can Disable Timer", centerDialogX, rowY, Color.Yellow, Assets.DialogFont, UILabel.XMode.Center);

            rowY += m_HintsLabel.Height + lineSpace;
            m_DisableExpBonus = new UILabel("Can Disable VIP Exp Bonus", centerDialogX, rowY, Color.Yellow, Assets.DialogFont, UILabel.XMode.Center);

            rowY += m_HintsLabel.Height + lineSpace;
            m_ErrorDetector = new UILabel("Can enable the Error Detector", centerDialogX, rowY, Color.Yellow, Assets.DialogFont, UILabel.XMode.Center);

            rowY += m_HintsLabel.Height + lineSpace;
            m_ErrorDetector2 = new UILabel("Can enable the Super Error Detector", centerDialogX, rowY, Color.Yellow, Assets.DialogFont, UILabel.XMode.Center);

            SetDetailLevel(game.TheGameInfo.VipData.Level);            
        }

        void SetDetailLevel(int level)
        {
            m_DetailsLevel = Math.Max(Math.Min(level, VIPDetails.Levels.Length - 1), 0);
            m_DetailsLevelLabel.Text = "VIP Level: " + m_DetailsLevel;
            m_DetailsRequiredPoints.Text = "Points Required: " + ((m_DetailsLevel == 0) ? "0" : VIPLevels.Levels[m_DetailsLevel - 1].ToString("n0"));            
            m_Hints.Text = VIPDetails.DisplayString(VIPDetails.Levels[m_DetailsLevel].Hints);
            m_MegaHints.Text = VIPDetails.DisplayString(VIPDetails.Levels[m_DetailsLevel].MegaHints);
            m_UndoSize.Text = VIPDetails.DisplayString(VIPDetails.Levels[m_DetailsLevel].UndoSize);
            m_ExpBonus.Text = VIPDetails.Levels[m_DetailsLevel].ExpBonus.ToString() + "x";

            m_DisableTimer.Hidden = level < 1;
            m_DisableExpBonus.Hidden = level < 2;
            m_ErrorDetector.Hidden = level < 4;
            m_ErrorDetector2.Hidden = level < 8;

            m_BtnLeft.Enabled = m_DetailsLevel > 0;
            m_BtnRight.Enabled = m_DetailsLevel < (VIPDetails.Levels.Length - 1);
        }

        public bool HandleClick(int x, int y)
        {
            if (m_Close.Click(x, y))
                return false;
            else if (m_BtnLeft.Click(x, y))
                SetDetailLevel(m_DetailsLevel - 1);
            else if (m_BtnRight.Click(x, y))
                SetDetailLevel(m_DetailsLevel + 1);

            return true;
        }

        public void Draw(Renderer sb)
        {
            // Draw frame & background
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);

            m_LevelLabel.Draw(sb);
            m_Level.Draw(sb);
            m_ExpBar.Draw(sb);
            m_ExpText.Draw(sb);

            m_LevelsAreaFrame.Draw(sb);
            m_BtnLeft.Draw(sb);
            m_BtnRight.Draw(sb);

            m_DetailsLevelLabel.Draw(sb);
            m_DetailsRequiredPoints.Draw(sb);
            m_HintsLabel.Draw(sb);
            m_Hints.Draw(sb);
            m_MegaHintsLabel.Draw(sb);
            m_MegaHints.Draw(sb);
            m_UndoSizeLabel.Draw(sb);
            m_UndoSize.Draw(sb);
            m_ExpBonusLabel.Draw(sb);
            m_ExpBonus.Draw(sb);
            m_DisableTimer.Draw(sb);
            m_DisableExpBonus.Draw(sb);
            m_ErrorDetector.Draw(sb);
            m_ErrorDetector2.Draw(sb);

            m_Close.Draw(sb);
        }
    }
}
