using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    class UISlider
    {
        float m_fValue;
        float m_fMin;
        float m_fMax;
        Rectangle m_BarRect;
        Texture2D m_BarTexture;
        Texture2D m_CursorTexture;
        Rectangle m_CursorRect;
        string m_szText;
        SpriteFont m_Font;
        Vector2 m_vTextPosition;
        int m_iID;
        
        public System.Action OnChanged;

        public UISlider(float value, float min, float max, Rectangle barRect, Texture2D barTex, Texture2D cursorTex, Rectangle cursorRect, string text = null, SpriteFont textFont = null, int id = -1)
        {
            m_fValue = value;
            m_fMin = min;
            m_fMax = max;
            m_BarRect = barRect;
            m_BarTexture = barTex;
            m_CursorTexture = cursorTex;
            m_CursorRect = cursorRect;
            m_szText = text;
            m_Font = textFont;
            m_iID = id;

            m_CursorRect.Y = (m_BarRect.Top + (m_BarRect.Height >> 1)) - (m_CursorRect.Height >> 1);
            if (text != null)
            {
                Vector2 size = textFont.MeasureString(text);
                m_vTextPosition = new Vector2((m_BarRect.Left + (m_BarRect.Width >> 1)) - (size.X * 0.5f), m_BarRect.Bottom);
            }
        }

        public void Drag(DragArgs e)
        {
            if (e.StartY >= m_BarRect.Top &&
                e.StartY <= m_BarRect.Bottom)
            {
                float x = e.CurrentX - m_BarRect.Left;
                if( x < 0 )
                    x = 0;
                if( x > m_BarRect.Width )
                    x = m_BarRect.Width;
                float percent = x / (float)m_BarRect.Width;
                Percentage = percent;
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            // Draw Bar
            sb.Draw(m_BarTexture, m_BarRect, Color.White);

            // Draw cursor
            float barWidth = m_BarRect.Width;
            float xdelta = m_BarRect.Left + (Percentage * barWidth);
            m_CursorRect.X = (int)xdelta - (m_CursorRect.Width >> 1);
            sb.Draw(m_CursorTexture, m_CursorRect, Color.White);

            // Draw text
            if (m_szText != null)
            {
                Happiness.ShadowString(sb, m_Font, m_szText, m_vTextPosition, Color.White);
            }
        }

        public float Percentage
        {
            get
            {
                float range = m_fMax - m_fMin;
                float val = m_fValue - m_fMin;
                return val / range;
            }
            set
            {
                float range = m_fMax - m_fMin;
                float val = range * value;
                float oldVal = m_fValue;
                m_fValue = m_fMin + val;
                if( m_fValue < m_fMin )
                    m_fValue = m_fMin;
                if( m_fValue > m_fMax )
                    m_fValue = m_fMax;
                if( oldVal != m_fValue && OnChanged != null )
                    OnChanged();
            }
        }

        public float Value
        {
            get { return m_fValue; }
            set { m_fValue = Math.Max(m_fMin, Math.Min(m_fMax, value)); }
        }
    }
}
