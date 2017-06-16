using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness
{
    public class PauseMenu
    {
        MessageBox m_ConfirmDialog;
        
        int m_iScrollOffset;
        int m_iScrollMaximum = 1800;
        
        public int m_iSelection;
        public int m_iPuzzleNumber;

        int m_iScreenWidth;
        int m_iScreenHeight;
        DateTime m_PauseStart;
        UIButton[] m_Buttons;
        Rectangle m_ButtonBackground;

        int m_iDragY;
        Rectangle m_HelpRectangle;
        int m_HelpTextLeft;
        int m_HelpTextTop;
        int m_iIconSize;
        int m_iIconSizeSmall;

        Options m_OptionsDialog;

        public PauseMenu(int screenWidth, int screenHeight)
        {
            m_iSelection = 0;
            m_iScreenWidth = screenWidth;
            m_iScreenHeight = screenHeight;

            m_iSelection = 0;
            m_iScrollOffset = 0;


            int buttonX = (int)(Constants.PauseMenu_ButtonX * screenWidth);
            int buttonY = (int)(Constants.PauseMenu_ButtonY * screenHeight);
            int buttonWidth = (int)(Constants.PauseMenu_ButtonWidth * screenWidth);
            int buttonHeight = (int)(Constants.PauseMenu_ButtonHeight * screenWidth);
            int buttonSpace = (int)(Constants.PauseMenu_ButtonSpace * screenHeight);

            m_Buttons = new UIButton[6];
            Rectangle brect = new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
            m_Buttons[0] = new UIButton(0, "Resume Game", Assets.DialogFont, brect, Assets.ScrollBar);
            brect.Y += buttonHeight + buttonSpace;
            m_Buttons[1] = new UIButton(1, "Reset Puzzle", Assets.DialogFont, brect, Assets.ScrollBar);
            brect.Y += buttonHeight + buttonSpace;
            m_Buttons[2] = new UIButton(2, "Unhide Clues", Assets.DialogFont, brect, Assets.ScrollBar);
            m_Buttons[2].ClickSound = SoundManager.SEInst.GameUnhideClues;
            brect.Y += buttonHeight + buttonSpace;
            m_Buttons[3] = new UIButton(3, "Buy Coins", Assets.DialogFont, brect, Assets.ScrollBar);
            brect.Y += buttonHeight + buttonSpace;
            m_Buttons[4] = new UIButton(4, "Options", Assets.DialogFont, brect, Assets.ScrollBar);
            brect.Y += buttonHeight + buttonSpace;
            m_Buttons[5] = new UIButton(5, "Save & Exit", Assets.DialogFont, brect, Assets.ScrollBar);

            m_ButtonBackground = new Rectangle((int)(Constants.PauseMenu_ButtonAreaBGX * screenWidth), (int)(Constants.PauseMenu_ButtonAreaBGY * screenHeight), (int)(Constants.PauseMenu_ButtonAreaBGW * screenWidth), (int)(Constants.PauseMenu_ButtonAreaBGH * screenHeight));


            int helpLeft = (int)(Constants.PauseMenu_HelpLeft * screenWidth);
            m_HelpRectangle = new Rectangle(helpLeft, 0, screenWidth - helpLeft, screenHeight);
            m_HelpTextLeft = (int)(Constants.PauseMenu_HelpTextLeft * screenWidth);
            m_HelpTextTop = (int)(Constants.PauseMenu_HelpTextTop * screenHeight);
            m_iIconSize = (int)(Constants.PauseMenu_IconSize * screenHeight);
            m_iIconSizeSmall = (int)(Constants.PauseMenu_IconSizeSmall * screenHeight);

            m_PauseStart = DateTime.Now;
        }

        #region Input
        public bool HandleClick(int x, int y)
        {
            if (m_OptionsDialog != null)
            {
                if( !m_OptionsDialog.HandleClick(x, y) )
                    m_OptionsDialog = null;
                return true;
            }

            if (m_ConfirmDialog != null)
            {
                MessageBoxResult res = m_ConfirmDialog.HandleClick(x, y);
                if (res == MessageBoxResult.Yes)
                {
                    return DoButtonClick(1);
                }
                else if( res == MessageBoxResult.No )
                {
                    m_ConfirmDialog = null;
                }
            }
            else
            {

                foreach (UIButton b in m_Buttons)
                {
                    if (b.Click(x, y))
                    {
                        return DoButtonClick(b.ButtonID);
                    }
                }
            }

            return true;    // Return true and remain paused
        }

        bool DoButtonClick(int buttonID)
        {
            m_iSelection = buttonID;
            switch (buttonID)
            {
                case 0: // Resume Game
                    return false;
                case 1: // Reset Puzzle
                    if (m_ConfirmDialog == null)
                    {
                        m_ConfirmDialog = new MessageBox("Are you sure you want to reset this puzzle?\nAll progress will be lost.", MessageBoxButtons.YesNo, 0, m_iScreenWidth, m_iScreenHeight);
                    }
                    else
                    {
                        //m_Game.m_SoundManager.PlayMenuAccept();
                        m_ConfirmDialog = null;
                        return false;
                    }
                    break;
                case 2: // Unhide Clues
                    return false;
                case 3: // Buy Coins
                    return false;
                case 4: // Options
                    m_OptionsDialog = new Options(Happiness.Game);
                    break;
                case 5: // Save & Exit
                    return false;
            }
            return true;
        }

        public void OnDragBegin(DragArgs e)
        {
            m_iDragY = e.StartY;
        }

        public void OnDrag(DragArgs e)
        {
            if (m_OptionsDialog != null)
            {
                m_OptionsDialog.Drag(e);
                return;
            }

            int deltaY = e.CurrentY - m_iDragY;
            m_iDragY = e.CurrentY;
            m_iScrollOffset -= deltaY;
            if( m_iScrollOffset < 0 )
                m_iScrollOffset = 0;
            if( m_iScrollOffset > m_iScrollMaximum )
                m_iScrollOffset = m_iScrollMaximum;
        }

        public void OnDragEnd(DragArgs e)
        {
        }
        #endregion

        public void Draw(Renderer spriteBatch)
        {
            spriteBatch.Draw(Assets.Background, new Rectangle(0, 0, m_iScreenWidth, m_iScreenHeight), Color.White);
            

            spriteBatch.Draw(Assets.TransparentBox, m_ButtonBackground, Color.White);
            foreach( UIButton b in m_Buttons )
                b.Draw(spriteBatch);

            DrawHelp(spriteBatch);
            
            if( m_ConfirmDialog != null )
                m_ConfirmDialog.Draw(spriteBatch);
            if( m_OptionsDialog != null )
                m_OptionsDialog.Draw(spriteBatch);
        }

        private int DrawString(Renderer spriteBatch, string text, int iX, int iY, Color cColor)
        {
            return DrawString(spriteBatch, Assets.DialogFont, text, iX, iY, cColor, 2);
        }

        private int DrawString(Renderer spriteBatch, SpriteFont font, string text, int iX, int iY, Color cColor, int iShadowOffset)
        {
            if (iX < 0)
                iX = Math.Abs(iX) - (int)font.MeasureString(text).X;

            spriteBatch.DrawString(font, text, new Vector2(iX + iShadowOffset, iY + iShadowOffset), Color.Black);
            spriteBatch.DrawString(font, text, new Vector2(iX, iY), cColor);
            return (int)font.MeasureString(text).X + iShadowOffset;
        }
                        
        private void DrawHelp(Renderer spriteBatch)
        {
            int iClueMargin = 5;
            int iClueIconGap = 2;
            int iTextIconGap = 2;

            // Draw Background
            spriteBatch.Draw(Assets.HelpBackground, m_HelpRectangle, Color.White);

            int iX = m_HelpTextLeft;
            
            int iY = m_HelpTextTop - m_iScrollOffset;

            // Draw Objective
            DrawString(spriteBatch, Assets.MenuFont, "Objective", iX, iY, Color.Goldenrod, 2);
            iY += 48;

            DrawString(spriteBatch, Assets.HelpFont, "Use the clues to figure out where each icon belongs in the puzzle", iX + 20, iY, Color.White, 1);
            iY += 60;

            // Draw some Vertical Clue help
            DrawString(spriteBatch, Assets.MenuFont, "Vertical Clues", iX, iY, Color.Goldenrod, 2);
            iY += 48;

            DrawString(spriteBatch, Assets.HelpFont, "Vertical clues give information about one column of the puzzle", iX + 20, iY, Color.White, 1);
            iY += 50;


            //eVerticalType.Two,        

            // Draw a VerticalType Two clue here
            Texture2D tIcon1 = Assets.Flowers[4];
            Texture2D tIcon2 = Assets.Puppies[6];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);

            // Icon1 is in the same column as Icon2. 
            int iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is in the same column as the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            //  If you already know where Icon1 is, you know that Icon2 is in the same column and visa versa.  
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you know that the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            DrawString(spriteBatch, Assets.HelpFont, " is in the same column and visa versa.", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know any other icons in the same row as Icon1, you can eliminate Icon2 in that column.    
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know any other icons in the same row as the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, ", you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            DrawString(spriteBatch, Assets.HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know that Icon1 is not in a column, you know that Icon2 is also not in that column so it can be elimnated.
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know that the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is not in a column, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            DrawString(spriteBatch, Assets.HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 50;


            //eVerticalType.TwoNot,
            tIcon1 = Assets.Cars[2];
            tIcon2 = Assets.Hubble[7];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.NotOverlay, new Rectangle(iX + iClueMargin, iY + (m_iIconSize / 2), m_iIconSize, m_iIconSize), Color.White);

            // Icon1 is not in the same colum as Icon2.
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is not the same column as the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            //  If you already know where Icon1 is, you can eliminate Icon2 from that column and visa versa.
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            DrawString(spriteBatch, Assets.HelpFont, " from that column and visa versa.", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 90;

            //eVerticalType.Three
            tIcon1 = Assets.Princesses[0];
            tIcon2 = Assets.Cats[3];
            Texture2D tIcon3 = Assets.Simpsons[6];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);

            // Icon1, Icon2, and Icon3 are all in the same column.
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, ", the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, ", and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " are all in the same column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where one of the icons is, you know the other two are in the same column
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where one icon is, you know the other two are in that same column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know any icons in the same row as one of these icons, you can elminate the other two icons from that column.
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know any icons in the same row as one of these icons, you can elminate the other two icons from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know that any one of the icons is not in a column, you can eliminate the other two icons from that column as well.
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know that any one of the icons is not in a column, you can eliminate the other two icons from that column as well", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 50;


            //eVerticalType.ThreeTopNot,                       
            tIcon1 = Assets.Superheros[4];
            tIcon2 = Assets.Flowers[5];
            tIcon3 = Assets.Hubble[2];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.NotOverlay, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);

            // IconA, and IconB are in the same column and IconC is not in that same column
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, ", and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " are in the same column, and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is not in that same column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //   If you know where IconA is, you know IconB is in that same column and you can eliminate IconC from that column
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you know the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is in that same column and you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //   If you know where IconC is you can eliminate both IconA and IconB from that column
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate both the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from that column ", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 70;

            //eVerticalType.ThreeMidNot,
            tIcon1 = Assets.Hubble[7];
            tIcon2 = Assets.Cats[1];
            tIcon3 = Assets.Cars[5];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.NotOverlay, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);

            // IconA, and IconB are in the same column and IconC is not in that same column
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, ", and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " are in the same column, and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is not in that same column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //   If you know where IconA is, you know IconB is in that same column and you can eliminate IconC from that column
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you know the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is in that same column and you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //   If you know where IconC is you can eliminate both IconA and IconB from that column
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate both the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from that column ", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 70;


            //eVerticalType.ThreeBotNot,
            tIcon1 = Assets.Superheros[7];
            tIcon2 = Assets.Princesses[1];
            tIcon3 = Assets.Flowers[0];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.NotOverlay, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);

            // IconA, and IconB are in the same column and IconC is not in that same column
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, ", and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " are in the same column, and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is not in that same column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //   If you know where IconA is, you know IconB is in that same column and you can eliminate IconC from that column
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you know the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is in that same column and you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //   If you know where IconC is you can eliminate both IconA and IconB from that column
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate both the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from that column ", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 70;


            //eVerticalType.EitherOr
            tIcon1 = Assets.Cars[6];
            tIcon2 = Assets.Puppies[3];
            tIcon3 = Assets.Cats[5];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.EitherOrOverlay, new Rectangle(iX + iClueMargin, iY + m_iIconSize + iClueIconGap + (m_iIconSize / 2), m_iIconSize, m_iIconSize), Color.White);

            // Icon1 is either in the column with Icon2 or the column Icon3 but not both
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is either in the column with the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " or in the column with the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " but not both", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon2 is you can eliminate Icon3 from that column and visa versa since these cant be in the same column.
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from that column and visa versa", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon2 is and you know where Icon3 is, you can eliminate Icon1 from all other columns since it has to be in one of these two columns.
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, and you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can elminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from all other columns", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If a column doesnt have either Icon2 or Icon3 then you can eliminate Icon1 from this column
            iXPos = iX + iClueMargin + m_iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If a column doesnt have either the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " or the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " then you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 70;


            // Draw some Horizontal Clue help
            DrawString(spriteBatch, Assets.MenuFont, "Horizontal Clues", iX, iY, Color.Goldenrod, 2);
            iY += 40;

            DrawString(spriteBatch, Assets.HelpFont, "Horizontal Clues give information about the rows of the puzzle", iX + 20, iY, Color.White, 1);
            iY += 50;



            //eHorizontalType.NextTo,
            tIcon1 = Assets.Princesses[4];
            tIcon2 = Assets.Cats[6];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            iY += m_iIconSize + iClueIconGap;
            
            // Icon1 is next to Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is in a column next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);            
            iY += 20;

            //  If you know where Icon1 is, you can eliminate Icon2 from all columns that arent next to the column with Icon1.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from all columns that aren't next to the column with the", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            //  If a column does not have Icon1 to the right or to the left, then you can eliminate Icon2 from this column.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If a column does not have the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " to the right or to the left, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 80;


            //eHorizontalType.NotNextTo,
            tIcon1 = Assets.Hubble[1];
            tIcon2 = Assets.Simpsons[6];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.NotOverlay, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            iY += m_iIconSize + iClueIconGap;

            // Icon1 is not next to Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is not in a column next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            //  If you know where Icon1 is, you can eliminate Icon2 from the column to the left and the column to the right and visa versa.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from the columns to the left and right", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 100;


            //eHorizontalType.LeftOf
            tIcon1 = Assets.Flowers[2];
            tIcon2 = Assets.Superheros[5];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.LeftOfIcon, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            iY += m_iIconSize + iClueIconGap;

            // Icon1 is to the left of Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is somwhere to the left of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            //  If you know where Icon1 is, you can eliminate Icon2 from that column and all columns to the left.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from that column and all columns to the left", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon2 is, you can eliminate Icon1 from that column and all columns to the right.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from that column and all columns to the right", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  You can eliminate Icon2 from any columns on the left side of the puzzle that would result in Icon1 being in the same column or any column to the right.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "You can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from any columns that would result in the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " being in the same column or any column to the right", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;                       

            //  You can eliminate Icon1 from any columns on the right side of the puzzle that would result in Icon2 being in the same column or any column to the left
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "You can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from any columns that would result in the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " being in the same column or any column to the left", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 40; 


            //eHorizontalType.NotLeftOf,
            tIcon1 = Assets.Cats[7];
            tIcon2 = Assets.Princesses[2];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.LeftOfIcon, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.NotOverlay, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            iY += m_iIconSize + iClueIconGap;

            // Icon1 is not to the left of Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is not to the left of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            //  Icon1 and Icon2 can be in the same column as Icon1 would not be left of Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " can be in the same column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon1 is, you can eliminate Icon2 from all columns to the right
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from all columns to the right", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon2 is, you can eliminate Icon1 from all columns to the left
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from all columns to the left", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 60;


            //eHorizontalType.Span
            tIcon1 = Assets.Flowers[3];
            tIcon2 = Assets.Hubble[6];
            tIcon3 = Assets.Puppies[0];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.SpanOverlay, new Rectangle(iX + iClueMargin, iY, (m_iIconSize * 3) + (iClueIconGap * 2) , m_iIconSize), Color.White);
            iY += m_iIconSize + iClueIconGap;

            // Icon2 has Icon1 on one side and Icon3 on the other side
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " has the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " on one side and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " on the other side", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon1 is, Icon2 has to be next to it on either side and Icon3 has to be next to Icon2 in the same direction.  You can eliminate Icon2 from all columns that arent next to Icon1 and Icon3 from all columns that arent 2 columns away from Icon1
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " has to be on either side of it and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " has to be next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " in the same direction", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon3 is, Icon2 has to be next to it on either side and Icon1 has to be next to Icon2 in the same direction.  You can eliminate Icon2 from all columns that arent next to Icon3 and Icon1 from all columns that arent 2 columns away from Icon3
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " has to be on either side of it and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " has to be next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " in the same direction", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon2 is, you can eliminate Icon1 and Icon3 from all columns that arent next to Icon2.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from all columns that arent next to ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            //  Icon2 is in the middle of Icon1 and Icon3, thus you can eliminate Icon2 from the left most column and the right most column.
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is in the middle of ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " so you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from the left most and right most columns", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 40;


            //eHorizontalType.SpanNotLeft
            tIcon1 = Assets.Simpsons[1];
            tIcon2 = Assets.Princesses[0];
            tIcon3 = Assets.Cats[7];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.SpanOverlay, new Rectangle(iX + iClueMargin, iY, (m_iIconSize * 3) + (iClueIconGap * 2), m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.NotOverlay, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            iY += m_iIconSize + iClueIconGap;

            // Icon2 has Icon3 on one side and not Icon1 on the other side
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " has the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " on one side and not the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " on the other side", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon3 is, you can eliminate Icon2 from all columns that arent next to Icon3
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from all columns that arent next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            //  If you know where Icon2 is, you can eliminate Icon3 from all columns that arent next to Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from all columns that aren't next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            //  If you know where Icon2 is and you know where Icon3 is, you can eliminate Icon1 from the other side of Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is and you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from the other side of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            //  Icon2 is in the middle of Icon3 and not Icon1, thus you can eliminate Icon2 from the left most column and the right most column.
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is in the middle of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " and not the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " so you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from the left most and right most columns", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 40;

            //eHorizontalType.SpanNotMid,
            tIcon1 = Assets.Cars[4];
            tIcon2 = Assets.Hubble[7];
            tIcon3 = Assets.Puppies[3];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.SpanOverlay, new Rectangle(iX + iClueMargin, iY, (m_iIconSize * 3) + (iClueIconGap * 2), m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.NotOverlay, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            iY += m_iIconSize + iClueIconGap;

            // Icon1 and Icon3 have an icon between them that is not Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " have a column between them that does not have the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " in it", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            // If you know where Icon1 is, you can eliminate Icon3 from all columns that are not 2 columns left or right of Icon1.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from all columns that aren't 2 columns left or right of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            // If you know where Icon3 is, you can eliminate Icon1 from all columns that are not 2 columns left or right of Icon3.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from all columns that aren't 2 columns left or right of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            // If you know where Icon1 is and you know where Icon3 is, you can eliminate Icon2 from the middle column.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is and you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from the column in between", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 60;
            
            //eHorizontalType.SpanNotRight
            tIcon1 = Assets.Cats[0];
            tIcon2 = Assets.Flowers[5];
            tIcon3 = Assets.Superheros[0];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.SpanOverlay, new Rectangle(iX + iClueMargin, iY, (m_iIconSize * 3) + (iClueIconGap * 2), m_iIconSize), Color.White);
            spriteBatch.Draw(Assets.NotOverlay, new Rectangle(iX + iClueMargin + m_iIconSize + iClueIconGap + m_iIconSize + iClueIconGap, iY, m_iIconSize, m_iIconSize), Color.White);
            iY += m_iIconSize + iClueIconGap;

            // Icon2 has Icon1 on one side and not Icon3 on the other side
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " has the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " on one side and not the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " on the other side", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon1 is, you can eliminate Icon2 from all columns that arent next to Icon1
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from all columns that aren't next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            //  If you know where Icon2 is, you can eliminate Icon1 from all columns that arent next to Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from all columns that aren't next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            //  If you know where Icon2 is and you know where Icon1 is, you can eliminate Icon3 from the other side of Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is and you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from the other side of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iY += 20;

            //  Icon2 is in the middle of Icon1 and not Icon3, thus you can eliminate Icon2 from the left most column and the right most column.
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " is in the middle of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " and not the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " so you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, m_iIconSizeSmall, m_iIconSizeSmall), Color.White);
            iXPos += m_iIconSizeSmall;
            iXPos += DrawString(spriteBatch, Assets.HelpFont, " from the left most and right most columns", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 40;
        }

        #region Accessors
        public double Elapsed
        {
            get { return (DateTime.Now - m_PauseStart).TotalSeconds; }
        }
        #endregion
    }
}
