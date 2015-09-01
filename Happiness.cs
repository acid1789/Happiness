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
        MainMenu m_MainMenu;
        PauseMenu m_PauseMenu;
        EndPuzzleScreen m_EndScreen;
        public Options m_Options;
        public InputController m_Input;
        public SoundManager m_SoundManager;        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Texture2D m_Background;
        public Texture2D m_Logo;
        public Texture2D m_Transparent;
        public Texture2D m_BlueArrow;
        public Texture2D m_GoldArrowLeft;
        public Texture2D m_GoldArrowRight;
        public Texture2D m_CheckBox;
        public Texture2D m_Check;
        public SpriteFont m_MenuFont;
        public SpriteFont m_DialogFont;
        public SpriteFont m_HelpFont;
        public Texture2D m_ScrollBar;
        public Texture2D m_ScrollCursor;
        public Texture2D m_ScrollArrow;
        public Texture2D m_NotOverlay;
        public Texture2D m_EitherOrOverlay;
        public Texture2D m_SpanOverlay;
        public Texture2D m_LeftOfIcon;
        public Texture2D m_HelpBackground;
        public Texture2D m_MouseImage;
        public Texture2D m_ControllerImage;
        public Texture2D m_Splash;
        Texture2D m_GoldBarVertical;
        Texture2D m_GoldBarHorizontal;
        Texture2D m_TransGrey;
        Texture2D m_SelectionIcon;
        Texture2D m_SelectionIconWide;
        Texture2D m_SelectionIcon2Tall;
        Texture2D m_SelectionIcon3Tall;
        Texture2D[] m_HintIcons;
        AnimatedSprite m_HintSprite;
        public Texture2D[] m_aCars;
        public Texture2D[] m_aCats;
        public Texture2D[] m_aFlowers;
        public Texture2D[] m_aHubble;
        public Texture2D[] m_aPrincesses;
        public Texture2D[] m_aPuppies;
        public Texture2D[] m_aSimpsons;
        public Texture2D[] m_aSuperheros;
        Texture2D[,] m_aIcons;
        DisplayRow[] m_aDisplayRows;
        DisplayVerticalClue[] m_aVerticalClues;
        DisplayHorizontalClue[] m_aHorizontalClues;
        ArrayList m_aVisibleHorizontalClues;
        ArrayList m_aVisibleVerticalClues;
        ArrayList m_aHistory;
        ArrayList m_aFuture;
        bool m_bMainMenu;
        bool m_bPauseMenu;
        bool m_bEndScreen;
        public bool m_bOptionsDialog;
        int m_iDragRow;
        int m_iDragIcon;

        int m_iClueIconSize = 30;
        int m_iClueDescriptionX = 40;
        int m_iClueDescriptionY = 668;
        int m_iNumHorizontalClueColumns;
        int m_iNumHorizontalCluesPerColumn;
        int m_iIconsPerRow;

        public bool m_bAutoArangeClues;
        public bool m_bShowClueDescriptions;
        public bool m_bShowClock;
        public bool m_bShowPuzzleNumber;
        public bool m_bRandomizeIcons;
        
        int m_iPuzzleIndex;
        int m_iSize;
        int m_iDifficulty;
        Puzzle m_Puzzle;
        Hint m_Hint = null;
        SaveGame m_SaveGame = null;
        int m_iFrameCount = 0;
        double m_dfSeconds = 0;

        Clue m_SelectedClue = null;

        int m_iSelectedHorizontalClue = -1;
        int m_iSelectedVerticalClue = -1;
        int m_iSelectedRow = -1;
        int m_iSelectedCol = -1;
        int m_iSelectedIcon = -1;
        double m_dfGameRepeatDelay = 0.15;

        bool m_bVerticalScrollBar;
        bool m_bHorizontalScrollBar;
        int m_iScrollBarWidth = 18;
        int m_iFirstVisibleHorizontalClue = 0;
        int m_iFirstVisibleVerticalClue = 0;
        Rectangle m_rVerticalScrollBarUp;
        Rectangle m_rVerticalScrollBarDown;
        Rectangle m_rHorizontalScrollBarLeft;
        Rectangle m_rHorizontalScrollBarRight;

        float m_fTempSoundVol = 1.0f;
        float m_fTempMusicVol = 1.0f;

        public bool m_bLetterbox = false;
        public int m_iScreenTop = 0;
        public int m_iScreenWidth = 1280;
        public int m_iScreenHeight  = 720;
        Viewport m_vpFull;
        Viewport m_vpDraw;

        bool m_bDoneLoading = false;
        bool m_bCanLoad = false;

        public Happiness()
        {
            m_Input = new InputController();
            m_MainMenu = new MainMenu(this);
            m_PauseMenu = new PauseMenu(this);
            m_EndScreen = new EndPuzzleScreen(this);
            m_Options = new Options(this);            

            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = m_iScreenWidth;
            graphics.PreferredBackBufferHeight = m_iScreenHeight;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            m_bMainMenu = true;

            //FindBogusPuzzle(0);
            
            m_iPuzzleIndex = 0;

            m_iDragRow = -1;
            m_iDragIcon = -1;
        }

        void FindBogusPuzzle(int iStart)
        {
            //m_Puzzle = new Puzzle(271, 4);
            for (int i = iStart; i < iStart + 100000; i++)
            {
                for (int j = 3; j <= 8; j++)
                {
                  System.Diagnostics.Debug.Write("Testing Puzzle: " + i.ToString() + " ");                    
                  System.Diagnostics.Debug.WriteLine(j);
                  m_Puzzle = new Puzzle(i, j, 1);
                }
            }
        }

        private void InitPuzzle(bool bCreatePuzzle)
        {
            /*
            m_iSize = 8;
            m_iDifficulty = 0;
            for (int i = 0; i < 100000; i++)
            {
                m_Puzzle = new Puzzle(i, m_iSize, m_iDifficulty);
                if (m_Puzzle.m_VeritcalClues.Count > 24)
                {
                    break;
                }
            }
            */
            if( bCreatePuzzle )
                m_Puzzle = new Puzzle(m_iPuzzleIndex, m_iSize, m_iDifficulty);

            m_aVisibleVerticalClues = new ArrayList(m_Puzzle.m_VeritcalClues);
            m_aVisibleHorizontalClues = new ArrayList(m_Puzzle.m_HorizontalClues);

            m_aHistory = new ArrayList();
            m_aFuture = new ArrayList();           

            int iPlayGridHeight = 500;

            m_aDisplayRows = new DisplayRow[m_iSize];
            m_iIconsPerRow = (m_iSize + 1) >> 1;
            int iRowSpacerHeight = 2;
            int iColSpacerWidth = 6;

            int iRowHeight = (iPlayGridHeight / m_iSize);
            int iIconSize = (iRowHeight - iRowSpacerHeight) / 2;

            int iColWidth = (iIconSize * m_iIconsPerRow) + iColSpacerWidth;
            int iRowWidth = (iColWidth * m_iSize) - iColSpacerWidth;

            int iGridRightSpace = 8;
            int iTopMargin = 30;
            int iBottomMargin = 60;            
            
            int iClueIconSize = 38;
            int iVerticalClueSpace = 4;
            int iHorizontalClueSpace = 6;
            
            int iHorizontalClueWidth = (iClueIconSize * 3) + iVerticalClueSpace;
            int iHorizontalClueHeight = iClueIconSize + iHorizontalClueSpace;

            m_iNumHorizontalCluesPerColumn = (m_iScreenHeight - (iTopMargin + iBottomMargin)) / iHorizontalClueHeight;

            int iGridWidth = (iRowWidth + iGridRightSpace);
            int iHorizontalClueArea = (m_iScreenWidth - 60) - iGridWidth;
            m_iNumHorizontalClueColumns = iHorizontalClueArea / iHorizontalClueWidth;
            int iNumHorizontalClues = m_iNumHorizontalClueColumns * m_iNumHorizontalCluesPerColumn;
            if (m_Puzzle.m_HorizontalClues.Count < iNumHorizontalClues)
            {
                m_iNumHorizontalClueColumns = m_Puzzle.m_HorizontalClues.Count / m_iNumHorizontalCluesPerColumn;
                if (m_Puzzle.m_HorizontalClues.Count % m_iNumHorizontalCluesPerColumn != 0)
                    m_iNumHorizontalClueColumns++;
                iNumHorizontalClues = m_iNumHorizontalClueColumns * m_iNumHorizontalCluesPerColumn;
            }
            if (m_Puzzle.m_HorizontalClues.Count > iNumHorizontalClues)
            {
                // Add space for a scroll bar                
                m_iNumHorizontalClueColumns = (iHorizontalClueArea - m_iScrollBarWidth) / iHorizontalClueWidth;
                iNumHorizontalClues = m_iNumHorizontalClueColumns * m_iNumHorizontalCluesPerColumn;
                m_bVerticalScrollBar = true;
            }
            
            int iTotalWidth = iGridWidth + ((m_iNumHorizontalClueColumns * iHorizontalClueWidth) - iHorizontalClueSpace);
            int iExtraWidth = (m_iScreenWidth - 60) - iTotalWidth;
            int iLeftMargin = 30 + (iExtraWidth / 2);
            int iRightMargin = 30 + (iExtraWidth / 2);

            int iX = (iLeftMargin + iRowWidth) + iGridRightSpace;            

            int iY = iTopMargin;
            for (int i = 0; i < m_iSize; i++)
            {
                Rectangle rRowBounds = new Rectangle(iLeftMargin, iY, iRowWidth, iRowHeight - iRowSpacerHeight);
                m_aDisplayRows[i] = new DisplayRow(m_iSize, rRowBounds, iColWidth, iColSpacerWidth);

                iY += iRowHeight;
            }
                        
            iX = iLeftMargin + iGridWidth;
            m_aHorizontalClues = new DisplayHorizontalClue[iNumHorizontalClues];
            for (int i = 0; i < m_iNumHorizontalClueColumns; i++)
            {
                iY = iTopMargin;
                for (int j = 0; j < m_iNumHorizontalCluesPerColumn; j++)
                {
                    m_aHorizontalClues[(i * m_iNumHorizontalCluesPerColumn) + j] = new DisplayHorizontalClue(iX, iY, iClueIconSize);
                    iY += iHorizontalClueHeight;
                }
                iX += iHorizontalClueWidth;
            }

            int iNumVerticalClues = iRowWidth / (iClueIconSize + iVerticalClueSpace);
            if (m_Puzzle.m_VeritcalClues.Count > iNumVerticalClues)
            {
                iNumVerticalClues = ((iLeftMargin - 45) + iRowWidth) / (iClueIconSize + iVerticalClueSpace);
                iX = 45;
            }
            else
            {
                iX = iLeftMargin;
            }
            m_aVerticalClues = new DisplayVerticalClue[iNumVerticalClues];            
            iY = (iTopMargin + iPlayGridHeight) + 4;
            for (int i = 0; i < m_aVerticalClues.Length; i++)
            {
                m_aVerticalClues[i] = new DisplayVerticalClue(iX, iY, iClueIconSize);
                iX += iClueIconSize + iVerticalClueSpace;
            }

            if (m_Puzzle.m_VeritcalClues.Count > iNumVerticalClues)
                m_bHorizontalScrollBar = true;
            

            if (m_bRandomizeIcons)
            {
                Random rand = new Random(m_iFrameCount);
                int[] iScatterArray = new int[8];

                Puzzle.RandomDistribution(rand, iScatterArray);
                Texture2D[] aAllIcons = new Texture2D[64];
                for (int i = 0; i < 8; i++)
                {
                    aAllIcons[i] = m_aCars[iScatterArray[i]];
                    aAllIcons[8 + i] = m_aCats[iScatterArray[i]];
                    aAllIcons[16 + i] = m_aFlowers[iScatterArray[i]];
                    aAllIcons[24 + i] = m_aHubble[iScatterArray[i]];
                    aAllIcons[32 + i] = m_aPrincesses[iScatterArray[i]];
                    aAllIcons[40 + i] = m_aPuppies[iScatterArray[i]];
                    aAllIcons[48 + i] = m_aSimpsons[iScatterArray[i]];
                    aAllIcons[56 + i] = m_aSuperheros[iScatterArray[i]];
                }
                                
                Puzzle.RandomDistribution(rand, iScatterArray);
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        m_aIcons[i, j] = aAllIcons[(iScatterArray[i] * 8) + j];
                    }
                }
            }
            else
            {
                for (int iIcon = 0; iIcon < m_iSize; iIcon++)
                {
                    m_aIcons[0, iIcon] = m_aCars[iIcon];
                    m_aIcons[1, iIcon] = m_aCats[iIcon];
                    m_aIcons[2, iIcon] = m_aFlowers[iIcon];
                    m_aIcons[3, iIcon] = m_aHubble[iIcon];
                    m_aIcons[4, iIcon] = m_aPrincesses[iIcon];
                    m_aIcons[5, iIcon] = m_aPuppies[iIcon];
                    m_aIcons[6, iIcon] = m_aSimpsons[iIcon];
                    m_aIcons[7, iIcon] = m_aSuperheros[iIcon];
                }
            }

            SelectDefault();

            m_iFirstVisibleHorizontalClue = 0;
            m_iFirstVisibleVerticalClue = 0;

            m_dfSeconds = 0;

            m_Input.SetRepeatDelay(m_dfGameRepeatDelay);

            m_SoundManager.PlayGameMusic();

            if (m_SaveGame != null)
                m_SaveGame.SavePuzzle(m_Puzzle);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            m_aCars = new Texture2D[8];
            m_aCats = new Texture2D[8];
            m_aFlowers = new Texture2D[8];
            m_aHubble = new Texture2D[8];
            m_aPrincesses = new Texture2D[8];
            m_aPuppies = new Texture2D[8];
            m_aSimpsons = new Texture2D[8];
            m_aSuperheros = new Texture2D[8];
            m_aIcons = new Texture2D[8, 8];

            m_HintIcons = new Texture2D[6];

            m_SoundManager = new SoundManager();
            m_SoundManager.m_fSoundVolume = m_fTempSoundVol;
            m_SoundManager.m_fMusicVolume = m_fTempMusicVol;
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Init the save game
#if !XBOX
            m_SaveGame = new SaveGame();
            m_SaveGame.LoadOptions(out m_bAutoArangeClues, out m_bShowClueDescriptions, out m_bShowClock, out m_bShowPuzzleNumber, out m_bRandomizeIcons, out m_fTempSoundVol, out m_fTempMusicVol);
#endif
            m_vpFull = GraphicsDevice.Viewport;

#if !XBOX
            if (GraphicsDevice.DisplayMode.Width < m_iScreenWidth)
            {                
                m_bLetterbox = true;
                m_iScreenTop = 152;
                graphics.PreferredBackBufferHeight = 1024;
                m_vpFull.Height = graphics.PreferredBackBufferHeight;
                graphics.SynchronizeWithVerticalRetrace = true;
                graphics.ApplyChanges();
                graphics.ToggleFullScreen();
            }
#endif

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            m_bDoneLoading = false;
            m_bCanLoad = false;
            m_Splash = Content.Load<Texture2D>("Splash");
        }

        private void FullLoad()
        {
            m_Background = Content.Load<Texture2D>("Clouds4");
            m_Logo = Content.Load<Texture2D>("Logo");
            m_Transparent = Content.Load<Texture2D>("TransparentGray");
            m_BlueArrow = Content.Load<Texture2D>("BlueArrow");
            m_GoldArrowLeft = Content.Load<Texture2D>("GoldArrowLeft");
            m_GoldArrowRight = Content.Load<Texture2D>("GoldArrowRight");
            m_HelpBackground = Content.Load<Texture2D>("HelpBackground");
            m_MouseImage = Content.Load<Texture2D>("Mouse");
            m_ControllerImage = Content.Load<Texture2D>("Controller");

            m_MenuFont = Content.Load<SpriteFont>("MenuFont");
            m_DialogFont = Content.Load<SpriteFont>("DialogFont");
            m_HelpFont = Content.Load<SpriteFont>("HelpFont");

            m_GoldBarVertical = Content.Load<Texture2D>("GoldBarVertical");
            m_GoldBarHorizontal = Content.Load<Texture2D>("GoldBarHorizontal");
            m_TransGrey = Content.Load<Texture2D>("TransGrey");
            m_NotOverlay = Content.Load<Texture2D>("NotOverlay");
            m_EitherOrOverlay = Content.Load<Texture2D>("EitherOrOverlay");
            m_SpanOverlay = Content.Load<Texture2D>("Span");
            m_LeftOfIcon = Content.Load<Texture2D>("NextTo");
            m_SelectionIcon = Content.Load<Texture2D>("SelectionIcon");
            m_SelectionIconWide = Content.Load<Texture2D>("SelectionIconWide");
            m_SelectionIcon2Tall = Content.Load<Texture2D>("SelectionIcon2Tall");
            m_SelectionIcon3Tall = Content.Load<Texture2D>("SelectionIcon3Tall");
            m_ScrollArrow = Content.Load<Texture2D>("ScrollArrow");
            m_ScrollBar = Content.Load<Texture2D>("ScrollBar");
            m_ScrollCursor = Content.Load<Texture2D>("ScrollCursor");
            m_CheckBox = Content.Load<Texture2D>("CheckBox");
            m_Check = Content.Load<Texture2D>("Check");

            m_HintIcons[0] = Content.Load<Texture2D>("HintIcon");
            m_HintIcons[1] = Content.Load<Texture2D>("HintIcon2");
            m_HintIcons[2] = Content.Load<Texture2D>("HintIcon3");
            m_HintIcons[3] = Content.Load<Texture2D>("HintIcon4");
            m_HintIcons[4] = Content.Load<Texture2D>("HintIcon5");
            m_HintIcons[5] = Content.Load<Texture2D>("HintIcon6");
            m_HintSprite = new AnimatedSprite(m_HintIcons, 0.0625);

            m_aCars[0] = Content.Load<Texture2D>("Cars/AudiLE");
            m_aCars[1] = Content.Load<Texture2D>("Cars/BugattiVeyron");
            m_aCars[2] = Content.Load<Texture2D>("Cars/GTO");
            m_aCars[3] = Content.Load<Texture2D>("Cars/Hummer");
            m_aCars[4] = Content.Load<Texture2D>("Cars/Lamborghini");
            m_aCars[5] = Content.Load<Texture2D>("Cars/Lotus");
            m_aCars[6] = Content.Load<Texture2D>("Cars/Porsche");
            m_aCars[7] = Content.Load<Texture2D>("Cars/Viper");

            m_aCats[0] = Content.Load<Texture2D>("Cats/DarkGreyCat");
            m_aCats[1] = Content.Load<Texture2D>("Cats/GreyCat");
            m_aCats[2] = Content.Load<Texture2D>("Cats/Kitten");
            m_aCats[3] = Content.Load<Texture2D>("Cats/OrangeCat");
            m_aCats[4] = Content.Load<Texture2D>("Cats/SnowConeCat");
            m_aCats[5] = Content.Load<Texture2D>("Cats/StripedCat");
            m_aCats[6] = Content.Load<Texture2D>("Cats/WaterCat");
            m_aCats[7] = Content.Load<Texture2D>("Cats/WhiteCat");

            m_aFlowers[0] = Content.Load<Texture2D>("Flowers/BlueWildFlower");
            m_aFlowers[1] = Content.Load<Texture2D>("Flowers/ConvolvulusArvensis");
            m_aFlowers[2] = Content.Load<Texture2D>("Flowers/Dahlia");
            m_aFlowers[3] = Content.Load<Texture2D>("Flowers/Dandelion");
            m_aFlowers[4] = Content.Load<Texture2D>("Flowers/LotusFlower");
            m_aFlowers[5] = Content.Load<Texture2D>("Flowers/OrangeGazania");
            m_aFlowers[6] = Content.Load<Texture2D>("Flowers/Osteospermum");
            m_aFlowers[7] = Content.Load<Texture2D>("Flowers/PinkRose");

            m_aHubble[0] = Content.Load<Texture2D>("Hubble/CatsEyeNebula");
            m_aHubble[1] = Content.Load<Texture2D>("Hubble/CigarNebula");
            m_aHubble[2] = Content.Load<Texture2D>("Hubble/CrabNebula");
            m_aHubble[3] = Content.Load<Texture2D>("Hubble/RaisingHeaven");
            m_aHubble[4] = Content.Load<Texture2D>("Hubble/RedRectangle");
            m_aHubble[5] = Content.Load<Texture2D>("Hubble/RingGalaxy");
            m_aHubble[6] = Content.Load<Texture2D>("Hubble/StarryNight");
            m_aHubble[7] = Content.Load<Texture2D>("Hubble/TwoGalaxies");

            m_aPrincesses[0] = Content.Load<Texture2D>("Princesses/Ariel");
            m_aPrincesses[1] = Content.Load<Texture2D>("Princesses/Belle");
            m_aPrincesses[2] = Content.Load<Texture2D>("Princesses/Cinderella");
            m_aPrincesses[3] = Content.Load<Texture2D>("Princesses/Dora");
            m_aPrincesses[4] = Content.Load<Texture2D>("Princesses/Jasmine");
            m_aPrincesses[5] = Content.Load<Texture2D>("Princesses/SleepingBeauty");
            m_aPrincesses[6] = Content.Load<Texture2D>("Princesses/SnowWhite");
            m_aPrincesses[7] = Content.Load<Texture2D>("Princesses/TinkerBell");

            m_aPuppies[0] = Content.Load<Texture2D>("Puppies/BichonPuppy");
            m_aPuppies[1] = Content.Load<Texture2D>("Puppies/BlackLabPuppy");
            m_aPuppies[2] = Content.Load<Texture2D>("Puppies/BulldogPuppy");
            m_aPuppies[3] = Content.Load<Texture2D>("Puppies/BWBPuppy");
            m_aPuppies[4] = Content.Load<Texture2D>("Puppies/GoldenRetrieverPuppy");
            m_aPuppies[5] = Content.Load<Texture2D>("Puppies/IrishTerrier");
            m_aPuppies[6] = Content.Load<Texture2D>("Puppies/WhitePuppy");
            m_aPuppies[7] = Content.Load<Texture2D>("Puppies/WinkingPuppy");

            m_aSimpsons[0] = Content.Load<Texture2D>("Simpsons/Apu");
            m_aSimpsons[1] = Content.Load<Texture2D>("Simpsons/Barney");
            m_aSimpsons[2] = Content.Load<Texture2D>("Simpsons/Bartman");
            m_aSimpsons[3] = Content.Load<Texture2D>("Simpsons/Homer");
            m_aSimpsons[4] = Content.Load<Texture2D>("Simpsons/Lisa");
            m_aSimpsons[5] = Content.Load<Texture2D>("Simpsons/Marge");
            m_aSimpsons[6] = Content.Load<Texture2D>("Simpsons/Moe");
            m_aSimpsons[7] = Content.Load<Texture2D>("Simpsons/MrBurns");

            m_aSuperheros[0] = Content.Load<Texture2D>("Superheros/Batman");
            m_aSuperheros[1] = Content.Load<Texture2D>("Superheros/CaptainAmerica");
            m_aSuperheros[2] = Content.Load<Texture2D>("Superheros/GreenLantern");
            m_aSuperheros[3] = Content.Load<Texture2D>("Superheros/Hulk");
            m_aSuperheros[4] = Content.Load<Texture2D>("Superheros/Spiderman");
            m_aSuperheros[5] = Content.Load<Texture2D>("Superheros/Superman");
            m_aSuperheros[6] = Content.Load<Texture2D>("Superheros/Wolverine");
            m_aSuperheros[7] = Content.Load<Texture2D>("Superheros/WonderWoman");

            m_SoundManager.Load();
            ShowMainMenu();
            m_bDoneLoading = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

#if XBOX
        private bool IsPlayerSignedIn()
        {
            if (m_CurrentGamer != null && m_CurrentGamer.IsSignedInToLive)
                return true;
            return false;
        }
#endif

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!m_bDoneLoading)
            {
                if (m_bCanLoad)
                    FullLoad();
            }
            else
            {

                m_iFrameCount++;

                if (!m_bEndScreen && !m_bPauseMenu && IsActive)
                    m_dfSeconds += gameTime.ElapsedGameTime.TotalSeconds;

                m_Options.m_GameTime = gameTime;
                m_MainMenu.m_GameTime = gameTime;
                m_PauseMenu.m_GameTime = gameTime;

                m_HintSprite.Update(gameTime);

                m_Input.Update(this, gameTime, PlayerIndex.One);

                if (m_bOptionsDialog)
                {
                    m_Options.UpdateMouse(m_Input.m_iMouseX, m_Input.m_iMouseY, m_Input.m_bLeftButtonPressed);
                }
                else if (m_bMainMenu)
                {
                    m_MainMenu.SetSaveGame(m_SaveGame);
                    m_MainMenu.UpdateMouse(m_Input.m_iMouseX, m_Input.m_iMouseY, m_Input.m_bLeftButtonPressed);
                }
                else if (m_bPauseMenu)
                {
                    m_PauseMenu.SetSaveGame(m_SaveGame);
                    m_PauseMenu.UpdateMouse(m_Input.m_iMouseX, m_Input.m_iMouseY, m_Input.m_bLeftButtonPressed);
                }
                else if (m_bEndScreen)
                {
                    m_EndScreen.UpdateMouse(m_Input.m_iMouseX, m_Input.m_iMouseY, m_Input.m_bLeftButtonPressed);
                }
                else
                {
                    SelectClue(m_Input.m_iMouseX, m_Input.m_iMouseY);

                    if (m_Puzzle.IsCompleted())
                    {
                        m_bEndScreen = true;
                        m_Input.SetRepeatDelay(0.15);
                        m_EndScreen.m_bSuccess = m_Puzzle.IsSolved();
                        if (m_EndScreen.m_bSuccess)
                            m_SoundManager.PlayPuzzleComplete();
                        else
                            m_SoundManager.PlayPuzzleFailed();
                    }
                }
            }

            m_SoundManager.Update();
            base.Update(gameTime);
        }

        public void Pause()
        {
            if (!m_bMainMenu && !m_bPauseMenu && !m_bOptionsDialog && !m_bEndScreen)
            {
                m_PauseMenu.Init();
                m_PauseMenu.m_iPuzzleNumber = GetDisplayPuzzleNumber();
                m_Input.SetRepeatDelay(0.15);
                m_bPauseMenu = true;
                m_SoundManager.PlayMenuAccept();
            }
        }

        public void SavePuzzle()
        {
            if (!m_bMainMenu && !m_bPauseMenu && !m_bOptionsDialog && !m_bEndScreen && m_SaveGame != null)
            {
                m_SaveGame.SavePuzzle(m_Puzzle);
                m_SoundManager.PlayGameSave();
            }
        }

        public void LeftClick(int iX, int iY)
        {
            if (m_bOptionsDialog)
            {
                if (!m_Options.HandleClick(iX, iY))
                {
                    m_bOptionsDialog = false;
                    if (m_SaveGame != null)
                        m_SaveGame.SaveOptions(m_bAutoArangeClues, m_bShowClueDescriptions, m_bShowClock, m_bShowPuzzleNumber, m_bRandomizeIcons, m_SoundManager.m_fSoundVolume, m_SoundManager.m_fMusicVolume);

                    if (m_bAutoArangeClues)
                    {
                        if (m_aVisibleVerticalClues != null)
                        {
                            for (int i = m_aVisibleVerticalClues.Count - 1; i >= 0; i--)
                            {
                                Clue C = (Clue)m_aVisibleVerticalClues[i];
                                if (C == null)
                                    m_aVisibleVerticalClues.RemoveAt(i);
                            }
                        }

                        if (m_aVisibleHorizontalClues != null)
                        {
                            for (int i = m_aVisibleHorizontalClues.Count - 1; i >= 0; i--)
                            {
                                Clue C = (Clue)m_aVisibleHorizontalClues[i];
                                if (C == null)
                                    m_aVisibleHorizontalClues.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            else if (m_bMainMenu)
            {
                if (!m_MainMenu.HandleClick(iX, iY))
                {
                    HandleMainMenuExit();                    
                }
            }
            else if (m_bPauseMenu)
            {
                if (!m_PauseMenu.HandleClick(iX, iY))
                {
                    HandlePauseMenuExit();
                }
            }
            else if (m_bEndScreen)
            {
                if (!m_EndScreen.HandleClick(iX, iY))
                {
                    HandleEndScreenExit();
                }
            }
            else
            {
                iY -= m_iScreenTop;
                // Main game
                if (m_bVerticalScrollBar)
                {
                    if (m_rVerticalScrollBarUp.Contains(iX, iY))
                    {
                        ScrollUp();                        
                        return;
                    }
                    if (m_rVerticalScrollBarDown.Contains(iX, iY))
                    {
                        ScrollDown();
                        return;
                    }
                }
                if (m_bHorizontalScrollBar)
                {
                    if (m_rHorizontalScrollBarLeft.Contains(iX, iY))
                    {
                        ScrollLeft();
                        return;
                    }
                    if (m_rHorizontalScrollBarRight.Contains(iX, iY))
                    {
                        ScrollRight();
                        return;
                    }
                }

                SetFinalIcon(iX, iY);
            }
        }

        public void RightClick(int iX, int iY)
        {
            iY -= m_iScreenTop;
            if (!m_bMainMenu && !m_bPauseMenu && !m_bOptionsDialog && !m_bEndScreen)
            {
                EliminateIcon(iX, iY);
                HideClue(iX, iY);
            }
        }

        public bool SelectClue(int iX, int iY)
        {
            iY -= m_iScreenTop;
            for (int i = 0; i < m_aVerticalClues.Length; i++)
            {
                if (m_aVerticalClues[i].m_rBounds.Contains(iX, iY))
                {
                    int iClueIndex = m_iFirstVisibleVerticalClue + i;
                    if (iClueIndex < m_aVisibleVerticalClues.Count)
                    {
                        Clue C = (Clue)m_aVisibleVerticalClues[iClueIndex];
                        if (C != m_SelectedClue)
                        {
                            m_SelectedClue = C;
                            m_SoundManager.PlayGameNavigate();
                        }
                    }
                    return true;
                }
            }
            for (int i = 0; i < m_aHorizontalClues.Length; i++)
            {
                if (m_aHorizontalClues[i].m_rBounds.Contains(iX, iY))
                {
                    int iClueIndex = m_iFirstVisibleHorizontalClue + i;
                    if (iClueIndex < m_aVisibleHorizontalClues.Count)
                    {
                        Clue C = (Clue)m_aVisibleHorizontalClues[iClueIndex];
                        if (C != m_SelectedClue)
                        {
                            m_SelectedClue = C;
                            m_SoundManager.PlayGameNavigate();
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private void SelectHorizontalClue()
        {
            if (m_aVisibleHorizontalClues != null)
            {
                int iClueIndex = m_iFirstVisibleHorizontalClue + m_iSelectedHorizontalClue;
                if (iClueIndex < m_aVisibleHorizontalClues.Count)
                    m_SelectedClue = (Clue)m_aVisibleHorizontalClues[iClueIndex];
            }
        }

        private void SelectVerticalClue()
        {
            if (m_aVisibleVerticalClues != null)
            {
                int iClueIndex = m_iFirstVisibleVerticalClue + m_iSelectedVerticalClue;
                if (iClueIndex < m_aVisibleVerticalClues.Count)
                    m_SelectedClue = (Clue)m_aVisibleVerticalClues[iClueIndex];
            }
        }

        private void SelectDefault()
        {
            m_iSelectedVerticalClue = -1;
            m_iSelectedHorizontalClue = -1;
            m_iSelectedRow = -1;
            if (m_aVisibleVerticalClues.Count > 0)
            {
                m_iSelectedVerticalClue = 0;
                SelectVerticalClue();
            }
            else if (m_aVisibleHorizontalClues.Count > 0)
            {
                m_iSelectedHorizontalClue = 0;
                SelectHorizontalClue();
            }
            else
            {
                m_iSelectedRow = (m_iSize / 2);
                m_iSelectedCol = (m_iSize / 2);
                m_iSelectedIcon = Math.Max(0, m_Puzzle.m_Rows[m_iSelectedRow].m_Cells[m_iSelectedCol].m_iFinalIcon);
            }
        }

        public void SetFinalIcon(int iX, int iY)
        {
            // Find the cell that was clicked on
            int iRow = GetClickedRow(iX, iY);
            if (iRow < 0)
                return;
            int iCol = GetClickedCol(iX, iY);
            if (iCol < 0)
                return;

            int iFinal = m_Puzzle.m_Rows[iRow].m_Cells[iCol].m_iFinalIcon;
            if (iFinal < 0)
            {
                int iIcon = GetClickedIcon(iX, iY);
                if (iIcon >= 0)
                {
                    if (m_Puzzle.m_Rows[iRow].m_Cells[iCol].m_bValues[iIcon])
                    {
                        DoAction(eActionType.eAT_SetFinalIcon, iRow, iCol, iIcon);
                    }
                }
            }
        }

        public void EliminateIcon(int iX, int iY)
        {
            // Find the cell that was clicked on
            int iRow = GetClickedRow(iX, iY);
            if (iRow < 0)
                return;
            int iCol = GetClickedCol(iX, iY);
            if (iCol < 0)
                return;

            int iIcon = GetClickedIcon(iX, iY);
            if (iIcon >= 0)
            {
                if (m_Puzzle.m_Rows[iRow].m_Cells[iCol].m_iFinalIcon >= 0)
                {
                    bool bGiven = false;
                    for (int i = 0; i < m_Puzzle.m_GivenClues.Count; i++)
                    {
                        Clue given = (Clue)m_Puzzle.m_GivenClues[i];
                        if (given.m_iRow == iRow && given.m_iCol == iCol)
                        {
                            bGiven = true;
                            break;
                        }
                    }

                    if (!bGiven)
                    {
                        iIcon = m_Puzzle.m_Rows[iRow].m_Cells[iCol].m_iFinalIcon;
                        m_Puzzle.m_Rows[iRow].Reset(iCol);
                        if (m_Puzzle.m_Rows[iRow].m_Cells[iCol].m_iFinalIcon >= 0)
                            m_Puzzle.ResetRow(iRow);    // Whole row is set, Break the entire row

                        DoAction(eActionType.eAT_EliminateIcon, iRow, iCol, iIcon);
                    }
                }
                else if (m_Puzzle.m_Rows[iRow].m_Cells[iCol].m_bValues[iIcon])
                {
                    DoAction(eActionType.eAT_EliminateIcon, iRow, iCol, iIcon);
                }
            }
        }

        public void HideVerticalClue(int iClue)
        {
            if (iClue < m_aVisibleVerticalClues.Count)
            {
                if (m_bAutoArangeClues)
                    m_aVisibleVerticalClues.RemoveAt(iClue);
                else
                    m_aVisibleVerticalClues[iClue] = null;
                m_SoundManager.PlayGameHideClue();
            }
        }

        public void HideHorizontalClue(int iClue)
        {
            if (iClue < m_aVisibleHorizontalClues.Count)
            {
                if (m_bAutoArangeClues)
                    m_aVisibleHorizontalClues.RemoveAt(iClue);
                else
                    m_aVisibleHorizontalClues[iClue] = null;
                m_SoundManager.PlayGameHideClue();
            }
        }

        public void HideClue(int iX, int iY)
        {
            for (int i = 0; i < m_aVerticalClues.Length; i++)
            {
                if (m_aVerticalClues[i].m_rBounds.Contains(iX, iY) && m_aVisibleVerticalClues.Count > i)
                {
                    int iClueIndex = m_iFirstVisibleVerticalClue + i;
                    Clue C = (Clue)m_aVisibleVerticalClues[iClueIndex];
                    if (C != null)
                    {
                        if (m_bAutoArangeClues)
                        {
                            m_aVisibleVerticalClues.Remove(C);
                            SelectClue(iX, iY);
                        }
                        else
                            m_aVisibleVerticalClues[iClueIndex] = null;
                        m_SoundManager.PlayGameHideClue();
                    }
                    return;
                }
            }

            for (int i = 0; i < m_aHorizontalClues.Length; i++)
            {
                if (m_aHorizontalClues[i].m_rBounds.Contains(iX, iY) && m_aVisibleHorizontalClues.Count > i)
                {
                    int iClueIndex = m_iFirstVisibleHorizontalClue + i;
                    Clue C = (Clue)m_aVisibleHorizontalClues[iClueIndex];
                    if (C != null)
                    {
                        if (m_bAutoArangeClues)
                        {
                            m_aVisibleHorizontalClues.Remove(C);
                            SelectClue(iX, iY);
                        }
                        else
                            m_aVisibleHorizontalClues[iClueIndex] = null;
                        m_SoundManager.PlayGameHideClue();
                    }
                    return;
                }
            }
        }

        public void UnHideAllClues()
        {
            m_aVisibleVerticalClues = new ArrayList(m_Puzzle.m_VeritcalClues);
            m_aVisibleHorizontalClues = new ArrayList(m_Puzzle.m_HorizontalClues);
            m_SoundManager.PlayGameUnhideClues();
        }

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

        public void DragIcon(int iFromX, int iFromY, int iToX, int iToY)
        {
            iFromY -= m_iScreenTop;
            iToY -= m_iScreenTop;
            if (!m_bMainMenu && !m_bPauseMenu && !m_bEndScreen)
            {
                int iFromRow = GetClickedRow(iFromX, iFromY);
                int iToRow = GetClickedRow(iToX, iToY);
                if (iFromRow >= 0 && iToRow >= 0)
                {
                    int iFromIcon = GetClickedIcon(iFromX, iFromY);
                    int iToCol = GetClickedCol(iToX, iToY);
                    if (iFromIcon >= 0 && iToCol >= 0)
                    {
                        if (m_Puzzle.m_Rows[iToRow].m_Cells[iToCol].m_iFinalIcon >= 0)
                        {
                            bool bGiven = false;
                            for (int i = 0; i < m_Puzzle.m_GivenClues.Count; i++)
                            {
                                Clue given = (Clue)m_Puzzle.m_GivenClues[i];
                                if (given.m_iRow == iToRow && given.m_iCol == iToCol)
                                {
                                    bGiven = true;
                                    break;
                                }
                            }

                            if (!bGiven)
                            {
                                m_Puzzle.m_Rows[iToRow].Reset(iToCol);
                                if (m_Puzzle.m_Rows[iToRow].m_Cells[iToCol].m_iFinalIcon >= 0)
                                    m_Puzzle.ResetRow(iToRow);
                            }
                        }
                        else if (!m_Puzzle.m_Rows[iToRow].m_Cells[iToCol].m_bValues[iFromIcon])
                        {
                            DoAction(eActionType.eAT_RestoreIcon, iToRow, iToCol, iFromIcon);
                        }
                    }
                }
            }
        }

        public void DragFrom(int iFromX, int iFromY)
        {
            iFromY -= m_iScreenTop;
            if (!m_bMainMenu && !m_bPauseMenu && !m_bEndScreen)
            {
                m_iDragRow = GetClickedRow(iFromX, iFromY);
                m_iDragIcon = GetClickedIcon(iFromX, iFromY);
            }
        }

        public void ClearDragIcon()
        {
            m_iDragRow = -1;
            m_iDragIcon = -1;
        }

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

        private void DoAction(eActionType type, int iRow, int iCol, int iIcon)
        {
            switch (type)
            {
                case eActionType.eAT_EliminateIcon:
                    m_SoundManager.PlayGameEliminateIcon();
                    break;
                case eActionType.eAT_RestoreIcon:
                    m_SoundManager.PlayGameRestoreIcon();
                    break;
                case eActionType.eAT_SetFinalIcon:
                    m_SoundManager.PlayGameSetFinalIcon();
                    break;
            }

            Action a = new Action(type, iRow, iCol, iIcon, m_Puzzle);
            a.Perform(m_Puzzle);
            m_aHistory.Add(a);
            m_aFuture.Clear();
            HideHint(false);
        }

        private Point GetIconXY(int iRow, int iCol, int iIcon)
        {
            if (m_Puzzle.m_Rows[iRow].m_Cells[iCol].m_iFinalIcon >= 0)
            {
                return m_aDisplayRows[iRow].m_aCells[iCol].m_rFinal.Center;
            }
            
            return m_aDisplayRows[iRow].m_aCells[iCol].m_aDisplayRects[iIcon].Center;
        }

        #region Navigation
        public void ScrollUp()
        {
            if (m_bPauseMenu)
            {
                m_PauseMenu.ScrollUp();
            }
            else
            {
                if (m_bVerticalScrollBar)
                {
                    if (m_iFirstVisibleHorizontalClue > 0)
                    {
                        m_iFirstVisibleHorizontalClue--;
                        m_SoundManager.PlaySliderMove();
                    }
                }
            }
        }

        public void ScrollDown()
        {
            if (m_bPauseMenu)
            {
                m_PauseMenu.ScrollDown();
            }
            else
            {
                if (m_bVerticalScrollBar)
                {
                    int iLastVisibleClue = 0;
                    int iClues = m_aVisibleHorizontalClues.Count;
                    while ((iClues / m_iNumHorizontalClueColumns) > m_iNumHorizontalCluesPerColumn)
                    {
                        iClues -= m_iNumHorizontalClueColumns;
                        iLastVisibleClue += m_iNumHorizontalClueColumns;
                    }
                    if (m_iFirstVisibleHorizontalClue < iLastVisibleClue)
                    {
                        m_iFirstVisibleHorizontalClue++;
                        m_SoundManager.PlaySliderMove();
                    }
                }
            }
        }

        public void ScrollLeft()
        {
            if (m_bHorizontalScrollBar)
            {
                if (m_iFirstVisibleVerticalClue > 0)
                {
                    m_iFirstVisibleVerticalClue--;
                    m_SoundManager.PlaySliderMove();
                }
            }
        }

        public void ScrollRight()
        {
            if (m_bHorizontalScrollBar)
            {
                if (m_iFirstVisibleVerticalClue < (m_aVisibleVerticalClues.Count - m_aVerticalClues.Length))
                {
                    m_iFirstVisibleVerticalClue++;
                    m_SoundManager.PlaySliderMove();
                }
            }
        }

        public void NavigateUp()
        {
            if (m_bOptionsDialog)
                m_Options.NavigateUp();
            else if (m_bMainMenu)
                m_MainMenu.NavigateUp();
            else if (m_bPauseMenu)
                m_PauseMenu.NavigateUp();
            else if (m_bEndScreen)
                m_EndScreen.NavigateUp();
            else
            {
                if (m_iSelectedRow >= 0)
                {
                    // Deal with the grid
                    int iMidIcon = (m_iSize + 1) / 2;
                    int iFinalIcon = m_Puzzle.m_Rows[m_iSelectedRow].m_Cells[m_iSelectedCol].m_iFinalIcon;
                    if (m_iSelectedIcon >= iMidIcon && iFinalIcon < 0)
                    {
                        // Bottom icon row
                        m_iSelectedIcon -= iMidIcon;
                    }
                    else
                    {
                        // Top icon row
                        if (m_iSelectedRow > 0)
                        {
                            m_iSelectedRow--;
                            if (iFinalIcon < 0)
                                m_iSelectedIcon += iMidIcon;
                            else
                                m_iSelectedIcon = iMidIcon + 1;
                        }
                    }
                    m_iSelectedIcon = Math.Min(Math.Max(m_iSelectedIcon, 0), (m_iSize - 1));
                }
                else if (m_iSelectedVerticalClue >= 0)
                {
                    int iX = m_aVerticalClues[m_iSelectedVerticalClue].m_rBounds.Center.X;
                    m_iSelectedRow = m_iSize - 1;
                    for (int iCol = 0; iCol < m_iSize; iCol++)
                    {
                        if (m_aDisplayRows[m_iSelectedRow].m_aCells[iCol].m_rBounds.Right > iX)
                        {
                            m_iSelectedCol = iCol;
                            int iFinalIcon = m_Puzzle.m_Rows[m_iSelectedRow].m_Cells[iCol].m_iFinalIcon;
                            if (iFinalIcon < 0)
                            {
                                for (int j = ((m_iSize - 1) / 2) + 1; j < m_iSize; j++)
                                {
                                    if (m_aDisplayRows[m_iSelectedRow].m_aCells[iCol].m_aDisplayRects[j].Right > iX)
                                    {
                                        m_iSelectedIcon = j;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                m_iSelectedIcon = iFinalIcon;
                            }
                            break;
                        }
                    }
                    m_iSelectedVerticalClue = -1;
                }
                else if (m_iSelectedHorizontalClue >= 0)
                {
                    if (m_iSelectedHorizontalClue % m_iNumHorizontalCluesPerColumn != 0)
                        m_iSelectedHorizontalClue--;
                    else
                        ScrollUp();

                    SelectHorizontalClue();
                }
                else
                {
                    // Nothing selected?  select the default clue
                    SelectDefault();
                }
                m_SoundManager.PlayGameNavigate();
            }
        }

        public void NavigateDown()
        {
            if (m_bOptionsDialog)
                m_Options.NavigateDown();
            else if (m_bMainMenu)
                m_MainMenu.NavigateDown();
            else if (m_bPauseMenu)
                m_PauseMenu.NavigateDown();
            else if (m_bEndScreen)
                m_EndScreen.NavigateDown();
            else
            {
                if (m_iSelectedRow >= 0)
                {
                    // Deal with the grid
                    int iMidIcon = (m_iSize + 1) / 2;
                    int iFinalIcon = m_Puzzle.m_Rows[m_iSelectedRow].m_Cells[m_iSelectedCol].m_iFinalIcon;
                    if (m_iSelectedIcon < iMidIcon && iFinalIcon < 0)
                    {
                        // Top icon row
                        m_iSelectedIcon += iMidIcon;
                        m_iSelectedIcon = Math.Min(m_iSelectedIcon, (m_iSize - 1));
                    }
                    else
                    {
                        // Bottom icon row
                        if (m_iSelectedRow < (m_iSize - 1))
                        {
                            m_iSelectedRow++;
                            if (iFinalIcon < 0)
                                m_iSelectedIcon -= iMidIcon;
                            else
                                m_iSelectedIcon = 1;
                        }
                        else
                        {
                            Point XY = GetIconXY(m_iSelectedRow, m_iSelectedCol, m_iSelectedIcon);
                            XY.Y += 100;
                            for (int i = 0; i < m_aVerticalClues.Length; i++)
                            {
                                if (m_aVerticalClues[i].m_rBounds.Contains(XY) || XY.X < m_aVerticalClues[i].m_rBounds.X)
                                {
                                    m_iSelectedVerticalClue = i;
                                    SelectVerticalClue();
                                    break;
                                }
                            }
                            m_iSelectedRow = -1;
                        }
                    }
                    m_iSelectedIcon = Math.Min(Math.Max(m_iSelectedIcon, 0), (m_iSize - 1));
                }
                else if (m_iSelectedVerticalClue >= 0)
                {
                }
                else if (m_iSelectedHorizontalClue >= 0)
                {
                    if ((m_iSelectedHorizontalClue + 1) % m_iNumHorizontalCluesPerColumn != 0)
                        m_iSelectedHorizontalClue++;
                    else
                        ScrollDown();

                    m_iSelectedHorizontalClue = Math.Min(m_iSelectedHorizontalClue, m_aHorizontalClues.Length - 1);

                    SelectHorizontalClue();
                }
                else
                {
                    // Nothing selected?  select the default clue
                    SelectDefault();
                }
                m_SoundManager.PlayGameNavigate();
            }
        }

        public void NavigateLeft()
        {
            if (m_bOptionsDialog)
                m_Options.NavigateLeft();
            else if (m_bMainMenu)
                m_MainMenu.NavigateLeft();
            else if (m_bPauseMenu)
                m_PauseMenu.NavigateLeft();
            else if (m_bEndScreen)
                m_EndScreen.NavigateLeft();
            else
            {
                if (m_iSelectedRow >= 0)
                {
                    // Deal with the grid
                    int iMidIcon = (m_iSize + 1) / 2;
                    int iFinalIcon = m_Puzzle.m_Rows[m_iSelectedRow].m_Cells[m_iSelectedCol].m_iFinalIcon;
                    if (m_iSelectedIcon == iMidIcon || m_iSelectedIcon == 0 || iFinalIcon >= 0)
                    {
                        if (m_iSelectedCol > 0)
                        {
                            // Move to next column
                            m_iSelectedCol--;
                            if (iFinalIcon < 0)
                                m_iSelectedIcon += (iMidIcon - 1);
                            else
                                m_iSelectedIcon = iMidIcon - 1; ;
                            m_iSelectedIcon = Math.Min(m_iSelectedIcon, (m_iSize - 1));
                        }
                    }
                    else
                    {
                        // Move to the next icon
                        m_iSelectedIcon--;
                    }
                    m_iSelectedIcon = Math.Min(Math.Max(m_iSelectedIcon, 0), (m_iSize - 1));
                }
                else if (m_iSelectedVerticalClue >= 0)
                {
                    if (m_iSelectedVerticalClue > 0)
                        m_iSelectedVerticalClue--;
                    else
                        ScrollLeft();

                    m_iSelectedVerticalClue = Math.Min(m_iSelectedVerticalClue, m_aVerticalClues.Length);
                    SelectVerticalClue();
                }
                else if (m_iSelectedHorizontalClue >= 0)
                {
                    if (m_iSelectedHorizontalClue < m_iNumHorizontalCluesPerColumn)
                    {
                        m_iSelectedCol = m_iSize - 1;
                        for (int i = 0; i < m_aDisplayRows.Length; i++)
                        {
                            int iY = m_aHorizontalClues[m_iSelectedHorizontalClue].m_rBounds.Y;
                            if (iY > m_aDisplayRows[i].m_rBounds.Bottom)
                                continue;

                            m_iSelectedRow = i;
                            int iFinalIcon = m_Puzzle.m_Rows[m_iSelectedRow].m_Cells[m_iSelectedCol].m_iFinalIcon;
                            if (iFinalIcon >= 0)
                                m_iSelectedIcon = iFinalIcon;
                            else if (iY > m_aDisplayRows[i].m_aCells[m_iSize - 1].m_aDisplayRects[(m_iSize - 1) / 2].Bottom)
                                m_iSelectedIcon = (m_iSize - 1);
                            else
                                m_iSelectedIcon = (m_iSize - 1) / 2;
                            break;
                        }
                        m_iSelectedHorizontalClue = -1;
                    }
                    else
                    {
                        m_iSelectedHorizontalClue -= m_iNumHorizontalCluesPerColumn;
                        SelectHorizontalClue();
                    }
                }
                else
                {
                    // Nothing selected?  select the default clue
                    SelectDefault();
                }
                m_SoundManager.PlayGameNavigate();
            }
        }

        public void NavigateRight()
        {
            if (m_bOptionsDialog)
                m_Options.NavigateRight();
            else if (m_bMainMenu)
                m_MainMenu.NavigateRight();
            else if (m_bPauseMenu)
                m_PauseMenu.NavigateRight();
            else if (m_bEndScreen)
                m_EndScreen.NavigateRight();
            else
            {
                if (m_iSelectedRow >= 0)
                {
                    // Deal with the grid
                    int iMidIcon = (m_iSize - 1) / 2;
                    int iEndIcon = (m_iSize - 1);
                    int iFinalIcon = m_Puzzle.m_Rows[m_iSelectedRow].m_Cells[m_iSelectedCol].m_iFinalIcon;
                    if (m_iSelectedIcon == iMidIcon || m_iSelectedIcon == iEndIcon || iFinalIcon >= 0)
                    {
                        if (m_iSelectedCol < (m_iSize - 1))
                        {
                            // Move to next column
                            m_iSelectedCol++;
                            if (iFinalIcon >= 0)
                                m_iSelectedIcon = 0;
                            else if (m_iSelectedIcon == iEndIcon)
                                m_iSelectedIcon = (m_iSize + 1) / 2;
                            else
                                m_iSelectedIcon -= iMidIcon;
                        }
                        else
                        {
                            // Move to horizontal clue bank
                            Point XY = GetIconXY(m_iSelectedRow, m_iSelectedCol, m_iSelectedIcon);
                            for (int i = 0; i < m_iNumHorizontalCluesPerColumn; i++)
                            {
                                if (m_aHorizontalClues[i].m_rBounds.Bottom > XY.Y)
                                {
                                    m_iSelectedHorizontalClue = i;
                                    SelectHorizontalClue();
                                    break;
                                }
                            }
                            m_iSelectedRow = -1;
                        }
                    }
                    else
                    {
                        // Move to the next icon
                        m_iSelectedIcon++;
                    }

                    m_iSelectedIcon = Math.Min(Math.Max(m_iSelectedIcon, 0), (m_iSize - 1));
                }
                else if (m_iSelectedVerticalClue >= 0)
                {
                    if (m_iSelectedVerticalClue < m_aVerticalClues.Length - 1)
                        m_iSelectedVerticalClue++;
                    else
                        ScrollRight();
                    
                    SelectVerticalClue();
                }
                else if (m_iSelectedHorizontalClue >= 0)
                {
                    int iClueColumn = m_iSelectedHorizontalClue / m_iNumHorizontalCluesPerColumn;
                    if (iClueColumn < (m_iNumHorizontalClueColumns - 1))
                        m_iSelectedHorizontalClue += m_iNumHorizontalCluesPerColumn;

                    m_iSelectedHorizontalClue = Math.Min(m_iSelectedHorizontalClue, m_aHorizontalClues.Length - 1);

                    SelectHorizontalClue();
                }
                else
                {
                    // Nothing selected?  select the default clue
                    SelectDefault();
                }
                m_SoundManager.PlayGameNavigate();
            }
        }

        public void JumpUp()
        {
            if (m_bPauseMenu)
            {
                m_PauseMenu.ScrollUp();
            }
            else if (!m_bMainMenu && !m_bEndScreen)
            {
                if (m_iSelectedRow > 0)
                {
                    m_iSelectedRow--;
                }
                else
                {
                    m_SoundManager.m_bSupressGameNavigation = true;
                    for( int i = 0; i < 4; i++ )
                        NavigateUp();
                    m_SoundManager.m_bSupressGameNavigation = false;
                }
                m_SoundManager.PlayGameJump();
            }
        }

        public void JumpDown()
        {
            if (m_bPauseMenu)
            {
                m_PauseMenu.ScrollDown();
            }
            else if (!m_bMainMenu && !m_bEndScreen)
            {
                if (m_iSelectedRow >= 0)
                {
                    if (m_iSelectedRow < (m_iSize - 1))
                        m_iSelectedRow++;
                    else
                    {
                        Point XY = GetIconXY(m_iSelectedRow, m_iSelectedCol, m_iSelectedIcon);
                        XY.Y += 100;
                        for (int i = 0; i < m_aVerticalClues.Length; i++)
                        {
                            if (m_aVerticalClues[i].m_rBounds.Contains(XY) || XY.X < m_aVerticalClues[i].m_rBounds.X)
                            {
                                m_iSelectedVerticalClue = i;
                                SelectVerticalClue();
                                break;
                            }
                        }
                        m_iSelectedRow = -1;
                    }
                }
                else
                {
                    m_SoundManager.m_bSupressGameNavigation = true;
                    for (int i = 0; i < 4; i++)
                        NavigateDown();
                    m_SoundManager.m_bSupressGameNavigation = false;
                }
                m_SoundManager.PlayGameJump();
            }
        }

        public void JumpLeft()
        {
            if (!m_bPauseMenu && !m_bMainMenu && !m_bEndScreen)
            {
                if (m_iSelectedRow >= 0 && m_iSelectedCol > 0)
                {
                    m_iSelectedCol--;
                }
                else if (m_iSelectedHorizontalClue >= 0)
                {
                    m_SoundManager.m_bSupressGameNavigation = true;
                    NavigateLeft();
                    m_SoundManager.m_bSupressGameNavigation = false;
                }
                else
                {
                    m_SoundManager.m_bSupressGameNavigation = true;
                    for (int i = 0; i < 4; i++)
                        NavigateLeft();
                    m_SoundManager.m_bSupressGameNavigation = false;
                }
                m_SoundManager.PlayGameJump();
            }
        }

        public void JumpRight()
        {
            if (!m_bPauseMenu && !m_bMainMenu && !m_bEndScreen)
            {
                if (m_iSelectedRow >= 0)
                {
                    if (m_iSelectedCol < (m_iSize - 1))
                        m_iSelectedCol++;
                    else
                    {
                        // Move to horizontal clue bank
                        Point XY = GetIconXY(m_iSelectedRow, m_iSelectedCol, m_iSelectedIcon);
                        for (int i = 0; i < m_aHorizontalClues.Length; i += m_iNumHorizontalClueColumns)
                        {
                            if (m_aHorizontalClues[i].m_rBounds.Bottom > XY.Y)
                            {
                                m_iSelectedHorizontalClue = i;
                                SelectHorizontalClue();
                                break;
                            }
                        }
                        m_iSelectedRow = -1;
                    }
                }
                else if (m_iSelectedHorizontalClue >= 0)
                {
                    m_SoundManager.m_bSupressGameNavigation = true;
                    NavigateRight();
                    m_SoundManager.m_bSupressGameNavigation = false;
                }
                else
                {
                    m_SoundManager.m_bSupressGameNavigation = true;
                    for( int i = 0; i < 4; i++ )
                        NavigateRight();
                    m_SoundManager.m_bSupressGameNavigation = false;
                }
                m_SoundManager.PlayGameJump();
            }
        }

        public void JumpToVerticalClues()
        {
            if (m_iSelectedVerticalClue >= 0)
            {
                // Already in the vertical clue bank, jump to the grid
                m_iSelectedRow = (m_iSize / 2);
                m_iSelectedCol = (m_iSize / 2);
                m_iSelectedIcon = 0;
                m_iSelectedVerticalClue = -1;
            }
            else
            {
                m_iSelectedRow = -1;
                m_iSelectedVerticalClue = 0;
                m_iSelectedHorizontalClue = -1;
                SelectVerticalClue();
            }
            m_SoundManager.PlayGameJump();
        }

        public void JumpToHorizontalClues()
        {
            if (m_iSelectedHorizontalClue >= 0)
            {
                m_iSelectedRow = (m_iSize / 2);
                m_iSelectedCol = (m_iSize / 2);
                m_iSelectedIcon = 0;
                m_iSelectedHorizontalClue = -1;
            }
            else
            {
                m_iSelectedRow = -1;
                m_iSelectedHorizontalClue = 0;
                m_iSelectedVerticalClue = -1;
                SelectHorizontalClue();
            }
            m_SoundManager.PlayGameJump();
        }
        #endregion

        public void SelectItem()
        {
            if (m_bOptionsDialog)
            {
                if (!m_Options.CommitSelection())
                {
                    m_bOptionsDialog = false;
                    if (m_SaveGame != null)
                        m_SaveGame.SaveOptions(m_bAutoArangeClues, m_bShowClueDescriptions, m_bShowClock, m_bShowPuzzleNumber, m_bRandomizeIcons, m_SoundManager.m_fSoundVolume, m_SoundManager.m_fMusicVolume);

                    if (m_bAutoArangeClues)
                    {
                        if (m_aVisibleVerticalClues != null)
                        {
                            for (int i = m_aVisibleVerticalClues.Count - 1; i >= 0; i--)
                            {
                                Clue C = (Clue)m_aVisibleVerticalClues[i];
                                if (C == null)
                                    m_aVisibleVerticalClues.RemoveAt(i);
                            }
                        }

                        if (m_aVisibleHorizontalClues != null)
                        {
                            for (int i = m_aVisibleHorizontalClues.Count - 1; i >= 0; i--)
                            {
                                Clue C = (Clue)m_aVisibleHorizontalClues[i];
                                if (C == null)
                                    m_aVisibleHorizontalClues.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            else if (m_bMainMenu)
            {
                if (!m_MainMenu.CommitSelection())
                {
                    HandleMainMenuExit();                    
                }
            }
            else if (m_bPauseMenu)
            {
                if (!m_PauseMenu.CommitSelection())
                {
                    HandlePauseMenuExit();
                }
            }
            else if (m_bEndScreen)
            {
                if (!m_EndScreen.CommitSelection())
                {
                    HandleEndScreenExit();
                }
            }
            else
            {
                if (m_iSelectedRow >= 0)
                {
                    if (m_Puzzle.m_Rows[m_iSelectedRow].m_Cells[m_iSelectedCol].m_bValues[m_iSelectedIcon])
                        DoAction(eActionType.eAT_SetFinalIcon, m_iSelectedRow, m_iSelectedCol, m_iSelectedIcon);
                }
                else if (m_iSelectedHorizontalClue >= 0)
                {
                    SelectHorizontalClue();
                }
                else if (m_iSelectedVerticalClue >= 0)
                {
                    SelectVerticalClue();
                }
            }
        }

        public void CancelItem()
        {
            if (m_bOptionsDialog)
            {
                m_bOptionsDialog = m_Options.CancelSelection();
            }
            else if (m_bMainMenu)
            {
                m_bMainMenu = m_MainMenu.CancelSelection();
            }
            else if (m_bPauseMenu)
            {
                m_bPauseMenu = m_PauseMenu.CancelSelection();
            }
            else if (m_bEndScreen)
            {
                m_bEndScreen = m_EndScreen.CancelSelection();
            }
            else
            {
                if (m_iSelectedRow >= 0)
                {
                    if (m_Puzzle.m_Rows[m_iSelectedRow].m_Cells[m_iSelectedCol].m_iFinalIcon >= 0)
                    {
                        bool bGiven = false;
                        for (int i = 0; i < m_Puzzle.m_GivenClues.Count; i++)
                        {
                            Clue given = (Clue)m_Puzzle.m_GivenClues[i];
                            if (given.m_iRow == m_iSelectedRow && given.m_iCol == m_iSelectedCol)
                            {
                                bGiven = true;
                                break;
                            }
                        }

                        if (!bGiven)
                        {
                            m_Puzzle.m_Rows[m_iSelectedRow].Reset(m_iSelectedCol);
                            DoAction(eActionType.eAT_EliminateIcon, m_iSelectedRow, m_iSelectedCol, m_iSelectedIcon);
                        }
                    }
                    else if (m_Puzzle.m_Rows[m_iSelectedRow].m_Cells[m_iSelectedCol].m_bValues[m_iSelectedIcon])
                    {
                        DoAction(eActionType.eAT_EliminateIcon, m_iSelectedRow, m_iSelectedCol, m_iSelectedIcon);
                    }
                }
                else if (m_iSelectedVerticalClue >= 0)
                {
                    HideVerticalClue(m_iSelectedVerticalClue);
                }
                else if (m_iSelectedHorizontalClue >= 0)
                {
                    HideHorizontalClue(m_iSelectedHorizontalClue);
                }
            }
        }

        public void RestoreIcon()
        {
            if (!m_bOptionsDialog && !m_bMainMenu && !m_bPauseMenu && !m_bEndScreen)
            {
                if (m_iSelectedRow >= 0)
                {
                    if (m_Puzzle.m_Rows[m_iSelectedRow].m_Cells[m_iSelectedCol].m_iFinalIcon >= 0)
                    {
                        bool bGiven = false;
                        for (int i = 0; i < m_Puzzle.m_GivenClues.Count; i++)
                        {
                            Clue given = (Clue)m_Puzzle.m_GivenClues[i];
                            if (given.m_iRow == m_iSelectedRow && given.m_iCol == m_iSelectedCol)
                            {
                                bGiven = true;
                                break;
                            }
                        }

                        if (!bGiven)
                            m_Puzzle.m_Rows[m_iSelectedRow].Reset(m_iSelectedCol);
                    }
                    else if (!m_Puzzle.m_Rows[m_iSelectedRow].m_Cells[m_iSelectedCol].m_bValues[m_iSelectedIcon])
                    {
                        DoAction(eActionType.eAT_RestoreIcon, m_iSelectedRow, m_iSelectedCol, m_iSelectedIcon);
                    }
                }
            }
        }

        private void ShowMainMenu()
        {            
#if XBOX
            if (m_CurrentGamer != null)
                m_CurrentGamer.Presence.PresenceMode = GamerPresenceMode.AtMenu;
#endif
            m_SoundManager.PlayMainMenuMusic();
            m_Input.SetRepeatDelay(0.15);
            m_bMainMenu = true;
        }

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

#if XBOX
            if (m_CurrentGamer != null)
            {
                int iDifficulty = (m_iSize - 3) + m_iDifficulty;
                switch (iDifficulty)
                {
                    case 0:
                    case 1:
                        m_CurrentGamer.Presence.PresenceMode = GamerPresenceMode.DifficultyEasy;
                        break;
                    case 2:
                    case 3:
                        m_CurrentGamer.Presence.PresenceMode = GamerPresenceMode.DifficultyMedium;
                        break;
                    case 4:
                    case 5:
                        m_CurrentGamer.Presence.PresenceMode = GamerPresenceMode.DifficultyHard;
                        break;
                    case 6:
                    case 7:
                    default:
                        m_CurrentGamer.Presence.PresenceMode = GamerPresenceMode.DifficultyExtreme;
                        break;
                }
            }
#endif
        }

        private void HandlePauseMenuExit()
        {
            m_bPauseMenu = false;
            m_Input.SetRepeatDelay(m_dfGameRepeatDelay);
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

        private void HandleEndScreenExit()
        {
            m_bEndScreen = false;
            m_Input.SetRepeatDelay(m_dfGameRepeatDelay);
            switch (m_EndScreen.m_iSelection)
            {
                case 0: // Next Puzzle
                    SetPuzzleNumber(GetDisplayPuzzleNumber() + 1);
                    InitPuzzle(true);
                    break;
                case 1: // Restart Puzzle
                    m_Puzzle.Reset();
                    UnHideAllClues();
                    break;
                case 2: // Main Menu
                    ShowMainMenu();
                    break;
            }
        }

        private void SetPuzzleNumber(int iDisplayNumber)
        {
            m_iPuzzleIndex = (iDisplayNumber << 5) | ((m_iSize - 3) << 2) | (m_iDifficulty);
        }

        private int GetDisplayPuzzleNumber()
        {
            return (m_iPuzzleIndex >> 5);
        }

        private int GetClickedRow(int iX, int iY)
        {
            for (int i = 0; i < m_aDisplayRows.Length; i++)
            {
                if (m_aDisplayRows[i].m_rBounds.Contains(iX, iY))
                    return i;
            }

            return -1;
        }

        private int GetClickedCol(int iX, int iY)
        {
            int iRow = GetClickedRow(iX, iY);
            if (iRow >= 0)
            {
                for (int i = 0; i < m_aDisplayRows[iRow].m_aCells.Length; i++)
                {
                    if (m_aDisplayRows[iRow].m_aCells[i].m_rBounds.Contains(iX, iY))
                        return i;
                }
            }
            return -1;
        }

        private int GetClickedIcon(int iX, int iY)
        {
            int iRow = GetClickedRow(iX, iY);
            if (iRow >= 0)
            {
                int iCol = GetClickedCol(iX, iY);
                if (iCol >= 0)
                {
                    int iFinal = m_Puzzle.m_Rows[iRow].m_Cells[iCol].m_iFinalIcon;
                    if (iFinal < 0)
                    {
                        for (int i = 0; i < m_aDisplayRows[iRow].m_aCells[iCol].m_aDisplayRects.Length; i++)
                        {
                            if (m_aDisplayRows[iRow].m_aCells[iCol].m_aDisplayRects[i].Contains(iX, iY))
                                return i;
                        }
                    }
                    else
                        return iFinal;
                }
            }
            return -1;
        }

        #region Drawing
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Viewport = m_vpFull;
            GraphicsDevice.Clear(Color.Black);

            if (m_bLetterbox)
            {
                m_vpDraw = m_vpFull;
                m_vpDraw.Y = m_iScreenTop;
                m_vpDraw.Height = m_iScreenHeight;
                GraphicsDevice.Viewport = m_vpDraw;
            }

            spriteBatch.Begin(SpriteSortMode.Immediate);

            if (m_bDoneLoading)
            {
                if (m_bMainMenu)
                {
                    m_MainMenu.Draw(gameTime, spriteBatch);
                }
                else if (m_bPauseMenu)
                {
                    m_PauseMenu.Draw(gameTime, spriteBatch);
                }
                else
                {
                    spriteBatch.Draw(m_Background, new Rectangle(0, 0, m_iScreenWidth, m_iScreenHeight), Color.White);

                    DrawGrid();
                    DrawVerticalClues();
                    DrawHorizontalClues();
                    DrawScrollBars();

                    if (m_bShowPuzzleNumber)
                        DrawPuzzleNumber();
                    if (m_bShowClock)
                        DrawClock();

                    if (m_bEndScreen)
                    {
                        m_EndScreen.Draw(gameTime, spriteBatch);
                    }
                    else
                    {
                        if (m_bShowClueDescriptions)
                            DrawClueDescription();

                        if (m_Input.IsControllerConnected())
                            DrawSelectionIcon();

                        DrawDragIcon();
                    }
                }

                if (m_bOptionsDialog)
                {
                    m_Options.Draw(gameTime, spriteBatch);
                }
            }
            else
            {
                spriteBatch.Draw(m_Splash, new Rectangle(0, 0, m_iScreenWidth, m_iScreenHeight), Color.White);
                m_bCanLoad = true;
            }
                        
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawSelectionIcon()
        {
            if (m_iSelectedVerticalClue >= 0)
            {
                bool b2Tall = false;
                int iClueIndex = m_iFirstVisibleVerticalClue + m_iSelectedVerticalClue;
                if (iClueIndex < m_aVisibleVerticalClues.Count)
                {
                    Clue C = (Clue)m_aVisibleVerticalClues[iClueIndex];
                    if (C != null)
                    {
                        int[] iIcons = new int[3];
                        int iNumIcons = C.GetIcons(m_Puzzle, iIcons);
                        if (iNumIcons <= 2)
                            b2Tall = true;
                    }
                }

                if (b2Tall)
                {
                    Rectangle rRect = m_aVerticalClues[m_iSelectedVerticalClue].m_rBounds;
                    rRect.Height -= (rRect.Height / 3);
                    spriteBatch.Draw(m_SelectionIcon2Tall, rRect, Color.White);
                }
                else
                    spriteBatch.Draw(m_SelectionIcon3Tall, m_aVerticalClues[m_iSelectedVerticalClue].m_rBounds, Color.White);
            }
            else if (m_iSelectedHorizontalClue >= 0)
            {
                spriteBatch.Draw(m_SelectionIconWide, m_aHorizontalClues[m_iSelectedHorizontalClue].m_rBounds, Color.White);
            }

            if (m_iSelectedRow >= 0)
            {
                if (m_Puzzle.m_Rows[m_iSelectedRow].m_Cells[m_iSelectedCol].m_iFinalIcon >= 0)
                {
                    // Draw selection icon around the final icon
                    spriteBatch.Draw(m_SelectionIcon, m_aDisplayRows[m_iSelectedRow].m_aCells[m_iSelectedCol].m_rFinal, Color.White);
                }
                else
                {
                    // Draw selection icon around a small icon
                    spriteBatch.Draw(m_SelectionIcon, m_aDisplayRows[m_iSelectedRow].m_aCells[m_iSelectedCol].m_aDisplayRects[m_iSelectedIcon], Color.White);
                }
            }
        }

        private void DrawGrid()
        {
            for (int iRow = 0; iRow < m_iSize; iRow++)
            {
                for (int iCol = 0; iCol < m_iSize; iCol++)
                {
                    // Draw the background
                    Rectangle rTemp = m_aDisplayRows[iRow].m_aCells[iCol].m_rBounds;                    
                    spriteBatch.Draw(m_TransGrey, rTemp, Color.White);

                    // Draw the gold bars
                    rTemp = m_aDisplayRows[iRow].m_aCells[iCol].m_rBounds;
                    rTemp.X -= 3;
                    rTemp.Y -= 1;
                    rTemp.Width = 3;
                    rTemp.Height += 2;
                    spriteBatch.Draw(m_GoldBarVertical, rTemp, Color.White);
                    rTemp.X = m_aDisplayRows[iRow].m_aCells[iCol].m_rBounds.Right;
                    spriteBatch.Draw(m_GoldBarVertical, rTemp, Color.White);
                    rTemp = m_aDisplayRows[iRow].m_aCells[iCol].m_rBounds;
                    rTemp.Y -= 1;
                    rTemp.Height = 1;
                    spriteBatch.Draw(m_GoldBarHorizontal, rTemp, Color.White);
                    rTemp.Y = m_aDisplayRows[iRow].m_aCells[iCol].m_rBounds.Bottom;
                    spriteBatch.Draw(m_GoldBarHorizontal, rTemp, Color.White);

                    int iFinal = m_Puzzle.m_Rows[iRow].m_Cells[iCol].m_iFinalIcon;
                    if (iFinal >= 0)
                    {
                        //if (iFinal == m_Puzzle.m_Solution[iRow, iCol])
                            spriteBatch.Draw(m_aIcons[iRow, iFinal], m_aDisplayRows[iRow].m_aCells[iCol].m_rFinal, Color.White);
                        //else
                        //    spriteBatch.Draw(m_aIcons[iRow, iFinal], m_aDisplayRows[iRow].m_aCells[iCol].m_rFinal, Color.Red);

                        if (m_Hint != null && m_Hint.ShouldDraw(iRow, iCol, iFinal))
                            m_HintSprite.Draw(spriteBatch, m_aDisplayRows[iRow].m_aCells[iCol].m_rFinal, Color.White);
                    }
                    else
                    {
                        for (int iIcon = 0; iIcon < m_iSize; iIcon++)
                        {
                            if( m_Puzzle.m_Rows[iRow].m_Cells[iCol].m_bValues[iIcon] )
                            {
                                spriteBatch.Draw(m_aIcons[iRow, iIcon], m_aDisplayRows[iRow].m_aCells[iCol].m_aDisplayRects[iIcon], Color.White);

                                if (m_Hint != null && m_Hint.ShouldDraw(iRow, iCol, iIcon))
                                    m_HintSprite.Draw(spriteBatch, m_aDisplayRows[iRow].m_aCells[iCol].m_aDisplayRects[iIcon], Color.White);
                            }
                        }
                    }
                }
            }
        }

        private void DrawVerticalClues()
        {
            Rectangle rTemp;
            for (int i = 0; i < m_aVerticalClues.Length; i++)
            {
                rTemp = m_aVerticalClues[i].m_rBounds;
                spriteBatch.Draw(m_TransGrey, rTemp, Color.White);
                rTemp.X -= 3;
                rTemp.Width = 3;
                spriteBatch.Draw(m_GoldBarVertical, rTemp, Color.White);
            }
            rTemp = m_aVerticalClues[m_aVerticalClues.Length - 1].m_rBounds;
            rTemp.X = rTemp.Right;
            rTemp.Width = 3;
            spriteBatch.Draw(m_GoldBarVertical, rTemp, Color.White);
            rTemp = m_aVerticalClues[0].m_rBounds;
            rTemp.X -= 3;
            rTemp.Y -= 1;
            rTemp.Width = m_aVerticalClues[m_aVerticalClues.Length - 1].m_rBounds.Right - rTemp.X;
            rTemp.Height = 1;
            spriteBatch.Draw(m_GoldBarHorizontal, rTemp, Color.White);
            rTemp.Y = m_aVerticalClues[0].m_rBounds.Bottom;
            spriteBatch.Draw(m_GoldBarHorizontal, rTemp, Color.White);            

            int iNumToDraw = Math.Min(m_aVisibleVerticalClues.Count, m_aVerticalClues.Length);
            for (int i = 0; i < iNumToDraw; i++)
            {
                Clue C = (Clue)m_aVisibleVerticalClues[m_iFirstVisibleVerticalClue + i];
                if (C == null)
                    continue;

                int[] iIcons = new int[3];
                int[] iRows = C.GetRows();
                int iNumIcons = C.GetIcons(m_Puzzle, iIcons);

                bool bHintClue = false;
                if (m_Hint != null && m_Hint.ShouldDraw(C))
                    bHintClue = true;

                // Draw the icons
                for (int j = 0; j < iNumIcons; j++)
                {
                    spriteBatch.Draw(m_aIcons[iRows[j], iIcons[j]], m_aVerticalClues[i].m_aDisplayRects[j], Color.White);
                    if (bHintClue)
                        m_HintSprite.Draw(spriteBatch, m_aVerticalClues[i].m_aDisplayRects[j], Color.White);
                }

                // Draw the operational overlay
                switch (C.m_VerticalType)
                {
                    case eVerticalType.Two:
                    case eVerticalType.Three:
                        break;
                    case eVerticalType.EitherOr:
                        spriteBatch.Draw(m_EitherOrOverlay, m_aVerticalClues[i].m_aOverlayRects[1], Color.White);
                        break;
                    case eVerticalType.TwoNot:
                        spriteBatch.Draw(m_NotOverlay, m_aVerticalClues[i].m_aOverlayRects[0], Color.White);
                        break;
                    case eVerticalType.ThreeTopNot:
                        spriteBatch.Draw(m_NotOverlay, m_aVerticalClues[i].m_aDisplayRects[0], Color.White);
                        break;
                    case eVerticalType.ThreeMidNot:
                        spriteBatch.Draw(m_NotOverlay, m_aVerticalClues[i].m_aDisplayRects[1], Color.White);
                        break;
                    case eVerticalType.ThreeBotNot:
                        spriteBatch.Draw(m_NotOverlay, m_aVerticalClues[i].m_aDisplayRects[2], Color.White);
                        break;
                }
            }
        }

        private void DrawHorizontalClues()
        {
            Rectangle rTemp;
            for (int i = 0; i < m_aHorizontalClues.Length; i++)
            {
                rTemp = m_aHorizontalClues[i].m_rBounds;
                spriteBatch.Draw(m_TransGrey, rTemp, Color.White);
                rTemp.X -= 3;
                rTemp.Y -= 3;
                rTemp.Width += 6;
                rTemp.Height = 3;
                spriteBatch.Draw(m_GoldBarHorizontal, rTemp, Color.White);
                rTemp = m_aHorizontalClues[i].m_rBounds;
                rTemp.X -= 3;
                rTemp.Y = rTemp.Bottom;
                rTemp.Width += 6;
                rTemp.Height = 3;
                spriteBatch.Draw(m_GoldBarHorizontal, rTemp, Color.White);
                rTemp = m_aHorizontalClues[i].m_rBounds;                
                rTemp.X -= 3;
                rTemp.Y -= 1;
                rTemp.Width = 3;
                rTemp.Height += 2;
                spriteBatch.Draw(m_GoldBarVertical, rTemp, Color.White);
                rTemp = m_aHorizontalClues[i].m_rBounds;
                rTemp.X = rTemp.Right;
                rTemp.Y -= 1;
                rTemp.Width = 3;
                rTemp.Height += 2;
                spriteBatch.Draw(m_GoldBarVertical, rTemp, Color.White);
            }

            int iNumToDraw = Math.Min(m_aVisibleHorizontalClues.Count, m_aHorizontalClues.Length);
            for (int i = 0; i < iNumToDraw; i++)
            {
                Clue C = (Clue)m_aVisibleHorizontalClues[m_iFirstVisibleHorizontalClue + i];
                if (C == null)
                    continue;

                int[] iIcons = new int[3];
                int[] iRows = C.GetRows();
                int iNumIcons = C.GetIcons(m_Puzzle, iIcons);

                bool bHintClue = false;
                if (m_Hint != null && m_Hint.ShouldDraw(C))
                    bHintClue = true;

                // Draw the icons
                if (C.m_HorizontalType == eHorizontalType.LeftOf || C.m_HorizontalType == eHorizontalType.NotLeftOf)
                {
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], m_aHorizontalClues[i].m_aDisplayRects[0], Color.White);
                    spriteBatch.Draw(m_LeftOfIcon, m_aHorizontalClues[i].m_aDisplayRects[1], Color.White);
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], m_aHorizontalClues[i].m_aDisplayRects[2], Color.White);

                    if (bHintClue)
                    {
                        m_HintSprite.Draw(spriteBatch, m_aHorizontalClues[i].m_aDisplayRects[0], Color.White);
                        m_HintSprite.Draw(spriteBatch, m_aHorizontalClues[i].m_aDisplayRects[2], Color.White);
                    }
                }
                else
                {
                    for (int j = 0; j < iNumIcons; j++)
                    {
                        spriteBatch.Draw(m_aIcons[iRows[j], iIcons[j]], m_aHorizontalClues[i].m_aDisplayRects[j], Color.White);
                        if( bHintClue)
                            m_HintSprite.Draw(spriteBatch, m_aHorizontalClues[i].m_aDisplayRects[j], Color.White);
                    }
                }

                // Draw the operational overlay
                switch (C.m_HorizontalType)
                {
                    case eHorizontalType.NextTo:
                    case eHorizontalType.LeftOf:
                        break;
                    case eHorizontalType.NotLeftOf:
                    case eHorizontalType.NotNextTo:
                        spriteBatch.Draw(m_NotOverlay, m_aHorizontalClues[i].m_aDisplayRects[1], Color.White);
                        break;
                    case eHorizontalType.Span:
                        spriteBatch.Draw(m_SpanOverlay, m_aHorizontalClues[i].m_rBounds, Color.White);
                        break;
                    case eHorizontalType.SpanNotLeft:
                        spriteBatch.Draw(m_SpanOverlay, m_aHorizontalClues[i].m_rBounds, Color.White);
                        spriteBatch.Draw(m_NotOverlay, m_aHorizontalClues[i].m_aDisplayRects[0], Color.White);
                        break;
                    case eHorizontalType.SpanNotMid:
                        spriteBatch.Draw(m_SpanOverlay, m_aHorizontalClues[i].m_rBounds, Color.White);
                        spriteBatch.Draw(m_NotOverlay, m_aHorizontalClues[i].m_aDisplayRects[1], Color.White);
                        break;
                    case eHorizontalType.SpanNotRight:
                        spriteBatch.Draw(m_SpanOverlay, m_aHorizontalClues[i].m_rBounds, Color.White);
                        spriteBatch.Draw(m_NotOverlay, m_aHorizontalClues[i].m_aDisplayRects[2], Color.White);
                        break;
                }
            }
        }

        private void DrawScrollBars()
        {
            if (m_bVerticalScrollBar)
            {
                int iX = 0;
                int iTop = 720;
                int iBottom = 0;
                for (int i = 0; i < m_aHorizontalClues.Length; i++)
                {
                    iX = Math.Max(m_aHorizontalClues[i].m_rBounds.Right, iX);
                    iTop = Math.Min(m_aHorizontalClues[i].m_rBounds.Top, iTop);
                    iBottom = Math.Max(m_aHorizontalClues[i].m_rBounds.Bottom, iBottom);
                }
                iX += 4;

                // Draw bar
                spriteBatch.Draw(m_ScrollBar, new Rectangle(iX, iTop, 16, iBottom - iTop), Color.White);
                
                // Draw Cursor
                float fClueCount = (float)((m_aVisibleHorizontalClues.Count / m_iNumHorizontalClueColumns) - m_iNumHorizontalCluesPerColumn);
                float fDisplayCount = (float)m_iNumHorizontalCluesPerColumn;
                float fPosition = (float)m_iFirstVisibleHorizontalClue / fClueCount;
                float fY = (float)((iBottom - iTop) - 32) * fPosition;
                int iY = (int)fY + iTop;
                spriteBatch.Draw(m_ScrollCursor, new Rectangle(iX, iY + 8, 16, 16), Color.White);

                // Draw top arrow
                m_rVerticalScrollBarUp = new Rectangle(iX, iTop, 16, 16);
                spriteBatch.Draw(m_ScrollArrow, m_rVerticalScrollBarUp, Color.White);

                // Draw bottom arrow
                m_rVerticalScrollBarDown = new Rectangle(iX, iBottom - 16, 16, 16);
                spriteBatch.Draw(m_ScrollArrow, m_rVerticalScrollBarDown, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipVertically, 0);
            }

            if (m_bHorizontalScrollBar)
            {
                int iX = 1280;
                int iTop = 0;
                int iRight = 900;
                for (int i = 0; i < m_aVerticalClues.Length; i++)
                {
                    iX = Math.Min(m_aVerticalClues[i].m_rBounds.Left, iX);
                    iTop = Math.Max(m_aVerticalClues[i].m_rBounds.Bottom, iTop);
                }
                iTop += 2;

                // Draw bar
                spriteBatch.Draw(m_ScrollBar, new Rectangle(iX, iTop, iRight - iX, 16), Color.White);
                
                // Draw Cursor
                float fDisplayCount = (float)m_aVerticalClues.Length;
                float fPosition = (float)m_iFirstVisibleVerticalClue / (m_aVisibleVerticalClues.Count - m_aVerticalClues.Length);
                float fX = (float)((iRight - iX) - 32) * fPosition;
                int iLeft = (int)fX + iX;
                spriteBatch.Draw(m_ScrollCursor, new Rectangle(iLeft + 24, iTop, 16, 16), null, Color.White, (float)(Math.PI / 2), new Vector2(0, 0), SpriteEffects.None, 0);

                // Draw top arrow
                m_rHorizontalScrollBarLeft = new Rectangle(iX, iTop, 16, 16);
                spriteBatch.Draw(m_ScrollArrow, new Rectangle(iX + 16, iTop, 16, 16), null, Color.White, (float)-(Math.PI/2), new Vector2(15,15), SpriteEffects.None, 0);

                // Draw bottom arrow
                m_rHorizontalScrollBarRight = new Rectangle(iRight - 16, iTop, 16, 16);
                spriteBatch.Draw(m_ScrollArrow, new Rectangle(iRight, iTop, 16, 16), null, Color.White, (float)-(Math.PI + (Math.PI / 2)), new Vector2(0,0), SpriteEffects.None, 0);
            }
        }

        private void DrawString(string szText, int iX, int iY, Color cColor)
        {
            spriteBatch.DrawString(m_DialogFont, szText, new Vector2(iX + 2, iY + 2), Color.Black);
            spriteBatch.DrawString(m_DialogFont, szText, new Vector2(iX, iY), cColor);
        }

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

        private void DrawClueDescription()
        {
            if (m_SelectedClue != null)
            {
                switch (m_SelectedClue.m_Type)
                {
                    case eClueType.Horizontal:
                        DrawClueDescription_Horizontal();
                        break;
                    case eClueType.Vertical:
                        DrawClueDescription_Vertical();
                        break;
                }
            }
        }

        private void DrawClueDescription_Horizontal()
        {
            int iX = m_iClueDescriptionX;
            int[] iIcons = new int[3];
            int[] iRows = m_SelectedClue.GetRows();
            m_SelectedClue.GetIcons(m_Puzzle, iIcons);
            string szDesc;

            switch (m_SelectedClue.m_HorizontalType)
            {
                case eHorizontalType.NextTo:
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is next to";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eHorizontalType.NotNextTo:
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is not next to";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eHorizontalType.LeftOf:
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is left of";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eHorizontalType.NotLeftOf:
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is not left of";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eHorizontalType.Span:
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "has";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "on one side, and";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[2], iIcons[2]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "on the other";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);                    
                    break;
                case eHorizontalType.SpanNotLeft:
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "has";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[2], iIcons[2]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "on one side, and not";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "on the other";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    break;
                case eHorizontalType.SpanNotMid:
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "and";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[2], iIcons[2]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "have one column between them without";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eHorizontalType.SpanNotRight:
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "has";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "on one side, and not";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[2], iIcons[2]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "on the other";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    break;
            }
        }

        private void DrawClueDescription_Vertical()
        {
            int iX = m_iClueDescriptionX;
            int[] iIcons = new int[3];
            int[] iRows = m_SelectedClue.GetRows();
            m_SelectedClue.GetIcons(m_Puzzle, iIcons);
            string szDesc;

            switch (m_SelectedClue.m_VerticalType)
            {
                case eVerticalType.Two:
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is in the same column as";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eVerticalType.Three:
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is in the same column as";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "and";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[2], iIcons[2]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eVerticalType.EitherOr:
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is either in the column with";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "or the column with";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[2], iIcons[2]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eVerticalType.TwoNot:
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;
                    szDesc = "is not in the same column as";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    break;
                case eVerticalType.ThreeTopNot:
                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "and";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;

                    spriteBatch.Draw(m_aIcons[iRows[2], iIcons[2]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "are in the same column but";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;

                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "is not";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    break;
                case eVerticalType.ThreeMidNot:
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "and";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;

                    spriteBatch.Draw(m_aIcons[iRows[2], iIcons[2]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "are in the same column but";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;

                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "is not";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    break;
                case eVerticalType.ThreeBotNot:
                    spriteBatch.Draw(m_aIcons[iRows[0], iIcons[0]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "and";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;

                    spriteBatch.Draw(m_aIcons[iRows[1], iIcons[1]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "are in the same column but";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    iX += (int)m_DialogFont.MeasureString(szDesc).X + 6;

                    spriteBatch.Draw(m_aIcons[iRows[2], iIcons[2]], new Rectangle(iX, m_iClueDescriptionY, m_iClueIconSize, m_iClueIconSize), Color.White);
                    iX += m_iClueIconSize + 6;

                    szDesc = "is not";
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX + 2, m_iClueDescriptionY + 2), Color.Black);
                    spriteBatch.DrawString(m_DialogFont, szDesc, new Vector2(iX, m_iClueDescriptionY), Color.White);
                    break;
            }
        }

        private void DrawDragIcon()
        {
            if (m_iDragRow >= 0 && m_iDragIcon >= 0)
            {
                spriteBatch.Draw(m_aIcons[m_iDragRow, m_iDragIcon], new Rectangle(m_Input.m_iMouseX, m_Input.m_iMouseY - m_iScreenTop, 30, 30), Color.White);
            }
        }
        #endregion
    }
}
