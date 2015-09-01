using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class PauseMenu
    {
        Happiness m_Game;
        public GameTime m_GameTime;

        string m_szConfirmText1;
        string m_szConfirmText2;
        string m_szConfirmCancelText = "No";
        string m_szConfirmAcceptText = "Yes";
        Rectangle m_rConfirmAccept;
        Rectangle m_rConfirmCancel;
        bool m_bConfirmDialog;
        int m_iConfirmSelection;
        SaveGame m_SaveGame = null;

        Rectangle m_rScrollBarUp;
        Rectangle m_rScrollBarDown;
        int m_iScrollOffset;
        int m_iScrollAmmount = 50;
        int m_iScrollMaximum = 1600;

        Rectangle m_rGotoLeft;
        Rectangle m_rGotoRight;
        bool m_bGotoDialog;
        int m_iGotoSelection;

        int m_iTextX;
        int m_iTextY;
        int m_iMouseX;
        int m_iMouseY;
        double m_dfScrollSpeed = 1.0;
        double m_dfScrollTime = 0;

        public int m_iSelection;
        public int m_iPuzzleNumber;

        public PauseMenu(Happiness theGame)
        {
            m_iSelection = 0;
            m_Game = theGame;
        }

        public void Init()
        {
            m_iSelection = 0;
            m_bConfirmDialog = false;
            m_bGotoDialog = false;
            m_iScrollOffset = 0;
        }

        public void SetSaveGame(SaveGame save)
        {
            if (m_SaveGame != save)
                m_SaveGame = save;
        }

        public void NavigateDown()
        {
            if (m_bConfirmDialog)
            {
                m_iConfirmSelection++;
                if (m_iConfirmSelection > 1)
                    m_iConfirmSelection = 0;
            }
            else if (m_bGotoDialog)
            {
                m_iGotoSelection++;
                if (m_iGotoSelection > 2)
                    m_iGotoSelection = 0;
            }
            else
            {
                m_iSelection++;
                if (m_iSelection > 8)
                    m_iSelection = 0;
            }
            m_Game.m_SoundManager.PlayMenuNavigate();
        }

        public void NavigateUp()
        {
            if (m_bConfirmDialog)
            {
                m_iConfirmSelection--;
                if (m_iConfirmSelection < 0)
                    m_iConfirmSelection = 1;
            }
            else if (m_bGotoDialog)
            {
                m_iGotoSelection--;
                if (m_iGotoSelection < 0)
                    m_iGotoSelection = 2;
            }
            else
            {
                m_iSelection--;
                if (m_iSelection < 0)
                    m_iSelection = 8;
            }
            m_Game.m_SoundManager.PlayMenuNavigate();
        }

        public void NavigateLeft()
        {
            if (m_bConfirmDialog)
            {
                m_iConfirmSelection--;
                if (m_iConfirmSelection < 0)
                    m_iConfirmSelection = 1;
                m_Game.m_SoundManager.PlayMenuNavigate();
            }
            else if (m_bGotoDialog)
            {
                if (m_iGotoSelection == 2)
                    m_iGotoSelection = 1;
                else if (m_iGotoSelection == 1)
                    m_iGotoSelection = 2;
                else
                    DecrementPuzzleNumber();
                m_Game.m_SoundManager.PlayMenuNavigate();
            }
        }

        public void NavigateRight()
        {
            if (m_bConfirmDialog)
            {
                m_iConfirmSelection++;
                if (m_iConfirmSelection > 1)
                    m_iConfirmSelection = 0;
                m_Game.m_SoundManager.PlayMenuNavigate();
            }
            else if (m_bGotoDialog)
            {
                if (m_iGotoSelection == 2)
                    m_iGotoSelection = 1;
                else if (m_iGotoSelection == 1)
                    m_iGotoSelection = 2;
                else
                    IncrementPuzzleNumber();
                m_Game.m_SoundManager.PlayMenuNavigate();
            }
        }

        private void DecrementPuzzleNumber()
        {
            m_iPuzzleNumber--;
            if (m_iPuzzleNumber < 0)
                m_iPuzzleNumber = 67108863;
        }

        private void IncrementPuzzleNumber()
        {
            m_iPuzzleNumber++;
            if (m_iPuzzleNumber > 67108863)
                m_iPuzzleNumber = 0;
        }

        // Return false if this menu should close
        public bool CommitSelection()
        {
            if (m_bConfirmDialog)
            {
                if (m_iConfirmSelection == 0)
                {
                    // No
                    m_bConfirmDialog = false;
                    m_Game.m_SoundManager.PlayMenuCancel();
                    return true;
                }
            }
            else if (m_bGotoDialog)
            {
                if (m_iGotoSelection == 1)
                {
                    // Cancel
                    m_bGotoDialog = false;
                    m_Game.m_SoundManager.PlayMenuCancel();
                    return true;
                }
                else if (m_iGotoSelection == 0)
                {
                    IncrementPuzzleNumber();
                    m_Game.m_SoundManager.PlayMenuNavigate();
                    return true;
                }
            }

            switch (m_iSelection)
            {
                case 0:         // Resume Game
                    m_Game.m_SoundManager.PlayMenuCancel();
                    return false;
                case 1:         // Reset Puzzle
                    if (!m_bConfirmDialog)
                    {
                        m_szConfirmText1 = "Are you sure you want to reset this puzzle?";
                        m_szConfirmText2 = "All progress will be lost";
                        m_bConfirmDialog = true;
                        m_iConfirmSelection = 0;
                        m_Game.m_SoundManager.PlayMenuAccept();
                    }
                    else
                    {
                        m_Game.m_SoundManager.PlayMenuAccept();
                        return false;
                    }
                    break;
                case 2:         // Goto Puzzle
                    if (!m_bGotoDialog && !m_bConfirmDialog)
                    {
                        m_bGotoDialog = true;
                        m_Game.m_SoundManager.PlayMenuAccept();
                    }
                    else if (m_bGotoDialog && !m_bConfirmDialog)
                    {
                        m_bGotoDialog = false;
                        m_szConfirmText1 = "Are you sure you want to go to puzzle: " + m_iPuzzleNumber.ToString();
                        m_szConfirmText2 = "All progress on this puzzle will be lost";
                        m_bConfirmDialog = true;
                        m_iConfirmSelection = 0;
                        m_Game.m_SoundManager.PlayMenuAccept();
                    }
                    else
                    {
                        m_Game.m_SoundManager.PlayMenuAccept();
                        return false;
                    }
                    break;
                case 3:         // Controls
                    break;
                case 4:         // Options
                    m_Game.m_Options.Init();
                    m_Game.m_bOptionsDialog = true;
                    m_Game.m_SoundManager.PlayMenuAccept();
                    break;
                case 5:         // Save Game
                    m_Game.m_SoundManager.PlayMenuAccept();
                    return false;
                case 6:         // Load Game
                    if (m_SaveGame != null && m_SaveGame.m_bIsValid)
                    {
                        if (!m_bConfirmDialog)
                        {
                            m_szConfirmText1 = "Are you sure you want to load your save game?";
                            m_szConfirmText2 = "All progress on this puzzle will be lost";
                            m_bConfirmDialog = true;
                            m_iConfirmSelection = 0;
                            m_Game.m_SoundManager.PlayMenuAccept();
                        }
                        else
                        {
                            m_Game.m_SoundManager.PlayMenuAccept();
                            return false;
                        }
                    }
                    break;
                case 7:         // Save & Exit
                    m_Game.m_SoundManager.PlayMenuAccept();
                    return false;
                case 8:         // Exit
                    if (!m_bConfirmDialog)
                    {
                        m_szConfirmText1 = "Are you sure you want to exit?";
                        m_szConfirmText2 = "All progress will be lost";
                        m_bConfirmDialog = true;
                        m_iConfirmSelection = 0;
                        m_Game.m_SoundManager.PlayMenuAccept();
                    }
                    else
                    {
                        m_Game.m_SoundManager.PlayMenuAccept();
                        return false;
                    }
                    break;
            }            
            return true;
        }

        public bool CancelSelection()
        {
            if (m_bConfirmDialog)
            {
                m_bConfirmDialog = false;
                m_Game.m_SoundManager.PlayMenuCancel();
            }
            else if (m_bGotoDialog)
            {
                m_bGotoDialog = false;
                m_Game.m_SoundManager.PlayMenuCancel();
            }
            return true;
        }

        // Return false if this menu should close
        public bool HandleClick(int iX, int iY)
        {
            bool bRet = true;
            if (m_bConfirmDialog)
            {
                if (m_rConfirmCancel.Contains(iX, iY))
                {
                    m_bConfirmDialog = false;
                    m_Game.m_SoundManager.PlayMenuCancel();
                }
                else if (m_rConfirmAccept.Contains(iX, iY))
                {
                    bRet = CommitSelection();                    
                }
            }
            else if (m_bGotoDialog)
            {
                if (m_rGotoLeft.Contains(iX, iY))
                {
                    DecrementPuzzleNumber();
                    m_Game.m_SoundManager.PlayMenuNavigate();
                }
                if (m_rGotoRight.Contains(iX, iY))
                {
                    IncrementPuzzleNumber();
                    m_Game.m_SoundManager.PlayMenuNavigate();
                }

                if (m_rConfirmCancel.Contains(iX, iY))
                {
                    m_bGotoDialog = false;
                    m_Game.m_SoundManager.PlayMenuCancel();
                }
                else if (m_rConfirmAccept.Contains(iX, iY))
                {
                    bRet = CommitSelection();
                }
            }
            else
            {
                if (m_rScrollBarUp.Contains(iX, iY))
                {
                    ScrollUp();
                }
                else if (m_rScrollBarDown.Contains(iX, iY))
                {
                    ScrollDown();
                }
                else
                {
                    bRet = CommitSelection();
                }
            }
            return bRet;
        }

        public void UpdateMouse(int iX, int iY, bool bLeftButtonDown)
        {
            m_iMouseX = iX;
            m_iMouseY = iY;
            if (m_bConfirmDialog)
            {
                if (m_rConfirmAccept.Contains(iX, iY))
                {
                    if (m_iConfirmSelection != 1)
                    {
                        m_iConfirmSelection = 1;
                        m_Game.m_SoundManager.PlayMenuNavigate();
                    }
                }
                else if (m_rConfirmCancel.Contains(iX, iY))
                {
                    if (m_iConfirmSelection != 0)
                    {
                        m_iConfirmSelection = 0;
                        m_Game.m_SoundManager.PlayMenuNavigate();
                    }
                }
            }
            else if (m_bGotoDialog)
            {
                if (m_rConfirmAccept.Contains(iX, iY))
                {
                    if (m_iGotoSelection != 2)
                    {
                        m_iGotoSelection = 2;
                        m_Game.m_SoundManager.PlayMenuNavigate();
                    }
                }
                else if (m_rConfirmCancel.Contains(iX, iY))
                {
                    if (m_iGotoSelection != 1)
                    {
                        m_iGotoSelection = 1;
                        m_Game.m_SoundManager.PlayMenuNavigate();
                    }
                }

                bool bLeft = m_Game.m_Input.IsAControllerHoldingLeft();
                bool bRight = m_Game.m_Input.IsAControllerHoldingRight();
                if (bLeftButtonDown || bLeft || bRight)
                {
                    if (m_dfScrollTime <= 0)
                        m_dfScrollTime = m_GameTime.TotalGameTime.TotalSeconds;
                    if (m_GameTime.TotalGameTime.TotalSeconds - m_dfScrollTime > 0.8)
                    {
                        const double dfSpeed = 0.04;
                        if (m_rGotoLeft.Contains(iX, iY) || (m_iGotoSelection == 0 && bLeft))
                        {
                            m_iPuzzleNumber -= (int)m_dfScrollSpeed;
                            if (m_iPuzzleNumber < 0)
                                m_iPuzzleNumber = 67108864 - m_iPuzzleNumber;
                            m_dfScrollSpeed += dfSpeed;
                            m_Game.m_SoundManager.PlayMenuNavigate();
                        }
                        else if (m_rGotoRight.Contains(iX, iY) || (m_iGotoSelection == 0 && bRight))
                        {
                            m_iPuzzleNumber += (int)m_dfScrollSpeed;
                            if (m_iPuzzleNumber > 67108863)
                                m_iPuzzleNumber = m_iPuzzleNumber - 67108864;
                            m_dfScrollSpeed += dfSpeed;
                            m_Game.m_SoundManager.PlayMenuNavigate();
                        }
                    }
                }
                else
                {
                    m_dfScrollTime = 0;
                    m_dfScrollSpeed = 1.0;
                }
            }
            else
            {
                for (int i = 0; i < 9; i++)
                {
                    Rectangle r = new Rectangle(m_iTextX - 20, m_Game.m_iScreenTop + m_iTextY + (i * 30), 250, 30);
                    if (r.Contains(iX, iY))
                    {
                        if (m_iSelection != i)
                        {
                            m_iSelection = i;
                            m_Game.m_SoundManager.PlayMenuNavigate();
                        }
                        break;
                    }
                }
            }
        }

        public void ScrollUp()
        {
            if (m_iScrollOffset > 0)
            {
                m_iScrollOffset -= m_iScrollAmmount;
                if (m_iScrollOffset < 0)
                    m_iScrollOffset = 0;
                m_Game.m_SoundManager.PlaySliderMove();
            }
        }

        public void ScrollDown()
        {
            if (m_iScrollOffset < m_iScrollMaximum)
            {
                m_iScrollOffset += m_iScrollAmmount;
                if (m_iScrollOffset > m_iScrollMaximum)
                    m_iScrollOffset = m_iScrollMaximum;
                m_Game.m_SoundManager.PlaySliderMove();
            }            
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int iWidth = m_Game.m_iScreenWidth;
            int iHeight = m_Game.m_iScreenHeight;
            spriteBatch.Draw(m_Game.m_Background, new Rectangle(0, 0, iWidth, iHeight), Color.White);

            int iX = 50;
            int iY = 50;
            spriteBatch.Draw(m_Game.m_Transparent, new Rectangle(iX, iY, 230, 310), Color.White);

            m_iTextX = iX + 20;
            m_iTextY = iY + 20;
            DrawString(spriteBatch, "Resume Game", m_iTextX, m_iTextY, Color.Goldenrod);

            const int iTextSpacing = 30;
            m_iTextY += iTextSpacing;
            DrawString(spriteBatch, "Reset Puzzle", m_iTextX, m_iTextY, Color.Goldenrod);

            m_iTextY += iTextSpacing;
            DrawString(spriteBatch, "Goto Puzzle", m_iTextX, m_iTextY, Color.Goldenrod);

            m_iTextY += iTextSpacing;
            DrawString(spriteBatch, "Controls", m_iTextX, m_iTextY, Color.Goldenrod);

            m_iTextY += iTextSpacing;
            DrawString(spriteBatch, "Options", m_iTextX, m_iTextY, Color.Goldenrod);

            m_iTextY += iTextSpacing;
            DrawString(spriteBatch, "Save Game", m_iTextX, m_iTextY, Color.Goldenrod);

            m_iTextY += iTextSpacing;
            Color cLoadColor = (m_SaveGame != null && m_SaveGame.m_bIsValid) ? Color.Goldenrod : Color.Gray;
            DrawString(spriteBatch, "Load Game", m_iTextX, m_iTextY, cLoadColor);

            m_iTextY += iTextSpacing;
            DrawString(spriteBatch, "Save & Exit", m_iTextX, m_iTextY, Color.Goldenrod);

            m_iTextY += iTextSpacing;
            DrawString(spriteBatch, "Exit", m_iTextX, m_iTextY, Color.Goldenrod);

            int iArrowWidth = 50;
            int iArrowX = m_iTextX - iArrowWidth;
            int iArrowY = iY + 16 + (m_iSelection * iTextSpacing);
            spriteBatch.Draw(m_Game.m_BlueArrow, new Rectangle(iArrowX, iArrowY, iArrowWidth, 40), Color.White);

            if (m_iSelection == 3)
                DrawControls(spriteBatch);
            else
                DrawHelp(spriteBatch);

            if (m_bGotoDialog)
                DrawGotoDialog(spriteBatch, iWidth, iHeight);
            if (m_bConfirmDialog)
                DrawConfirmDialog(spriteBatch, iWidth, iHeight);

            m_iTextY = iY + 20;
        }

        private int DrawString(SpriteBatch spriteBatch, string text, int iX, int iY, Color cColor)
        {
            return DrawString(spriteBatch, m_Game.m_DialogFont, text, iX, iY, cColor, 2);
        }

        private int DrawString(SpriteBatch spriteBatch, SpriteFont font, string text, int iX, int iY, Color cColor, int iShadowOffset)
        {
            if (iX < 0)
                iX = Math.Abs(iX) - (int)font.MeasureString(text).X;

            spriteBatch.DrawString(font, text, new Vector2(iX + iShadowOffset, iY + iShadowOffset), Color.Black);
            spriteBatch.DrawString(font, text, new Vector2(iX, iY), cColor);
            return (int)font.MeasureString(text).X + iShadowOffset;
        }

        private void DrawGotoDialog(SpriteBatch spriteBatch, int iScreenW, int iScreenH)
        {
            int iDialogWidth = 400;
            int iDialogHeight = 100;
            int iHalfScreenW = iScreenW / 2;
            int iHalfScreenH = iScreenH / 2;
            int iDialogX = iHalfScreenW - (iDialogWidth / 2);
            int iDialogY = iHalfScreenH - (iDialogHeight / 2);

            Rectangle r = new Rectangle(iDialogX, iDialogY, iDialogWidth, iDialogHeight);
            spriteBatch.Draw(m_Game.m_Transparent, r, Color.SteelBlue);
            spriteBatch.Draw(m_Game.m_Transparent, r, Color.SteelBlue);
            spriteBatch.Draw(m_Game.m_Transparent, r, Color.SteelBlue);
            spriteBatch.Draw(m_Game.m_Transparent, r, Color.SteelBlue);

            int iTextX = iDialogX + 20;
            int iTextY = iDialogY + 20;
            spriteBatch.DrawString(m_Game.m_DialogFont, "Puzzle #:", new Vector2(iTextX + 2, iTextY + 2), Color.Black);
            spriteBatch.DrawString(m_Game.m_DialogFont, "Puzzle #:", new Vector2(iTextX, iTextY), Color.Goldenrod);

            // Draw the value box
            int iValueBoxWidth = 140;
            int iValueBoxHeight = 32;
            int iValueBoxX = iTextX + 200;
            int iValueBoxY = iTextY - 4;
            Color cBoxColor = (m_iGotoSelection == 0) ? Color.Yellow : Color.White;
            spriteBatch.Draw(m_Game.m_Transparent, new Rectangle(iValueBoxX, iValueBoxY, iValueBoxWidth, iValueBoxHeight), cBoxColor);

            // Draw the value
            string szValue = m_iPuzzleNumber.ToString();
            int iValueX = iValueBoxX + (iValueBoxWidth / 2) - ((int)m_Game.m_DialogFont.MeasureString(szValue).X / 2);
            int iValueY = iTextY;
            spriteBatch.DrawString(m_Game.m_DialogFont, szValue, new Vector2(iValueX + 2, iValueY + 2), Color.Black);
            spriteBatch.DrawString(m_Game.m_DialogFont, szValue, new Vector2(iValueX, iValueY), Color.White);

            // Draw the left arrow
            int iLeftArrowX = iValueBoxX - 24;
            int iArrowWidth = 32;
            spriteBatch.Draw(m_Game.m_GoldArrowLeft, new Vector2(iLeftArrowX, iValueBoxY), Color.White);

            // Draw the right arrow
            int iRightArrowX = iValueBoxX + iValueBoxWidth - 8;
            spriteBatch.Draw(m_Game.m_GoldArrowRight, new Vector2(iRightArrowX, iValueBoxY), Color.White);

            // Update boxes
            if (m_rGotoLeft.IsEmpty)
                m_rGotoLeft = new Rectangle(iLeftArrowX, m_Game.m_iScreenTop + iValueBoxY, iArrowWidth, iValueBoxHeight);
            if (m_rGotoRight.IsEmpty)
                m_rGotoRight = new Rectangle(iRightArrowX, m_Game.m_iScreenTop + iValueBoxY, iArrowWidth, iValueBoxHeight);

            int iButtonsY = iDialogY + 70;
            int iButtonXSpace = 20;
            Vector2 vSize = m_Game.m_DialogFont.MeasureString("Cancel");
            m_rConfirmCancel = new Rectangle(iHalfScreenW - ((int)vSize.X + iButtonXSpace), m_Game.m_iScreenTop + iButtonsY, (int)vSize.X, (int)vSize.Y);
            Color cButtonColor = (m_rConfirmCancel.Contains(m_iMouseX, m_iMouseY) || m_iGotoSelection == 1) ? Color.Turquoise : Color.CornflowerBlue;
            DrawString(spriteBatch, "Cancel", m_rConfirmCancel.X, iButtonsY, cButtonColor);

            vSize = m_Game.m_DialogFont.MeasureString("Go");
            m_rConfirmAccept = new Rectangle(iHalfScreenW + iButtonXSpace, m_Game.m_iScreenTop + iButtonsY, (int)vSize.X, (int)vSize.Y);
            cButtonColor = (m_rConfirmAccept.Contains(m_iMouseX, m_iMouseY) || m_iGotoSelection == 2) ? Color.Turquoise : Color.CornflowerBlue;
            DrawString(spriteBatch, "Go", m_rConfirmAccept.X, iButtonsY, cButtonColor);
        }

        private void DrawConfirmDialog(SpriteBatch spriteBatch, int iScreenW, int iScreenH)
        {
            int iHalfScreenW = iScreenW / 2;
            int iHalfScreenH = iScreenH / 2;
            int iWidth1 = (int)m_Game.m_DialogFont.MeasureString(m_szConfirmText1).X;
            int iWidth2 = (int)m_Game.m_DialogFont.MeasureString(m_szConfirmText2).X;
            int iDialogWidth = Math.Max(iWidth1, iWidth2) + 40;
            int iDialogHeight = 100;
            int iDialogX = iHalfScreenW - (iDialogWidth / 2);
            int iDialogY = iHalfScreenH - (iDialogHeight / 2);

            Rectangle r = new Rectangle(iDialogX, iDialogY, iDialogWidth, iDialogHeight);
            spriteBatch.Draw(m_Game.m_Transparent, r, Color.SteelBlue);
            spriteBatch.Draw(m_Game.m_Transparent, r, Color.SteelBlue);
            spriteBatch.Draw(m_Game.m_Transparent, r, Color.SteelBlue);
            spriteBatch.Draw(m_Game.m_Transparent, r, Color.SteelBlue);

            DrawString(spriteBatch, m_szConfirmText1, iHalfScreenW - (iWidth1 / 2), iDialogY + 20, Color.White);
            DrawString(spriteBatch, m_szConfirmText2, iHalfScreenW - (iWidth1 / 2), iDialogY + 40, Color.White);

            int iButtonsY = iDialogY + 70;
            int iButtonXSpace = 20;
            Vector2 vSize = m_Game.m_DialogFont.MeasureString(m_szConfirmCancelText);
            m_rConfirmCancel = new Rectangle(iHalfScreenW - ((int)vSize.X + iButtonXSpace), m_Game.m_iScreenTop + iButtonsY, (int)vSize.X, (int)vSize.Y);
            Color cButtonColor = (m_rConfirmCancel.Contains(m_iMouseX, m_iMouseY) || m_iConfirmSelection == 0) ? Color.Turquoise : Color.CornflowerBlue;
            DrawString(spriteBatch, m_szConfirmCancelText, m_rConfirmCancel.X, iButtonsY, cButtonColor);

            vSize = m_Game.m_DialogFont.MeasureString(m_szConfirmAcceptText);
            m_rConfirmAccept = new Rectangle(iHalfScreenW + iButtonXSpace, m_Game.m_iScreenTop + iButtonsY, (int)vSize.X, (int)vSize.Y);
            cButtonColor = (m_rConfirmAccept.Contains(m_iMouseX, m_iMouseY) || m_iConfirmSelection == 1) ? Color.Turquoise : Color.CornflowerBlue;
            DrawString(spriteBatch, m_szConfirmAcceptText, m_rConfirmAccept.X, iButtonsY, cButtonColor);
        }

        private void DrawControls(SpriteBatch spriteBatch)
        {
            int iX = 300;
            int iY = 20;

            // Draw Background
            spriteBatch.Draw(m_Game.m_HelpBackground, new Rectangle(iX, 0, 980, 720), Color.White);

#if !XBOX
            iY += DrawMouseControls(spriteBatch, iY);
#endif

            if (m_Game.m_Input.IsControllerConnected())
            {
                DrawControllerControls(spriteBatch, iY);
            }            
        }

        private int DrawMouseControls(SpriteBatch spriteBatch, int iY)
        {
            int iFuncX = -510;
            int iKeyX = 520;
            int iKeyYOffset = 3;
            int iYSpacing = 25;
            int iMouseX = 800;
            int iMouseY = iY + 20;

            spriteBatch.Draw(m_Game.m_MouseImage, new Rectangle(iMouseX, iMouseY, 256, 256), Color.White);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Scroll Up/Down", iMouseX + 70, iMouseY - 20, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Eliminate Icon", iMouseX + 210, iMouseY + 50, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Set Icon", iMouseX - 15, iMouseY + 50, Color.White, 1);

            DrawString(spriteBatch, "Pause:", iFuncX, iY, Color.Goldenrod);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Esc", iKeyX, iY + iKeyYOffset, Color.White, 1);
            iY += iYSpacing;

            DrawString(spriteBatch, "Save Game:", iFuncX, iY, Color.Goldenrod);
            DrawString(spriteBatch, m_Game.m_HelpFont, "S", iKeyX, iY + iKeyYOffset, Color.White, 1);
            iY += iYSpacing;

            DrawString(spriteBatch, "Hint:", iFuncX, iY, Color.Goldenrod);
            DrawString(spriteBatch, m_Game.m_HelpFont, "H", iKeyX, iY + iKeyYOffset, Color.White, 1);
            iY += iYSpacing;

            DrawString(spriteBatch, "Unhide Clues:", iFuncX, iY, Color.Goldenrod);
            DrawString(spriteBatch, m_Game.m_HelpFont, "U", iKeyX, iY + iKeyYOffset, Color.White, 1);
            iY += iYSpacing;

            DrawString(spriteBatch, "Undo:", iFuncX, iY, Color.Goldenrod);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Ctrl + Z", iKeyX, iY + iKeyYOffset, Color.White, 1);
            iY += iYSpacing;

            DrawString(spriteBatch, "Redo:", iFuncX, iY, Color.Goldenrod);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Ctrl + Shift + Z", iKeyX, iY + iKeyYOffset, Color.White, 1);
            iY += iYSpacing;

            DrawString(spriteBatch, "Move Up:", iFuncX, iY, Color.Goldenrod);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Up Arrow", iKeyX, iY + iKeyYOffset, Color.White, 1);
            iY += iYSpacing;

            DrawString(spriteBatch, "Move Down:", iFuncX, iY, Color.Goldenrod);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Down Arrow", iKeyX, iY + iKeyYOffset, Color.White, 1);
            iY += iYSpacing;

            DrawString(spriteBatch, "Move Left:", iFuncX, iY, Color.Goldenrod);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Left Arrow", iKeyX, iY + iKeyYOffset, Color.White, 1);
            iY += iYSpacing;

            DrawString(spriteBatch, "Move Right:", iFuncX, iY, Color.Goldenrod);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Right Arrow", iKeyX, iY + iKeyYOffset, Color.White, 1);
            iY += iYSpacing;

            DrawString(spriteBatch, "Select Item:", iFuncX, iY, Color.Goldenrod);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Enter/Space", iKeyX, iY + iKeyYOffset, Color.White, 1);
            iY += iYSpacing;
            iY += iYSpacing;

            return iY;
        }

        private void DrawControllerControls(SpriteBatch spriteBatch, int iY)
        {
            int iX = 500;
            int iTop = iY + 50;

#if XBOX
            iTop += 200;
#endif

            spriteBatch.Draw(m_Game.m_ControllerImage, new Rectangle(iX, iTop, 512, 256), Color.White);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Pause",             iX + 265, iTop + 10, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Unhide Clues",      iX + 160, iTop - 20, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Vertical Clues",    iX - 25, iTop - 5, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Undo",              iX + 10, iTop + 55, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Navigate",          iX - 20, iTop + 120, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Navigate",          iX + 100, iTop + 250, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Navigate Fast",     iX + 280, iTop + 250, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Set Icon",          iX + 450, iTop + 140, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Eliminate Icon",    iX + 450, iTop + 110, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Restore Icon",      iX + 450, iTop + 82, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Hint",              iX + 450, iTop + 60, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Redo",              iX + 470, iTop + 30, Color.White, 1);
            DrawString(spriteBatch, m_Game.m_HelpFont, "Horizontal Clues",  iX + 445, iTop + 5, Color.White, 1);
        }

        private void DrawHelp(SpriteBatch spriteBatch)
        {
            int iX = 300;
            int iY = 20;
            int iIconSize = 30;
            int iIconSizeSmall = 16;
            int iClueMargin = 5;
            int iClueIconGap = 2;
            int iTextIconGap = 2;

            // Draw Background
            spriteBatch.Draw(m_Game.m_HelpBackground, new Rectangle(iX, 0, 980, 720), Color.White);

            // Draw Scrollbar
            {
                int iTop = 4;
                int iBottom = 716;
                iX -= 18;

                // Draw bar
                spriteBatch.Draw(m_Game.m_ScrollBar, new Rectangle(iX, iTop, 16, iBottom - iTop), Color.White);

                // Draw Cursor
                float fPercent = (float)m_iScrollOffset / (float)m_iScrollMaximum;
                int iCursorY = iTop + 8 + (int)((float)((iBottom - iTop) - 32) * fPercent);
                spriteBatch.Draw(m_Game.m_ScrollCursor, new Rectangle(iX, iCursorY, 16, 16), Color.White);

                // Draw top arrow
                m_rScrollBarUp = new Rectangle(iX, iTop, 16, 16);
                spriteBatch.Draw(m_Game.m_ScrollArrow, m_rScrollBarUp, Color.White);

                // Draw bottom arrow
                m_rScrollBarDown = new Rectangle(iX, iBottom - 16, 16, 16);
                spriteBatch.Draw(m_Game.m_ScrollArrow, m_rScrollBarDown, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipVertically, 0);

                iX += 45;
                iY -= m_iScrollOffset;
            }


            // Draw Objective
            DrawString(spriteBatch, m_Game.m_MenuFont, "Objective", iX, iY, Color.Goldenrod, 2);
            iY += 40;

            DrawString(spriteBatch, m_Game.m_HelpFont, "Use the clues to figure out where each icon belongs in the puzzle", iX + 20, iY, Color.White, 1);
            iY += 60;

            // Draw some Vertical Clue help
            DrawString(spriteBatch, m_Game.m_MenuFont, "Vertical Clues", iX, iY, Color.Goldenrod, 2);
            iY += 40;

            DrawString(spriteBatch, m_Game.m_HelpFont, "Vertical clues give information about one column of the puzzle", iX + 20, iY, Color.White, 1);
            iY += 50;


            //eVerticalType.Two,        

            // Draw a VerticalType Two clue here
            Texture2D tIcon1 = m_Game.m_aFlowers[4];
            Texture2D tIcon2 = m_Game.m_aPuppies[6];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);

            // Icon1 is in the same column as Icon2. 
            int iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is in the same column as the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            //  If you already know where Icon1 is, you know that Icon2 is in the same column and visa versa.  
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you know that the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            DrawString(spriteBatch, m_Game.m_HelpFont, " is in the same column and visa versa.", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know any other icons in the same row as Icon1, you can eliminate Icon2 in that column.    
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know any other icons in the same row as the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, ", you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            DrawString(spriteBatch, m_Game.m_HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know that Icon1 is not in a column, you know that Icon2 is also not in that column so it can be elimnated.
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know that the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is not in a column, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            DrawString(spriteBatch, m_Game.m_HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 50;


            //eVerticalType.TwoNot,
            tIcon1 = m_Game.m_aCars[2];
            tIcon2 = m_Game.m_aHubble[7];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_NotOverlay, new Rectangle(iX + iClueMargin, iY + (iIconSize / 2), iIconSize, iIconSize), Color.White);

            // Icon1 is not in the same colum as Icon2.
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is not the same column as the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            //  If you already know where Icon1 is, you can eliminate Icon2 from that column and visa versa.
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            DrawString(spriteBatch, m_Game.m_HelpFont, " from that column and visa versa.", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 90;

            //eVerticalType.Three
            tIcon1 = m_Game.m_aPrincesses[0];
            tIcon2 = m_Game.m_aCats[3];
            Texture2D tIcon3 = m_Game.m_aSimpsons[6];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);

            // Icon1, Icon2, and Icon3 are all in the same column.
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, ", the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, ", and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " are all in the same column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where one of the icons is, you know the other two are in the same column
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where one icon is, you know the other two are in that same column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know any icons in the same row as one of these icons, you can elminate the other two icons from that column.
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know any icons in the same row as one of these icons, you can elminate the other two icons from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know that any one of the icons is not in a column, you can eliminate the other two icons from that column as well.
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know that any one of the icons is not in a column, you can eliminate the other two icons from that column as well", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 50;


            //eVerticalType.ThreeTopNot,                       
            tIcon1 = m_Game.m_aSuperheros[4];
            tIcon2 = m_Game.m_aFlowers[5];
            tIcon3 = m_Game.m_aHubble[2];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_NotOverlay, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);

            // IconA, and IconB are in the same column and IconC is not in that same column
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, ", and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " are in the same column, and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is not in that same column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //   If you know where IconA is, you know IconB is in that same column and you can eliminate IconC from that column
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you know the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is in that same column and you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //   If you know where IconC is you can eliminate both IconA and IconB from that column
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate both the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from that column ", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 70;

            //eVerticalType.ThreeMidNot,
            tIcon1 = m_Game.m_aHubble[7];
            tIcon2 = m_Game.m_aCats[1];
            tIcon3 = m_Game.m_aCars[5];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_NotOverlay, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);

            // IconA, and IconB are in the same column and IconC is not in that same column
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, ", and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " are in the same column, and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is not in that same column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //   If you know where IconA is, you know IconB is in that same column and you can eliminate IconC from that column
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you know the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is in that same column and you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //   If you know where IconC is you can eliminate both IconA and IconB from that column
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate both the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from that column ", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 70;


            //eVerticalType.ThreeBotNot,
            tIcon1 = m_Game.m_aSuperheros[7];
            tIcon2 = m_Game.m_aPrincesses[1];
            tIcon3 = m_Game.m_aFlowers[0];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_NotOverlay, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);

            // IconA, and IconB are in the same column and IconC is not in that same column
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, ", and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " are in the same column, and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is not in that same column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //   If you know where IconA is, you know IconB is in that same column and you can eliminate IconC from that column
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you know the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is in that same column and you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //   If you know where IconC is you can eliminate both IconA and IconB from that column
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate both the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from that column ", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 70;


            //eVerticalType.EitherOr
            tIcon1 = m_Game.m_aCars[6];
            tIcon2 = m_Game.m_aPuppies[3];
            tIcon3 = m_Game.m_aCats[5];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_EitherOrOverlay, new Rectangle(iX + iClueMargin, iY + iIconSize + iClueIconGap + (iIconSize / 2), iIconSize, iIconSize), Color.White);

            // Icon1 is either in the column with Icon2 or the column Icon3 but not both
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is either in the column with the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " or in the column with the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " but not both", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon2 is you can eliminate Icon3 from that column and visa versa since these cant be in the same column.
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from that column and visa versa", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon2 is and you know where Icon3 is, you can eliminate Icon1 from all other columns since it has to be in one of these two columns.
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, and you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can elminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from all other columns", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If a column doesnt have either Icon2 or Icon3 then you can eliminate Icon1 from this column
            iXPos = iX + iClueMargin + iIconSize + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If a column doesnt have either the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " or the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " then you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 70;


            // Draw some Horizontal Clue help
            DrawString(spriteBatch, m_Game.m_MenuFont, "Horizontal Clues", iX, iY, Color.Goldenrod, 2);
            iY += 40;

            DrawString(spriteBatch, m_Game.m_HelpFont, "Horizontal Clues give information about the rows of the puzzle", iX + 20, iY, Color.White, 1);
            iY += 50;



            //eHorizontalType.NextTo,
            tIcon1 = m_Game.m_aPrincesses[4];
            tIcon2 = m_Game.m_aCats[6];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            iY += iIconSize + iClueIconGap;
            
            // Icon1 is next to Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is in a column next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);            
            iY += 20;

            //  If you know where Icon1 is, you can eliminate Icon2 from all columns that arent next to the column with Icon1.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from all columns that aren't next to the column with the", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            //  If a column does not have Icon1 to the right or to the left, then you can eliminate Icon2 from this column.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If a column does not have the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " to the right or to the left, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from that column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 80;


            //eHorizontalType.NotNextTo,
            tIcon1 = m_Game.m_aHubble[1];
            tIcon2 = m_Game.m_aSimpsons[6];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_NotOverlay, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            iY += iIconSize + iClueIconGap;

            // Icon1 is not next to Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is not in a column next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            //  If you know where Icon1 is, you can eliminate Icon2 from the column to the left and the column to the right and visa versa.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from the columns to the left and right", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 100;


            //eHorizontalType.LeftOf
            tIcon1 = m_Game.m_aFlowers[2];
            tIcon2 = m_Game.m_aSuperheros[5];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_LeftOfIcon, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            iY += iIconSize + iClueIconGap;

            // Icon1 is to the left of Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is somwhere to the left of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            //  If you know where Icon1 is, you can eliminate Icon2 from that column and all columns to the left.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from that column and all columns to the left", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon2 is, you can eliminate Icon1 from that column and all columns to the right.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from that column and all columns to the right", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  You can eliminate Icon2 from any columns on the left side of the puzzle that would result in Icon1 being in the same column or any column to the right.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "You can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from any columns that would result in the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " being in the same column or any column to the right", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;                       

            //  You can eliminate Icon1 from any columns on the right side of the puzzle that would result in Icon2 being in the same column or any column to the left
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "You can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from any columns that would result in the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " being in the same column or any column to the left", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 40; 


            //eHorizontalType.NotLeftOf,
            tIcon1 = m_Game.m_aCats[7];
            tIcon2 = m_Game.m_aPrincesses[2];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_LeftOfIcon, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_NotOverlay, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            iY += iIconSize + iClueIconGap;

            // Icon1 is not to the left of Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is not to the left of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            //  Icon1 and Icon2 can be in the same column as Icon1 would not be left of Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " can be in the same column", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon1 is, you can eliminate Icon2 from all columns to the right
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from all columns to the right", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon2 is, you can eliminate Icon1 from all columns to the left
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from all columns to the left", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 60;


            //eHorizontalType.Span
            tIcon1 = m_Game.m_aFlowers[3];
            tIcon2 = m_Game.m_aHubble[6];
            tIcon3 = m_Game.m_aPuppies[0];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_SpanOverlay, new Rectangle(iX + iClueMargin, iY, (iIconSize * 3) + (iClueIconGap * 2) , iIconSize), Color.White);
            iY += iIconSize + iClueIconGap;

            // Icon2 has Icon1 on one side and Icon3 on the other side
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " has the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " on one side and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " on the other side", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon1 is, Icon2 has to be next to it on either side and Icon3 has to be next to Icon2 in the same direction.  You can eliminate Icon2 from all columns that arent next to Icon1 and Icon3 from all columns that arent 2 columns away from Icon1
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " has to be on either side of it and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " has to be next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " in the same direction", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon3 is, Icon2 has to be next to it on either side and Icon1 has to be next to Icon2 in the same direction.  You can eliminate Icon2 from all columns that arent next to Icon3 and Icon1 from all columns that arent 2 columns away from Icon3
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " has to be on either side of it and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " has to be next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " in the same direction", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon2 is, you can eliminate Icon1 and Icon3 from all columns that arent next to Icon2.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from all columns that arent next to ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            //  Icon2 is in the middle of Icon1 and Icon3, thus you can eliminate Icon2 from the left most column and the right most column.
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is in the middle of ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " so you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from the left most and right most columns", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 40;


            //eHorizontalType.SpanNotLeft
            tIcon1 = m_Game.m_aSimpsons[1];
            tIcon2 = m_Game.m_aPrincesses[0];
            tIcon3 = m_Game.m_aCats[7];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_SpanOverlay, new Rectangle(iX + iClueMargin, iY, (iIconSize * 3) + (iClueIconGap * 2), iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_NotOverlay, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            iY += iIconSize + iClueIconGap;

            // Icon2 has Icon3 on one side and not Icon1 on the other side
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " has the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " on one side and not the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " on the other side", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon3 is, you can eliminate Icon2 from all columns that arent next to Icon3
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from all columns that arent next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            //  If you know where Icon2 is, you can eliminate Icon3 from all columns that arent next to Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from all columns that aren't next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            //  If you know where Icon2 is and you know where Icon3 is, you can eliminate Icon1 from the other side of Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is and you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from the other side of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            //  Icon2 is in the middle of Icon3 and not Icon1, thus you can eliminate Icon2 from the left most column and the right most column.
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is in the middle of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " and not the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " so you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from the left most and right most columns", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 40;

            //eHorizontalType.SpanNotMid,
            tIcon1 = m_Game.m_aCars[4];
            tIcon2 = m_Game.m_aHubble[7];
            tIcon3 = m_Game.m_aPuppies[3];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_SpanOverlay, new Rectangle(iX + iClueMargin, iY, (iIconSize * 3) + (iClueIconGap * 2), iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_NotOverlay, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            iY += iIconSize + iClueIconGap;

            // Icon1 and Icon3 have an icon between them that is not Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " and the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " have a column between them that does not have the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " in it", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            // If you know where Icon1 is, you can eliminate Icon3 from all columns that are not 2 columns left or right of Icon1.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from all columns that aren't 2 columns left or right of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            // If you know where Icon3 is, you can eliminate Icon1 from all columns that are not 2 columns left or right of Icon3.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from all columns that aren't 2 columns left or right of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            // If you know where Icon1 is and you know where Icon3 is, you can eliminate Icon2 from the middle column.
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is and you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from the column in between", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 60;
            
            //eHorizontalType.SpanNotRight
            tIcon1 = m_Game.m_aCats[0];
            tIcon2 = m_Game.m_aFlowers[5];
            tIcon3 = m_Game.m_aSuperheros[0];
            spriteBatch.Draw(tIcon1, new Rectangle(iX + iClueMargin, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon2, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(tIcon3, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_SpanOverlay, new Rectangle(iX + iClueMargin, iY, (iIconSize * 3) + (iClueIconGap * 2), iIconSize), Color.White);
            spriteBatch.Draw(m_Game.m_NotOverlay, new Rectangle(iX + iClueMargin + iIconSize + iClueIconGap + iIconSize + iClueIconGap, iY, iIconSize, iIconSize), Color.White);
            iY += iIconSize + iClueIconGap;

            // Icon2 has Icon1 on one side and not Icon3 on the other side
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " has the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " on one side and not the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " on the other side", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 20;

            //  If you know where Icon1 is, you can eliminate Icon2 from all columns that arent next to Icon1
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from all columns that aren't next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            //  If you know where Icon2 is, you can eliminate Icon1 from all columns that arent next to Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from all columns that aren't next to the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            //  If you know where Icon2 is and you know where Icon1 is, you can eliminate Icon3 from the other side of Icon2
            iXPos = iX + iClueMargin + iClueMargin;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, "If you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is and you know where the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is, you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from the other side of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iY += 20;

            //  Icon2 is in the middle of Icon1 and not Icon3, thus you can eliminate Icon2 from the left most column and the right most column.
            iXPos = iX + iClueMargin + iClueMargin;
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " is in the middle of the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon1, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " and not the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon3, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " so you can eliminate the ", iXPos, iY - iTextIconGap, Color.White, 1);
            spriteBatch.Draw(tIcon2, new Rectangle(iXPos, iY, iIconSizeSmall, iIconSizeSmall), Color.White);
            iXPos += iIconSizeSmall;
            iXPos += DrawString(spriteBatch, m_Game.m_HelpFont, " from the left most and right most columns", iXPos, iY - iTextIconGap, Color.White, 1);
            iY += 40;
        }
    }
}
