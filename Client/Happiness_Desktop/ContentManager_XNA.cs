using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Happiness_Desktop
{
    class ContentManager_XNA : Happiness.ContentManager
    {
        delegate object LoaderFunction(string fileName);
        Dictionary<Type, LoaderFunction> HappinessToXNAMap;

        public ContentManager Content { get; set; }

        public ContentManager_XNA()
        {
            HappinessToXNAMap = new Dictionary<Type, LoaderFunction>();
            HappinessToXNAMap[typeof(Happiness.Texture2D)] = Tex2DLoader;
            HappinessToXNAMap[typeof(Happiness.SpriteFont)] = SpriteFontLoader;
            HappinessToXNAMap[typeof(Happiness.Song)] = SongLoader;
            HappinessToXNAMap[typeof(Happiness.SoundEffect)] = SoundEffectLoader;
        }


        public override T Load<T>(string fileName)
        {
            Type happinessType = typeof(T);
            LoaderFunction loader = HappinessToXNAMap[happinessType];

            return (T)loader(fileName);
        }

        Texture2D_XNA Tex2DLoader(string fileName)
        {
            Texture2D content = Content.Load<Texture2D>(fileName);
            return new Texture2D_XNA() { XNATexture = content };
        }

        SpriteFont_XNA SpriteFontLoader(string fileName)
        {
            SpriteFont content = Content.Load<SpriteFont>(fileName);
            return new SpriteFont_XNA() { XNAFont = content };
        }

        Song_XNA SongLoader(string fileName)
        {
            Song content = Content.Load<Song>(fileName);
            return new Song_XNA() { XNASong = content };
        }

        SoundEffect_XNA SoundEffectLoader(string fileName)
        {
            SoundEffect content = Content.Load<SoundEffect>(fileName);
            return new SoundEffect_XNA() { XNASoundEffect = content };
        }
    }
}
