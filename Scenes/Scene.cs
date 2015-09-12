using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public abstract class Scene
    {
        Happiness m_Game;

        public Scene(Happiness hgame)
        {
            m_Game = hgame;
        }

        public virtual void Shutdown()  {  }

        public virtual void Update(GameTime gameTime)
        {
            Assets.HintSprite.Update(gameTime);
            InputController.Update(gameTime);
        }

        public abstract void Draw(SpriteBatch spriteBatch);

        public Happiness Game
        {
            get { return m_Game; }
        }
    }
}
