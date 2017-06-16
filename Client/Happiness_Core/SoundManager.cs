using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            GameSliderMove,
            Happiness
        }

        float m_fSoundVolume;
        float m_fMusicVolume;      

        PlayingMusic m_CurrentMusic;
        List<Song> m_PlayList;
        int m_CurrentSong;

        private SoundManager()
        {
            m_fMusicVolume = 0.1f;
            m_fSoundVolume = 0.15f;
            m_CurrentMusic = PlayingMusic.None;
            MediaPlayer.Instance.SetMediaStateChangedHandler(MediaPlayer_MediaStateChanged);
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

        public void StopMusic()
        {
            MediaPlayer.Instance.Stop();
            m_CurrentMusic = PlayingMusic.None;
        }

        public void PlayMainMenuMusic()
        {
            if (m_CurrentMusic != PlayingMusic.Menu)
            {
                MediaPlayer.Instance.Volume = m_fMusicVolume;
                MediaPlayer.Instance.Play(Assets.MenuSong);
                MediaPlayer.Instance.IsRepeating = true;
                m_CurrentMusic = PlayingMusic.Menu;
            }
        }
        
        public void PlayGameMusic()
        {
            if (m_CurrentMusic != PlayingMusic.Game)
            {
                PlayGameSong();
                MediaPlayer.Instance.IsRepeating = false;
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
            MediaPlayer.Instance.Play(s);
            MediaPlayer.Instance.Volume = m_fMusicVolume;
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
                case SEInst.Happiness:
                    inst = Assets.HappinessWav.CreateInstance();
                    break;
            }

            if (inst != null)
            {
                inst.Volume = m_fSoundVolume;
                inst.Play();
            }
        }

        public void Update()
        {
        }

        static SoundManager s_SM;
        public static SoundManager Inst
        {
            get { if (s_SM == null) s_SM = new SoundManager(); return s_SM; }
        }

        public float SoundVolume
        {
            get { return m_fSoundVolume; }
            set { m_fSoundVolume = Math.Max(0, Math.Min(value, 1.0f)); }
        }

        public float MusicVolume
        {
            get { return m_fMusicVolume; }
            set
            {
                m_fMusicVolume = Math.Max(0, Math.Min(value, 1.0f));
                MediaPlayer.Instance.Volume = m_fMusicVolume;
            }
        }
    }
}
