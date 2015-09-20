using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HappinessNetwork;

namespace Happiness
{
    public class HubScene : Scene
    {
        Tower[] m_Towers;

        UILabel m_LevelLabel;
        UILabel m_Level;
        UIProgressBar m_ExpBar;

        FloorSelectDialog m_FloorSelect;

        public HubScene(Happiness game) : base(game)
        {
            InputController.IC.OnClick += IC_OnClick;
            InputController.IC.OnDragBegin += IC_OnDragBegin;
            InputController.IC.OnDrag += IC_OnDrag;

            NetworkManager nm = NetworkManager.Net;

            // Setup Towers
            int centerX = Game.ScreenWidth >> 1;
            int centerY = Game.ScreenHeight >> 1;
            int towerSize = (int)(Constants.HubScene_TowerSize * Game.ScreenHeight);
            int towerTop = (int)(Constants.HubScene_TowerAreaTop * Game.ScreenHeight);
            int leftX = centerX - towerSize - towerSize;
            int midX = centerX - (towerSize >> 1);
            int rightX = centerX + towerSize;
            m_Towers = new Tower[6];
            m_Towers[0] = new Tower(3, nm.GameData.TowerFloors[0], new Rectangle(leftX, towerTop, towerSize, towerSize), Assets.Towers[0]);
            m_Towers[1] = new Tower(4, nm.GameData.TowerFloors[1], new Rectangle(midX, towerTop, towerSize, towerSize), Assets.Towers[1]);
            m_Towers[2] = new Tower(5, nm.GameData.TowerFloors[2], new Rectangle(rightX, towerTop, towerSize, towerSize), Assets.Towers[2]);
            m_Towers[3] = new Tower(6, nm.GameData.TowerFloors[3], new Rectangle(leftX, centerY, towerSize, towerSize), Assets.Towers[3]);
            m_Towers[4] = new Tower(7, nm.GameData.TowerFloors[4], new Rectangle(midX, centerY, towerSize, towerSize), Assets.Towers[0]);
            m_Towers[5] = new Tower(8, nm.GameData.TowerFloors[5], new Rectangle(rightX, centerY, towerSize, towerSize), Assets.Towers[0]);

            // Level/Exp display
            int expBarWidth = (int)(Constants.HubScene_ExpBarWidth * Game.ScreenWidth);
            int expBarHeight = (int)(Constants.HubScene_ExpBarHeight * Game.ScreenHeight);
            int expBarLeft = (int)(Constants.HubScene_MarginLeftRight * Game.ScreenWidth);
            int levelY = (int)(Constants.HubScene_MarginTopBottom * Game.ScreenHeight);
            GameDataArgs gd = NetworkManager.Net.GameData;
            m_LevelLabel = new UILabel("Level: ", expBarLeft, levelY, Color.Goldenrod, Assets.HelpFont, UILabel.XMode.Left);
            m_Level = new UILabel(gd.Level.ToString(), expBarLeft + m_LevelLabel.Width, levelY, Color.White, Assets.HelpFont, UILabel.XMode.Left);
            m_ExpBar = new UIProgressBar(new Rectangle(expBarLeft, levelY + m_Level.Height, expBarWidth, expBarHeight));
            m_ExpBar.ProgressColor = Color.Yellow;
            m_ExpBar.Progress = (float)gd.Exp / Balance.ExpForNextLevel(gd.Level);
        }

        public override void Shutdown()
        {
            base.Shutdown();

            // Remove input handlers
            InputController.IC.OnClick -= IC_OnClick;
            InputController.IC.OnDrag -= IC_OnDrag;
            InputController.IC.OnDragBegin -= IC_OnDragBegin;
        }

        #region InputHandlers
        private void IC_OnClick(object sender, DragArgs e)
        {
            if (m_FloorSelect != null)
            {
                if( !m_FloorSelect.HandleClick(e.CurrentX, e.CurrentY) )
                    m_FloorSelect = null;
            }
            else
            {
                foreach (Tower t in m_Towers)
                {
                    if (t.Click(e.CurrentX, e.CurrentY))
                    {
                        //ActivateTower(t);
                        m_FloorSelect = new FloorSelectDialog(t.Size - 3, Game.ScreenWidth, Game.ScreenHeight, Game);
                        break;
                    }
                }
            }
        }

        private void IC_OnDrag(object sender, DragArgs e)
        {
            if( m_FloorSelect != null )
                m_FloorSelect.Drag(e);
        }

        private void IC_OnDragBegin(object sender, DragArgs e)
        {
            if( m_FloorSelect != null )
                m_FloorSelect.DragBegin(e);
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if( m_FloorSelect != null )
                m_FloorSelect.Update(gameTime);
        }
                
        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach( Tower t in m_Towers )
                t.Draw(spriteBatch);

            m_LevelLabel.Draw(spriteBatch);
            m_Level.Draw(spriteBatch);
            m_ExpBar.Draw(spriteBatch);
            
            if ( m_FloorSelect != null )
                m_FloorSelect.Draw(spriteBatch);
        }

        void ActivateTower(Tower t)
        {
            if (t.Locked)
                ShowLockedMessage();
            else
            {
                // Launch the puzzle
                GameScene gs = new GameScene(Game);
                gs.Initialize(t.Floor, t.Size, true);
                Game.GotoScene(gs);
            }
        }

        void ShowLockedMessage()
        {
        }
    }
}
