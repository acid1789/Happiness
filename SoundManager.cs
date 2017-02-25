using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Happiness
{
    public class SoundManager
    {
        enum PlayingMusic
        {
            None,
            Menu,
            Game
        }

        public enum SEInst
        {
            MenuNavigate,
            MenuAccept,
            MenuCancel,
            GameLoad,
            GameSave,
            GameUnhideClues,
            GameAction1,
            GameAction2,
            GameAction3,
            GameAction4,
            GameAction5,
            GameAction6,
            GamePuzzleFailed,
            GamePuzzleComplete,
            GameSliderMove
        }

        public float m_fSoundVolume;
        public float m_fMusicVolume;
        public bool m_bSupressGameNavigation = false;        

        PlayingMusic m_CurrentMusic;
        List<Song> m_PlayList;
        int m_CurrentSong;

        private SoundManager()
        {
            m_fSoundVolume = 1.0f;
            m_CurrentMusic = PlayingMusic.None;
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
        }

        private void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if (m_CurrentMusic == PlayingMusic.Menu)
            {
                // menu music is already looping, nothing to do here
            }
            else if (m_CurrentMusic == PlayingMusic.Game)
            {
                // Game song done, go on to another
                PlayGameSong();
            }
        }

        public void PlayMainMenuMusic()
        {
            if (m_CurrentMusic != PlayingMusic.Menu)
            {
                MediaPlayer.Play(Assets.MenuSong);
                MediaPlayer.IsRepeating = true;
                m_CurrentMusic = PlayingMusic.Menu;
            }
        }
        
        public void PlayGameMusic()
        {
            if (m_CurrentMusic != PlayingMusic.Game)
            {
                PlayGameSong();
                MediaPlayer.IsRepeating = false;
                m_CurrentMusic = PlayingMusic.Game;
            }
        }

        void PlayGameSong()
        {
            m_CurrentSong++;
            if (m_PlayList == null || m_CurrentSong >= m_PlayList.Count)
            {
                // Build a randomized play list
                List<Song> tempList = new List<Song>(Assets.GameSongs);

                Random r = new Random();
                m_PlayList = new List<Song>();
                while (tempList.Count > 0)
                {
                    int randIdx = r.Next(0, tempList.Count);
                    m_PlayList.Add(tempList[randIdx]);
                    tempList.RemoveAt(randIdx);
                }
                m_CurrentSong = 0;
            }

            Song s = m_PlayList[m_CurrentSong];
            MediaPlayer.Play(s);
        }

        public void PlaySound(SEInst se)
        {
            SoundEffectInstance inst = null;
            switch (se)
            {
                case SEInst.MenuNavigate:
                    inst = Assets.MenuNavigate.CreateInstance();
                    break;
                case SEInst.MenuAccept:
                    inst = Assets.MenuAccept.CreateInstance();
                    inst.Pitch = -0.4175f;
                    break;
                case SEInst.MenuCancel:
                    inst = Assets.MenuAccept.CreateInstance();
                    inst.Pitch = -0.78833333333333333333333333333333f;
                    break;
                case SEInst.GameLoad:
                    inst = Assets.GameLoad.CreateInstance();
                    break;
                case SEInst.GameSave:
                    inst = Assets.GameSave.CreateInstance();
                    break;
                case SEInst.GameUnhideClues:
                    inst = Assets.UnhideClues.CreateInstance();
                    break;
                case SEInst.GameAction1:
                    inst = Assets.GameAction1.CreateInstance();
                    break;
                case SEInst.GameAction2:
                    inst = Assets.GameAction2.CreateInstance();
                    break;
                case SEInst.GameAction3:
                    inst = Assets.GameAction3.CreateInstance();
                    break;
                case SEInst.GameAction4:
                    inst = Assets.GameAction4.CreateInstance();
                    break;
                case SEInst.GameAction5:
                    inst = Assets.GameAction5.CreateInstance();
                    break;
                case SEInst.GameAction6:
                    inst = Assets.GameAction6.CreateInstance();
                    break;
                case SEInst.GamePuzzleFailed:
                    inst = Assets.PuzzleFailed.CreateInstance();
                    break;
                case SEInst.GamePuzzleComplete:
                    inst = Assets.PuzzleComplete.CreateInstance();
                    break;
                case SEInst.GameSliderMove:
                    inst = Assets.SliderMove.CreateInstance();
                    break;
            }

            if (inst != null)
            {
                inst.Volume = m_fSoundVolume;
                inst.Play();
            }
        }

        public void PlayMenuNavigate()
        {
            //m_SoundBank.PlayCue("menu_navigate");
        }

        public void PlayMenuAccept()
        {
            //m_SoundBank.PlayCue("menu_accept");
        }

        public void PlayMenuCancel()
        {
            //m_SoundBank.PlayCue("menu_cancel");
        }

        public void PlaySliderMove()
        {
            //m_SoundBank.PlayCue("slider_move");
        }


        public void PlayGameLoad()
        {
            //m_SoundBank.PlayCue("game_load");
        }

        public void PlayGameSave()
        {
            //m_SoundBank.PlayCue("game_save");
        }

        public void PlayGameNavigate()
        {
            //if( !m_bSupressGameNavigation )
            //    m_SoundBank.PlayCue("menu_navigate");
        }

        public void PlayGameJump()
        {
            //m_SoundBank.PlayCue("menu_navigate");
        }

        public void PlayGameEliminateIcon()
        {
            //m_SoundBank.PlayCue("game_action6");
        }

        public void PlayGameSetFinalIcon()
        {
            //m_SoundBank.PlayCue("game_action4");
        }

        public void PlayGameRestoreIcon()
        {
            //m_SoundBank.PlayCue("game_action2");
        }

        public void PlayGameHint()
        {
            //m_SoundBank.PlayCue("game_action5");
        }

        public void PlayGameUndo()
        {
            //m_SoundBank.PlayCue("game_action3");
        }

        public void PlayGameRedo()
        {
            //m_SoundBank.PlayCue("game_action1");
        }

        public void PlayGameHideClue()
        {
            //m_SoundBank.PlayCue("game_action6");
        }

        public void PlayGameUnhideClues()
        {
            //m_SoundBank.PlayCue("unhide_clues");
        }

        public void PlayPuzzleFailed()
        {
            //m_SoundBank.PlayCue("puzzle_failed");
        }

        public void PlayPuzzleComplete()
        {
            //m_SoundBank.PlayCue("puzzle_complete");
        }

        public void Update()
        {
            //AudioCategory catMusic = m_AudioEngine.GetCategory("Music");
            //catMusic.SetVolume(m_fMusicVolume);

            //AudioCategory catSound = m_AudioEngine.GetCategory("Default");
            //catSound.SetVolume(m_fSoundVolume);

            //m_AudioEngine.Update();
        }

        static SoundManager s_SM;
        public static SoundManager Inst
        {
            get { if (s_SM == null) s_SM = new SoundManager(); return s_SM; }
        }
    }
}
