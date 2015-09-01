using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;


namespace Happiness
{
    public class SoundManager
    {
        //AudioEngine m_AudioEngine;
        //SoundBank m_SoundBank;
        //WaveBank m_MusicBank;
        //WaveBank m_EffectBank;

        Cue m_GameMusicCue;
        Cue m_MainMenuMusicCue;

        public float m_fSoundVolume;
        public float m_fMusicVolume;
        public bool m_bSupressGameNavigation = false;

        bool m_bLoaded = false;

        public SoundManager()
        {
            m_GameMusicCue = null;
            m_MainMenuMusicCue = null;
            m_bLoaded = false;
        }

        public void Load()
        {
            //m_AudioEngine = new AudioEngine("Content\\Audio\\Happiness.xgs");
            //m_SoundBank = new SoundBank(m_AudioEngine, "Content\\Audio\\Sound Bank.xsb");
            //m_MusicBank = new WaveBank(m_AudioEngine, "Content\\Audio\\MusicBank.xwb");
            //m_EffectBank = new WaveBank(m_AudioEngine, "Content\\Audio\\EffectBank.xwb");
            m_bLoaded = true;
        }

        public void PlayMainMenuMusic()
        {
            if( !m_bLoaded )
                return;

            if (m_GameMusicCue != null)
            {
                m_GameMusicCue.Stop(AudioStopOptions.Immediate);
                //while (!m_GameMusicCue.IsStopped)
                //    m_AudioEngine.Update();

                m_GameMusicCue = null;
            }

            //if (m_MainMenuMusicCue == null)
            //    m_MainMenuMusicCue = m_SoundBank.GetCue("MainMenuMusic");

            //if( !m_MainMenuMusicCue.IsPlaying )
            //    m_MainMenuMusicCue.Play();
        }
        
        public void PlayGameMusic()
        {            
            if( !m_bLoaded )
                return;

            if (m_MainMenuMusicCue != null)
            {
                m_MainMenuMusicCue.Stop(AudioStopOptions.Immediate);
                //while (!m_MainMenuMusicCue.IsStopped)
                //    m_AudioEngine.Update();

                m_MainMenuMusicCue = null;
            }

            //if (m_GameMusicCue == null)
            //    m_GameMusicCue = m_SoundBank.GetCue("GameMusic");                

            //if (!m_GameMusicCue.IsPlaying)
            //    m_GameMusicCue.Play();
        }

        public void PlayMenuNavigate()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("menu_navigate");
        }

        public void PlayMenuAccept()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("menu_accept");
        }

        public void PlayMenuCancel()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("menu_cancel");
        }

        public void PlaySliderMove()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("slider_move");
        }

        public void PlayGameLoad()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("game_load");
        }

        public void PlayGameSave()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("game_save");
        }

        public void PlayGameNavigate()
        {            
            if( !m_bLoaded )
                return;

            //if( !m_bSupressGameNavigation )
            //    m_SoundBank.PlayCue("menu_navigate");
        }

        public void PlayGameJump()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("menu_navigate");
        }

        public void PlayGameEliminateIcon()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("game_action6");
        }

        public void PlayGameSetFinalIcon()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("game_action4");
        }

        public void PlayGameRestoreIcon()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("game_action2");
        }

        public void PlayGameHint()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("game_action5");
        }

        public void PlayGameUndo()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("game_action3");
        }

        public void PlayGameRedo()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("game_action1");
        }

        public void PlayGameHideClue()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("game_action6");
        }

        public void PlayGameUnhideClues()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("unhide_clues");
        }

        public void PlayPuzzleFailed()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("puzzle_failed");
        }

        public void PlayPuzzleComplete()
        {            
            if( !m_bLoaded )
                return;

            //m_SoundBank.PlayCue("puzzle_complete");
        }

        public void Update()
        {            
            if( !m_bLoaded )
                return;

            //AudioCategory catMusic = m_AudioEngine.GetCategory("Music");
            //catMusic.SetVolume(m_fMusicVolume);

            //AudioCategory catSound = m_AudioEngine.GetCategory("Default");
            //catSound.SetVolume(m_fSoundVolume);

            //m_AudioEngine.Update();
        }
    }
}
