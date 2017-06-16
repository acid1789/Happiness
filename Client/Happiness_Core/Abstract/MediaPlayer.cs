using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness
{
    public abstract class MediaPlayer
    {
        protected static MediaPlayer _instance;
        public static MediaPlayer Instance { get { return _instance; } }

        public abstract void SetMediaStateChangedHandler(EventHandler<EventArgs> MediaStateChanged);

        public abstract float Volume { get; set; }
        public abstract bool IsRepeating { get; set; }

        public abstract void Play(Song s);
        public abstract void Stop();
    }
}
