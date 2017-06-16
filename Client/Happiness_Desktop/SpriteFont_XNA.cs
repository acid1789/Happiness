using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness_Desktop
{
    public class SpriteFont_XNA : Happiness.SpriteFont
    {
        public SpriteFont XNAFont { get; set; }
        

        public override Happiness.Vector2 MeasureString(string str)
        {
            Vector2 size = XNAFont.MeasureString(str);
            return Renderer_XNA.XNAV2ToHappinessV2(size);
        }
    }
}
