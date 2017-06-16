using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;

namespace Happiness_Desktop
{
    public class MediaPlayer_XNA : Happiness.MediaPlayer
    {
        public MediaPlayer_XNA()
        {
            _instance = this;
        }

        public override bool IsRepeating { get { return MediaPlayer.IsRepeating; } set { MediaPlayer.IsRepeating = value; } }
        public override float Volume { get { return MediaPlayer.Volume; } set { MediaPlayer.Volume = value; } }

        public override void SetMediaStateChangedHandler(EventHandler<EventArgs> MediaStateChanged)
        {
            MediaPlayer.MediaStateChanged += MediaStateChanged;
        }

        public override void Play(Happiness.Song s)
        {
            Song_XNA sx = (Song_XNA)s;
            MediaPlayer.Play(sx.XNASong);
        }

        public override void Stop()
        {
            MediaPlayer.Stop();
        }
    }

    public class Song_XNA : Happiness.Song
    {
        public Song XNASong { get; set; }
    }
}
