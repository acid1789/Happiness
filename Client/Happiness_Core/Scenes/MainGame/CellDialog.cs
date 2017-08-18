using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness
{
    class CellDialog
    {
        enum ButtonID
        {
            Cancel,
            Done,
            Reset,
            Confirm,
            Eliminate,

            Last
        }

        GameScene m_Game;
        Rectangle m_Rect;
        int m_iRow;
        int m_iCol;
        int m_iSelectedIcon;

        int m_iLargeIcon;
        Rectangle m_LargeIconRect;
        Rectangle[] m_IconRects;
        bool[] m_bIconStatus;

        UIButton[] m_Buttons;       

        public CellDialog(GameScene game, int row, int col)
        {
            m_Game = game;
            m_iRow = row;
            m_iCol = col;
            m_iSelectedIcon = -1;

            int width = (int)(game.Game.ScreenWidth * Constants.CellDialogWidth);
            int height = (int)(game.Game.ScreenHeight * Constants.CellDialogHeight);

            int midScreenX = game.Game.ScreenWidth >> 1;
            int midScreenY = game.Game.ScreenHeight >> 1;
            int dialogHalfWidth = (width >> 1);
            int dialogHalfHeight = (height >> 1);

            int x = midScreenX - dialogHalfWidth;
            int y = midScreenY - dialogHalfHeight;
            m_Rect = new Rectangle(x, y, width, height);

            m_bIconStatus = new bool[game.Puzzle.m_iSize];
            for (int i = 0; i < m_bIconStatus.Length; i++)
                m_bIconStatus[i] = m_Game.Puzzle.m_Rows[m_iRow].m_Cells[m_iCol].m_bValues[i];

            // Setup large icon
            {
                m_iLargeIcon = m_Game.Puzzle.m_Rows[m_iRow].m_Cells[m_iCol].m_iFinalIcon;
                int largeIconSize = (int)(game.Game.ScreenHeight * Constants.CellDialogLargeIconSize);
                int halfLargeIconSize = largeIconSize >> 1;
                m_LargeIconRect = new Rectangle(m_Rect.Left + dialogHalfWidth - halfLargeIconSize, dialogHalfHeight - halfLargeIconSize, largeIconSize, largeIconSize);
            }

            // Setup small icons
            {
                m_IconRects = new Rectangle[game.Puzzle.m_iSize];
                int topRowCount = game.Puzzle.m_iSize >> 1;
                int botRowCount = game.Puzzle.m_iSize - topRowCount;

                // Make sure bottom row is the smaller of the two
                while (topRowCount < botRowCount)
                {
                    topRowCount++;
                    botRowCount--;
                }
                int iconSize = (int)(game.Game.ScreenHeight * Constants.CellDialogSmallIconSize);

                // Setup the top row
                int halfTopRow = topRowCount >> 1;
                int topRowLeft = m_Rect.Left + dialogHalfWidth - (iconSize * halfTopRow);
                if ((topRowCount & 1) == 1)
                    topRowLeft = m_Rect.Left + dialogHalfWidth - (iconSize / 2) - (halfTopRow * iconSize);
                for (int i = 0; i < topRowCount; i++)
                    m_IconRects[i] = new Rectangle(topRowLeft + (i * iconSize), dialogHalfHeight - iconSize, iconSize, iconSize);

                // Setup the bottom row
                int halfBotRow = botRowCount >> 1;
                int botRowLeft = m_Rect.Left + dialogHalfWidth - (iconSize * halfBotRow);
                if ((botRowCount & 1) == 1)
                    botRowLeft = m_Rect.Left + dialogHalfWidth - (iconSize / 2) - (halfBotRow * iconSize);
                for (int i = 0; i < botRowCount; i++)
                    m_IconRects[topRowCount + i] = new Rectangle(botRowLeft + (i * iconSize), dialogHalfHeight, iconSize, iconSize);
            }

            // Setup Buttons
            {
                int cancelButtonX = (int)(m_Game.Game.ScreenWidth * Constants.CellDialog_CancelButtonX);
                int cancelButtonY = (int)(m_Game.Game.ScreenHeight * Constants.CellDialog_CancelButtonY);
                int smallButtonWidth = (int)(m_Game.Game.ScreenWidth * Constants.CellDialog_SmallButtonWidth);
                int smallButtonHeight = (int)(m_Game.Game.ScreenHeight * Constants.CellDialog_SmallButtonHeight);
                int buttonWidth = (int)(m_Game.Game.ScreenWidth * Constants.CellDialog_ButtonWidth);
                int buttonHeight = (int)(m_Game.Game.ScreenHeight * Constants.CellDialog_ButtonHeight);

                int bottomAreaHeight = m_Rect.Bottom - m_IconRects[m_IconRects.Length - 1].Bottom;
                int centerOfBottomArea = bottomAreaHeight >> 1;
                int buttonRowY = centerOfBottomArea + (buttonHeight >> 1);

                int innerButtonX = (m_Rect.Left + dialogHalfWidth) - (buttonWidth >> 1);
                int outerButtonX = ((innerButtonX - m_Rect.Left) >> 1) - (buttonWidth >> 1);

                m_Buttons = new UIButton[(int)ButtonID.Last];
                Texture2D buttonTex = Assets.ScrollBar;
                m_Buttons[(int)ButtonID.Cancel]    = new UIButton((int)ButtonID.Cancel,   "<",          Assets.MenuFont, new Rectangle(m_Rect.Left + cancelButtonX, m_Rect.Top + cancelButtonY, smallButtonWidth, smallButtonHeight), buttonTex);
                m_Buttons[(int)ButtonID.Done]      = new UIButton((int)ButtonID.Done,     ">",          Assets.MenuFont, new Rectangle(m_Rect.Right - (cancelButtonX + smallButtonWidth), m_Rect.Top + cancelButtonY, smallButtonWidth, smallButtonHeight) , buttonTex);
                m_Buttons[(int)ButtonID.Reset]     = new UIButton((int)ButtonID.Reset,    "Reset All",  Assets.MenuFont, new Rectangle(innerButtonX, m_Rect.Bottom - buttonRowY, buttonWidth, buttonHeight), buttonTex);
                m_Buttons[(int)ButtonID.Confirm]   = new UIButton((int)ButtonID.Confirm,  "Confirm",    Assets.MenuFont, new Rectangle(m_Rect.Left + outerButtonX, m_Rect.Bottom - buttonRowY, buttonWidth, buttonHeight), buttonTex);
                m_Buttons[(int)ButtonID.Eliminate] = new UIButton((int)ButtonID.Eliminate,"Eliminate",  Assets.MenuFont, new Rectangle(m_Rect.Right - (outerButtonX + buttonWidth), m_Rect.Bottom - buttonRowY, buttonWidth, buttonHeight), buttonTex);
                m_Buttons[0].ClickSound = SoundManager.SEInst.MenuCancel;

                foreach ( UIButton b in m_Buttons )
                    b.MainColor = Color.GreenYellow;

                m_Buttons[(int)ButtonID.Confirm].Enabled = false;
                m_Buttons[(int)ButtonID.Eliminate].Enabled = false;
                m_Buttons[(int)ButtonID.Eliminate].ClickSound = SoundManager.SEInst.MenuCancel;
            }
            if (!game.Game.Tutorial.IsPieceSetup(TutorialSystem.TutorialPiece.EliminateRedNebula))
            {
                Rectangle instRect = new Rectangle(m_IconRects[1].Right, m_IconRects[1].Top + (m_IconRects[1].Height >> 1) + game.Game.Tutorial.ArrowHeight, 250, 0);
                Vector2 rightIconTarget = new Vector2(m_IconRects[1].Right, m_IconRects[1].Top + (m_IconRects[1].Height >> 1));
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.EliminateRedNebula, rightIconTarget, Constants.ArrowLeft,
                    instRect, "Tap the {icon:Hubble[4]} to select it.", TutorialSystem.TutorialPiece.EliminateRedNebula2, m_IconRects[1]);

                Rectangle eliminate = m_Buttons[(int)ButtonID.Eliminate].Rect;
                Vector2 eliminatePos = new Vector2(eliminate.Left + (eliminate.Width >> 1), eliminate.Top);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.EliminateRedNebula2, eliminatePos, Constants.ArrowDown, instRect, "Tap the 'Eliminate' button to eliminate the {icon:Hubble[4]}.", TutorialSystem.TutorialPiece.EliminateRedNebula3, eliminate);

                Rectangle done = m_Buttons[(int)ButtonID.Done].Rect;
                Vector2 donePos = new Vector2(done.Left, done.Bottom);
                string doneText = "Tap the '>' button to confirm the changes and return to the puzzle.";
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.EliminateRedNebula3, donePos, Constants.ArrowDiagonalUpRight, instRect, doneText, TutorialSystem.TutorialPiece.BartMan1, done);

                Vector2 bottomIconTarget = new Vector2(m_IconRects[2].Right, m_IconRects[2].Top + (m_IconRects[2].Height >> 1));
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.BartMan2, bottomIconTarget, Constants.ArrowLeft, instRect, "Tap the {icon:Simpsons[2]} to select it.", TutorialSystem.TutorialPiece.BartMan3, m_IconRects[2]);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.BartMan3, eliminatePos, Constants.ArrowDown, instRect, "Tap the 'Eliminte' button to eliminate the {icon:Simpsons[2]}", TutorialSystem.TutorialPiece.BartMan4, eliminate);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.BartMan4, donePos, Constants.ArrowDiagonalUpRight, instRect, doneText, TutorialSystem.TutorialPiece.Hulk1, done);

                Vector2 icon0Pos = new Vector2(m_IconRects[0].Right, m_IconRects[0].Top + (m_IconRects[0].Height >> 1));
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Hulk2, icon0Pos, Constants.ArrowLeft, instRect, "Tap the {icon:Superheros[3]} to select it.", TutorialSystem.TutorialPiece.Hulk3, m_IconRects[0]);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Hulk3, eliminatePos, Constants.ArrowDown, instRect, "Tap the 'Eliminte' button to eliminate the {icon:Superheros[3]}", TutorialSystem.TutorialPiece.Hulk4, eliminate);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Hulk4, donePos, Constants.ArrowDiagonalUpRight, instRect, doneText, TutorialSystem.TutorialPiece.HorizontalClue2a, done);

                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.CrabNebula1, bottomIconTarget, Constants.ArrowLeft, instRect, "Tap the {icon:Hubble[2]} to select it.", TutorialSystem.TutorialPiece.CrabNebula2, m_IconRects[2]);

                Rectangle confirm = m_Buttons[(int)ButtonID.Confirm].Rect;
                Vector2 confirmButtonPos = new Vector2(confirm.Left + (confirm.Width >> 1), confirm.Top);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.CrabNebula2, confirmButtonPos, Constants.ArrowDown, instRect, "Tap the 'Confirm' button to confirm the {icon:Hubble[2]} belongs in this grid cell.", TutorialSystem.TutorialPiece.CrabNebula3, confirm);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.CrabNebula3, donePos, Constants.ArrowDiagonalUpRight, instRect, doneText, TutorialSystem.TutorialPiece.HorizontalClue2c, done);

                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.GreenLantern1, bottomIconTarget, Constants.ArrowLeft, instRect, "Tap the {icon:Superheros[2]} to select it.", TutorialSystem.TutorialPiece.GreenLantern2, m_IconRects[2]);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.GreenLantern2, eliminatePos, Constants.ArrowDown, instRect, "Tap the 'Eliminate' button to eliminate the {icon:Superheros[2]}", TutorialSystem.TutorialPiece.GreenLantern3, eliminate);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.GreenLantern3, donePos, Constants.ArrowDiagonalUpRight, instRect, doneText, TutorialSystem.TutorialPiece.HorizontalClue2d, done);

                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Homer1, icon0Pos, Constants.ArrowLeft, instRect, "Tap the {icon:Simpsons[3]} to select it.", TutorialSystem.TutorialPiece.Homer2, m_IconRects[0]);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Homer2, confirmButtonPos, Constants.ArrowDown, instRect, "Tap the 'Confirm' button to confirm the {icon:Simpsons[3]} belongs in this grid cell.", TutorialSystem.TutorialPiece.Homer3, confirm);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Homer3, donePos, Constants.ArrowDiagonalUpRight, instRect, doneText, TutorialSystem.TutorialPiece.HorizontalClue4, done);

                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.GreenLantern4, bottomIconTarget, Constants.ArrowLeft, instRect, "Tap the {icon:Superheros[2]} to select it.", TutorialSystem.TutorialPiece.GreenLantern5, m_IconRects[2]);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.GreenLantern5, confirmButtonPos, Constants.ArrowDown, instRect, "Tap the 'Confirm' button to confirm the {icon:Superheros[2]} belongs in this grid cell.", TutorialSystem.TutorialPiece.GreenLantern6, confirm);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.GreenLantern6, donePos, Constants.ArrowDiagonalUpRight, instRect, doneText, TutorialSystem.TutorialPiece.HideClue1, done);

                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Bartman5, bottomIconTarget, Constants.ArrowLeft, instRect, "Tap the {icon:Simpsons[2]} to select it.", TutorialSystem.TutorialPiece.Bartman6, m_IconRects[2]);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Bartman6, confirmButtonPos, Constants.ArrowDown, instRect, "Tap the 'Confirm' button to confirm the {icon:Simpsons[2]} belongs in this grid cell.", TutorialSystem.TutorialPiece.Bartman7, confirm);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Bartman7, donePos, Constants.ArrowDiagonalUpRight, instRect, doneText, TutorialSystem.TutorialPiece.Undo, done);

                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.RedNebula4, rightIconTarget, Constants.ArrowLeft, instRect, "Tap the {icon:Hubble[4]} to select it.", TutorialSystem.TutorialPiece.RedNebula5, m_IconRects[1]);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.RedNebula5, eliminatePos, Constants.ArrowDown, instRect, "Tap the 'Eliminate' button to eliminate the {icon:Hubble[4]}.", TutorialSystem.TutorialPiece.RedNebula6, eliminate);
                game.Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.RedNebula6, donePos, Constants.ArrowDiagonalUpRight, instRect, doneText, TutorialSystem.TutorialPiece.EndScreen1, done);
            }
        }

        public bool OnKeyUp(KeyArgs e)
        {
            if (e.Key == Keys.Escape)
                return false;
            return true;
        }

        public bool Click(int x, int y)
        {
            foreach (UIButton btn in m_Buttons)
            {
                if( btn.Click(x, y) )
                    return DoButtonClick((ButtonID)btn.ButtonID);
            }

            if (m_iLargeIcon < 0)
            {
                for( int i = 0; i < m_IconRects.Length; i++ )
                {
                    if (m_IconRects[i].Contains(x, y))
                    {
                        SelectIcon(i);
                        break;
                    }
                }
            }

            if (!m_Rect.Contains(x, y))
            {
                SoundManager.Inst.PlaySound(SoundManager.SEInst.MenuCancel);
                return false;
            }

            return true;
        }

        bool DoButtonClick(ButtonID btn)
        {       
            switch (btn)
            {
                case ButtonID.Cancel:
                    return false;
                case ButtonID.Done:
                    Commit();
                    m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.EliminateRedNebula3);
                    m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.BartMan4);
                    m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Hulk4);
                    m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.CrabNebula3);
                    m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.GreenLantern3);
                    m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Homer3);
                    m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.GreenLantern6);
                    m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Bartman7);
                    m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.RedNebula6);
                    return false;
                case ButtonID.Reset:
                    {
                        m_iLargeIcon = -1;
                        for (int i = 0; i < m_bIconStatus.Length; i++)
                            m_bIconStatus[i] = true;
                    }
                    break;
                case ButtonID.Confirm:
                    m_iLargeIcon = m_iSelectedIcon;
                    if (m_iSelectedIcon == 2)
                    {
                        m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.CrabNebula2);
                        m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.GreenLantern5);
                        m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Bartman6);
                    }
                    else if( m_iSelectedIcon == 0 )
                        m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Homer2);
                    SelectIcon(-1);
                    break;
                case ButtonID.Eliminate:
                    m_bIconStatus[m_iSelectedIcon] = !m_bIconStatus[m_iSelectedIcon];
                    SelectIcon(m_iSelectedIcon);

                    if (m_iSelectedIcon == 1)
                    {
                        m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.EliminateRedNebula2);
                        m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.RedNebula5);
                    }
                    else if (m_iSelectedIcon == 2)
                    {
                        m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.BartMan3);
                        m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.GreenLantern2);
                    }
                    else if (m_iSelectedIcon == 0)
                        m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Hulk3);
                    break;

            }
            return true;
        }

        void SelectIcon(int icon)
        {
            SoundManager.Inst.PlaySound(SoundManager.SEInst.MenuAccept);
            m_iSelectedIcon = icon;
            if (m_iSelectedIcon >= 0)
            {
                m_Buttons[(int)ButtonID.Confirm].Enabled = true;
                m_Buttons[(int)ButtonID.Eliminate].Enabled = true;

                if (m_bIconStatus[icon])
                    m_Buttons[(int)ButtonID.Eliminate].Text = "Eliminate";
                else
                    m_Buttons[(int)ButtonID.Eliminate].Text = "Restore";
            }
            else
            {
                m_Buttons[(int)ButtonID.Confirm].Enabled = false;
                m_Buttons[(int)ButtonID.Eliminate].Enabled = false;
            }

            if (m_iSelectedIcon == 1)
            {
                m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.EliminateRedNebula);
                m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.RedNebula4);
            }
            else if (m_iSelectedIcon == 2)
            {
                m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.BartMan2);
                m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.CrabNebula1);
                m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.GreenLantern1);
                m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.GreenLantern4);
                m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Bartman5);
            }
            else if (m_iSelectedIcon == 0)
            {
                m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Hulk2);
                m_Game.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Homer1);
            }
        }

        void Commit()
        {
            // Commit all changes to the actual puzzle
            for (int i = 0; i < m_bIconStatus.Length; i++)
            {
                if (m_Game.Puzzle.m_Rows[m_iRow].m_Cells[m_iCol].m_bValues[i] != m_bIconStatus[i])
                {
                    if( m_bIconStatus[i] )
                        m_Game.DoAction(eActionType.eAT_RestoreIcon, m_iRow, m_iCol, i);
                    else
                        m_Game.DoAction(eActionType.eAT_EliminateIcon, m_iRow, m_iCol, i);
                }
            }
            if( m_iLargeIcon >= 0 && m_iLargeIcon != m_Game.Puzzle.m_Rows[m_iRow].m_Cells[m_iCol].m_iFinalIcon)
                m_Game.DoAction(eActionType.eAT_SetFinalIcon, m_iRow, m_iCol, m_iLargeIcon);

        }

        public void Draw(Renderer sb)
        {
            // Draw background/frame
            sb.Draw(Assets.TransGray, m_Rect, Color.White);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.SteelBlue);

            // Draw Icons
            if (m_iLargeIcon >= 0)
            {
                sb.Draw(m_Game.GetIcon(m_iRow, m_iLargeIcon), m_LargeIconRect, Color.White);
            }
            else
            {
                for (int i = 0; i < m_IconRects.Length; i++)
                {
                    if (m_bIconStatus[i])
                    {
                        sb.Draw(m_Game.GetIcon(m_iRow, i), m_IconRects[i], Color.White);                        
                    }

                    if( i == m_iSelectedIcon )
                        Assets.HintSprite.Draw(sb, m_IconRects[i], Color.White);
                }
            }

            // Draw Buttons
            foreach( UIButton b in m_Buttons )
                b.Draw(sb);
        }
    }
}