using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class AnimatedSprite
    {
        private Texture2D[] m_Textures;
        private int m_iCurrentTexture;
        private double m_dfChangeSpeed;
        private double m_dfTimeTilTextureChange;

        public AnimatedSprite(Texture2D[] Textures, double dfChangeSpeed)
        {
            m_Textures = Textures;
            m_iCurrentTexture = 0;
            m_dfChangeSpeed = dfChangeSpeed;
            m_dfTimeTilTextureChange = m_dfChangeSpeed;
        }

        public void Update(GameTime time)
        {
            m_dfTimeTilTextureChange -= time.ElapsedGameTime.TotalSeconds;
            if (m_dfTimeTilTextureChange < 0)
            {
                m_dfTimeTilTextureChange = m_dfChangeSpeed;
                m_iCurrentTexture++;
                if (m_iCurrentTexture >= m_Textures.Length)
                    m_iCurrentTexture = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rDrawRect, Color DrawColor)
        {
            spriteBatch.Draw(m_Textures[m_iCurrentTexture], rDrawRect, DrawColor);
        }
    }
}
