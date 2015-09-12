using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class HubScene : Scene
    {
        Tower[] m_Towers;

        public HubScene(Happiness game) : base(game)
        {
            InputController.IC.OnClick += IC_OnClick;

            NetworkManager nm = NetworkManager.Net;

            // Setup Towers
            int towerSize = 100;
            m_Towers = new Tower[4];
            m_Towers[0] = new Tower(3, nm.GameData.TowerFloors[0], new Rectangle(200, 200, towerSize, towerSize), Assets.Towers[0]);
            m_Towers[1] = new Tower(4, nm.GameData.TowerFloors[1], new Rectangle(500, 200, towerSize, towerSize), Assets.Towers[1]);
            m_Towers[2] = new Tower(5, nm.GameData.TowerFloors[2], new Rectangle(200, 500, towerSize, towerSize), Assets.Towers[2]);
            m_Towers[3] = new Tower(6, nm.GameData.TowerFloors[3], new Rectangle(500, 500, towerSize, towerSize), Assets.Towers[3]);
        }

        public override void Shutdown()
        {
            base.Shutdown();

            // Remove input handlers
            InputController.IC.OnClick -= IC_OnClick;
        }

        private void IC_OnClick(object sender, DragArgs e)
        {
            foreach (Tower t in m_Towers)
            {
                if (t.Click(e.CurrentX, e.CurrentY))
                {
                    ActivateTower(t);
                    break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
                
        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach( Tower t in m_Towers )
                t.Draw(spriteBatch);
        }

        void ActivateTower(Tower t)
        {
            if (t.Locked)
                ShowLockedMessage();
            else
            {
                // Launch the puzzle
                GameScene gs = new GameScene(Game);
                gs.Initialize(t.Floor, t.Size);
                Game.GotoScene(gs);
            }
        }

        void ShowLockedMessage()
        {
        }
    }
}
