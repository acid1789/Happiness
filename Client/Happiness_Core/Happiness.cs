using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using HappinessNetwork;

namespace Happiness
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Happiness
    {
        public Options m_Options;
        public SoundManager m_SoundManager;

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
        GameInfo m_GameInfo;
        ServerWriter m_ServerWriter;

        public event Action<int> OnCurrencyChange;
        public event Action<int> OnVipDataChange;

        public Happiness()
        {
            Game = this;
            m_SoundManager = SoundManager.Inst;
            Settings s = Settings.LoadSettings();
            m_SoundManager.SoundVolume = s.SoundVolume;
            m_SoundManager.MusicVolume = s.MusicVolume;
            ExpSlowdown = s.ExpSlowdown;
            ErrorDetector = s.ErrorDetector;
            ErrorDetector2 = s.ErrorDetector2;
            DisableTimer = s.DisableTimer;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public void Initialize(int width, int height)
        {
            m_ServerWriter = new ServerWriter();

            InputController.IC.OnClick += IC_OnClick;

            m_iScreenWidth = width;
            m_iScreenHeight = height;
            m_Tutorial = new TutorialSystem(m_iScreenWidth, m_iScreenHeight, this);

            //m_CurrentScene = new GameScene(this);
            //((GameScene)m_CurrentScene).Initialize(0, 6, true);
            m_CurrentScene = new StartupScene(this);
        }

        public void ExitGame()
        {
            Platform.Instance.ExitApp();
        }

        public void OnExiting()
        {
            m_ServerWriter.Shutdown();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public void LoadContent(ContentManager Content)
        {
            // Load everything
            Assets.LoadAll(Content);
        }        

        private void IC_OnClick(object sender, DragArgs e)
        {
            e.Abort = m_Tutorial.HandleClick(e.CurrentX, e.CurrentY);
        }
        
        public void Update(double deltaTime, Renderer renderer)
        {
            // Cache local width/height
            m_iScreenWidth = renderer.ViewportWidth;
            m_iScreenHeight = renderer.ViewportHeight;

            // Update tutorial
            m_Tutorial.Update(deltaTime);

            // Update the scene
            if (m_CurrentScene != null)
                m_CurrentScene.Update(deltaTime);
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
            if (TheGameInfo.GameData.Tutorial != tutorialData)
            {
                // Store local copy
                TheGameInfo.GameData.Tutorial = tutorialData;

                // Send to the server
                m_ServerWriter.SaveTutorialData(tutorialData, TheGameInfo.AuthString, DateTime.Now);
            }
        }

        public void SavePuzzleData(int tower, int floor, double elapsedTime, ServerWriter.JobCompleteDelegate jobCompleteCB)
        {
            bool updateExistingFloor = false;
            foreach (TowerFloorRecord tfr in TheGameInfo.TowerData[tower].Floors)
            {
                if (tfr.Floor == floor)
                {
                    if (tfr.BestTime > elapsedTime)
                        tfr.BestTime = (int)elapsedTime;
                    updateExistingFloor = true;
                    break;
                }
            }
            if (!updateExistingFloor)
            {
                // Floor isnt in the local list, add it now
                List<TowerFloorRecord> records = new List<TowerFloorRecord>(TheGameInfo.TowerData[tower].Floors);
                TowerFloorRecord tfr = new TowerFloorRecord();
                tfr.Floor = floor;
                tfr.BestTime = (int)elapsedTime;
                records.Add(tfr);
                TheGameInfo.TowerData[tower].Floors = records.ToArray();
            }

            m_ServerWriter.SavePuzzleData(TheGameInfo, tower, floor, elapsedTime, ExpSlowdown, jobCompleteCB);
        }

        public void SpendCoins(int coinCount, int spentOn)
        {
            m_ServerWriter.SpendCoins(TheGameInfo.AuthString, coinCount, spentOn, TheGameInfo);
        }

        public void ResetTutorial()
        {
            // Nuke any existing save for stage 3_1
            DeletePuzzleSave(3, 1);
            DeletePuzzleSave(3, 2);
            DeletePuzzleSave(3, 3);

            TheGameInfo.GameData.Tutorial = 0;

        }

        void DeletePuzzleSave(int size, int index)
        {
            string saveName = PuzzleSaveName(size, index);
            if (FileManager.Instance.Exists(saveName))
                FileManager.Instance.Delete(saveName);
        }

        private void M_GameInfo_OnCurrencyChange(int obj)
        {
            OnCurrencyChange?.Invoke(obj);
        }

        private void M_GameInfo_OnVipDataChange()
        {
            OnVipDataChange?.Invoke(0);
        }

        public void ValidateVIPSettings()
        {
            // Make sure settings match vip level
#if !DEBUG
            if( ErrorDetector && m_GameInfo.VipData.Level < 4 )
                ErrorDetector = false;
            if( ErrorDetector2 && m_GameInfo.VipData.Level < 8 )
                ErrorDetector2 = false;
            if( DisableTimer && m_GameInfo.VipData.Level < 1 )
                DisableTimer = false;
            if( ExpSlowdown && m_GameInfo.VipData.Level < 2 )
                ExpSlowdown = false;
#endif
        }


        #region Drawing
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(Renderer spriteBatch)
        {
            spriteBatch.Draw(Assets.Background, new Rectangle(0, 0, m_iScreenWidth, m_iScreenHeight), Color.White);

            if (m_CurrentScene != null)
                m_CurrentScene.Draw(spriteBatch);

            m_Tutorial.Draw(spriteBatch);            
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

        public ServerWriter ServerWriter { get { return m_ServerWriter; } }

        public static Happiness Game;
        public TutorialSystem Tutorial
        {
            get { return m_Tutorial; }
        }

        public SoundManager SoundManager
        {
            get { return m_SoundManager; }
        }

        public GameInfo TheGameInfo
        {
            get { return m_GameInfo; }
            set
            {
                m_GameInfo = value;
                m_GameInfo.OnCurrencyChange += M_GameInfo_OnCurrencyChange; ;
                m_GameInfo.OnVipDataChange += M_GameInfo_OnVipDataChange;
            }
        }

        public int AccountId { get; set; }

        public bool ExpSlowdown { get; set; }
        public bool ErrorDetector { get; set; }
        public bool ErrorDetector2 { get; set; }
        public bool DisableTimer { get; set; }
        #endregion

        public static string PuzzleSaveName(int puzzleSize, int puzzleIndex)
        {
            string localHappinessDir = FileManager.Instance.HappinessPath;
            FileManager.Instance.CreateDirectory(localHappinessDir);

            string saveDir = localHappinessDir + "saves/";
            FileManager.Instance.CreateDirectory(saveDir);

            string saveName = string.Format("{0}{1}_{2}.save", saveDir, puzzleSize, puzzleIndex);
            return saveName;
        }


        public static void ShadowString(Renderer sb, SpriteFont font, string text, Vector2 position, Color color)
        {
            sb.DrawString(font, text, new Vector2(position.X + 2, position.Y + 2), Color.Black);
            sb.DrawString(font, text, position, color);
        }

        public static Vector2 CenterText(Vector2 center, string text, SpriteFont font)
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
