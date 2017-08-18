using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public GamePanel(GameScene game, Rectangle rect) : base(game)
        {
            m_Rect = rect;

            m_iCellWidth = (rect.Width - 6) / game.Puzzle.m_iSize;
            m_iCellHeight = (rect.Height - 6) / game.Puzzle.m_iSize;
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

        public void CloseCellDialog()
        {
            m_CellDialog = null;
        }

        #region Input
        public override void Click(int x, int y)
        {
            if (m_CellDialog != null)
            {
                if (!m_CellDialog.Click(x, y))
                    CloseCellDialog();
            }
            else
            {
                // Find the cell clicked
                int row = (y - m_Rect.Top) / m_iCellHeight;
                int col = (x - m_Rect.Left) / m_iCellWidth;

                // Show the cell dialog
                m_CellDialog = new CellDialog(GameScene, row, col);
                SoundManager.Inst.PlaySound(SoundManager.SEInst.MenuAccept);

                if (row == 1 && col == 1)
                {
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.SpanHelp1);
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.HorizontalClue2b);
                }
                else if (row == 2 && col == 1)
                {
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.BartMan1);
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.HorizontalClue3b);
                }
                else if (row == 0 && col == 1)
                {
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Hulk1);
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.HorizontalClue2c);
                }
                else if (row == 0 && col == 2)
                {
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.HorizontalClue5b);
                }
                else if (row == 2 && col == 2)
                {
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Hint3);
                }
                else if (row == 1 && col == 2)
                {
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.HorizontalClue4c);
                }
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
        #endregion

        #region Drawing
        public override void Draw(Renderer sb)
        {
            // Draw Background
            sb.Draw(Assets.TransGray, m_Rect, Color.White);

            // Draw Icons
            DrawIcons(sb);

            // Draw Borders
            DrawBorders(sb);

            if(m_CellDialog != null )
                m_CellDialog.Draw(sb);
        }

        void DrawIcons(Renderer sb)
        {
            for (int y = 0; y < GameScene.Puzzle.m_iSize; y++)
            {
                int ycoord = m_Rect.Top + (y * m_iCellHeight);

                for (int x = 0; x < GameScene.Puzzle.m_iSize; x++)
                {
                    int xcoord = m_Rect.Left + (x * m_iCellWidth);


                    int iFinal = GameScene.Puzzle.m_Rows[y].m_Cells[x].m_iFinalIcon;
                    if (iFinal >= 0)
                    {
                        Rectangle r = m_FinalRect;
                        r.Offset(xcoord, ycoord);
                        bool error = Happiness.Game.ErrorDetector && !GameScene.Puzzle.IsCorrect(y, x);
                        sb.Draw(GameScene.GetIcon(y, iFinal), r, error ? Color.Red : Color.White);

                        if (GameScene.ShouldDrawHint(y, x, iFinal))
                            Assets.HintSprite.Draw(sb, r, Color.White);
                    }
                    else
                    {
                        for (int iIcon = 0; iIcon < GameScene.Puzzle.m_iSize; iIcon++)
                        {
                            if (GameScene.Puzzle.m_Rows[y].m_Cells[x].m_bValues[iIcon])
                            {
                                Rectangle r = m_SmallRects[iIcon];
                                r.Offset(xcoord, ycoord);
                                sb.Draw(GameScene.GetIcon(y, iIcon), r, Color.White);

                                if (GameScene.ShouldDrawHint(y, x, iIcon))
                                    Assets.HintSprite.Draw(sb, r, Color.White);
                            }
                            else if (Happiness.Game.ErrorDetector2 && GameScene.Puzzle.SolutionIcon(y, x) == iIcon)
                            {
                                Rectangle r = m_SmallRects[iIcon];
                                r.Offset(xcoord, ycoord);
                                sb.Draw(GameScene.GetIcon(y, iIcon), r, Color.Red_A0625);
                            }
                        }
                    }
                }
            }
        }

        void DrawBorders(Renderer sb)
        {
            for (int y = 0; y < GameScene.Puzzle.m_iSize; y++)
            {
                int ycoord = m_Rect.Top + (y * m_iCellHeight);

                for (int x = 0; x < GameScene.Puzzle.m_iSize; x++)
                {
                    int xcoord = m_Rect.Left + (x * m_iCellWidth);                    

                    // Draw the border
                    sb.Draw(Assets.GoldBarVertical, new Rectangle(xcoord - 3, ycoord - 1, 3, m_iCellHeight + 2), Color.White);
                    sb.Draw(Assets.GoldBarVertical, new Rectangle(xcoord + m_iCellWidth, ycoord - 1, 3, m_iCellHeight + 2), Color.White);
                    sb.Draw(Assets.GoldBarHorizontal, new Rectangle(xcoord, ycoord - 1, m_iCellWidth, 1), Color.White);
                    sb.Draw(Assets.GoldBarHorizontal, new Rectangle(xcoord, ycoord + m_iCellHeight, m_iCellWidth, 1), Color.White);
                }
            }
        }
        #endregion

        public Vector2 IconPosition(int row, int col, int icon)
        {
            int cellX = m_Rect.Left + (col * m_iCellWidth);
            int cellY = m_Rect.Top + (row * m_iCellHeight);
            Rectangle r = m_SmallRects[icon];
            r.Offset(cellX, cellY);

            return new Vector2(r.Left, r.Bottom);
        }

        #region Accessors
        public GameScene GameScene
        {
            get { return (GameScene)m_Scene; }
        }

        public int IconSize
        {
            get { return m_SmallRects[0].Width; }
        }

        public int CellWidth
        {
            get { return m_iCellWidth; }        
        }

        public int CellHeight
        {
            get { return m_iCellHeight; }
        }

        public CellDialog CellDialog
        {
            get { return m_CellDialog; }
        }
        #endregion

    }
}
