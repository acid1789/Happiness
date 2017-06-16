using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness
{
    class UICoinsDisplay
    {
        Rectangle m_CoinClickRect;
        Rectangle m_GoldCoinRect;
        string m_szCoins;
        Vector2 m_vCoinsPosition;

        public UICoinsDisplay(int startY, int left, int top, int width)
        {
            int coinMargin = (int)(Constants.ButtonPanel_CoinMargin * Happiness.Game.ScreenHeight);
            int coinSize = (int)(Constants.ButtonPanel_CoinSize * Happiness.Game.ScreenHeight);
            m_GoldCoinRect = new Rectangle(left + coinMargin, startY - (coinSize + coinMargin), coinSize, coinSize);
            m_vCoinsPosition.X = m_GoldCoinRect.Right + coinMargin;
            m_vCoinsPosition.Y = m_GoldCoinRect.Bottom - coinSize;
            m_CoinClickRect = new Rectangle(left, top, width, startY - coinMargin);
        }

        public bool HandleClick(int x, int y)
        {
            if (m_CoinClickRect.Contains(x, y))
            {
                Happiness.Game.SoundManager.PlaySound(SoundManager.SEInst.MenuAccept);
                return true;
            }

            return false;
        }

        public void Draw(Renderer sb)
        {
            sb.Draw(Assets.GoldCoin, m_GoldCoinRect, Color.White);
            sb.DrawString(Assets.HelpFont, m_szCoins, m_vCoinsPosition, Color.Goldenrod);
        }

        public void SetCoins(int coins)
        {
            m_szCoins = coins.ToString("N0");
        }

        public int Height { get { return m_GoldCoinRect.Height; } }
    }
}
