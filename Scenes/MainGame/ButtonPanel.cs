using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class ButtonPanel : UIPanel
    {
        UIButton[] m_Buttons;

        public ButtonPanel(GameScene game, Rectangle rect) : base(game)
        {
            m_Rect = rect;

            int mid = m_Rect.Left + (m_Rect.Width >> 1);
            int buttonSize = (int)(Constants.ButtonPanel_ButtonSize * Scene.Game.ScreenHeight);
            int buttonSpace = (int)(Constants.ButtonPanel_ButtonSpace * Scene.Game.ScreenHeight);
            int buttonsLeft = mid - (buttonSize >> 1);

            m_Buttons = new UIButton[1];
            m_Buttons[0] = new UIButton(0, "HC", Assets.DialogFont, new Rectangle(buttonsLeft, rect.Bottom - (buttonSize + buttonSpace), buttonSize, buttonSize), Assets.ScrollBar);

            HideClueEnabled = false;
        }

        public override void Click(int x, int y)
        {
            foreach (UIButton b in m_Buttons)
            {
                if (b.Enabled)
                {
                    if( b.Click(x, y) )
                        OnButton(b.ButtonID);
                }
            }
        }

        void OnButton(int buttonID)
        {
            switch (buttonID)
            {
                case 0:
                    GameScene.HideSelectedClue();
                    HideClueEnabled = false;
                    break;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            foreach( UIButton b in m_Buttons )
                b.Draw(sb);
        }

        public bool HideClueEnabled
        {
            get { return m_Buttons[0].Enabled; }
            set { m_Buttons[0].Enabled = value; }
        }

        #region Accessors
        public GameScene GameScene
        {
            get { return (GameScene)Scene; }
        }
        #endregion
    }
}
