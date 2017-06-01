using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class Options
    {
        Happiness m_Game;

        int m_iCenterDialogX;
        Rectangle m_Rect;

        UISlider m_AudioSlider;
        UISlider m_MusicSlider;
        UICheckbox m_ExpSlowdown;
        UICheckbox m_ErrorDetector;
        UICheckbox m_ErrorDetector2;
        UICheckbox m_DisableTimer;

        UIButton m_DoneButton;

        public Options(Happiness game)
        {
            m_Game = game;

            int screenWidth = game.ScreenWidth;
            int screenHeight = game.ScreenHeight;

            m_iCenterDialogX = (screenWidth >> 1);
            int centerY = (screenHeight >> 1);

            int width = (int)(Constants.OptionsDialog_Width * screenWidth);
            int height = (int)(Constants.OptionsDialog_Height * screenHeight);
            int left = m_iCenterDialogX - (width >> 1);
            int top = centerY - (height >> 1);
            m_Rect = new Rectangle(left, top, width, height);

            int bottom = top + height;
            int marginTopBottom = (int)(Constants.OptionsDialog_TopBottomMargin * screenHeight);
            int buttonWidth = (int)(Constants.OptionsDialog_ButtonWidth * screenWidth);
            int buttonHeight = (int)(Constants.OptionsDialog_ButtonHeight * screenHeight);
            int centerButtonLeft = m_iCenterDialogX - (buttonWidth >> 1);
            m_DoneButton = new UIButton(0, "Done", Assets.HelpFont, new Rectangle(centerButtonLeft, ((bottom - marginTopBottom) - buttonHeight), buttonWidth, buttonHeight), Assets.ScrollBar);

            int sliderWidth = (int)(Constants.OptionsDialog_SliderWidth * screenWidth);
            int sliderHeight = (int)(Constants.OptionsDialog_SliderHeight * screenHeight);
            int cursorWidth = (int)(Constants.OptionsDialog_SliderCursorWidth * screenWidth);
            int cursorHeight = (int)(Constants.OptionsDialog_SliderCursorHeight * screenHeight);
            int sliderLeft = m_iCenterDialogX - (sliderWidth >> 1);
            m_AudioSlider = new UISlider(SoundManager.Inst.SoundVolume, 0.0f, 1.0f, new Rectangle(sliderLeft, top + marginTopBottom, sliderWidth, sliderHeight), Assets.SliderBar, Assets.SliderCursor, new Rectangle(0, 0, cursorWidth, cursorHeight), "Sound Volume", Assets.HelpFont);
            m_AudioSlider.OnChanged += OnSoundChanged;
            m_MusicSlider = new UISlider(SoundManager.Inst.MusicVolume, 0.0f, 0.2f, new Rectangle(sliderLeft, top + marginTopBottom + (sliderHeight * 3), sliderWidth, sliderHeight), Assets.SliderBar, Assets.SliderCursor, new Rectangle(0, 0, cursorWidth, cursorHeight), "Music Volume", Assets.HelpFont);
            m_MusicSlider.OnChanged += OnMusicChanged;

            int cbY = top + (marginTopBottom * 2) + (sliderHeight * 5);
            m_ExpSlowdown = new UICheckbox("Disable Exp Bonus (VIP 2)", m_iCenterDialogX, cbY, screenHeight, UICheckbox.XMode.Center);
            m_ExpSlowdown.Checked = m_Game.ExpSlowdown;

            cbY += m_ExpSlowdown.Rect.Height + marginTopBottom;
            m_ErrorDetector = new UICheckbox("Error Detector (VIP 4)", m_iCenterDialogX, cbY, screenHeight, UICheckbox.XMode.Center);
            m_ErrorDetector.Checked = m_Game.ErrorDetector;

            cbY += m_ExpSlowdown.Rect.Height + marginTopBottom;
            m_ErrorDetector2 = new UICheckbox("Super Error Detector (VIP 8)", m_iCenterDialogX, cbY, screenHeight, UICheckbox.XMode.Center);
            m_ErrorDetector2.Checked = m_Game.ErrorDetector2;

            cbY += m_ExpSlowdown.Rect.Height + marginTopBottom;
            m_DisableTimer = new UICheckbox("Disable Timer (VIP 1)", m_iCenterDialogX, cbY, screenHeight, UICheckbox.XMode.Center);
            m_DisableTimer.Checked = m_Game.DisableTimer;


#if !DEBUG
            m_ExpSlowdown.Enabled = m_Game.m_GameInfo.VipData.Level >= 2;
            m_ErrorDetector.Enabled = m_Game.m_GameInfo.VipData.Level >= 4;
            m_ErrorDetector2.Enabled = m_Game.m_GameInfo.VipData.Level >= 8;
            m_DisableTimer.Enabled = m_Game.m_GameInfo.VipData.Level >= 1;
#endif
            if( m_Game.CurrentScene is GameScene )
                m_DisableTimer.Enabled = false;
        }

        // Return false if this menu should close
        public bool HandleClick(int iX, int iY)
        {
            m_ExpSlowdown.HandleClick(iX, iY);
            m_ErrorDetector.HandleClick(iX, iY);
            m_ErrorDetector2.HandleClick(iX, iY);
            m_DisableTimer.HandleClick(iX, iY);

            if (m_DoneButton.Click(iX, iY))
            {
                Settings s = Settings.LoadSettings();
                s.SoundVolume = SoundManager.Inst.SoundVolume;
                s.MusicVolume = SoundManager.Inst.MusicVolume;
                m_Game.ExpSlowdown = s.ExpSlowdown = m_ExpSlowdown.Enabled ? m_ExpSlowdown.Checked : false;
                m_Game.ErrorDetector = s.ErrorDetector = m_ErrorDetector.Enabled ? m_ErrorDetector.Checked : false;
                m_Game.ErrorDetector2 = s.ErrorDetector2 = m_ErrorDetector2.Enabled ? m_ErrorDetector2.Checked : false;
                m_Game.DisableTimer = s.DisableTimer = m_DisableTimer.Checked;
                s.Save();
                return false;
            }

            return true;
        }

        public void Drag(DragArgs e)
        {
            m_AudioSlider.Drag(e);
            m_MusicSlider.Drag(e);
        }

        public void Update(GameTime gt)
        {
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw frame & background
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);

            m_AudioSlider.Draw(sb);
            m_MusicSlider.Draw(sb);
            m_ExpSlowdown.Draw(sb);
            m_ErrorDetector.Draw(sb);
            m_ErrorDetector2.Draw(sb);
            m_DisableTimer.Draw(sb);

            m_DoneButton.Draw(sb);
        }

        void OnSoundChanged()
        {
            SoundManager.Inst.SoundVolume = m_AudioSlider.Value;
            SoundManager.Inst.PlaySound(SoundManager.SEInst.MenuAccept);
        }

        void OnMusicChanged()
        {
            SoundManager.Inst.MusicVolume = m_MusicSlider.Value;
        }
    }
}
