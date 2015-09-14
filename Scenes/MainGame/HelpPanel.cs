using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LogicMatrix;

namespace Happiness
{
    public class HelpPanel : UIPanel
    {
        int m_iClueIconSize;
        Clue m_Clue;

        public HelpPanel(GameScene game, Rectangle rect) : base(game)
        {
            m_Rect = rect;

            m_iClueIconSize = (int)(Constants.HelpClueIconSize * game.Game.ScreenHeight);
        }

        #region Drawing
        public override void Draw(SpriteBatch sb)
        {
            if (m_Clue != null)
            {
                switch (m_Clue.m_Type)
                {
                    case eClueType.Horizontal:
                        DrawClueDescription_Horizontal(sb, m_Clue);
                        break;
                    case eClueType.Vertical:
                        DrawClueDescription_Vertical(sb, m_Clue);
                        break;
                }
            }

            DrawClock(sb);
        }

        void DrawClueDescription_Horizontal(SpriteBatch spriteBatch, Clue clue)
        {
            int iX = m_Rect.Left;
            int iY = (m_Rect.Top + (m_Rect.Height >> 1)) - (m_iClueIconSize >> 1);
            int[] iIcons = new int[3];
            int[] iRows = clue.GetRows();
            clue.GetIcons(GameScene.Puzzle, iIcons);
            string szDesc;

            switch (clue.m_HorizontalType)
            {
                case eHorizontalType.NextTo:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is next to";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eHorizontalType.NotNextTo:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is not next to";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eHorizontalType.LeftOf:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is left of";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eHorizontalType.NotLeftOf:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is not left of";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eHorizontalType.Span:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "has";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "next to it on one side, and";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[2], iIcons[2]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "next to it on the other";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    break;
                case eHorizontalType.SpanNotLeft:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "has";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[2], iIcons[2]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "next to it on one side, and not";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "next to it on the other";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    break;
                case eHorizontalType.SpanNotMid:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "and";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[2], iIcons[2]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "have one column between them without";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eHorizontalType.SpanNotRight:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "has";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "on one side, and not";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[2], iIcons[2]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "on the other";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    break;
            }
        }

        void DrawClueDescription_Vertical(SpriteBatch spriteBatch, Clue clue)
        {
            int iX = m_Rect.Left;
            int iY = (m_Rect.Top + (m_Rect.Height >> 1)) - (m_iClueIconSize >> 1);
            int[] iIcons = new int[3];
            int[] iRows = clue.GetRows();
            clue.GetIcons(GameScene.Puzzle, iIcons);
            string szDesc;

            switch (clue.m_VerticalType)
            {
                case eVerticalType.Two:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is in the same column as";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eVerticalType.Three:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is in the same column as";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "and";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[2], iIcons[2]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eVerticalType.EitherOr:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is either in the column with";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "or the column with";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[2], iIcons[2]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eVerticalType.TwoNot:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is not in the same column as";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eVerticalType.ThreeTopNot:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "and";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;

                    spriteBatch.Draw(GameScene.GetIcon(iRows[2], iIcons[2]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "are in the same column but";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;

                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "is not";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    break;
                case eVerticalType.ThreeMidNot:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "and";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;

                    spriteBatch.Draw(GameScene.GetIcon(iRows[2], iIcons[2]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "are in the same column but";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;

                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "is not";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    break;
                case eVerticalType.ThreeBotNot:
                    spriteBatch.Draw(GameScene.GetIcon(iRows[0], iIcons[0]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "and";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;

                    spriteBatch.Draw(GameScene.GetIcon(iRows[1], iIcons[1]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "are in the same column but";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    iX += (int)Assets.DialogFont.MeasureString(szDesc).X + 6;

                    spriteBatch.Draw(GameScene.GetIcon(iRows[2], iIcons[2]), new Rectangle(iX, iY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "is not";
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX + 2, iY + 2), Color.Black);
                    spriteBatch.DrawString(Assets.DialogFont, szDesc, new Vector2(iX, iY), Color.White);
                    break;
            }
        }

        private void DrawClock(SpriteBatch sb)
        {
            double seconds = GameScene.ElapsedTime;

            int hours = (int)(seconds / 3600);
            seconds -= (hours * 3600);
            int minutes = (int)(seconds / 60);
            seconds -= (minutes * 60);

            string timeString = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, (int)seconds);
            Vector2 size = Assets.DialogFont.MeasureString(timeString);
            

            sb.DrawString(Assets.DialogFont, timeString, new Vector2(m_Rect.Right - (size.X + 5), (m_Rect.Top + (m_Rect.Height >> 1)) - (size.Y * 0.5f)), Color.White);

                /*
            int iX = 910;
            int iY = (m_iScreenHeight - 50);

            string szText = "Puzzle Time:  ";
            DrawString(szText, iX, iY, Color.White);
            iX += (int)m_DialogFont.MeasureString(szText).X;

            // Days
            int iDays = (int)(((m_dfSeconds / 60.0) / 60.0) / 24.0);
            double dfRemainingSeconds = m_dfSeconds - ((((double)iDays * 24.0) * 60.0) * 60.0);
            if (iDays > 0)
            {
                string szDays = iDays.ToString();
                DrawString(szDays, iX, iY, Color.Goldenrod);
                iX += (int)m_DialogFont.MeasureString(szDays).X;

                szDays = " Days, ";
                DrawString(szDays, iX, iY, Color.White);
                iX += (int)m_DialogFont.MeasureString(szDays).X;
            }

            // Hours
            int iHours = (int)((dfRemainingSeconds / 60.0) / 60.0);
            dfRemainingSeconds = dfRemainingSeconds - (((double)iHours * 60.0) * 60.0);

            string szHours = iHours.ToString("D2") + ":";
            DrawString(szHours, iX, iY, Color.Goldenrod);
            iX += (int)m_DialogFont.MeasureString(szHours).X;

            // Minutes
            int iMinutes = (int)(dfRemainingSeconds / 60);
            dfRemainingSeconds = dfRemainingSeconds - ((double)iMinutes * 60.0);

            string szMinutes = iMinutes.ToString("D2") + ":";
            DrawString(szMinutes, iX, iY, Color.Goldenrod);
            iX += (int)m_DialogFont.MeasureString(szMinutes).X;

            // Seconds
            int iSeconds = (int)dfRemainingSeconds;

            string szSeconds = iSeconds.ToString("D2");
            DrawString(szSeconds, iX, iY, Color.Goldenrod);
            iX += (int)m_DialogFont.MeasureString(szSeconds).X;
            */
        }
        #endregion

        #region Accessors
        public Clue SelectedClue
        {
            get { return m_Clue; }
            set { m_Clue = value; }
        }

        public GameScene GameScene
        {
            get { return (GameScene)Scene; }
        }
        #endregion
    }
}
