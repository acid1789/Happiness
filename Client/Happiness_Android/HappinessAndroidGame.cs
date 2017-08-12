using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Happiness_Shared;
using System;

namespace Happiness_Android
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class HappinessAndroidGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RasterizerState m_RasterizerState = new RasterizerState() { ScissorTestEnable = true };
        
        GoogleAuth_Android _google;
        FacebookAuth_Android _facebook;
        PurchaseSystem_Android _purchase;
        FileManager_Android _fileManager;
        Platform_Android _platform;
        InputController_XNA _input;
        MediaPlayer_XNA _mediaPlayer;
        Renderer_XNA _renderer;
        Happiness.Happiness _theGame;

        public HappinessAndroidGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Console.WriteLine("HappinessAndroidGame");
            
            _google = new GoogleAuth_Android();
            _facebook = new FacebookAuth_Android();
            _purchase = new PurchaseSystem_Android();
            _platform = new Platform_Android() { TheGame = this };
            _input = new InputController_XNA();
            _fileManager = new FileManager_Android();
            _mediaPlayer = new MediaPlayer_XNA();
            _theGame = new Happiness.Happiness();

            graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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
            spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height);
            _renderer = new Renderer_XNA(spriteBatch);

            _theGame.LoadContent(new ContentManager_XNA() { Content = Content });
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
            _theGame.Update(gameTime.ElapsedGameTime.TotalSeconds, _renderer);
            base.Update(gameTime);
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
