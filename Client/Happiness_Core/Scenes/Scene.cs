using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public virtual void Update(double deltaTime)
        {
            Assets.WaitIcon.Update(deltaTime);
            Assets.HintSprite.Update(deltaTime);
            InputController.Update(deltaTime);
        }

        public abstract void Draw(Renderer spriteBatch);

        public Happiness Game
        {
            get { return m_Game; }
        }
    }
}
