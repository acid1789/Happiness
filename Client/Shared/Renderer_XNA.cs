using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness_Shared
{
    class Renderer_XNA : Happiness.Renderer
    {
        SpriteBatch _sb;
        public static Vector2 FontScale;

        public Renderer_XNA(SpriteBatch sb)
        {
            _sb = sb;
            float ws = _sb.GraphicsDevice.Viewport.Width / 1280f;
            float hs = _sb.GraphicsDevice.Viewport.Height / 720f;
            FontScale = new Vector2(ws, hs);
        }

        public override Happiness.Rectangle ScissorRectangle
        {
            get { return XNARectToHappinessRect(_sb.GraphicsDevice.ScissorRectangle); }
            set { _sb.GraphicsDevice.ScissorRectangle = HappinessRectToXNARect(value); }
        }

        public override int ViewportWidth { get { return _sb.GraphicsDevice.Viewport.Width; } }
        public override int ViewportHeight { get { return _sb.GraphicsDevice.Viewport.Height; } }        

        public override void Draw(Happiness.Texture2D tex, Happiness.Rectangle rect, Happiness.Color color)
        {
            Texture2D_XNA t = (Texture2D_XNA)tex;
            _sb.Draw(t.XNATexture, HappinessRectToXNARect(rect), HappinessColorToXNAColor(color));
        }

        public override void Draw(Happiness.Texture2D texture, Happiness.Rectangle destinationRectangle, Happiness.Rectangle? sourceRectangle, Happiness.Color color, float rotation, Happiness.Vector2 origin, Happiness.SpriteEffects effects, float layerDepth)
        {
            Texture2D_XNA t = (Texture2D_XNA)texture;
            Rectangle? srcRect = (sourceRectangle.HasValue ? HappinessRectToXNARect(sourceRectangle.Value) : (Rectangle?)null);
            _sb.Draw(t.XNATexture, HappinessRectToXNARect(destinationRectangle), srcRect, HappinessColorToXNAColor(color), rotation, HappinessV2ToXNAV2(origin), (SpriteEffects)effects, layerDepth);
        }

        public override void Draw(Happiness.Texture2D texture, Happiness.Vector2? position = default(Happiness.Vector2?), Happiness.Rectangle? destinationRectangle = default(Happiness.Rectangle?), Happiness.Rectangle? sourceRectangle = default(Happiness.Rectangle?), Happiness.Vector2? origin = default(Happiness.Vector2?), float rotation = 0, Happiness.Vector2? scale = default(Happiness.Vector2?), Happiness.Color? color = default(Happiness.Color?), Happiness.SpriteEffects effects = Happiness.SpriteEffects.None, float layerDepth = 0)
        {
            Texture2D_XNA t = (Texture2D_XNA)texture;
            Vector2? positionV2 = (position.HasValue ? HappinessV2ToXNAV2(position.Value) : (Vector2?)null);
            Rectangle? dstRect = (destinationRectangle.HasValue ? HappinessRectToXNARect(destinationRectangle.Value) : (Rectangle?)null);
            Rectangle? srcRect = (sourceRectangle.HasValue ? HappinessRectToXNARect(sourceRectangle.Value) : (Rectangle?)null);
            Vector2? originV2 = (origin.HasValue ? HappinessV2ToXNAV2(origin.Value) : (Vector2?)null);
            Vector2? scaleV2 = (scale.HasValue ? HappinessV2ToXNAV2(scale.Value) : (Vector2?)null);

            _sb.Draw(t.XNATexture, positionV2, dstRect, srcRect, originV2, rotation, scaleV2, HappinessColorToXNAColor(color.Value), (SpriteEffects)effects, layerDepth);
        }

        public override void DrawString(Happiness.SpriteFont font, string text, Happiness.Vector2 position, Happiness.Color color)
        {
            SpriteFont_XNA f = (SpriteFont_XNA)font;
            _sb.DrawString(f.XNAFont, text, HappinessV2ToXNAV2(position), HappinessColorToXNAColor(color), 0, Vector2.Zero, FontScale.Y, SpriteEffects.None, 0);
        }

        public static Happiness.Rectangle XNARectToHappinessRect(Rectangle r)
        {
            return new Happiness.Rectangle(r.X, r.Y, r.Width, r.Height);
        }

        public static Rectangle HappinessRectToXNARect(Happiness.Rectangle r)
        {
            return new Rectangle(r.X, r.Y, r.Width, r.Height);
        }

        public static Happiness.Vector2 XNAV2ToHappinessV2(Vector2 v)
        {
            return new Happiness.Vector2(v.X, v.Y);
        }

        public static Vector2 HappinessV2ToXNAV2(Happiness.Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Color HappinessColorToXNAColor(Happiness.Color c)
        {
            switch (c)
            {
                case Happiness.Color.White: return Color.White;
                case Happiness.Color.Black: return Color.Black;
                case Happiness.Color.Gray: return Color.Gray;
                case Happiness.Color.LightGray: return Color.LightGray;
                case Happiness.Color.DarkGray: return Color.DarkGray;
                case Happiness.Color.Goldenrod: return Color.Goldenrod;
                case Happiness.Color.SteelBlue: return Color.SteelBlue;
                case Happiness.Color.Yellow: return Color.Yellow;
                case Happiness.Color.Green: return Color.Green;
                case Happiness.Color.Red: return Color.Red;
                case Happiness.Color.Red_A0625: return new Color(Color.Red, 0.0625f);
                case Happiness.Color.GreenYellow: return Color.GreenYellow;
                case Happiness.Color.LightGreen: return Color.LightGreen;
                case Happiness.Color.Bisque: return Color.Bisque;
                case Happiness.Color.CornflowerBlue: return Color.CornflowerBlue;
                case Happiness.Color.LightYellow: return Color.LightYellow;
                case Happiness.Color.DarkBlue: return Color.DarkBlue;
                case Happiness.Color.WhiteSmoke: return Color.WhiteSmoke;
            }
            return Color.White;
        }
    }
}
