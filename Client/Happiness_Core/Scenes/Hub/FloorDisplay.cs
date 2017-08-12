using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness
{
    public class FloorDisplay
    {
        int m_iLeft;
        int m_iWidth;
        int m_iFloor;
        int m_iRank_Friends;
        int m_iRank_Global;
        double m_dfBestTime;

        bool m_bSelected;

        float m_fHeight;
        int m_iTextLineSpace;

        UILabel m_FloorDisplay;

        UILabel m_BestTimeLabel;
        UILabel m_BestTimeDisplay;
        UILabel m_FriendRankLabel;
        UILabel m_FriendRankDisplay;
        UILabel m_GlobalRankLabel;
        UILabel m_GlobalRankDisplay;


        public FloorDisplay(int left, int width, int floor, int friendRank, int globalRank, double bestTime, double parTime)
        {
            m_iLeft = left;
            m_iWidth = width;
            m_iFloor = floor;
            m_iRank_Friends = friendRank;
            m_iRank_Global = globalRank;
            m_dfBestTime = bestTime;


            int halfWidth = width >> 1;
            int quarterWidth = halfWidth >> 1;
            int textGap = (int)(Constants.FloorDisplay_TextGap * Happiness.Game.ScreenWidth);
            int textLeft = left + quarterWidth - textGap;
            int textMid = left + halfWidth;
            int textRight = textMid + textGap + quarterWidth;
            m_BestTimeLabel = new UILabel("Best Time", textLeft, 0, Color.LightYellow, Assets.HelpFont, UILabel.XMode.Center);
            m_FriendRankLabel = new UILabel("Friends Rank", textMid, 0, Color.LightYellow, Assets.HelpFont, UILabel.XMode.Center);
            m_GlobalRankLabel = new UILabel("Global Rank", textRight, 0, Color.LightYellow, Assets.HelpFont, UILabel.XMode.Center);
            
            m_BestTimeDisplay = new UILabel(m_dfBestTime == 0 ? "- -" : Happiness.TimeString(m_dfBestTime), textLeft, 0, (bestTime <= parTime) ? Color.Green : Color.Red, Assets.HelpFont, UILabel.XMode.Center);
            m_FriendRankDisplay = new UILabel(m_iRank_Friends.ToString("n0"), textMid, 0, Color.White, Assets.HelpFont, UILabel.XMode.Center);
            m_GlobalRankDisplay = new UILabel(m_iRank_Global.ToString("n0"), textRight, 0, Color.White, Assets.HelpFont, UILabel.XMode.Center);

            m_iTextLineSpace = (int)(Constants.FloorDisplay_TextLineSpace * Happiness.Game.ScreenHeight);
            m_fHeight = (Assets.HelpFont.MeasureString("qQ").Y * 2);// + m_iTextLineSpace;

            m_FloorDisplay = new UILabel(floor.ToString(), left + 4, 0, Color.Goldenrod, Assets.DialogFont, UILabel.XMode.Left);
        }

        public void Draw(Renderer sb, int y, float height)
        {
            // Background
            sb.Draw(Assets.TransGray, new Rectangle(m_iLeft, y, m_iWidth, (int)m_fHeight), m_bSelected ? Color.White : Color.Gray);

            // Floor display
            m_FloorDisplay.PositionY = (int)(y + ((m_fHeight * 0.5f) - (m_FloorDisplay.Height * 0.5f)));
            m_FloorDisplay.Draw(sb);

            sb.Draw(Assets.GoldBarHorizontal, new Rectangle(m_iLeft, y, m_iWidth, 1), Color.Black);
            sb.Draw(Assets.GoldBarHorizontal, new Rectangle(m_iLeft, y + (int)(m_fHeight - 1), m_iWidth, 1), Color.Black);

            // Draw Best time
            m_BestTimeLabel.PositionY = y;
            m_BestTimeLabel.Draw(sb);

            m_BestTimeDisplay.PositionY = y + m_iTextLineSpace;
            m_BestTimeDisplay.Draw(sb);

            // Draw Friend rank
            m_FriendRankLabel.PositionY = y;
            m_FriendRankLabel.Draw(sb);

            m_FriendRankDisplay.PositionY = y + m_iTextLineSpace;
            m_FriendRankDisplay.Draw(sb);

            // Draw Global Rank            
            m_GlobalRankLabel.PositionY = y;
            m_GlobalRankLabel.Draw(sb);

            m_GlobalRankDisplay.PositionY = y + m_iTextLineSpace;
            m_GlobalRankDisplay.Draw(sb);
        }

        #region Accessors
        public float Height
        {
            get { return m_fHeight; }
        }

        public bool Selected
        {
            get { return m_bSelected; }
            set { m_bSelected = value; }
        }

        public int Floor
        {
            get { return m_iFloor; }
        }
        #endregion
    }
}
