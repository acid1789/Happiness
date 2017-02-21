using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    class SoundDialog
    {
        Rectangle m_Rect;
        UIFrame m_Frame;

        UIButton[] m_Buttons;

        public SoundDialog(int screenWidth, int screenHeight, Happiness game)
        {
            int width = 1200;
            int height = 700;

            int buttonWidth = 120;
            int buttonHeight = 32;
            int halfWidth = buttonWidth / 2;
            int halfHeight = buttonHeight / 2;
            int screenCenterX = screenWidth / 2;
            int screenCenterY = screenHeight / 2;

            m_Rect = new Rectangle(screenCenterX - (width / 2), screenCenterY - (height / 2), width, height);
            m_Frame = new UIFrame(2, m_Rect);

            int buttonSpace = 25;
            int buttonLeft = m_Rect.Left + 10;
            int buttonTop = m_Rect.Top + 10;
            m_Buttons = new UIButton[15];
            m_Buttons[0] = new UIButton(0, "MenuNavigate", Assets.HelpFont, new Rectangle(buttonLeft, buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[0].ClickSound = SoundManager.SEInst.MenuNavigate;
            m_Buttons[1] = new UIButton(1, "MenuAccept", Assets.HelpFont, new Rectangle(buttonLeft + (buttonWidth + buttonSpace), buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[1].ClickSound = SoundManager.SEInst.MenuAccept;
            m_Buttons[2] = new UIButton(2, "MenuCancel", Assets.HelpFont, new Rectangle(buttonLeft + ((buttonWidth + buttonSpace) * 2), buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[2].ClickSound = SoundManager.SEInst.MenuCancel;
            m_Buttons[3] = new UIButton(3, "GameLoad", Assets.HelpFont, new Rectangle(buttonLeft + ((buttonWidth + buttonSpace) * 3), buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[3].ClickSound = SoundManager.SEInst.GameLoad;
            m_Buttons[4] = new UIButton(4, "GameSave", Assets.HelpFont, new Rectangle(buttonLeft + ((buttonWidth + buttonSpace) * 4), buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[4].ClickSound = SoundManager.SEInst.GameSave;
            m_Buttons[5] = new UIButton(5, "GameUnhideClues", Assets.HelpFont, new Rectangle(buttonLeft + ((buttonWidth + buttonSpace) * 5), buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[5].ClickSound = SoundManager.SEInst.GameUnhideClues;
            m_Buttons[6] = new UIButton(6, "GameAction1", Assets.HelpFont, new Rectangle(buttonLeft + ((buttonWidth + buttonSpace) * 6), buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[6].ClickSound = SoundManager.SEInst.GameAction1;
            m_Buttons[7] = new UIButton(7, "GameAction2", Assets.HelpFont, new Rectangle(buttonLeft + ((buttonWidth + buttonSpace) * 7), buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[7].ClickSound = SoundManager.SEInst.GameAction2;

            buttonTop += buttonHeight + buttonSpace;
            m_Buttons[8] = new UIButton(8, "GameAction3", Assets.HelpFont, new Rectangle(buttonLeft, buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[8].ClickSound = SoundManager.SEInst.GameAction3;
            m_Buttons[9] = new UIButton(9, "GameAction4", Assets.HelpFont, new Rectangle(buttonLeft + (buttonWidth + buttonSpace), buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[9].ClickSound = SoundManager.SEInst.GameAction4;
            m_Buttons[10]= new UIButton(10,"GameAction5", Assets.HelpFont, new Rectangle(buttonLeft + ((buttonWidth + buttonSpace) * 2), buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[10].ClickSound = SoundManager.SEInst.GameAction5;
            m_Buttons[11]= new UIButton(11,"GameAction6", Assets.HelpFont, new Rectangle(buttonLeft + ((buttonWidth + buttonSpace) * 3), buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[11].ClickSound = SoundManager.SEInst.GameAction6;
            m_Buttons[12]= new UIButton(12,"GamePuzzleFailed", Assets.HelpFont, new Rectangle(buttonLeft + ((buttonWidth + buttonSpace) * 4), buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[12].ClickSound = SoundManager.SEInst.GamePuzzleFailed;
            m_Buttons[13]= new UIButton(13,"GamePuzzleComplete", Assets.HelpFont, new Rectangle(buttonLeft + ((buttonWidth + buttonSpace) * 5), buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[13].ClickSound = SoundManager.SEInst.GamePuzzleComplete;
            m_Buttons[14]= new UIButton(14,"GameSliderMove", Assets.HelpFont, new Rectangle(buttonLeft + ((buttonWidth + buttonSpace) * 6), buttonTop, buttonWidth, buttonHeight), Assets.ScrollBar); m_Buttons[14].ClickSound = SoundManager.SEInst.GameSliderMove;

        }

        public bool HandleClick(int x, int y)
        {
            foreach (UIButton b in m_Buttons)
            {
                if( b.Click(x, y) )
                    break;
            }
            return false;
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw frame
            sb.Draw(Assets.TransGray, m_Rect, Color.White);
            sb.Draw(Assets.TransGray, m_Rect, Color.White);
            sb.Draw(Assets.TransGray, m_Rect, Color.White);
            m_Frame.Draw(sb);

            // Draw Buttons
            foreach (UIButton b in m_Buttons)
                b.Draw(sb);
        }
    }
}
