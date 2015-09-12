using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class EndPuzzleScreen
    {
        public GameTime m_GameTime;
        public int m_iSelection;
        int m_iDialogLeft;
        int m_iDialogTop;
        public bool m_bSuccess;

        public EndPuzzleScreen()
        {
        }

        // Return false if this menu should close
        public bool HandleClick(int iX, int iY)
        {
            if (iX > (m_iDialogLeft + 200) && iX < (m_iDialogLeft + 400))
            {
                if (iY > (m_iDialogTop + 200))
                {
                    if (iY < (m_iDialogTop + 230))
                        m_iSelection = 0;
                    else if (iY < (m_iDialogTop + 260))
                        m_iSelection = 1;
                    else if (iY < (m_iDialogTop + 290))
                        m_iSelection = 2;
                    else
                        return true;

                    return false;
                }
            }
            return true;
        }

        public void UpdateMouse(int iX, int iY, bool bLeftButtonDown)
        {
            if (iX > (m_iDialogLeft + 200) && iX < (m_iDialogLeft + 400))
            {
                if (iY > (m_iDialogTop + 200))
                {
                    if (iY < (m_iDialogTop + 230))
                        m_iSelection = 0;
                    else if (iY < (m_iDialogTop + 260))
                        m_iSelection = 1;
                    else if (iY < (m_iDialogTop + 290))
                        m_iSelection = 2;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            int iWidth = screenWidth;
            int iHeight = screenHeight;

            m_iDialogLeft = (iWidth / 2) - 300;
            m_iDialogTop = (iHeight / 2) - 200;
            int iX = m_iDialogLeft;
            int iY = m_iDialogTop;
            spriteBatch.Draw(Assets.TransparentBox, new Rectangle(iX, iY, 600, 400), Color.DarkGray);
            spriteBatch.Draw(Assets.TransparentBox, new Rectangle(iX, iY, 600, 400), Color.DarkGray);
            spriteBatch.Draw(Assets.TransparentBox, new Rectangle(iX, iY, 600, 400), Color.DarkGray);

            iY += 50;

            string szTitle = "";
            if (m_bSuccess)
                szTitle = "Puzzle Complete!";
            else
                szTitle = "Puzzle Failed";

            int iTitleX = (iWidth / 2) - ((int)Assets.MenuFont.MeasureString(szTitle).X / 2);
            spriteBatch.DrawString(Assets.MenuFont, szTitle, new Vector2(iTitleX + 2, iY + 2), Color.Black);
            spriteBatch.DrawString(Assets.MenuFont, szTitle, new Vector2(iTitleX, iY), (m_bSuccess) ? Color.Green : Color.Red);

            iY += 150;
            iX = (iWidth / 2) - 100;

            //int iArrowWidth = 50;
            //int iArrowX = iX - iArrowWidth;
            //int iArrowY = (iY - 4) + (m_iSelection * 30);
            //spriteBatch.Draw(m_Game.m_BlueArrow, new Rectangle(iArrowX, iArrowY, iArrowWidth, 40), Color.White);

            DrawString(spriteBatch, "Next Puzzle", iX, iY, Color.Goldenrod);
            iY += 30;

            DrawString(spriteBatch, "Restart Puzzle", iX, iY, Color.Goldenrod);
            iY += 30;

            DrawString(spriteBatch, "Main Menu", iX, iY, Color.Goldenrod);            
        }

        private void DrawString(SpriteBatch spriteBatch, string text, int iX, int iY, Color cColor)
        {
            spriteBatch.DrawString(Assets.DialogFont, text, new Vector2(iX + 2, iY + 2), Color.Black);
            spriteBatch.DrawString(Assets.DialogFont, text, new Vector2(iX, iY), cColor);
        }       
    }
}
