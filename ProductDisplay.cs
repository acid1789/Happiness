using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class ProductDisplay
    {
        int m_iProductID;
        int m_iLeft;
        int m_iWidth;
        float m_fHeight;
        float m_fCoinsHeight;

        bool m_bSelected;

        UIFrame m_Frame;
        Rectangle m_rIcon;

        UILabel m_Coins;
        UILabel m_CoinsLabel;
        UILabel m_VIP;
        UILabel m_VIPLabel;
        
        UIButton m_Purchase;

        public ProductDisplay(GlobalProduct p, int left, int width)
        {
            m_iProductID = p.ProductId;
            m_iLeft = left;
            m_iWidth = width;

            m_fHeight = (Assets.HelpFont.MeasureString("qQ").Y * 4);
            m_fCoinsHeight = (Assets.DialogFont.MeasureString("qQ").Y * 0.5f);
            float center = m_fHeight * 0.5f;

            int iconSize = (int)(Constants.ProductDisplay_CoinSize * Happiness.Game.ScreenWidth);
            int iconLeft = (int)(Constants.ProductDisplay_CoinLeft * Happiness.Game.ScreenWidth);
            m_rIcon = new Rectangle(m_iLeft + iconLeft, (int)(center - (iconSize >> 1)), iconSize, iconSize);

            int coinsLeft = m_rIcon.Right + iconLeft;
            m_Coins = new UILabel(p.Coins.ToString(), coinsLeft, 0, Color.Goldenrod, Assets.DialogFont, UILabel.XMode.Left);
            m_CoinsLabel = new UILabel("Coins", 0, 0, Color.White, Assets.HelpFont, UILabel.XMode.Left);

            m_VIP = new UILabel(p.VIP.ToString(), m_iLeft + (width >> 1), 0, Color.Bisque, Assets.DialogFont, UILabel.XMode.Left);
            m_VIPLabel = new UILabel("VIP Points", 0, 0, Color.White, Assets.HelpFont, UILabel.XMode.Left);

            int right = left + width;
            int buttonWidth = (int)(Constants.ProductDisplay_ButtonWidth * Happiness.Game.ScreenWidth);
            int buttonHeight = (int)(Constants.ProductDisplay_ButtonHeight * Happiness.Game.ScreenHeight);
            m_Purchase = new UIButton(0, "$" + p.USD.ToString("N2"), Assets.HelpFont, new Rectangle(right - iconLeft - buttonWidth, (int)center - (buttonHeight >> 1), buttonWidth, buttonHeight), Assets.ScrollBar);
        }

        public void Draw(SpriteBatch sb, int y)
        {
            float center = (m_fHeight * 0.5f);

            // Background
            sb.Draw(Assets.TransGray, new Rectangle(m_iLeft, y, m_iWidth, (int)m_fHeight), m_bSelected ? Color.White : Color.Gray);

            sb.Draw(Assets.GoldBarHorizontal, new Rectangle(m_iLeft, y, m_iWidth, 1), Color.Black);
            sb.Draw(Assets.GoldBarHorizontal, new Rectangle(m_iLeft, y + (int)(m_fHeight - 1), m_iWidth, 1), Color.Black);

            int frameLeft = (int)(Constants.ProductDisplay_FrameLeft * Happiness.Game.ScreenWidth);
            int frameWidth = m_iWidth - (frameLeft * 2);
            int frameHeight = (int)(m_fHeight / 1.5f);
            m_Frame = new UIFrame(3, new Rectangle(m_iLeft + frameLeft, y + (int)(center - (frameHeight >> 1)), frameWidth, frameHeight));
            m_Frame.Draw(sb);

            // Gold Icon
            sb.Draw(Assets.GoldCoin, new Rectangle(m_rIcon.Left, m_rIcon.Top + y, m_rIcon.Width, m_rIcon.Height), Color.White);

            m_Coins.PositionY = (int)((y + center) - m_fCoinsHeight);
            m_Coins.Draw(sb);

            m_CoinsLabel.PositionX = m_Coins.PositionX + m_Coins.Width + 5;
            m_CoinsLabel.PositionY = (m_Coins.PositionY + m_Coins.Height) - (m_CoinsLabel.Height + 2);
            m_CoinsLabel.Draw(sb);

            m_VIP.PositionY = (int)((y + center) - m_fCoinsHeight);
            m_VIP.Draw(sb);

            m_VIPLabel.PositionX = m_VIP.PositionX + m_VIP.Width + 5;
            m_VIPLabel.PositionY = (m_VIP.PositionY + m_VIP.Height) - (m_VIPLabel.Height + 2);
            m_VIPLabel.Draw(sb);
            
            int buttonHeight = (int)(Constants.ProductDisplay_ButtonHeight * Happiness.Game.ScreenHeight);
            m_Purchase.SetY((y + (int)center) - (buttonHeight >> 1));
            m_Purchase.Draw(sb);
        }

        #region Accessors
        public float Height
        {
            get { return m_fHeight; }
        }

        public int Width { get { return m_iWidth; } }

        public bool Selected
        {
            get { return m_bSelected; }
            set { m_bSelected = value; }
        }

        public UIButton Button
        {
            get { return m_Purchase; }
        }

        public int ProductID { get { return m_iProductID; } }
        #endregion
    }
}
