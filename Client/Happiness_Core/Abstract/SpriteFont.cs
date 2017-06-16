using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness
{
    public abstract class SpriteFont
    {
        public abstract Vector2 MeasureString(string str);
    }
}
