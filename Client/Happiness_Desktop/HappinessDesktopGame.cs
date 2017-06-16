using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Happiness_Desktop
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class HappinessDesktopGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RasterizerState m_RasterizerState = new RasterizerState() { ScissorTestEnable = true };

        GoolgleAuth_Desktop _google;
        FacebookAuth_Desktop _facebook;
        PurchaseSystem_Desktop _purchase;
        Platform_Desktop _platform;
        InputController_XNA _input;
        FileManager_Desktop _fileManager;
        MediaPlayer_XNA _mediaPlayer;
        Renderer_XNA _renderer;
        Happiness.Happiness _theGame;

        public HappinessDesktopGame()
        {
#if DEBUG
            System.Threading.Thread.Sleep(1000);
#endif
            graphics = new GraphicsDeviceManager(this);

            _google = new GoolgleAuth_Desktop();
            _facebook = new FacebookAuth_Desktop();
            _purchase = new PurchaseSystem_Desktop();
            _platform = new Platform_Desktop() { TheGame = this };
            _input = new InputController_XNA();
            _fileManager = new FileManager_Desktop();
            _mediaPlayer = new MediaPlayer_XNA();
            _theGame = new Happiness.Happiness();
                        
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            _theGame.Initialize(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            _theGame.OnExiting();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, 1280, 720);
            _renderer = new Renderer_XNA(spriteBatch);

            _theGame.LoadContent(new ContentManager_XNA() { Content = Content});
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _theGame.Update(gameTime.ElapsedGameTime.TotalSeconds, _renderer);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, m_RasterizerState);
            _theGame.Draw(_renderer);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
