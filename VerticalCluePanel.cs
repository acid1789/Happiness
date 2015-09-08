﻿using System;
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

        Clue[] m_Clues;
        int m_iSelectedIndex;

        public VerticalCluePanel(Happiness game, Clue[] clues, int width) : base(game)
        {
            m_Clues = clues;
            m_iSelectedIndex = -1;
            int screenHeight = m_Game.ScreenHeight;

            m_IconSize = (int)(Constants.IconSize * screenHeight);
            m_ClueSpace = (int)(Constants.ClueSpace * screenHeight);

            int clueHeight = m_IconSize * 3;
            int bottomMargin = (int)(screenHeight * Constants.MarginBottom);
            int leftMargin = (int)(m_Game.ScreenWidth * Constants.MarginLeft);

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

            m_Game.SelectClue(m_Clues[m_iSelectedIndex], this);
        }

        public void ClearSelected()
        {
            m_iSelectedIndex = -1;
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

        public void Draw(SpriteBatch sb, Clue[] clues)
        {
            float x = m_ScrollPosition;
            float y = m_Rect.Top;

            // Draw Clues
            foreach (Clue c in clues)
            {
                if ((x + m_IconSize) >= 0)
                {
                    // This icon is at least partially visible
                    bool bHintClue = m_Game.ShouldShowHint(c);
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
                sb.Draw(m_Game.SelectionIconTall, rect, Color.White);
            }

            int clueTotalSpace = (clues.Length * (m_IconSize + m_ClueSpace)) - m_ClueSpace;
            m_bCanScroll = (clueTotalSpace > m_Rect.Width);

            if (m_bCanScroll)
            {
                // Draw left arrow
                if (m_ScrollPosition != m_ScrollMax)
                {
                    sb.Draw(m_Game.ScrollArrow, new Rectangle(m_Rect.Left, m_Rect.Top + (m_IconSize * 2), m_IconSize, m_IconSize), null, Color.White, (float)-(Math.PI / 2), new Vector2(0, 0), SpriteEffects.None, 0);
                }

                // Draw right arrow
                if (m_ScrollPosition != m_ScrollMin)
                {
                    sb.Draw(m_Game.ScrollArrow, new Rectangle(m_Rect.Right, m_Rect.Top + m_IconSize, m_IconSize, m_IconSize), null, Color.White, (float)-(Math.PI + (Math.PI / 2)), new Vector2(0, 0), SpriteEffects.None, 0);
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
            int iNumIcons = c.GetIcons(m_Game.Puzzle, iIcons);

            // Draw the frame
            sb.Draw(m_Game.TransGrey, bounds, Color.White);
            sb.Draw(m_Game.GoldBarVertical, new Rectangle(x - 3, y - 3, 3, bounds.Height + 6), Color.White);
            sb.Draw(m_Game.GoldBarHorizontal, new Rectangle(x - 3, y - 3, bounds.Width + 6, 3), Color.White);
            sb.Draw(m_Game.GoldBarVertical, new Rectangle(bounds.Right, y - 3, 3, bounds.Height + 6), Color.White);
            sb.Draw(m_Game.GoldBarHorizontal, new Rectangle(x - 3, bounds.Bottom, bounds.Width + 6, 3), Color.White);

            // Draw the icons

            for (int j = 0; j < iNumIcons; j++)
            {
                sb.Draw(m_Game.GetIcon(iRows[j], iIcons[j]), rects[j], Color.White);
                if (bHintClue)
                    m_Game.HintSprite.Draw(sb, rects[j], Color.White);
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
                    sb.Draw(m_Game.EitherOrOverlay, overlayRects[1], Color.White);
                    break;
                case eVerticalType.TwoNot:
                    sb.Draw(m_Game.NotOverlay, overlayRects[0], Color.White);
                    break;
                case eVerticalType.ThreeTopNot:
                    sb.Draw(m_Game.NotOverlay, rects[0], Color.White);
                    break;
                case eVerticalType.ThreeMidNot:
                    sb.Draw(m_Game.NotOverlay, rects[1], Color.White);
                    break;
                case eVerticalType.ThreeBotNot:
                    sb.Draw(m_Game.NotOverlay, rects[2], Color.White);
                    break;
            }
        }

        #region Accessors
        #endregion
    }
}
