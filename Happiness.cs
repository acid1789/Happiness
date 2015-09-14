using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LogicMatrix;

namespace Happiness
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Happiness : Microsoft.Xna.Framework.Game
    {
        public Options m_Options;
        public SoundManager m_SoundManager;        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Texture2D m_Logo;
        public Texture2D m_BlueArrow;
        public Texture2D m_GoldArrowLeft;
        public Texture2D m_GoldArrowRight;
        public Texture2D m_CheckBox;
        public Texture2D m_Check;

        public Texture2D m_HelpBackground;
        public Texture2D m_MouseImage;
        public Texture2D m_ControllerImage;
           
        public int m_iScreenWidth = 1280;
        public int m_iScreenHeight = 720;

        Scene m_CurrentScene;

        public Happiness()
        {
            //m_MainMenu = new MainMenu(this);
            //m_Options = new Options(this); 

            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = m_iScreenWidth;
            graphics.PreferredBackBufferHeight = m_iScreenHeight;            

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

            //m_CurrentScene = new GameScene(this);
            //((GameScene)m_CurrentScene).Initialize(0, 6, true);
            m_CurrentScene = new StartupScene(this);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create sprite batch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load everything
            Assets.LoadAll(Content);
        }

        /*
        private void FullLoad()
        {
            m_Logo = Content.Load<Texture2D>("Logo");
                        
            m_SoundManager.Load();
            ShowMainMenu();
        }
        */

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Call base class
            base.Update(gameTime);
            
            // Cache local width/height
            m_iScreenWidth = graphics.GraphicsDevice.Viewport.Width;
            m_iScreenHeight = graphics.GraphicsDevice.Viewport.Height;

            // Update the scene
            if ( m_CurrentScene != null )
                m_CurrentScene.Update(gameTime);
        }

        public void GotoScene(Scene nextScene)
        {
            if (m_CurrentScene != null)
            {
                m_CurrentScene.Shutdown();
                m_CurrentScene = null;
            }
            
            m_CurrentScene = nextScene;            
        }
                
        #region Drawing
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);                        

            spriteBatch.Begin(SpriteSortMode.Immediate);
            spriteBatch.Draw(Assets.Background, new Rectangle(0, 0, m_iScreenWidth, m_iScreenHeight), Color.White);
        
            if ( m_CurrentScene != null )
                m_CurrentScene.Draw(spriteBatch);    

            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        #region Acccessors
        public int ScreenWidth
        {
            get { return m_iScreenWidth; }
        }

        public int ScreenHeight
        {
            get { return m_iScreenHeight; }
        }
        #endregion
    }
}
