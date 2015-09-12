using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class Options
    {
        Happiness m_Game;
        public GameTime m_GameTime;
        int m_iSelection;
        int m_iArrowY;
        string m_szContext1;
        string m_szContext2;
        Rectangle m_rCancel;
        Rectangle m_rSave;
        Rectangle[] m_rCheckBoxes;
        bool[] m_bChecks;
        float m_fSoundVolume;
        float m_fMusicVolume;
        float m_fVolumeStep = 0.1f;
        Rectangle m_rSoundLeft;
        Rectangle m_rSoundRight;
        Rectangle m_rMusicLeft;
        Rectangle m_rMusicRight;
        Rectangle m_rSoundVolume;
        Rectangle m_rMusicVolume;

        public Options(Happiness game)
        {
            m_Game = game;
            m_rCheckBoxes = new Rectangle[5];
            m_bChecks = new bool[5];
        }

        public void Init()
        {
            m_iSelection = 0;
            SetContextString();

            /*
            m_bChecks[0] = m_Game.m_bAutoArangeClues;
            m_bChecks[1] = m_Game.m_bShowClueDescriptions;
            m_bChecks[2] = m_Game.m_bShowClock;
            m_bChecks[3] = m_Game.m_bShowPuzzleNumber;
            m_bChecks[4] = m_Game.m_bRandomizeIcons;
            m_fSoundVolume = m_Game.m_SoundManager.m_fSoundVolume;
            m_fMusicVolume = m_Game.m_SoundManager.m_fMusicVolume;
            */
        }

        public void NavigateDown()
        {
            m_iSelection++;
            if (m_iSelection > 8)
                m_iSelection = 0;
            SetContextString();
            m_Game.m_SoundManager.PlayMenuNavigate();
        }

        public void NavigateUp()
        {
            m_iSelection--;
            if (m_iSelection < 0)
                m_iSelection = 8;
            SetContextString();
            m_Game.m_SoundManager.PlayMenuNavigate();
        }

        public void NavigateLeft()
        {
            if (m_iSelection == 5)
            {
                // Sound Volume
                m_Game.m_SoundManager.m_fSoundVolume -= m_fVolumeStep;
                m_Game.m_SoundManager.m_fSoundVolume = Math.Max(m_Game.m_SoundManager.m_fSoundVolume, 0.0f);
                m_Game.m_SoundManager.Update();
                m_Game.m_SoundManager.PlaySliderMove();
            }
            else if( m_iSelection == 6)
            {
                // Music Volume
                m_Game.m_SoundManager.m_fMusicVolume -= m_fVolumeStep;
                m_Game.m_SoundManager.m_fMusicVolume = Math.Max(m_Game.m_SoundManager.m_fMusicVolume, 0.0f);
                m_Game.m_SoundManager.PlaySliderMove();
            }
        }

        public void NavigateRight()
        {
            if (m_iSelection == 5)
            {
                // Sound Volume
                m_Game.m_SoundManager.m_fSoundVolume += m_fVolumeStep;
                m_Game.m_SoundManager.m_fSoundVolume = Math.Min(1.0f, m_Game.m_SoundManager.m_fSoundVolume);
                m_Game.m_SoundManager.Update();
                m_Game.m_SoundManager.PlaySliderMove();
            }
            else if (m_iSelection == 6)
            {
                // Music Volume
                m_Game.m_SoundManager.m_fMusicVolume += m_fVolumeStep;
                m_Game.m_SoundManager.m_fMusicVolume = Math.Min(1.0f, m_Game.m_SoundManager.m_fMusicVolume);
                m_Game.m_SoundManager.PlaySliderMove();
            }
        }

        // Return false if this menu should close
        public bool CommitSelection()
        {
            switch (m_iSelection)
            {
                case 0: // Auto Arange
                case 1: // Show Clue Descriptions
                case 2: // Show Clock
                case 3: // Show Puzzle Number
                case 4: // Randomize Icons
                    m_bChecks[m_iSelection] = !m_bChecks[m_iSelection];
                    m_Game.m_SoundManager.PlayMenuAccept();
                    break;
                case 5: // Sound Volume
                case 6: // Music Volume
                    break;
                case 7: // Cancel
                    m_Game.m_SoundManager.m_fSoundVolume = m_fSoundVolume;
                    m_Game.m_SoundManager.m_fMusicVolume = m_fMusicVolume;
                    m_Game.m_SoundManager.PlayMenuCancel();
                    return false;
                case 8: // Save
                    /*
                    m_Game.m_bAutoArangeClues = m_bChecks[0];
                    m_Game.m_bShowClueDescriptions = m_bChecks[1];
                    m_Game.m_bShowClock = m_bChecks[2];
                    m_Game.m_bShowPuzzleNumber = m_bChecks[3];
                    m_Game.m_bRandomizeIcons = m_bChecks[4];
                    m_Game.m_SoundManager.PlayMenuAccept();
                    */
                    return false;
            }
            return true;
        }

        public bool CancelSelection()
        {
            m_Game.m_SoundManager.PlayMenuCancel();
            return false;
        }

        // Return false if this menu should close
        public bool HandleClick(int iX, int iY)
        {
            bool bRet = true;
            if (m_rCancel.Contains(iX, iY))
            {
                m_iSelection = 7;
                SetContextString();
                bRet = CommitSelection();
            }
            else if (m_rSave.Contains(iX, iY))
            {
                m_iSelection = 8;
                SetContextString();
                bRet = CommitSelection();
            }
            else if (m_rSoundLeft.Contains(iX, iY))
            {
                m_Game.m_SoundManager.m_fSoundVolume = Math.Max(0.0f, m_Game.m_SoundManager.m_fSoundVolume - m_fVolumeStep);
                m_Game.m_SoundManager.Update();
                m_Game.m_SoundManager.PlaySliderMove();
            }
            else if (m_rSoundRight.Contains(iX, iY))
            {
                m_Game.m_SoundManager.m_fSoundVolume = Math.Min(1.0f, m_Game.m_SoundManager.m_fSoundVolume + m_fVolumeStep);
                m_Game.m_SoundManager.Update();
                m_Game.m_SoundManager.PlaySliderMove();
            }
            else if (m_rMusicLeft.Contains(iX, iY))
            {
                m_Game.m_SoundManager.m_fMusicVolume = Math.Max(0.0f, m_Game.m_SoundManager.m_fMusicVolume - m_fVolumeStep);
                m_Game.m_SoundManager.PlaySliderMove();
            }
            else if (m_rMusicRight.Contains(iX, iY))
            {
                m_Game.m_SoundManager.m_fMusicVolume = Math.Min(1.0f, m_Game.m_SoundManager.m_fMusicVolume + m_fVolumeStep);
                m_Game.m_SoundManager.PlaySliderMove();
            }
            else
            {
                for (int i = 0; i < m_rCheckBoxes.Length; i++)
                {
                    if (m_rCheckBoxes[i].Contains(iX, iY))
                    {
                        m_iSelection = i;
                        SetContextString();
                        bRet = CommitSelection();
                        break;
                    }
                }
            }
            return bRet;
        }

        public void UpdateMouse(int iX, int iY, bool bLeftButtonDown)
        {
            for (int i = 0; i < m_rCheckBoxes.Length; i++)
            {
                if (m_rCheckBoxes[i].Contains(iX, iY))
                {
                    if (m_iSelection != i)
                    {
                        m_iSelection = i;
                        SetContextString();
                        m_Game.m_SoundManager.PlayMenuNavigate();
                    }
                    break;
                }
            }
            if (m_rSoundVolume.Contains(iX, iY))
            {
                if (m_iSelection != 5)
                {
                    m_iSelection = 5;
                    SetContextString();
                    m_Game.m_SoundManager.PlayMenuNavigate();
                }
            }
            if (m_rMusicVolume.Contains(iX, iY))
            {
                if (m_iSelection != 6)
                {
                    m_iSelection = 6;
                    SetContextString();
                    m_Game.m_SoundManager.PlayMenuNavigate();
                }
            }
            if (m_rCancel.Contains(iX, iY))
            {
                if (m_iSelection != 7)
                {
                    m_iSelection = 7;
                    SetContextString();
                    m_Game.m_SoundManager.PlayMenuNavigate();
                }
            }
            if (m_rSave.Contains(iX, iY))
            {
                if (m_iSelection != 8)
                {
                    m_iSelection = 8;
                    SetContextString();
                    m_Game.m_SoundManager.PlayMenuNavigate();
                }
            }
        }

        public void SetContextString()
        {
            m_szContext2 = null;
            switch (m_iSelection)
            {
                case 0: // Auto Arange
                    m_szContext1 = "Clues will be arranged to take up empty spaces";
                    m_szContext2 = "when clues are hidden";
                    m_iArrowY = 55;
                    break;
                case 1: // Show Clue Descriptions
                    m_szContext1 = "Clue descriptions will be shown when";
                    m_szContext2 = "a clue is highlighted";
                    m_iArrowY = 105;
                    break;
                case 2: // Show Clock
                    m_szContext1 = "The time display will be shown";
                    m_iArrowY = 155;
                    break;
                case 3: // Show Puzzle Number
                    m_szContext1 = "The puzzle number will be shown";
                    m_iArrowY = 205;
                    break;
                case 4: // Randomize Icons
                    m_szContext1 = "Icons will be distributed randomly";
                    m_szContext2 = "  for each puzzle";
                    m_iArrowY = 255;
                    break;
                case 5: // Sound Volume
                    m_szContext1 = "Volume level of sound effects";
                    m_iArrowY = 305;
                    break;
                case 6: // Music Volume
                    m_szContext1 = "Volume level of music";
                    m_iArrowY = 355;
                    break;
                case 7: // Cancel
                    m_szContext1 = "Exit the options screen without saving changes";
                    m_iArrowY = 435;
                    break;
                case 8: // Save
                    m_szContext1 = "Exit the options screen and save changes";
                    m_iArrowY = 465;
                    break;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int iWidth = m_Game.m_iScreenWidth;
            int iHeight = m_Game.m_iScreenHeight;

            int iX = (iWidth / 2) - 400;
            int iY = 10;
            spriteBatch.Draw(Assets.TransparentBox, new Rectangle(iX, iY, 800, 700), Color.DarkGray);
            spriteBatch.Draw(Assets.TransparentBox, new Rectangle(iX, iY, 800, 700), Color.DarkGray);
            spriteBatch.Draw(Assets.TransparentBox, new Rectangle(iX, iY, 800, 700), Color.DarkGray);

            iX += 80;
            iY += 50;
            m_rCheckBoxes[0] = DrawCheckbox(spriteBatch, iX, iY, "Auto Arange Clues", m_bChecks[0], Color.Goldenrod);

            iY += 50;
            m_rCheckBoxes[1] = DrawCheckbox(spriteBatch, iX, iY, "Show Clue Descriptions", m_bChecks[1], Color.Goldenrod);

            iY += 50;
            m_rCheckBoxes[2] = DrawCheckbox(spriteBatch, iX, iY, "Show Clock", m_bChecks[2], Color.Goldenrod);

            iY += 50;
            m_rCheckBoxes[3] = DrawCheckbox(spriteBatch, iX, iY, "Show Puzzle Number", m_bChecks[3], Color.Goldenrod);

            iY += 50;
            m_rCheckBoxes[4] = DrawCheckbox(spriteBatch, iX, iY, "Randomize Clue Icons", m_bChecks[4], Color.Goldenrod);

            iY += 50;
            int iBarWidth = 300;
            m_rSoundVolume = new Rectangle(iX, iY, iBarWidth, 50);
            Rectangle rBarBounds = new Rectangle(iX + 200, iY + 8, iBarWidth, 16);
            DrawSlider(spriteBatch, iX, iY, rBarBounds, "Sound Volume", m_Game.m_SoundManager.m_fSoundVolume, Color.Goldenrod, out m_rSoundLeft, out m_rSoundRight);

            iY += 50;
            m_rMusicVolume = new Rectangle(iX, iY, iBarWidth, 50);
            rBarBounds = new Rectangle(iX + 200, iY + 8, iBarWidth, 16);
            DrawSlider(spriteBatch, iX, iY, rBarBounds, "Music Volume", m_Game.m_SoundManager.m_fMusicVolume, Color.Goldenrod, out m_rMusicLeft, out m_rMusicRight);

            iY += 80;
            m_rCancel = new Rectangle(iX, iY, 100, 30);
            DrawString(spriteBatch, "Cancel", iX, iY, Color.Goldenrod);

            iY += 30;
            m_rSave = new Rectangle(iX, iY, 100, 30);
            DrawString(spriteBatch, "Save", iX, iY, Color.Goldenrod);

            if (m_szContext2 == null)
                iY = iHeight - 60;
            else
                iY = iHeight - 90;
            DrawString(spriteBatch, m_szContext1, iX - 40, iY, Color.White);
            if (m_szContext2 != null)
                DrawString(spriteBatch, m_szContext2, iX - 40, iY + 30, Color.White);

            int iArrowWidth = 50;
            int iArrowX = iX - iArrowWidth;
            spriteBatch.Draw(m_Game.m_BlueArrow, new Rectangle(iArrowX, m_iArrowY, iArrowWidth, 40), Color.White);
        }

        private void DrawString(SpriteBatch spriteBatch, string text, int iX, int iY, Color cColor)
        {
            spriteBatch.DrawString(Assets.DialogFont, text, new Vector2(iX + 2, iY + 2), Color.Black);
            spriteBatch.DrawString(Assets.DialogFont, text, new Vector2(iX, iY), cColor);
        }

        private Rectangle DrawCheckbox(SpriteBatch spriteBatch, int iX, int iY, string szText, bool bState, Color cColor)
        {
            // Draw the check box
            Rectangle rBoxRect = new Rectangle(iX, iY, 32, 32);
            spriteBatch.Draw(m_Game.m_CheckBox, rBoxRect, Color.White);

            // Draw the check
            if (bState)
                spriteBatch.Draw(m_Game.m_Check, rBoxRect, Color.White);

            // Draw the text
            rBoxRect.Width += 40 + (int)Assets.DialogFont.MeasureString(szText).X;
            DrawString(spriteBatch, szText, iX + 40, iY, cColor);

            return rBoxRect;
        }

        private void DrawSlider(SpriteBatch spriteBatch, int iX, int iY, Rectangle rBarBounds, string szText, float fValue, Color cTextColor, out Rectangle rLeftArrow, out Rectangle rRightArrow)
        {
            DrawString(spriteBatch, szText, iX, iY, cTextColor);
            
            // Draw bar
            spriteBatch.Draw(Assets.ScrollBar, rBarBounds, Color.White);

            // Draw Cursor
            float fX = (float)(rBarBounds.Width - 32) * fValue;
            int iLeft = (int)fX + rBarBounds.Left;
            spriteBatch.Draw(Assets.ScrollCursor, new Rectangle(iLeft + 24, rBarBounds.Top, 16, 16), null, Color.White, (float)(Math.PI / 2), new Vector2(0, 0), SpriteEffects.None, 0);

            // Draw left arrow
            rLeftArrow = new Rectangle(rBarBounds.Left, rBarBounds.Top, 16, 16);
            spriteBatch.Draw(Assets.ScrollArrow, new Rectangle(rBarBounds.Left + 16, rBarBounds.Top, 16, 16), null, Color.White, (float)-(Math.PI / 2), new Vector2(15, 15), SpriteEffects.None, 0);

            // Draw right arrow
            rRightArrow = new Rectangle(rBarBounds.Right - 16, rBarBounds.Top, 16, 16);
            spriteBatch.Draw(Assets.ScrollArrow, new Rectangle(rBarBounds.Right, rBarBounds.Top, 16, 16), null, Color.White, (float)-(Math.PI + (Math.PI / 2)), new Vector2(0, 0), SpriteEffects.None, 0);
        }
    }
}
