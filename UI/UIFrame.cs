using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    class UIFrame
    {
        Rectangle m_Rect;
        Rectangle m_RectTop;
        Rectangle m_RectBot;
        Rectangle m_RectLeft;
        Rectangle m_RectRight;
        Color m_FrameColor;


        public UIFrame(int frameSize, Rectangle rect)
        {
            m_Rect = rect;
            m_FrameColor = Color.White;

            m_RectTop = new Rectangle(m_Rect.Left, m_Rect.Top - frameSize, m_Rect.Width, frameSize);
            m_RectBot = new Rectangle(m_Rect.Left, m_Rect.Bottom, m_Rect.Width, frameSize);
            m_RectLeft = new Rectangle(m_Rect.Left - frameSize, m_Rect.Top - frameSize, frameSize, m_Rect.Height + frameSize + frameSize);
            m_RectRight = new Rectangle(m_Rect.Right, m_Rect.Top - frameSize, frameSize, m_Rect.Height + frameSize + frameSize);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Assets.GoldBarHorizontal, m_RectTop, m_FrameColor);
            sb.Draw(Assets.GoldBarHorizontal, null, m_RectBot, null, null, 0, null, m_FrameColor, SpriteEffects.FlipVertically);
            sb.Draw(Assets.GoldBarVertical, null, m_RectLeft, null, null, 0, null, m_FrameColor, SpriteEffects.FlipHorizontally);
            sb.Draw(Assets.GoldBarVertical, m_RectRight, m_FrameColor);
        }

        #region Accessors
        public Rectangle Rect
        {
            get { return m_Rect; }
        }

        public Color FrameColor
        {
            get { return m_FrameColor; }
            set { m_FrameColor = value; }
        }
        #endregion
    }
}
