using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LogicMatrix;

namespace Happiness 
{
    class VerticalCluePanel : UIPanel
    {
        int m_IconSize;
        int m_ClueSpace;

        float m_ScrollPosition;
        float m_ScrollMin;
        float m_ScrollMax;
        bool m_bDragging;
        bool m_bCanScroll;
        float m_DragX;
        
        List<Clue> m_Clues;
        int m_iSelectedIndex;

        public VerticalCluePanel(GameScene scene, Clue[] clues, int width) : base(scene)
        {
            m_Clues = new List<Clue>(clues);
            m_iSelectedIndex = -1;
            int screenHeight = scene.Game.ScreenHeight;

            m_IconSize = (int)(Constants.IconSize * screenHeight);
            m_ClueSpace = (int)(Constants.ClueSpace * screenHeight);

            int clueHeight = m_IconSize * 3;
            int bottomMargin = (int)(screenHeight * Constants.MarginBottom);
            int leftMargin = (int)(scene.Game.ScreenWidth * Constants.MarginLeft);

            int y = screenHeight - (clueHeight + bottomMargin);
            m_Rect = new Rectangle(leftMargin, y, width, screenHeight - y);

            // Setup scroll min/max
            float totalWidth = ((m_IconSize + m_ClueSpace) * clues.Length) - m_ClueSpace;
            m_ScrollMin = (-totalWidth + width) - leftMargin;
            m_ScrollMax = leftMargin;
            m_ScrollPosition = leftMargin;
        }

        public override void Click(int x, int y)
        {
            float virtX = x - m_ScrollPosition;
            m_iSelectedIndex = (int)(virtX / (m_IconSize + m_ClueSpace));

            GameScene.SelectClue(m_Clues[m_iSelectedIndex], this);
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
            m_DragX = (float)args.StartX;
            m_bDragging = true;
        }

        public override void Drag(DragArgs args)
        {
            if (m_bDragging && m_bCanScroll)
            {
                float delta = m_DragX - args.CurrentX;
                m_DragX = args.CurrentX;

                if (delta != 0)
                {
                    m_ScrollPosition -= delta;
                    if (m_ScrollPosition < m_ScrollMin)
                        m_ScrollPosition = m_ScrollMin;
                    if (m_ScrollPosition > m_ScrollMax)
                        m_ScrollPosition = m_ScrollMax;
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            float x = m_ScrollPosition;
            float y = m_Rect.Top;

            // Draw Clues
            foreach (Clue c in m_Clues)
            {
                if ((x + m_IconSize) >= 0)
                {
                    // This icon is at least partially visible
                    bool bHintClue = GameScene.ShouldShowHint(c);
                    DrawClue(sb, (int)x, (int)y, c, bHintClue);
                }

                x += m_IconSize + m_ClueSpace;
                if (x > m_Rect.Width)
                    break;  // Cant draw anymore, just skip the rest
            }

            if (m_iSelectedIndex >= 0)
            {
                int xpos = (int)m_ScrollPosition + (m_iSelectedIndex * (m_IconSize + m_ClueSpace) - 3);

                Rectangle rect = new Rectangle(xpos, m_Rect.Top - 3, m_IconSize + 6, (m_IconSize * 3) + 6);
                sb.Draw(Assets.SelectionIconTall, rect, Color.White);
            }

            int clueTotalSpace = (m_Clues.Count * (m_IconSize + m_ClueSpace)) - m_ClueSpace;
            m_bCanScroll = (clueTotalSpace > m_Rect.Width);

            if (m_bCanScroll)
            {
                // Draw left arrow
                if (m_ScrollPosition != m_ScrollMax)
                {
                    sb.Draw(Assets.ScrollArrow, new Rectangle(m_Rect.Left, m_Rect.Top + (m_IconSize * 2), m_IconSize, m_IconSize), null, Color.White, (float)-(Math.PI / 2), new Vector2(0, 0), SpriteEffects.None, 0);
                }

                // Draw right arrow
                if (m_ScrollPosition != m_ScrollMin)
                {
                    sb.Draw(Assets.ScrollArrow, new Rectangle(m_Rect.Right, m_Rect.Top + m_IconSize, m_IconSize, m_IconSize), null, Color.White, (float)-(Math.PI + (Math.PI / 2)), new Vector2(0, 0), SpriteEffects.None, 0);
                }
            }
            else
                m_ScrollPosition = m_Rect.Left;
        }

        void DrawClue(SpriteBatch sb, int x, int y, Clue c, bool bHintClue)
        {
            Rectangle[] rects = new Rectangle[3];
            rects[0] = new Rectangle(x, y, m_IconSize, m_IconSize);
            rects[1] = new Rectangle(x, y + m_IconSize, m_IconSize, m_IconSize);
            rects[2] = new Rectangle(x, y + (m_IconSize * 2), m_IconSize, m_IconSize);
            Rectangle bounds = new Rectangle(x, y, m_IconSize, m_IconSize * 3);

            int[] iIcons = new int[3];
            int[] iRows = c.GetRows();
            int iNumIcons = c.GetIcons(GameScene.Puzzle, iIcons);

            // Draw the frame
            sb.Draw(Assets.TransGrey, bounds, Color.White);
            sb.Draw(Assets.GoldBarVertical, new Rectangle(x - 3, y - 3, 3, bounds.Height + 6), Color.White);
            sb.Draw(Assets.GoldBarHorizontal, new Rectangle(x - 3, y - 3, bounds.Width + 6, 3), Color.White);
            sb.Draw(Assets.GoldBarVertical, new Rectangle(bounds.Right, y - 3, 3, bounds.Height + 6), Color.White);
            sb.Draw(Assets.GoldBarHorizontal, new Rectangle(x - 3, bounds.Bottom, bounds.Width + 6, 3), Color.White);

            // Draw the icons

            for (int j = 0; j < iNumIcons; j++)
            {
                sb.Draw(GameScene.GetIcon(iRows[j], iIcons[j]), rects[j], Color.White);
                if (bHintClue)
                    Assets.HintSprite.Draw(sb, rects[j], Color.White);
            }

            // Draw the operational overlay
            Rectangle[] overlayRects = new Rectangle[2];
            overlayRects[0] = new Rectangle(x, y + (m_IconSize / 2), m_IconSize, m_IconSize);
            overlayRects[1] = new Rectangle(x, y + (m_IconSize / 2) + m_IconSize, m_IconSize, m_IconSize);
            switch (c.m_VerticalType)
            {
                case eVerticalType.Two:
                case eVerticalType.Three:
                    break;
                case eVerticalType.EitherOr:
                    sb.Draw(Assets.EitherOrOverlay, overlayRects[1], Color.White);
                    break;
                case eVerticalType.TwoNot:
                    sb.Draw(Assets.NotOverlay, overlayRects[0], Color.White);
                    break;
                case eVerticalType.ThreeTopNot:
                    sb.Draw(Assets.NotOverlay, rects[0], Color.White);
                    break;
                case eVerticalType.ThreeMidNot:
                    sb.Draw(Assets.NotOverlay, rects[1], Color.White);
                    break;
                case eVerticalType.ThreeBotNot:
                    sb.Draw(Assets.NotOverlay, rects[2], Color.White);
                    break;
            }
        }

        #region Accessors
        public GameScene GameScene
        {
            get { return (GameScene)m_Scene; }
        }
        #endregion
    }
}
