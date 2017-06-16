using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness
{
    public abstract class SoundEffectInstance
    {
        public abstract float Pitch { get; set; }
        public abstract float Volume { get; set; }

        public abstract void Play();
    }
}
