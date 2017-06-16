using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace Happiness_Desktop
{
    class SoundEffect_XNA : Happiness.SoundEffect
    {
        public SoundEffect XNASoundEffect { get; set; }

        public override Happiness.SoundEffectInstance CreateInstance()
        {
            return new SoundEffectInstance_XNA() { XNASEI = XNASoundEffect.CreateInstance() };
        }
    }

    class SoundEffectInstance_XNA : Happiness.SoundEffectInstance
    {
        public SoundEffectInstance XNASEI;

        public override float Pitch { get { return XNASEI.Pitch; } set { XNASEI.Pitch = value; } }
        public override float Volume { get { return XNASEI.Volume; } set { XNASEI.Volume = value; } }
        public override void Play()
        {
            XNASEI.Play();
        }
    }
}
