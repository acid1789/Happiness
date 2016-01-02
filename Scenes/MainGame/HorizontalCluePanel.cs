using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LogicMatrix;

namespace Happiness
{
    class HorizontalCluePanel : UIPanel
    {
        int m_IconSize;
        int m_ClueSpace;

        float m_ScrollPosition;
        float m_ScrollMin;
        float m_ScrollMax;
        bool m_bCanScroll;
        bool m_bDragging;
        float m_DragY;

        List<Clue> m_Clues;
        int m_iSelectedIndex;

        public HorizontalCluePanel(GameScene scene, Clue[] clues) : base(scene)
        {
            m_iSelectedIndex = -1;
            m_Clues = new List<Clue>(clues);
            int screenWidth = Scene.Game.ScreenWidth;
            int screenHeight = Scene.Game.ScreenHeight;

            int marginRight = (int)(Constants.MarginRight * screenWidth);
            int marginTop = (int)(Constants.MarginTop * screenHeight);

            m_IconSize = (int)(Constants.IconSize * screenHeight);
            m_ClueSpace = (int)(Constants.ClueSpace * screenHeight);
            int clueWidth = m_IconSize * 3;

            // Initialize the rectangle
            int width = clueWidth + marginRight;
            int x = screenWidth - width;
            m_Rect = new Rectangle(x, marginTop, width, screenHeight);

            // Setup scroll min/max
            float totalHeight = ((m_IconSize + m_ClueSpace) * clues.Length) - m_ClueSpace;            
            m_ScrollMin = (-totalHeight + screenHeight) - marginTop;
            m_ScrollMax = marginTop;
            m_ScrollPosition = m_ScrollMax;
        }

        public override void Click(int x, int y)
        {
            float virtY = y - m_ScrollPosition;
            m_iSelectedIndex = (int)(virtY / (m_IconSize + m_ClueSpace));
            if (m_iSelectedIndex < m_Clues.Count)
            {
                GameScene.SelectClue(m_Clues[m_iSelectedIndex], this);
                if( m_iSelectedIndex == 0 )
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.HorizontalClueArea);
                else if( m_iSelectedIndex == 1 )
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.HorizontalClue2a);
                else if( m_iSelectedIndex == 2 )
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.HorizontalClue3a);
                else if( m_iSelectedIndex == 3 )
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.HorizontalClue4b);
                else if ( m_iSelectedIndex == 4 )
                    GameScene.Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.HorizontalClue5a);
            }
            else
            {
                GameScene.SelectClue(null, this);
                ClearSelected();
            }
        }

        public void ClearSelected()
        {
            m_iSelectedIndex = -1;
        }

        public void HideSelectedClue()
        {
            if (m_iSelectedIndex >= 0)
            {
                m_Clues.RemoveAt(m_iSelectedIndex);
                m_iSelectedIndex = -1;
            }
        }

        public void UnhideClues(Clue[] clues)
        {
            m_Clues = new List<Clue>(clues);
            ClearSelected();
        }

        public override void DragBegin(DragArgs args)
        {
            m_DragY = (float)args.StartY;
            m_bDragging = true;
        }

        public override void Drag(DragArgs args)
        {
            if (m_bDragging)
            {
                float deltaY = m_DragY - args.CurrentY;
                m_DragY = args.CurrentY;

                if (deltaY != 0)
                {
                    m_ScrollPosition -= deltaY;
                    if (m_ScrollPosition < m_ScrollMin)
                        m_ScrollPosition = m_ScrollMin;
                    if (m_ScrollPosition > m_ScrollMax)
                        m_ScrollPosition = m_ScrollMax;
                }
            }
        }

        public override void DragEnd(DragArgs args)
        {
            m_bDragging = false;
        }        

        public override void Draw(SpriteBatch sb)
        {
            float x = m_Rect.Left;
            float y = m_ScrollPosition;

            foreach (Clue c in m_Clues)
            {
                if ((y + m_IconSize) >= 0)
                {
                    // This icon is at least partially visible
                    bool bHintClue = GameScene.ShouldShowHint(c);
                    DrawClue(sb, (int)x, (int)y, c, bHintClue);
                }

                y += m_IconSize + m_ClueSpace;
                if (y > m_Rect.Height)
                    break;  // Cant draw anymore, just skip the rest
            }

            if (m_iSelectedIndex >= 0)
            {
                int ypos = (int)m_ScrollPosition + (m_iSelectedIndex * (m_IconSize + m_ClueSpace) - 3);

                Rectangle rect = new Rectangle(m_Rect.Left - 3, ypos, (m_IconSize * 3) + 6, m_IconSize + 6);
                sb.Draw(Assets.SelectionIconWide, rect, Color.White);
            }

            int clueTotalSpace = (m_Clues.Count * (m_IconSize + m_ClueSpace)) - m_ClueSpace;
            m_bCanScroll = (clueTotalSpace > m_Rect.Height);

            if (m_bCanScroll)
            {
                // Draw up arrow
                if (m_ScrollPosition != m_ScrollMax)
                {
                    sb.Draw(Assets.ScrollArrow, new Rectangle(m_Rect.Left + m_IconSize, 0, m_IconSize, m_IconSize), Color.White);
                }

                // Draw down arrow
                if (m_ScrollPosition != m_ScrollMin)
                {
                    sb.Draw(Assets.ScrollArrow, new Rectangle(m_Rect.Left + m_IconSize, m_Rect.Bottom - m_IconSize, m_IconSize, m_IconSize), null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
                }
            }
            else
                m_ScrollPosition = m_Rect.Top;
        }

        void DrawClue(SpriteBatch sb, int x, int y, Clue c, bool bHintClue)
        {
            Rectangle[] rects = new Rectangle[3];
            rects[0] = new Rectangle(x, y, m_IconSize, m_IconSize);
            rects[1] = new Rectangle(x + m_IconSize, y, m_IconSize, m_IconSize);
            rects[2] = new Rectangle(x + (m_IconSize * 2), y, m_IconSize, m_IconSize);
            Rectangle bounds = new Rectangle(x, y, m_IconSize * 3, m_IconSize);            

            int[] iIcons = new int[3];
            int[] iRows = c.GetRows();
            int iNumIcons = c.GetIcons(GameScene.Puzzle, iIcons);

            // Draw the frame
            //sb.Draw(m_Game.TransGrey, bounds, Color.White);
            sb.Draw(Assets.GoldBarHorizontal, new Rectangle(x - 3, y - 3, bounds.Width + 6, 3), Color.White);
            sb.Draw(Assets.GoldBarHorizontal, new Rectangle(x - 3, bounds.Bottom, bounds.Width + 6, 3), Color.White);
            sb.Draw(Assets.GoldBarVertical, new Rectangle(x - 3, y - 3, 3, bounds.Height + 6), Color.White);
            sb.Draw(Assets.GoldBarVertical, new Rectangle(bounds.Right, y - 3, 3, bounds.Height + 6), Color.White);

            // Draw the icons
            if (c.m_HorizontalType == eHorizontalType.LeftOf || c.m_HorizontalType == eHorizontalType.NotLeftOf)
            {
                sb.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), rects[0], Color.White);
                sb.Draw(Assets.LeftOfIcon,                      rects[1], Color.White);
                sb.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), rects[2], Color.White);

                if (bHintClue)
                {
                    Assets.HintSprite.Draw(sb, rects[0], Color.White);
                    Assets.HintSprite.Draw(sb, rects[2], Color.White);
                }
            }
            else
            {
                for (int j = 0; j < iNumIcons; j++)
                {
                    sb.Draw(GameScene.GetIcon(iRows[j], iIcons[j]), rects[j], Color.White);
                    if (bHintClue)
                        Assets.HintSprite.Draw(sb, rects[j], Color.White);
                }
            }

            // Draw the operational overlay
            switch (c.m_HorizontalType)
            {
                case eHorizontalType.NextTo:
                case eHorizontalType.LeftOf:
                    break;
                case eHorizontalType.NotLeftOf:
                case eHorizontalType.NotNextTo:
                    sb.Draw(Assets.NotOverlay, rects[1], Color.White);
                    break;
                case eHorizontalType.Span:
                    sb.Draw(Assets.SpanOverlay, bounds, Color.White);
                    break;
                case eHorizontalType.SpanNotLeft:
                    sb.Draw(Assets.SpanOverlay, bounds, Color.White);
                    sb.Draw(Assets.NotOverlay, rects[0], Color.White);
                    break;
                case eHorizontalType.SpanNotMid:
                    sb.Draw(Assets.SpanOverlay, bounds, Color.White);
                    sb.Draw(Assets.NotOverlay, rects[1], Color.White);
                    break;
                case eHorizontalType.SpanNotRight:
                    sb.Draw(Assets.SpanOverlay, bounds, Color.White);
                    sb.Draw(Assets.NotOverlay, rects[2], Color.White);
                    break;
            }
        }

        #region Accessors
        public GameScene GameScene
        {
            get { return (GameScene)m_Scene; }
        }

        public Clue[] Clues
        {
            get { return m_Clues.ToArray(); }
        }

        public int ClueHeight
        {
            get { return m_IconSize + m_ClueSpace; }
        }
        #endregion
    }
}
