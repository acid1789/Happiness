using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness 
{
    class GamePanel : UIPanel
    {

        int m_iCellWidth;
        int m_iCellHeight;
        int m_iFinalIconSize;

        Rectangle m_FinalRect;
        Rectangle[] m_SmallRects;

        CellDialog m_CellDialog;

        public GamePanel(Happiness game, Rectangle rect) : base(game)
        {
            m_Rect = rect;

            m_iCellWidth = rect.Width / game.Puzzle.m_iSize;
            m_iCellHeight = rect.Height / game.Puzzle.m_iSize;
            m_Rect.Width = m_iCellWidth * game.Puzzle.m_iSize;
            m_Rect.Height = m_iCellHeight * game.Puzzle.m_iSize;

            m_iFinalIconSize = Math.Min(m_iCellWidth, m_iCellHeight);

            int cellHalfWidth = m_iCellWidth >> 1;
            m_FinalRect = new Rectangle(cellHalfWidth - (m_iFinalIconSize / 2), 0, m_iFinalIconSize, m_iFinalIconSize);

            // Build small rectangles
            m_SmallRects = new Rectangle[game.Puzzle.m_iSize];
            int topRowCount = game.Puzzle.m_iSize >> 1;
            int botRowCount = game.Puzzle.m_iSize - topRowCount;

            // Make sure bottom row is the smaller of the two
            while (topRowCount < botRowCount)
            {
                topRowCount++;
                botRowCount--;
            }
            int iconSize = Math.Min(m_iCellWidth / topRowCount, m_iCellHeight / 2);

            // Setup the top row
            int halfTopRow = topRowCount >> 1;
            int topRowLeft = cellHalfWidth - (iconSize * halfTopRow);
            if( (topRowCount & 1) == 1 )
                topRowLeft = cellHalfWidth - (iconSize / 2) - (halfTopRow * iconSize);
            for ( int i = 0; i < topRowCount; i++ )
                m_SmallRects[i] = new Rectangle(topRowLeft + (i * iconSize), 0, iconSize, iconSize);

            // Setup the bottom row
            int halfBotRow = botRowCount >> 1;
            int botRowLeft = cellHalfWidth - (iconSize * halfBotRow);
            if( (botRowCount & 1) == 1 )
                botRowLeft = cellHalfWidth - (iconSize / 2) - (halfBotRow * iconSize);
            for ( int i = 0; i < botRowCount; i++ )
                m_SmallRects[topRowCount + i] = new Rectangle(botRowLeft + (i * iconSize), iconSize, iconSize, iconSize);
        }

        public override void Click(int x, int y)
        {
            if (m_CellDialog != null)
            {
                if( !m_CellDialog.Click(x, y) )
                    m_CellDialog = null;
            }
            else
            {
                // Find the cell clicked
                int row = (y - m_Rect.Top) / m_iCellHeight;
                int col = (x - m_Rect.Left) / m_iCellWidth;

                // Show the cell dialog
                m_CellDialog = new CellDialog(m_Game, row, col);
            }
        }

        public override void DragBegin(DragArgs args)
        {
            base.DragBegin(args);
        }

        public override void Drag(DragArgs args)
        {
            base.Drag(args);
        }

        public override void DragEnd(DragArgs args)
        {
            base.DragEnd(args);
        }

        public override bool Contains(int x, int y)
        {
            if( m_CellDialog != null)
                return true;

            return base.Contains(x, y);
        }

        #region Drawing
        public void Draw(SpriteBatch sb)
        {
            // Draw Background
            sb.Draw(m_Game.TransGrey, m_Rect, Color.White);

            // Draw Icons
            DrawIcons(sb);

            // Draw Borders
            DrawBorders(sb);

            if(m_CellDialog != null )
                m_CellDialog.Draw(sb);
        }

        void DrawIcons(SpriteBatch sb)
        {
            for (int y = 0; y < m_Game.Puzzle.m_iSize; y++)
            {
                int ycoord = m_Rect.Top + (y * m_iCellHeight);

                for (int x = 0; x < m_Game.Puzzle.m_iSize; x++)
                {
                    int xcoord = m_Rect.Left + (x * m_iCellWidth);


                    int iFinal = m_Game.Puzzle.m_Rows[y].m_Cells[x].m_iFinalIcon;
                    if (iFinal >= 0)
                    {
                        Rectangle r = m_FinalRect;
                        r.Offset(xcoord, ycoord);
                        sb.Draw(m_Game.GetIcon(y, iFinal), r, Color.White);

                        //if (m_Hint != null && m_Hint.ShouldDraw(iRow, iCol, iFinal))
                        //    m_HintSprite.Draw(spriteBatch, m_aDisplayRows[iRow].m_aCells[iCol].m_rFinal, Color.White);
                    }
                    else
                    {
                        for (int iIcon = 0; iIcon < m_Game.Puzzle.m_iSize; iIcon++)
                        {
                            if (m_Game.Puzzle.m_Rows[y].m_Cells[x].m_bValues[iIcon])
                            {
                                Rectangle r = m_SmallRects[iIcon];
                                r.Offset(xcoord, ycoord);
                                sb.Draw(m_Game.GetIcon(y, iIcon), r, Color.White);

                                //if (m_Hint != null && m_Hint.ShouldDraw(iRow, iCol, iIcon))
                                //    m_HintSprite.Draw(spriteBatch, m_aDisplayRows[iRow].m_aCells[iCol].m_aDisplayRects[iIcon], Color.White);
                            }
                        }
                    }
                }
            }
        }

        void DrawBorders(SpriteBatch sb)
        {
            for (int y = 0; y < m_Game.Puzzle.m_iSize; y++)
            {
                int ycoord = m_Rect.Top + (y * m_iCellHeight);

                for (int x = 0; x < m_Game.Puzzle.m_iSize; x++)
                {
                    int xcoord = m_Rect.Left + (x * m_iCellWidth);                    

                    // Draw the border
                    sb.Draw(m_Game.GoldBarVertical, new Rectangle(xcoord - 3, ycoord - 1, 3, m_iCellHeight + 2), Color.White);
                    sb.Draw(m_Game.GoldBarVertical, new Rectangle(xcoord + m_iCellWidth, ycoord - 1, 3, m_iCellHeight + 2), Color.White);
                    sb.Draw(m_Game.GoldBarHorizontal, new Rectangle(xcoord, ycoord - 1, m_iCellWidth, 1), Color.White);
                    sb.Draw(m_Game.GoldBarHorizontal, new Rectangle(xcoord, ycoord + m_iCellHeight, m_iCellWidth, 1), Color.White);
                }
            }
        }
        #endregion
    }
}
