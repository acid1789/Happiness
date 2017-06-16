using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness_Desktop
{
    class Texture2D_XNA : Happiness.Texture2D
    {
        public Texture2D XNATexture { get; set; }

        public override int Height { get { return XNATexture.Height; } }
        public override int Width { get { return XNATexture.Width; } }
    }
}
