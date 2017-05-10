using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HappinessNetwork;

namespace Happiness
{
    class FloorSelectDialog
    {
        Rectangle m_Rect;
        Happiness m_Game;
        int m_iTower;

        int m_iLeftRightMargin;
        int m_iFloorSelectTutorialWidth;

        UILabel m_Title;
        UIButton[] m_Buttons;

        UIFrame m_ScrollFrame;
        Rectangle m_FloorScrollRect;
        float m_FloorScrollPosition;
        float m_DragY;
        float m_ScrollMin;
        List<FloorDisplay> m_Floors;
        int m_iSelectedFloor;

        bool m_bFloorDataRetrieved;
        Rectangle m_FloorWaitRectangle;
        UILabel m_FloorDataFetchText;

        #region Setup
        public FloorSelectDialog(int tower, int screenWidth, int screenHeight, Happiness game)
        {
            m_Game = game;
            m_iTower = tower;

            int iCenterX = screenWidth >> 1;
            int iCenterY = screenHeight >> 1;
            m_FloorScrollPosition = 0;
            m_iSelectedFloor = -1;

            // Frame rect
            int width = (int)(Constants.FloorSelectDialog_Width * screenWidth);
            int height = (int)(Constants.FloorSelectDialog_Height * screenHeight);
            m_Rect = new Rectangle(iCenterX - (width >> 1), iCenterY - (height >> 1), width, height);

            // Title
            int iTopMargin = (int)(Constants.FloorSelectDialog_MarginTopBottom * screenHeight);
            string title = string.Format("{0} x {0}", tower + 3);
            m_Title = new UILabel(title, iCenterX, m_Rect.Top + iTopMargin, Color.Goldenrod, Assets.MenuFont, UILabel.XMode.Center);

            // Buttons
            m_iLeftRightMargin = (int)(Constants.FloorSelectDialog_MarginLeftRight * screenWidth);
            int navButtonTop = m_Rect.Top + iTopMargin;
            int navButtonWidth = (int)(Constants.FloorSelectDialog_NavButtonWidth * screenWidth);
            int navButtonHeight = (int)(Constants.FloorSelectDialog_NavButtonHeight * screenHeight);
            int lbButtonWidth = (int)(Constants.FloorSelectDialog_LBButtonWidth * screenWidth);
            int lbButtonHeight = (int)(Constants.FloorSelectDialog_LBButtonHeight * screenHeight);
            m_Buttons = new UIButton[3];
            m_Buttons[0] = new UIButton(0, "<", Assets.MenuFont, new Rectangle(m_Rect.Left + m_iLeftRightMargin, navButtonTop, navButtonWidth, navButtonHeight), Assets.ScrollBar);
            m_Buttons[1] = new UIButton(1, ">", Assets.MenuFont, new Rectangle(m_Rect.Right - (m_iLeftRightMargin + navButtonWidth), navButtonTop, navButtonWidth, navButtonHeight), Assets.ScrollBar);
            m_Buttons[2] = new UIButton(2, "Leaderboard", Assets.DialogFont, new Rectangle(m_Rect.Right - (m_iLeftRightMargin + lbButtonWidth), m_Rect.Bottom - (iTopMargin + lbButtonHeight), lbButtonWidth, lbButtonHeight), Assets.ScrollBar);
            m_Buttons[1].Enabled = false;
            m_Buttons[2].Enabled = false;
            m_Buttons[0].ClickSound = SoundManager.SEInst.MenuCancel;

            // Floors
            int floorScrollWidth = (int)(Constants.FloorSelectDialog_FloorScrollWidth * screenWidth);
            int floorScrollTop = m_Title.PositionY + m_Title.Height + (iTopMargin >> 1);
            int floorScrollBottom = m_Rect.Bottom - iTopMargin;
            int floorScrollHeight = floorScrollBottom - floorScrollTop;
            int waitIconSize = (int)(Constants.FloorSelectDialog_WaitIconSize * screenHeight);
            int halfWaitIconSize = waitIconSize >> 1;
            m_FloorScrollRect = new Rectangle(iCenterX - (floorScrollWidth >> 1), floorScrollTop, floorScrollWidth, floorScrollHeight);
            m_FloorWaitRectangle = new Rectangle(iCenterX - halfWaitIconSize, iCenterY - halfWaitIconSize, waitIconSize, waitIconSize);

            m_FloorDataFetchText = new UILabel("Fetching Data...", iCenterX, m_FloorWaitRectangle.Bottom, Color.White, Assets.DialogFont, UILabel.XMode.Center);

            m_ScrollFrame = new UIFrame(5, m_FloorScrollRect);

            m_iFloorSelectTutorialWidth = (int)(Constants.FloorSelectDialog_FloorSelectTutorialWidth * screenWidth);
            int floorPlayTutorialWidth = (int)(Constants.FloorSelectDialog_PlayTutorialWidth * screenWidth);
            m_Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.FloorPlay, new Vector2(m_Buttons[1].Rect.Left, m_Buttons[1].Rect.Bottom), (float)-Math.PI / 4,
                                                                                 new Rectangle(m_FloorScrollRect.Right + m_iLeftRightMargin, m_Buttons[1].Rect.Bottom + m_Game.Tutorial.ArrowWidth, floorPlayTutorialWidth, 0), "Press this button to play the selected floor.", TutorialSystem.TutorialPiece.None, m_Buttons[1].Rect);

            SetupFloorData(game.m_GameInfo.TowerData[m_iTower]);
            m_bFloorDataRetrieved = true;
        }

        public int FloorDataComparer(FloorDisplay a, FloorDisplay b)
        {
            return b.Floor - a.Floor;
        }

        void SetupFloorData(TowerData td)
        {
            double parTime = Balance.ParTime(td.Tower);
            m_Floors = new List<FloorDisplay>();
            int highestFloor = 0;
            foreach (TowerFloorRecord floor in td.Floors)
            {
                m_Floors.Add(new FloorDisplay(m_FloorScrollRect.Left, m_FloorScrollRect.Width, floor.Floor, floor.RankFriends, floor.RankGlobal, floor.BestTime, parTime));
                if( floor.Floor > highestFloor )
                    highestFloor = floor.Floor;
            }
            m_Floors.Add(new FloorDisplay(m_FloorScrollRect.Left, m_FloorScrollRect.Width, highestFloor + 1, 0, 0, 0, parTime));
            m_Floors.Sort(FloorDataComparer);

            float floorHeight = m_Floors[0].Height;
            int visibleFloors = (int)(m_FloorScrollRect.Height / floorHeight);
            m_ScrollMin = -((m_Floors.Count - visibleFloors) * m_Floors[0].Height);

            // Set tutorial stuff
            float tutorialArrowY = m_FloorScrollRect.Top + m_Floors[0].Height * 0.5f;
            m_Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.FloorSelect, new Vector2(m_FloorScrollRect.Left, tutorialArrowY), 0,
                                         new Rectangle(m_FloorScrollRect.Left - (m_iFloorSelectTutorialWidth + m_iLeftRightMargin), m_FloorScrollRect.Top + m_Game.Tutorial.ArrowHeight, m_iFloorSelectTutorialWidth, 0), 
                                         "Here you can select the next incomplete tower floor or any floor you have previously completed.\n\nTap the first floor now to select it.", TutorialSystem.TutorialPiece.None, 
                                         new Rectangle(m_FloorScrollRect.Left, m_FloorScrollRect.Top, m_FloorScrollRect.Width, (int)m_Floors[0].Height));
            m_Game.Tutorial.TriggerPiece(TutorialSystem.TutorialPiece.FloorSelect);
        }
        #endregion

        #region Input
        public bool HandleClick(int x, int y)
        {
            if (m_bFloorDataRetrieved)      // Dont allow clicks to happen while we are waiting for data
            {
                foreach (UIButton b in m_Buttons)
                {
                    if (b.Click(x, y))
                    {
                        switch (b.ButtonID)
                        {
                            case 0:
                                m_Game.Tutorial.CancelPiece();
                                return false;
                            case 1:
                                LaunchGame();
                                break;
                            case 2:
                                ShowLeaderboard();
                                break;
                        }
                    }
                }

                if( m_FloorScrollRect.Contains(x, y) )
                    FloorScrollClick(x, y);
            }
            return true;
        }

        void FloorScrollClick(int x, int y)
        {
            // Clear any currently selected
            int lastSelected = m_iSelectedFloor;
            if (m_iSelectedFloor >= 0)
            {
                m_Floors[m_iSelectedFloor].Selected = false;
                m_iSelectedFloor = -1;
            }

            // Select the clicked floor
            float fY = (y - m_FloorScrollRect.Top) - m_FloorScrollPosition;
            int selectFloor = (int)(fY / m_Floors[0].Height);
            if (selectFloor >= 0 && selectFloor < m_Floors.Count)
            {
                m_iSelectedFloor = selectFloor;
                m_Floors[m_iSelectedFloor].Selected = true;

                m_Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.FloorSelect);
                m_Game.Tutorial.TriggerPiece(TutorialSystem.TutorialPiece.FloorPlay);

                SoundManager.Inst.PlaySound(SoundManager.SEInst.MenuAccept);
            }

            bool enableButtons = m_iSelectedFloor >= 0;
            m_Buttons[1].Enabled = enableButtons;
            m_Buttons[2].Enabled = enableButtons;

            if( m_iSelectedFloor == lastSelected && enableButtons )
                LaunchGame();
        }

        public void DragBegin(DragArgs args)
        {
            m_DragY = (float)args.StartY;
        }

        public void Drag(DragArgs args)
        {
            float deltaY = m_DragY - args.CurrentY;
            m_DragY = args.CurrentY;
            if (deltaY != 0)
            {
                m_FloorScrollPosition -= deltaY;
                if (m_FloorScrollPosition < m_ScrollMin)
                    m_FloorScrollPosition = m_ScrollMin;
                if (m_FloorScrollPosition > 0)
                    m_FloorScrollPosition = 0;
            }
        }
        #endregion

        public void Update(GameTime gt)
        {            
        }

        #region Drawing
        public void Draw(SpriteBatch sb)
        {
            // Draw frame
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);

            // Draw Buttons
            foreach( UIButton b in m_Buttons )
                b.Draw(sb);

            // Draw Title
            m_Title.Draw(sb);

            // Draw Floors
            DrawFloorScroll(sb);
        }

        void DrawFloorScroll(SpriteBatch sb)
        {
            // Draw floors
            if (m_bFloorDataRetrieved)
            {
                Rectangle r = sb.GraphicsDevice.ScissorRectangle;
                sb.GraphicsDevice.ScissorRectangle = m_FloorScrollRect;
                float y = m_FloorScrollRect.Top + m_FloorScrollPosition;
                foreach (FloorDisplay floor in m_Floors)
                {
                    floor.Draw(sb, (int)y, 1.0f);
                    y += floor.Height;
                }
                sb.GraphicsDevice.ScissorRectangle = r;
            }
            else
            {
                // Draw wait icon
                Assets.WaitIcon.Draw(sb, m_FloorWaitRectangle, Color.White);
                m_FloorDataFetchText.Draw(sb);
            }

            // Draw frame
            m_ScrollFrame.Draw(sb);
        }
        #endregion


        void LaunchGame()
        {
            if (m_iSelectedFloor >= 0)
            {
                m_Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.FloorPlay);
                FloorDisplay floor = m_Floors[m_iSelectedFloor];

                // Launch the puzzle
                GameScene gs = new GameScene(m_Game);
                gs.Initialize(floor.Floor, m_iTower + 3, true);
                m_Game.GotoScene(gs);
            }
        }

        void ShowLeaderboard()
        {
        }
    }
}
