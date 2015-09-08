using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        Happiness m_Game;
        Rectangle m_Rect;
        int m_iRow;
        int m_iCol;
        int m_iSelectedIcon;

        int m_iLargeIcon;
        Rectangle m_LargeIconRect;
        Rectangle[] m_IconRects;
        bool[] m_bIconStatus;

        UIButton[] m_Buttons;       

        public CellDialog(Happiness game, int row, int col)
        {
            m_Game = game;
            m_iRow = row;
            m_iCol = col;
            m_iSelectedIcon = -1;

            int width = (int)(game.ScreenWidth * Constants.CellDialogWidth);
            int height = (int)(game.ScreenHeight * Constants.CellDialogHeight);

            int midScreenX = game.ScreenWidth >> 1;
            int midScreenY = game.ScreenHeight >> 1;
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
                int largeIconSize = (int)(game.ScreenHeight * Constants.CellDialogLargeIconSize);
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
                int iconSize = (int)(game.ScreenHeight * Constants.CellDialogSmallIconSize);

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
                int cancelButtonX = (int)(m_Game.ScreenWidth * Constants.CellDialog_CancelButtonX);
                int cancelButtonY = (int)(m_Game.ScreenHeight * Constants.CellDialog_CancelButtonY);
                int smallButtonWidth = (int)(m_Game.ScreenWidth * Constants.CellDialog_SmallButtonWidth);
                int smallButtonHeight = (int)(m_Game.ScreenHeight * Constants.CellDialog_SmallButtonHeight);
                int buttonWidth = (int)(m_Game.ScreenWidth * Constants.CellDialog_ButtonWidth);
                int buttonHeight = (int)(m_Game.ScreenHeight * Constants.CellDialog_ButtonHeight);

                int bottomAreaHeight = m_Rect.Bottom - m_IconRects[m_IconRects.Length - 1].Bottom;
                int centerOfBottomArea = bottomAreaHeight >> 1;
                int buttonRowY = centerOfBottomArea + (buttonHeight >> 1);

                int innerButtonX = (m_Rect.Left + dialogHalfWidth) - (buttonWidth >> 1);
                int outerButtonX = ((innerButtonX - m_Rect.Left) >> 1) - (buttonWidth >> 1);

                m_Buttons = new UIButton[(int)ButtonID.Last];
                Texture2D buttonTex = m_Game.m_ScrollBar;
                m_Buttons[(int)ButtonID.Cancel]    = new UIButton((int)ButtonID.Cancel,   "<",       m_Game.m_MenuFont, new Rectangle(m_Rect.Left + cancelButtonX, m_Rect.Top + cancelButtonY, smallButtonWidth, smallButtonHeight), buttonTex);
                m_Buttons[(int)ButtonID.Done]      = new UIButton((int)ButtonID.Done,     ">",       m_Game.m_MenuFont, new Rectangle(m_Rect.Right - (cancelButtonX + smallButtonWidth), m_Rect.Top + cancelButtonY, smallButtonWidth, smallButtonHeight) , buttonTex);
                m_Buttons[(int)ButtonID.Reset]     = new UIButton((int)ButtonID.Reset,    "Reset All",    m_Game.m_MenuFont, new Rectangle(innerButtonX, m_Rect.Bottom - buttonRowY, buttonWidth, buttonHeight), buttonTex);
                m_Buttons[(int)ButtonID.Confirm]   = new UIButton((int)ButtonID.Confirm,  "Confirm",      m_Game.m_MenuFont, new Rectangle(m_Rect.Left + outerButtonX, m_Rect.Bottom - buttonRowY, buttonWidth, buttonHeight), buttonTex);
                m_Buttons[(int)ButtonID.Eliminate] = new UIButton((int)ButtonID.Eliminate,"Eliminate",    m_Game.m_MenuFont, new Rectangle(m_Rect.Right - (outerButtonX + buttonWidth), m_Rect.Bottom - buttonRowY, buttonWidth, buttonHeight), buttonTex);

                foreach( UIButton b in m_Buttons )
                    b.MainColor = Color.GreenYellow;

                m_Buttons[(int)ButtonID.Confirm].Enabled = false;
                m_Buttons[(int)ButtonID.Eliminate].Enabled = false;
            }
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
                    SelectIcon(-1);
                    break;
                case ButtonID.Eliminate:
                    m_bIconStatus[m_iSelectedIcon] = !m_bIconStatus[m_iSelectedIcon];
                    SelectIcon(m_iSelectedIcon);
                    break;

            }
            return true;
        }

        void SelectIcon(int icon)
        {
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

        public void Draw(SpriteBatch sb)
        {
            // Draw background/frame
            sb.Draw(m_Game.TransGrey, m_Rect, Color.White);
            sb.Draw(m_Game.TransparentBox, m_Rect, Color.SteelBlue);
            sb.Draw(m_Game.TransparentBox, m_Rect, Color.SteelBlue);

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
                        m_Game.HintSprite.Draw(sb, m_IconRects[i], Color.White);
                }
            }

            // Draw Buttons
            foreach( UIButton b in m_Buttons )
                b.Draw(sb);
        }
    }
}
