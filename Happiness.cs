using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HappinessNetwork;

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
        RasterizerState m_RasterizerState = new RasterizerState() { ScissorTestEnable = true };

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
        TutorialSystem m_Tutorial;
        public GameInfo m_GameInfo;
        ServerWriter m_ServerWriter;

        public Happiness()
        {
#if DEBUG
            System.Threading.Thread.Sleep(1000);
#endif

            m_SoundManager = SoundManager.Inst;
            Settings s = Settings.LoadSettings();
            m_SoundManager.SoundVolume = s.SoundVolume;
            m_SoundManager.MusicVolume = s.MusicVolume;

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
            m_ServerWriter = new ServerWriter();

            InputController.IC.OnClick += IC_OnClick;

            m_iScreenWidth = graphics.GraphicsDevice.Viewport.Width;
            m_iScreenHeight = graphics.GraphicsDevice.Viewport.Height;
            m_Tutorial = new TutorialSystem(m_iScreenWidth, m_iScreenHeight, this);            

            //m_CurrentScene = new GameScene(this);
            //((GameScene)m_CurrentScene).Initialize(0, 6, true);
            m_CurrentScene = new StartupScene(this);
        }

        public void ExitGame()
        {
            Exit();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            m_ServerWriter.Shutdown();
            NetworkManager.Net.Shutdown();
            base.OnExiting(sender, args);
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
        
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void IC_OnClick(object sender, DragArgs e)
        {
            e.Abort = m_Tutorial.HandleClick(e.CurrentX, e.CurrentY);
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

            // Update tutorial
            m_Tutorial.Update(gameTime);

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

        public void SaveTutorialData(ulong tutorialData)
        {
            // Store local copy
            m_GameInfo.GameData.Tutorial = tutorialData;

            // Send to the server
            m_ServerWriter.SaveTutorialData(tutorialData, m_GameInfo.AuthString, DateTime.Now);
        }

        public void SavePuzzleData(int tower, int floor, double elapsedTime, ServerWriter.JobCompleteDelegate jobCompleteCB)
        {
            bool updateExistingFloor = false;
            foreach (TowerFloorRecord tfr in m_GameInfo.TowerData[tower].Floors)
            {
                if (tfr.Floor == floor)
                {
                    if( tfr.BestTime > elapsedTime )
                        tfr.BestTime = (int)elapsedTime;
                    updateExistingFloor = true;
                    break;
                }
            }
            if (!updateExistingFloor)
            {
                // Floor isnt in the local list, add it now
                List<TowerFloorRecord> records = new List<TowerFloorRecord>(m_GameInfo.TowerData[tower].Floors);
                TowerFloorRecord tfr = new TowerFloorRecord();
                tfr.Floor = floor;
                tfr.BestTime = (int)elapsedTime;
                records.Add(tfr);
                m_GameInfo.TowerData[tower].Floors = records.ToArray();
            }

            m_ServerWriter.SavePuzzleData(m_GameInfo, tower, floor, elapsedTime, jobCompleteCB);
        }

        public void SpendCoins(int coinCount, int spentOn)
        {
            m_ServerWriter.SpendCoins(m_GameInfo.AuthString, coinCount, spentOn, m_GameInfo);
        }


        #region Drawing
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);                        

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, m_RasterizerState);
            spriteBatch.Draw(Assets.Background, new Rectangle(0, 0, m_iScreenWidth, m_iScreenHeight), Color.White);
        
            if ( m_CurrentScene != null )
                m_CurrentScene.Draw(spriteBatch);

            m_Tutorial.Draw(spriteBatch);

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

        public Scene CurrentScene
        {
            get { return m_CurrentScene; }
        }
        
        public static Happiness Game;
        public TutorialSystem Tutorial
        {
            get { return m_Tutorial; }
        }

        public SoundManager SoundManager
        {
            get { return m_SoundManager; }
        }
        #endregion




        public static void ShadowString(SpriteBatch sb, SpriteFont font, string text, Vector2 position, Color color)
        {
            sb.DrawString(font, text, new Vector2(position.X + 2, position.Y + 2), Color.Black);
            sb.DrawString(font, text, position, color);
        }

        public static Vector2 CenterText(Point center, string text, SpriteFont font)
        {
            Vector2 size = font.MeasureString(text);
            return new Vector2(center.X - (size.X / 2), center.Y - (size.Y / 2));
        }

        public static string TimeString(double seconds)
        {
            int hours = (int)(seconds / 3600);
            seconds -= (hours * 3600);
            int minutes = (int)(seconds / 60);
            seconds -= (minutes * 60);

            string timeString = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, (int)seconds);
            return timeString;
        }

        public static FormattedLine[] FormatLines(int width, string message, SpriteFont font)
        {
            List<FormattedLine> lines = new List<FormattedLine>();

            // Do all requested line breaks
            int start = 0;
            int idx = message.IndexOf('\n');
            while (idx >= 0)
            {
                string line = message.Substring(start, idx - start);
                start = idx + 1;
                lines.Add(new FormattedLine(line));

                idx = message.IndexOf('\n', start);
            }
            lines.Add(new FormattedLine(message.Substring(start, message.Length - start)));

            // Now wrap all lines that are to long
            FormattedLine[] longLines = lines.ToArray();
            lines = new List<FormattedLine>();
            foreach (FormattedLine line in longLines)
            {
                Vector2 size = line.Size(font);//font.MeasureString(line);
                if (size.X > width)
                {
                    // this line is to long, wrap it
                    FormattedLine.PieceInfo[] words = line.Split();
                    FormattedLine smallLine = new FormattedLine();
                    foreach (FormattedLine.PieceInfo word in words)
                    {
                        FormattedLine testLine = smallLine + word;
                        size = testLine.Size(font);//font.MeasureString(testLine);
                        if (size.X > width)
                        {
                            // Adding this word doesnt fit, add a line break right before this word
                            smallLine.Merge();
                            lines.Add(smallLine);
                            smallLine = new FormattedLine();
                            smallLine.Add(word);
                        }
                        else
                        {
                            // This line still fits, keep going
                            smallLine = testLine;
                        }
                    }
                    smallLine.Merge();
                    lines.Add(smallLine);
                }
                else
                {
                    // this line fits, just add it
                    lines.Add(line);
                }
            }

            return lines.ToArray();
        }
    }
}
