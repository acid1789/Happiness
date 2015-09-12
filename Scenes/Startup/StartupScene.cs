using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class StartupScene : Scene
    {
        public StartupScene(Happiness game) : base(game)
        {
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Load local config

            // Get Credentials

            // Connect
            NetworkManager nm = NetworkManager.Net;
            if (nm.Connect("127.0.0.1", 1255))
            {
                // Sign In
                nm.SignIn("ron", "test", "ronTest");

                // Wait for sign in
                while (nm.SignInState == NetworkManager.SignInStatus.CredentialsSent)
                {
                    Thread.Sleep(100);
                }

                if (nm.SignInState == NetworkManager.SignInStatus.SignedIn)
                {
                    // Fetch game data
                    nm.FetchGameData();

                    while( nm.GameData == null )
                        Thread.Sleep(100);
                    
                    // Goto Hub
                    Game.GotoScene(new HubScene(Game));
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
        }
    }
}
