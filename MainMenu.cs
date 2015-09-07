using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class MainMenu
    {
        Happiness m_Game;
        public GameTime m_GameTime;
        int m_iTextX = 0;
        int m_iTextY = 0;
        bool m_bNewGameDialog;
        int m_iNewGameSelection;

        int m_iMouseX = 0;
        int m_iMouseY = 0;
        double m_dfScrollSpeed = 1.0;
        double m_dfScrollTime = 0;
        Rectangle[] m_aNewGameBoxes;
        Rectangle m_rNewGameCancel;
        Rectangle m_rNewGameAccept;
        SaveGame m_SaveGame = null;

        public int m_iSelection;
        public int m_iPuzzleNumber;
        public int m_iSize;
        public int m_iDifficulty;

        public MainMenu(Happiness game)
        {
            m_Game = game;
            m_aNewGameBoxes = new Rectangle[6];

            m_iSelection = 0;
            m_bNewGameDialog = false;

            m_iPuzzleNumber = 0;            // 0 - 67,108,863
            m_iSize = 6;                    // 3 - 9
            m_iDifficulty = 0;              // 0 - 3
        }

        public void SetSaveGame(SaveGame save)
        {
            if (m_SaveGame != save)
                m_SaveGame = save;
        }

        public void NavigateDown()
        {            
            if (m_bNewGameDialog)
            {
                m_iNewGameSelection++;
                if (m_iNewGameSelection > 4)
                    m_iNewGameSelection = 0;
            }
            else
            {
                m_iSelection++;
                if (m_iSelection >= 4)
                    m_iSelection = 0;
            }

            m_Game.m_SoundManager.PlayMenuNavigate();
        }

        public void NavigateUp()
        {
            if (m_bNewGameDialog)
            {
                m_iNewGameSelection--;
                if (m_iNewGameSelection < 0)
                    m_iNewGameSelection = 4;
            }
            else
            {
                m_iSelection--;
                if (m_iSelection < 0)
                    m_iSelection = 3;
            }

            m_Game.m_SoundManager.PlayMenuNavigate();
        }

        public void NavigateLeft()
        {
            if (m_bNewGameDialog)
            {
                switch (m_iNewGameSelection)
                {
                    case 0:
                    case 1:
                    case 2:
                        ClickNewGameBox(m_iNewGameSelection * 2);
                        break;
                    case 3:
                        m_iNewGameSelection = 4;
                        break;
                    case 4:
                        m_iNewGameSelection = 3;
                        break;
                }
                m_Game.m_SoundManager.PlayMenuNavigate();
            }
        }

        public void NavigateRight()
        {
            if (m_bNewGameDialog)
            {
                switch (m_iNewGameSelection)
                {
                    case 0:
                    case 1:
                    case 2:
                        ClickNewGameBox((m_iNewGameSelection * 2) + 1);
                        break;
                    case 3:
                        m_iNewGameSelection = 4;
                        break;
                    case 4:
                        m_iNewGameSelection = 3;
                        break;
                }
                m_Game.m_SoundManager.PlayMenuNavigate();
            }
        }

        public bool CommitSelection()
        {
            // return false if this menu should exit
            if (m_bNewGameDialog)
            {
                switch (m_iNewGameSelection)
                {
                    case 0:
                    case 1:
                    case 2:
                        ClickNewGameBox((m_iNewGameSelection * 2) + 1);
                        m_Game.m_SoundManager.PlayMenuNavigate();
                        break;
                    case 3:
                        // Exit the dialog and the main menu
                        m_bNewGameDialog = false;
                        m_Game.m_SoundManager.PlayMenuAccept();
                        return false;
                    case 4:
                        // Just cancel the dialog
                        m_bNewGameDialog = false;
                        m_Game.m_SoundManager.PlayMenuCancel();
                        break;
                }
            }
            else
            {
                switch (m_iSelection)
                {
                    case 0:         // New Game
                        if (!m_bNewGameDialog)
                        {
                            m_bNewGameDialog = true;
                            m_iNewGameSelection = 3;
                            m_Game.m_SoundManager.PlayMenuAccept();
                            return true;
                        }
                        break;
                    case 1:         // Load Game
                        if (!m_bNewGameDialog && m_SaveGame != null && m_SaveGame.m_bIsValid)
                        {
                            m_Game.m_SoundManager.PlayMenuAccept();
                            return false;
                        }
                        break;
                    case 2:         // Options
                        m_Game.m_Options.Init();
                        m_Game.m_bOptionsDialog = true;
                        m_Game.m_SoundManager.PlayMenuAccept();
                        break;
                    case 3:         // Exit
                        return false;
                }
            }
            return true;
        }

        public bool CancelSelection()
        {
            if (m_bNewGameDialog)
            {
                m_bNewGameDialog = false;
                m_Game.m_SoundManager.PlayMenuCancel();
            }
            return true;
        }

        public void UpdateMouse(int iX, int iY, bool bLeftButtonDown)
        {
            m_iMouseX = iX;
            m_iMouseY = iY + m_Game.m_iScreenTop;
            if (m_bNewGameDialog)
            {
                if (m_rNewGameAccept.Contains(iX, iY))
                {
                    if (m_iNewGameSelection != 3)
                    {
                        m_iNewGameSelection = 3;
                        m_Game.m_SoundManager.PlayMenuNavigate();
                    }
                }
                else if (m_rNewGameCancel.Contains(iX, iY))
                {
                    if (m_iNewGameSelection != 4)
                    {
                        m_iNewGameSelection = 4;
                        m_Game.m_SoundManager.PlayMenuNavigate();
                    }
                }
                else
                {
                    if (bLeftButtonDown)
                    {
                        if (m_dfScrollTime <= 0)
                            m_dfScrollTime = m_GameTime.TotalGameTime.TotalSeconds;
                        if (m_GameTime.TotalGameTime.TotalSeconds - m_dfScrollTime > 0.8)
                        {
                            const double dfSpeed = 0.04;
                            if (m_aNewGameBoxes[0].Contains(iX, iY) || (m_iNewGameSelection == 0))
                            {
                                m_iPuzzleNumber -= (int)m_dfScrollSpeed;
                                if (m_iPuzzleNumber < 0)
                                    m_iPuzzleNumber = 67108864 - m_iPuzzleNumber;
                                m_dfScrollSpeed += dfSpeed;
                                m_Game.m_SoundManager.PlayMenuNavigate();
                            }
                            else if (m_aNewGameBoxes[1].Contains(iX, iY) || (m_iNewGameSelection == 0))
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
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    Rectangle r = new Rectangle(m_iTextX - 20, m_Game.m_iScreenTop + m_iTextY + (i * 40), 352, 40);
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

        // Return false if this menu should close
        public bool HandleClick(int iX, int iY)
        {
            if (m_bNewGameDialog)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (m_aNewGameBoxes[i].Contains(iX, iY))
                    {
                        ClickNewGameBox(i);
                        m_Game.m_SoundManager.PlayMenuNavigate();
                        return true;
                    }
                }

                if (m_rNewGameAccept.Contains(iX, iY))
                {
                    // Exit the dialog and the main menu
                    m_bNewGameDialog = false;
                    m_Game.m_SoundManager.PlayMenuAccept();
                    return false;
                }
                else if (m_rNewGameCancel.Contains(iX, iY))
                {
                    m_bNewGameDialog = false;
                    m_Game.m_SoundManager.PlayMenuCancel();
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    Rectangle r = new Rectangle(m_iTextX - 20, m_Game.m_iScreenTop + m_iTextY + (i * 40), 352, 40);
                    if (r.Contains(iX, iY))
                    {
                        m_iSelection = i;
                        return CommitSelection();
                    }
                }
            }

            return true;
        }

        private void ClickNewGameBox(int iBoxIndex)
        {
            int iSetting = iBoxIndex / 2;
            int iChange = ((iBoxIndex % 2) * 2) - 1;
            switch (iSetting)
            {
                case 0:     // Puzzle Number
                    m_iPuzzleNumber += iChange;
                    if (m_iPuzzleNumber < 0)
                        m_iPuzzleNumber = 67108863;
                    if (m_iPuzzleNumber > 67108863)
                        m_iPuzzleNumber = 0;
                    break;
                case 1:     // Puzzle Size
                    m_iSize += iChange;
                    if (m_iSize > 8)
                        m_iSize = 3;
                    if (m_iSize < 3)
                        m_iSize = 8;
                    break;
                case 2:     // Difficulty
                    m_iDifficulty += iChange;
                    if (m_iDifficulty < 0)
                        m_iDifficulty = 2;
                    if (m_iDifficulty > 2)
                        m_iDifficulty = 0;
                    break;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int iWidth = m_Game.m_iScreenWidth;
            int iHeight = m_Game.m_iScreenHeight;
            spriteBatch.Draw(m_Game.m_Background, new Rectangle(0, 0, iWidth, iHeight), Color.White);

            int iHalfWidth = iWidth / 2;
            int iHalfSizeW = m_Game.m_Logo.Width / 2;
            spriteBatch.Draw(m_Game.m_Logo, new Rectangle(40, 0, m_Game.m_Logo.Width, m_Game.m_Logo.Height), Color.White);
            
            m_iTextX = iWidth - 340;
            m_iTextY = (iHeight - 200);

            int iTextY = m_iTextY;
            spriteBatch.Draw(m_Game.m_Transparent, new Rectangle(m_iTextX - 20, m_iTextY - 10, 352, 190), Color.SteelBlue);

            spriteBatch.DrawString(m_Game.m_MenuFont, "New Game", new Vector2(m_iTextX + 2, m_iTextY + 2), Color.Black);
            spriteBatch.DrawString(m_Game.m_MenuFont, "New Game", new Vector2(m_iTextX, m_iTextY), Color.Goldenrod);

            iTextY += 40;
            Color cLoadColor = (m_SaveGame != null && m_SaveGame.m_bIsValid) ? Color.Goldenrod : Color.Gray;
            spriteBatch.DrawString(m_Game.m_MenuFont, "Load Game", new Vector2(m_iTextX + 2, iTextY + 2), Color.Black);
            spriteBatch.DrawString(m_Game.m_MenuFont, "Load Game", new Vector2(m_iTextX, iTextY), cLoadColor);

            iTextY += 40;
            spriteBatch.DrawString(m_Game.m_MenuFont, "Options", new Vector2(m_iTextX + 2, iTextY + 2), Color.Black);
            spriteBatch.DrawString(m_Game.m_MenuFont, "Options", new Vector2(m_iTextX, iTextY), Color.Goldenrod);

            iTextY += 40;
            spriteBatch.DrawString(m_Game.m_MenuFont, "Exit", new Vector2(m_iTextX + 2, iTextY + 2), Color.Black);
            spriteBatch.DrawString(m_Game.m_MenuFont, "Exit", new Vector2(m_iTextX, iTextY), Color.Goldenrod);

            int iArrowY = (iHeight - 210) + (m_iSelection * 40);
            spriteBatch.Draw(m_Game.m_BlueArrow, new Rectangle(m_iTextX - 105, iArrowY, 100, 70), Color.White);

            string szMeCredit = "Game Design, Art, Sound, & Programming by:";
            string szMe = "Ron O'Hara";
            string szJenkeesCredit = "Music by:";
            string szJenkees = "Ronald Jenkees";
            string szJenkeesWeb = "www.ronaldjenkees.com";

            spriteBatch.DrawString(m_Game.m_DialogFont, szMeCredit, new Vector2(57, 642), Color.Black);
            spriteBatch.DrawString(m_Game.m_DialogFont, szMeCredit, new Vector2(55, 640), Color.White);
            spriteBatch.DrawString(m_Game.m_DialogFont, szMe, new Vector2(697, 642), Color.Black);
            spriteBatch.DrawString(m_Game.m_DialogFont, szMe, new Vector2(695, 640), Color.Goldenrod);

            spriteBatch.DrawString(m_Game.m_DialogFont, szJenkeesCredit, new Vector2(57, 672), Color.Black);
            spriteBatch.DrawString(m_Game.m_DialogFont, szJenkeesCredit, new Vector2(55, 670), Color.White);
            spriteBatch.DrawString(m_Game.m_DialogFont, szJenkees, new Vector2(197, 672), Color.Black);
            spriteBatch.DrawString(m_Game.m_DialogFont, szJenkees, new Vector2(195, 670), Color.Goldenrod);
            spriteBatch.DrawString(m_Game.m_DialogFont, szJenkeesWeb, new Vector2(437, 672), Color.Black);
            spriteBatch.DrawString(m_Game.m_DialogFont, szJenkeesWeb, new Vector2(435, 670), Color.Turquoise);


            if (m_bNewGameDialog)
                DrawNewGameDialog(spriteBatch, iWidth, iHeight);
        }

        private void DrawNewGameDialog(SpriteBatch spriteBatch, int iScreenWidth, int iScreenHeight)
        {
            int iDialogWidth = 400;
            int iDialogHeight = 210;
            int iX = (iScreenWidth / 2) - (iDialogWidth / 2);
            int iY = ((iScreenHeight / 2) - (iDialogHeight / 2));

            spriteBatch.Draw(m_Game.m_Transparent, new Rectangle(iX, iY, iDialogWidth, iDialogHeight), Color.SteelBlue);
            spriteBatch.Draw(m_Game.m_Transparent, new Rectangle(iX, iY, iDialogWidth, iDialogHeight), Color.SteelBlue);
            spriteBatch.Draw(m_Game.m_Transparent, new Rectangle(iX, iY, iDialogWidth, iDialogHeight), Color.SteelBlue);
            spriteBatch.Draw(m_Game.m_Transparent, new Rectangle(iX, iY, iDialogWidth, iDialogHeight), Color.SteelBlue);

            iX += 25;
            iY += 30;

            DrawNewGameItem(spriteBatch, "Puzzle #:", m_iPuzzleNumber.ToString(), iX, iY, 0);

            iY += 40;
            DrawNewGameItem(spriteBatch, "Matrix Size:", GetMatrixSizeString(), iX, iY, 1);

            iY += 40;
            DrawNewGameItem(spriteBatch, "Difficulty:", GetDifficultyString(), iX, iY, 2);

            // Draw Cancel
            iX += 50;
            iY += 50;
            if (m_rNewGameCancel.IsEmpty)
            {
                Vector2 vSize = m_Game.m_DialogFont.MeasureString("Cancel");
                m_rNewGameCancel = new Rectangle(iX, m_Game.m_iScreenTop + iY, (int)vSize.X, (int)vSize.Y);
            }
            Color cCancel = (m_rNewGameCancel.Contains(m_iMouseX, m_iMouseY) || m_iNewGameSelection == 4) ? Color.Turquoise : Color.CornflowerBlue;
            spriteBatch.DrawString(m_Game.m_DialogFont, "Cancel", new Vector2(iX + 2, iY + 2), Color.Black);
            spriteBatch.DrawString(m_Game.m_DialogFont, "Cancel", new Vector2(iX, iY), cCancel);

            // Draw Accept
            iX += 150;
            if( m_rNewGameAccept.IsEmpty )
            {
                Vector2 vSize = m_Game.m_DialogFont.MeasureString("Accept");
                m_rNewGameAccept = new Rectangle(iX, m_Game.m_iScreenTop + iY, (int)vSize.X, (int)vSize.Y);
            }
            Color cAccept = (m_rNewGameAccept.Contains(m_iMouseX, m_iMouseY) || m_iNewGameSelection == 3) ? Color.Turquoise : Color.CornflowerBlue;
            spriteBatch.DrawString(m_Game.m_DialogFont, "Accept", new Vector2(iX + 2, iY + 2), Color.Black);
            spriteBatch.DrawString(m_Game.m_DialogFont, "Accept", new Vector2(iX, iY), cAccept);
        }

        private void DrawNewGameItem(SpriteBatch spriteBatch, string szText, string szValue, int iX, int iY, int iIndex)
        {
            spriteBatch.DrawString(m_Game.m_DialogFont, szText, new Vector2(iX + 2, iY + 2), Color.Black);
            spriteBatch.DrawString(m_Game.m_DialogFont, szText, new Vector2(iX, iY), Color.Goldenrod);

            // Draw the value box
            int iValueBoxWidth = 140;
            int iValueBoxHeight = 32;
            int iValueBoxX = iX + 200;
            int iValueBoxY = iY - 4;
            Color cBoxColor = (iIndex == m_iNewGameSelection) ? Color.Yellow : Color.White;
            spriteBatch.Draw(m_Game.m_Transparent, new Rectangle(iValueBoxX, iValueBoxY, iValueBoxWidth, iValueBoxHeight), cBoxColor);

            // Draw the value
            int iValueX = iValueBoxX + (iValueBoxWidth / 2) - ((int)m_Game.m_DialogFont.MeasureString(szValue).X / 2);
            int iValueY = iY;
            spriteBatch.DrawString(m_Game.m_DialogFont, szValue, new Vector2(iValueX + 2, iValueY + 2), Color.Black);
            spriteBatch.DrawString(m_Game.m_DialogFont, szValue, new Vector2(iValueX, iValueY), Color.White);

            // Draw the left arrow
            int iLeftArrowX = iValueBoxX - 24;
            int iArrowWidth = 32;
            spriteBatch.Draw(m_Game.m_GoldArrowLeft, new Vector2(iLeftArrowX, iValueBoxY), Color.White);

            // Draw the right arrow
            int iRightArrowX = iValueBoxX + iValueBoxWidth - 8;
            spriteBatch.Draw(m_Game.m_GoldArrowRight, new Vector2(iRightArrowX, iValueBoxY), Color.White);

            // Update box
            if (m_aNewGameBoxes[iIndex * 2].IsEmpty)
            {
                iValueBoxY += m_Game.m_iScreenTop;
                m_aNewGameBoxes[iIndex * 2] = new Rectangle(iLeftArrowX, iValueBoxY, iArrowWidth, iValueBoxHeight);
                m_aNewGameBoxes[(iIndex * 2) + 1] = new Rectangle(iRightArrowX, iValueBoxY, iArrowWidth, iValueBoxHeight);
            }
        }

        private string GetMatrixSizeString()
        {
            switch (m_iSize)
            {
                case 3:
                    return ("3x3");
                case 4:
                    return ("4x4");
                case 5:
                    return ("5x5");
                case 6:
                    return ("6x6");
                case 7:
                    return ("7x7");
                case 8:
                    return ("8x8");
                default:
                    m_iSize = 6;        // Invalid size
                    return "6x6";
            }
        }

        private string GetDifficultyString()
        {
            switch (m_iDifficulty)
            {
                case 0:
                    return "Casual";
                case 1:
                    return "Smarty";
                case 2:
                    return "Genious";
                default:
                    m_iDifficulty = 0;
                    return "Casual";
            }
        }
    }
}
