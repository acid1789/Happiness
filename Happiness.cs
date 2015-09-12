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
            //((GameScene)m_CurrentScene).Initialize(0, 4);
            m_CurrentScene = new StartupScene(this);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            /*
            // Init the save game
#if !XBOX
            m_SaveGame = new SaveGame();
            m_SaveGame.LoadOptions(out m_bAutoArangeClues, out m_bShowClueDescriptions, out m_bShowClock, out m_bShowPuzzleNumber, out m_bRandomizeIcons, out m_fTempSoundVol, out m_fTempMusicVol);
#endif
            */

            // Create sprite batch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load everything
            Assets.LoadAll(Content);
        }

        /*
        private void FullLoad()
        {
            m_Logo = Content.Load<Texture2D>("Logo");
            m_BlueArrow = Content.Load<Texture2D>("BlueArrow");
            m_GoldArrowLeft = Content.Load<Texture2D>("GoldArrowLeft");
            m_GoldArrowRight = Content.Load<Texture2D>("GoldArrowRight");
            m_HelpBackground = Content.Load<Texture2D>("HelpBackground");
            m_MouseImage = Content.Load<Texture2D>("Mouse");
            m_ControllerImage = Content.Load<Texture2D>("Controller");
            
            m_CheckBox = Content.Load<Texture2D>("CheckBox");
            m_Check = Content.Load<Texture2D>("Check");
            
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

        /*
        public void Pause()
        {
            if (!m_bMainMenu && !m_bPauseMenu && !m_bOptionsDialog)
            {
                m_PauseMenu.Init();
                m_PauseMenu.m_iPuzzleNumber = GetDisplayPuzzleNumber();
                m_bPauseMenu = true;
                m_SoundManager.PlayMenuAccept();
            }
        }
        */
        /*
        public void SavePuzzle()
        {
            if (!m_bMainMenu && !m_bPauseMenu && !m_bOptionsDialog && m_SaveGame != null)
            {
                m_SaveGame.SavePuzzle(m_Puzzle);
                m_SoundManager.PlayGameSave();
            }
        }
        */
        /*        
        public void ShowHint()
        {
            if (m_Hint == null)
            {
                int iVisibleClues = m_aVisibleVerticalClues.Count + m_aVisibleHorizontalClues.Count;
                Clue[] visibleClues = new Clue[iVisibleClues];
                int iClueIndex = 0;
                for (int i = 0; i < m_aVisibleVerticalClues.Count; i++)
                    visibleClues[iClueIndex++] = (Clue)m_aVisibleVerticalClues[i];
                for (int i = 0; i < m_aVisibleHorizontalClues.Count; i++)
                    visibleClues[iClueIndex++] = (Clue)m_aVisibleHorizontalClues[i];
                m_Hint = m_Puzzle.GenerateHint(visibleClues);
            }
            else
            {
                HideHint(true);
            }
            m_SoundManager.PlayGameHint();
        }
        
        public void HideHint(bool bForceHidden)
        {
            if( m_Hint != null && (bForceHidden || m_Hint.ShouldHide(m_Puzzle) ) )
            {
                m_Hint = null;
            }
        }
        */

        /*
        public void Undo()
        {
            if (m_aHistory.Count > 0)
            {
                Action a = (Action)m_aHistory[m_aHistory.Count - 1];
                m_aHistory.Remove(a);

                a.Revert(m_Puzzle);

                m_aFuture.Add(a);

                m_SoundManager.PlayGameUndo();
            }
        }

        public void Redo()
        {
            if (m_aFuture.Count > 0)
            {
                Action a = (Action)m_aFuture[m_aFuture.Count - 1];
                m_aFuture.Remove(a);

                a.Perform(m_Puzzle);

                m_aHistory.Add(a);

                m_SoundManager.PlayGameRedo();
            }
        }
        */
        
        /*
        private void HandleMainMenuExit()
        {
            switch (m_MainMenu.m_iSelection)
            {
                case 0:         // New Game
                    m_iSize = m_MainMenu.m_iSize;
                    m_iDifficulty = m_MainMenu.m_iDifficulty;
                    SetPuzzleNumber(m_MainMenu.m_iPuzzleNumber);
                    InitPuzzle(true);
                    m_bMainMenu = false;
                    break;
                case 1:         // Load Game
                    if (m_SaveGame != null)
                    {
                        if (m_SaveGame.LoadPuzzle(out m_Puzzle, out m_iDifficulty, out m_iPuzzleIndex))
                        {
                            m_SoundManager.PlayGameLoad();
                            m_iSize = m_Puzzle.m_iSize;
                            InitPuzzle(false);
                            m_bMainMenu = false;
                        }
                    }
                    break;
                case 3:         // Exit
                    this.Exit();
                    break;
            }
        }
        */

        /*
        private void HandlePauseMenuExit()
        {
            m_bPauseMenu = false;
            switch (m_PauseMenu.m_iSelection)
            {
                case 1: // Reset Puzzle
                    m_Puzzle.Reset();
                    UnHideAllClues();
                    break;
                case 2: // Goto Puzzle
                    SetPuzzleNumber(m_PauseMenu.m_iPuzzleNumber);
                    InitPuzzle(true);
                    break;
                case 5: // Save Game
                    SavePuzzle();
                    break;
                case 6: // Load Game
                    if (m_SaveGame != null)
                    {
                        if (m_SaveGame.LoadPuzzle(out m_Puzzle, out m_iDifficulty, out m_iPuzzleIndex))
                        {
                            m_SoundManager.PlayGameLoad();
                            m_iSize = m_Puzzle.m_iSize;
                            InitPuzzle(false);
                        }
                    }
                    break;
                case 7: // Save & Exit
                    SavePuzzle();
                    ShowMainMenu();
                    break;
                case 8: // Exit
                    ShowMainMenu();
                    break;
            }
        }
        */

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
        
        /*
        private void DrawPuzzleNumber()
        {
            string szText = "Puzzle Number:  ";
            int iLength = (int)m_DialogFont.MeasureString(szText).X;
            
            string szPuzzleNumber = GetDisplayPuzzleNumber().ToString();

            int iY = (m_iScreenHeight - 70);
            DrawString(szText, 910, iY, Color.White);
            DrawString(szPuzzleNumber, 910 + iLength, iY, Color.Goldenrod);
        }

        private void DrawClock()
        {
            int iX = 910;
            int iY = (m_iScreenHeight - 50);

            string szText = "Puzzle Time:  ";
            DrawString(szText, iX, iY, Color.White);
            iX += (int)m_DialogFont.MeasureString(szText).X;

            // Days
            int iDays = (int)(((m_dfSeconds / 60.0) / 60.0) / 24.0);
            double dfRemainingSeconds = m_dfSeconds - ((((double)iDays * 24.0) * 60.0) * 60.0);
            if (iDays > 0)
            {
                string szDays = iDays.ToString();
                DrawString(szDays, iX, iY, Color.Goldenrod);
                iX += (int)m_DialogFont.MeasureString(szDays).X;

                szDays = " Days, ";
                DrawString(szDays, iX, iY, Color.White);
                iX += (int)m_DialogFont.MeasureString(szDays).X;
            }

            // Hours
            int iHours = (int)((dfRemainingSeconds / 60.0) / 60.0);
            dfRemainingSeconds = dfRemainingSeconds - (((double)iHours * 60.0) * 60.0);

            string szHours = iHours.ToString("D2") + ":";
            DrawString(szHours, iX, iY, Color.Goldenrod);
            iX += (int)m_DialogFont.MeasureString(szHours).X;

            // Minutes
            int iMinutes = (int)(dfRemainingSeconds / 60);
            dfRemainingSeconds = dfRemainingSeconds - ((double)iMinutes * 60.0);

            string szMinutes = iMinutes.ToString("D2") + ":";
            DrawString(szMinutes, iX, iY, Color.Goldenrod);
            iX += (int)m_DialogFont.MeasureString(szMinutes).X;

            // Seconds
            int iSeconds = (int)dfRemainingSeconds;

            string szSeconds = iSeconds.ToString("D2");
            DrawString(szSeconds, iX, iY, Color.Goldenrod);
            iX += (int)m_DialogFont.MeasureString(szSeconds).X;
        }
        */
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
