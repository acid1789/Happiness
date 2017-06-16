using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness
{
    class UIProgressBar
    {
        Rectangle m_Rect;
        float m_fProgress;
        float m_fMin;
        float m_fMax;

        Texture2D m_ProgressTexture;
        Texture2D m_BackgroundTexture;
        Color m_ProgressColor;
        Color m_BackgroundColor;

        public UIProgressBar(Rectangle rect)
        {
            m_Rect = rect;
            m_ProgressTexture = Assets.TransGray;
            m_BackgroundTexture = Assets.TransGray;
            m_ProgressColor = Color.LightGreen;
            m_BackgroundColor = Color.Gray;

            m_fMin = 0;
            m_fMax = 1;
            SetProgress(0);
        }

        public void Draw(Renderer sb)
        {
            // Draw background bar
            sb.Draw(m_BackgroundTexture, m_Rect, m_BackgroundColor);

            // Draw Progress
            sb.Draw(m_ProgressTexture, new Rectangle(m_Rect.Left, m_Rect.Top, (int)(m_Rect.Width * m_fProgress), m_Rect.Height), m_ProgressColor);
        }

        public void SetProgress(float value)
        {
            float range = m_fMax - m_fMin;
            float rangeVal = value - m_fMin;
            m_fProgress = Math.Abs(rangeVal / range);
        }

        #region Accessors
        public float Min
        {
            get { return m_fMin; }
            set { m_fMin = value; }
        }

        public float Max
        {
            get { return m_fMax; }
            set { m_fMax = value; }
        }

        public float Progress
        {
            get { return m_fProgress; }
            set { SetProgress(value); }
        }

        public Rectangle Rect
        {
            get { return m_Rect; }
        }

        public Color ProgressColor
        {
            get { return m_ProgressColor; }
            set { m_ProgressColor = value; }
        }

        public Color BackgroundColor
        {
            get { return m_BackgroundColor; }
            set { m_BackgroundColor = value; }
        }
        #endregion
    }
}
