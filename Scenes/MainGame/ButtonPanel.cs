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
        UICoinsDisplay m_Coins;
        
        public ButtonPanel(GameScene game, Rectangle rect) : base(game)
        {
            m_Rect = rect;

            int mid = m_Rect.Left + (m_Rect.Width >> 1);
            int buttonSize = (int)(Constants.ButtonPanel_ButtonSize * Scene.Game.ScreenHeight);
            int buttonSpace = (int)(Constants.ButtonPanel_ButtonSpace * Scene.Game.ScreenHeight);
            int buttonsLeft = mid - (buttonSize >> 1);

            int startY = (int)(Constants.HelpPanel_Height * Scene.Game.ScreenHeight);

            m_Buttons = new UIButton[5];
            m_Buttons[0] = new UIButton(0, "P", Assets.DialogFont, new Rectangle(buttonsLeft, startY, buttonSize, buttonSize), Assets.ScrollBar);
            m_Buttons[1] = new UIButton(1, "H", Assets.DialogFont, new Rectangle(buttonsLeft, startY + buttonSize + buttonSpace, buttonSize, buttonSize), Assets.ScrollBar);
            m_Buttons[2] = new UIButton(2, "MH", Assets.DialogFont, new Rectangle(buttonsLeft, startY + ((buttonSize + buttonSpace) * 2), buttonSize, buttonSize), Assets.ScrollBar);
            m_Buttons[3] = new UIButton(3, "U", Assets.DialogFont, new Rectangle(buttonsLeft, startY + ((buttonSize + buttonSpace) * 3), buttonSize, buttonSize), Assets.ScrollBar);
            m_Buttons[4] = new UIButton(4, "HC", Assets.DialogFont, new Rectangle(buttonsLeft, startY + ((buttonSize + buttonSpace) * 4), buttonSize, buttonSize), Assets.ScrollBar);            

            m_Coins = new UICoinsDisplay(startY, m_Rect.Left, m_Rect.Top, m_Rect.Width);


            HideClueEnabled = false;
        }

        public override void Click(int x, int y)
        {
            if (m_Coins.HandleClick(x, y))
            {
                GameScene.DoBuyCoins(GameScene.MessageBoxContext.None);
            }
            else
            {
                foreach (UIButton b in m_Buttons)
                {
                    if (b.Enabled)
                    {
                        if (b.Click(x, y))
                            OnButton(b.ButtonID);
                    }
                }
            }
        }

        void OnButton(int buttonID)
        {
            switch (buttonID)
            {
                case 0: // Pause
                    GameScene.Pause();
                    break;
                case 1: // Hint
                    GameScene.DoHint();
                    break;
                case 2: // MegaHint
                    GameScene.DoMegaHint();
                    break;
                case 3: // Undo
                    GameScene.DoUndo();
                    break;
                case 4: // Hide Clue
                    GameScene.HideSelectedClue();
                    HideClueEnabled = false;
                    break;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            // Draw Buttons
            foreach( UIButton b in m_Buttons )
                b.Draw(sb);

            // Draw currency
            m_Coins.Draw(sb);            
        }

        public void SetHintCount(int current, int max)
        {
            if (max < 0)
                max = int.MaxValue;

            SetButtonCount(1, current, max);
        }

        public void SetMegaHintCount(int current, int max)
        {
            if (max < 0)
                max = int.MaxValue;

            SetButtonCount(2, current, max);
        }

        public void SetUndoCount(int current, int max)
        {
            if( max < 0 )
                max = int.MaxValue;

            SetButtonCount(3, current, max);
        }

        void SetButtonCount(int button, int current, int max)
        {
            m_Buttons[button].UnderText = max == int.MaxValue ? string.Format("~") : string.Format("{0} / {1}", current, max);
            m_Buttons[button].Enabled = (current != 0);
        }

        public void SetCoins(int coins)
        {
            m_Coins.SetCoins(coins);

        }

        #region Accessors
        public bool HintEnabled
        {
            get { return m_Buttons[1].Enabled; }
            set { m_Buttons[1].Enabled = value; }
        }

        public bool MegaHintEnabled
        {
            get { return m_Buttons[2].Enabled; }
            set { m_Buttons[2].Enabled = value; }
        }

        public bool UndoEnabled
        {
            get { return m_Buttons[3].Enabled; }
            set { m_Buttons[3].Enabled = value; }
        }

        public bool HideClueEnabled
        {
            get { return m_Buttons[4].Enabled; }
            set { m_Buttons[4].Enabled = value; }
        }

        public Rectangle HintRect
        {
            get { return m_Buttons[1].Rect; }
        }

        public Rectangle MegaHintRect
        {
            get { return m_Buttons[2].Rect; }
        }

        public Rectangle UndoRect
        {
            get { return m_Buttons[3].Rect; }
        }

        public Rectangle HideClueRect
        {
            get { return m_Buttons[4].Rect; }
        }

        public GameScene GameScene
        {
            get { return (GameScene)Scene; }
        }
        #endregion
    }
}
