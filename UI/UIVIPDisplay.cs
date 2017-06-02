using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    class UIVIPDisplay
    {
        UILabel m_VipLabel;
        UILabel m_VipLevel;
        Rectangle m_ClickArea;

        public UIVIPDisplay(int x, int y, int width)
        {
            m_VipLabel = new UILabel("VIP: ", x, y, Color.Goldenrod, Assets.HelpFont, UILabel.XMode.Left);
            
            int levelX = x + m_VipLabel.Width;
            m_VipLevel = new UILabel(Happiness.Game.TheGameInfo.VipData.Level.ToString(), levelX, y, Color.White, Assets.DialogFont, UILabel.XMode.Left);


            int labelY = (y + m_VipLevel.Height) - (m_VipLabel.Height + 2);
            m_VipLabel.PositionY = labelY;

            m_ClickArea = new Rectangle(x, y, width, m_VipLevel.Height);
        }

        public bool HandleClick(int x, int y)
        {
            if (m_ClickArea.Contains(x, y))
            {
                Happiness.Game.SoundManager.PlaySound(SoundManager.SEInst.MenuAccept);
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch sb)
        {
            m_VipLabel.Draw(sb);
            m_VipLevel.Draw(sb);
        }
    }
}
