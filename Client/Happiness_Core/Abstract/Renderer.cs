using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness
{
    public abstract class Renderer
    {
        public abstract Rectangle ScissorRectangle { get; set; }
        public abstract int ViewportWidth { get; }
        public abstract int ViewportHeight { get; }

        public abstract void Draw(Texture2D tex, Rectangle rect, Color color);
        public abstract void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
        public abstract void Draw(Texture2D texture, Vector2? position = default(Vector2?), Rectangle? destinationRectangle = default(Rectangle?), Rectangle? sourceRectangle = default(Rectangle?), Vector2? origin = default(Vector2?), float rotation = 0, Vector2? scale = default(Vector2?), Color? color = default(Color?), SpriteEffects effects = SpriteEffects.None, float layerDepth = 0);
        public abstract void DrawString(SpriteFont font, string text, Vector2 position, Color color);
    }
}
