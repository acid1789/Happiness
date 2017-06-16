using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness
{
    class IconButton : UIButton
    {
        Texture2D m_Icon;
        Rectangle m_IconRect;
        

        public IconButton(int buttonID, Rectangle rect, Texture2D background, Rectangle iconRect, Texture2D icon, string text, SpriteFont font)
            : base(buttonID, text, font, rect, background)
        {
            m_Icon = icon;
            m_IconRect = iconRect;
        }

        public override void Draw(Renderer sb)
        {
            // Draw the base button
            base.Draw(sb);

            // Draw the icon
            sb.Draw(m_Icon, m_IconRect, Color.White);
        }
    }
}
