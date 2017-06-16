using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness
{
    public static class Assets
    {
        public static Texture2D Background;
        public static Texture2D Logo;

        // Towers
        public static Texture2D[] Towers;

        // Icons
        public static Texture2D[] Cars;
        public static Texture2D[] Cats;
        public static Texture2D[] Flowers;
        public static Texture2D[] Hubble;
        public static Texture2D[] Princesses;
        public static Texture2D[] Puppies;
        public static Texture2D[] Simpsons;
        public static Texture2D[] Superheros;
        public static Dictionary<string, Texture2D> IconMap;

        // Markers
        public static AnimatedSprite WaitIcon;
        public static AnimatedSprite HintSprite;
        public static Texture2D SelectionIconWide;
        public static Texture2D SelectionIconTall;
        public static Texture2D Arrow;

        // Clue Overlays
        public static Texture2D NotOverlay;
        public static Texture2D EitherOrOverlay;
        public static Texture2D SpanOverlay;
        public static Texture2D LeftOfIcon;

        // Static Parts
        public static Texture2D TransGray;
        public static Texture2D TransparentBox;
        public static Texture2D ScrollArrow;
        public static Texture2D ScrollBar;
        public static Texture2D ScrollCursor;
        public static Texture2D SliderBar;
        public static Texture2D SliderCursor;
        public static Texture2D GoldBarHorizontal;
        public static Texture2D GoldBarVertical;
        public static Texture2D GoldArrowLeft;
        public static Texture2D GoldArrowRight;
        public static Texture2D HelpBackground;
        public static Texture2D GoldCoin;
        public static Texture2D CheckBox;
        public static Texture2D Check;

        // Fonts
        public static SpriteFont MenuFont;
        public static SpriteFont DialogFont;
        public static SpriteFont HelpFont;

        // Signin Icons
        public static Texture2D Facebook;
        public static Texture2D Google;
        public static Texture2D Email;

        // Music
        public static Song MenuSong;
        public static Song[] GameSongs;

        // Sounds
        public static SoundEffect MenuNavigate;
        public static SoundEffect SliderMove;
        public static SoundEffect MenuAccept;
        public static SoundEffect GameLoad;
        public static SoundEffect GameSave;
        public static SoundEffect UnhideClues;
        public static SoundEffect GameAction1;
        public static SoundEffect GameAction2;
        public static SoundEffect GameAction3;
        public static SoundEffect GameAction4;
        public static SoundEffect GameAction5;
        public static SoundEffect GameAction6;
        public static SoundEffect PuzzleFailed;
        public static SoundEffect PuzzleComplete;
        public static SoundEffect HappinessWav;


        public static void LoadAll(ContentManager Content)
        {
            Background = Content.Load<Texture2D>("Clouds4");
            Logo = Content.Load<Texture2D>("Logo");

            LoadIcons(Content);
            LoadMarkers(Content);
            LoadOverlays(Content);
            LoadStaticParts(Content);
            LoadFonts(Content);
            LoadTowers(Content);
            LoadSignInIcons(Content);

            LoadMusic(Content);
            LoadSounds(Content);
        }

        static void LoadIcons(ContentManager Content)
        {
            Cars = new Texture2D[8];
            Cats = new Texture2D[8];
            Flowers = new Texture2D[8];
            Hubble = new Texture2D[8];
            Princesses = new Texture2D[8];
            Puppies = new Texture2D[8];
            Simpsons = new Texture2D[8];
            Superheros = new Texture2D[8];

            Cars[0] = Content.Load<Texture2D>("Cars/AudiLE");
            Cars[1] = Content.Load<Texture2D>("Cars/BugattiVeyron");
            Cars[2] = Content.Load<Texture2D>("Cars/GTO");
            Cars[3] = Content.Load<Texture2D>("Cars/Hummer");
            Cars[4] = Content.Load<Texture2D>("Cars/Lamborghini");
            Cars[5] = Content.Load<Texture2D>("Cars/Lotus");
            Cars[6] = Content.Load<Texture2D>("Cars/Porsche");
            Cars[7] = Content.Load<Texture2D>("Cars/Viper");

            Cats[0] = Content.Load<Texture2D>("Cats/DarkGreyCat");
            Cats[1] = Content.Load<Texture2D>("Cats/GreyCat");
            Cats[2] = Content.Load<Texture2D>("Cats/Kitten");
            Cats[3] = Content.Load<Texture2D>("Cats/OrangeCat");
            Cats[4] = Content.Load<Texture2D>("Cats/SnowConeCat");
            Cats[5] = Content.Load<Texture2D>("Cats/StripedCat");
            Cats[6] = Content.Load<Texture2D>("Cats/WaterCat");
            Cats[7] = Content.Load<Texture2D>("Cats/WhiteCat");

            Flowers[0] = Content.Load<Texture2D>("Flowers/BlueWildFlower");
            Flowers[1] = Content.Load<Texture2D>("Flowers/ConvolvulusArvensis");
            Flowers[2] = Content.Load<Texture2D>("Flowers/Dahlia");
            Flowers[3] = Content.Load<Texture2D>("Flowers/Dandelion");
            Flowers[4] = Content.Load<Texture2D>("Flowers/LotusFlower");
            Flowers[5] = Content.Load<Texture2D>("Flowers/OrangeGazania");
            Flowers[6] = Content.Load<Texture2D>("Flowers/Osteospermum");
            Flowers[7] = Content.Load<Texture2D>("Flowers/PinkRose");

            Hubble[0] = Content.Load<Texture2D>("Hubble/CatsEyeNebula");
            Hubble[1] = Content.Load<Texture2D>("Hubble/CigarNebula");
            Hubble[2] = Content.Load<Texture2D>("Hubble/CrabNebula");
            Hubble[3] = Content.Load<Texture2D>("Hubble/RaisingHeaven");
            Hubble[4] = Content.Load<Texture2D>("Hubble/RedRectangle");
            Hubble[5] = Content.Load<Texture2D>("Hubble/RingGalaxy");
            Hubble[6] = Content.Load<Texture2D>("Hubble/StarryNight");
            Hubble[7] = Content.Load<Texture2D>("Hubble/TwoGalaxies");

            Princesses[0] = Content.Load<Texture2D>("Princesses/Ariel");
            Princesses[1] = Content.Load<Texture2D>("Princesses/Belle");
            Princesses[2] = Content.Load<Texture2D>("Princesses/Cinderella");
            Princesses[3] = Content.Load<Texture2D>("Princesses/Dora");
            Princesses[4] = Content.Load<Texture2D>("Princesses/Jasmine");
            Princesses[5] = Content.Load<Texture2D>("Princesses/SleepingBeauty");
            Princesses[6] = Content.Load<Texture2D>("Princesses/SnowWhite");
            Princesses[7] = Content.Load<Texture2D>("Princesses/TinkerBell");

            Puppies[0] = Content.Load<Texture2D>("Puppies/BichonPuppy");
            Puppies[1] = Content.Load<Texture2D>("Puppies/BlackLabPuppy");
            Puppies[2] = Content.Load<Texture2D>("Puppies/BulldogPuppy");
            Puppies[3] = Content.Load<Texture2D>("Puppies/BWBPuppy");
            Puppies[4] = Content.Load<Texture2D>("Puppies/GoldenRetrieverPuppy");
            Puppies[5] = Content.Load<Texture2D>("Puppies/IrishTerrier");
            Puppies[6] = Content.Load<Texture2D>("Puppies/WhitePuppy");
            Puppies[7] = Content.Load<Texture2D>("Puppies/WinkingPuppy");

            Simpsons[0] = Content.Load<Texture2D>("Simpsons/Apu");
            Simpsons[1] = Content.Load<Texture2D>("Simpsons/Barney");
            Simpsons[2] = Content.Load<Texture2D>("Simpsons/Bartman");
            Simpsons[3] = Content.Load<Texture2D>("Simpsons/Homer");
            Simpsons[4] = Content.Load<Texture2D>("Simpsons/Lisa");
            Simpsons[5] = Content.Load<Texture2D>("Simpsons/Marge");
            Simpsons[6] = Content.Load<Texture2D>("Simpsons/Moe");
            Simpsons[7] = Content.Load<Texture2D>("Simpsons/MrBurns");

            Superheros[0] = Content.Load<Texture2D>("Superheros/Batman");
            Superheros[1] = Content.Load<Texture2D>("Superheros/CaptainAmerica");
            Superheros[2] = Content.Load<Texture2D>("Superheros/GreenLantern");
            Superheros[3] = Content.Load<Texture2D>("Superheros/Hulk");
            Superheros[4] = Content.Load<Texture2D>("Superheros/Spiderman");
            Superheros[5] = Content.Load<Texture2D>("Superheros/Superman");
            Superheros[6] = Content.Load<Texture2D>("Superheros/Wolverine");
            Superheros[7] = Content.Load<Texture2D>("Superheros/WonderWoman");
            
            IconMap = new Dictionary<string, Texture2D>();
            IconMap["Superheros[0]"] = Superheros[0];
            IconMap["Superheros[1]"] = Superheros[1];
            IconMap["Superheros[2]"] = Superheros[2];
            IconMap["Superheros[3]"] = Superheros[3];
            IconMap["Superheros[4]"] = Superheros[4];
            IconMap["Superheros[5]"] = Superheros[5];
            IconMap["Superheros[6]"] = Superheros[6];
            IconMap["Superheros[7]"] = Superheros[7];
            IconMap["Simpsons[0]"] = Simpsons[0];
            IconMap["Simpsons[1]"] = Simpsons[1];
            IconMap["Simpsons[2]"] = Simpsons[2];
            IconMap["Simpsons[3]"] = Simpsons[3];
            IconMap["Simpsons[4]"] = Simpsons[4];
            IconMap["Simpsons[5]"] = Simpsons[5];
            IconMap["Simpsons[6]"] = Simpsons[6];
            IconMap["Simpsons[7]"] = Simpsons[7];
            IconMap["Puppies[0]"] = Puppies[0];
            IconMap["Puppies[1]"] = Puppies[1];
            IconMap["Puppies[2]"] = Puppies[2];
            IconMap["Puppies[3]"] = Puppies[3];
            IconMap["Puppies[4]"] = Puppies[4];
            IconMap["Puppies[5]"] = Puppies[5];
            IconMap["Puppies[6]"] = Puppies[6];
            IconMap["Puppies[7]"] = Puppies[7];
            IconMap["Princesses[0]"] = Princesses[0];
            IconMap["Princesses[1]"] = Princesses[1];
            IconMap["Princesses[2]"] = Princesses[2];
            IconMap["Princesses[3]"] = Princesses[3];
            IconMap["Princesses[4]"] = Princesses[4];
            IconMap["Princesses[5]"] = Princesses[5];
            IconMap["Princesses[6]"] = Princesses[6];
            IconMap["Princesses[7]"] = Princesses[7];
            IconMap["Hubble[0]"] = Hubble[0];
            IconMap["Hubble[1]"] = Hubble[1];
            IconMap["Hubble[2]"] = Hubble[2];
            IconMap["Hubble[3]"] = Hubble[3];
            IconMap["Hubble[4]"] = Hubble[4];
            IconMap["Hubble[5]"] = Hubble[5];
            IconMap["Hubble[6]"] = Hubble[6];
            IconMap["Hubble[7]"] = Hubble[7];
            IconMap["Flowers[0]"] = Flowers[0];
            IconMap["Flowers[1]"] = Flowers[1];
            IconMap["Flowers[2]"] = Flowers[2];
            IconMap["Flowers[3]"] = Flowers[3];
            IconMap["Flowers[4]"] = Flowers[4];
            IconMap["Flowers[5]"] = Flowers[5];
            IconMap["Flowers[6]"] = Flowers[6];
            IconMap["Flowers[7]"] = Flowers[7];
            IconMap["Cats[0]"] = Cats[0];
            IconMap["Cats[1]"] = Cats[1];
            IconMap["Cats[2]"] = Cats[2];
            IconMap["Cats[3]"] = Cats[3];
            IconMap["Cats[4]"] = Cats[4];
            IconMap["Cats[5]"] = Cats[5];
            IconMap["Cats[6]"] = Cats[6];
            IconMap["Cats[7]"] = Cats[7];
            IconMap["Cars[0]"] = Cars[0];
            IconMap["Cars[1]"] = Cars[1];
            IconMap["Cars[2]"] = Cars[2];
            IconMap["Cars[3]"] = Cars[3];
            IconMap["Cars[4]"] = Cars[4];
            IconMap["Cars[5]"] = Cars[5];
            IconMap["Cars[6]"] = Cars[6];
            IconMap["Cars[7]"] = Cars[7];            
        }

        static void LoadMarkers(ContentManager Content)
        {
            // Load Wait Icon
            Texture2D[] WaitFrames = new Texture2D[20];
            for (int i = 0; i < WaitFrames.Length; i++)
            {
                string frameName = string.Format("WaitIcon/Wait_{0:D2}", i + 1);
                WaitFrames[i] = Content.Load<Texture2D>(frameName);
            }
            WaitIcon = new AnimatedSprite(WaitFrames, 0.0625);

            // Load Hint Sprite
            Texture2D[] HintIcons = new Texture2D[6];
            HintIcons[0] = Content.Load<Texture2D>("HintIcon");
            HintIcons[1] = Content.Load<Texture2D>("HintIcon2");
            HintIcons[2] = Content.Load<Texture2D>("HintIcon3");
            HintIcons[3] = Content.Load<Texture2D>("HintIcon4");
            HintIcons[4] = Content.Load<Texture2D>("HintIcon5");
            HintIcons[5] = Content.Load<Texture2D>("HintIcon6");
            HintSprite = new AnimatedSprite(HintIcons, 0.0625);

            SelectionIconWide = Content.Load<Texture2D>("SelectionIconWide");
            SelectionIconTall = Content.Load<Texture2D>("SelectionIcon3Tall");

            Arrow = Content.Load<Texture2D>("BlueArrow");
        }

        static void LoadOverlays(ContentManager Content)
        {
            NotOverlay = Content.Load<Texture2D>("NotOverlay");
            EitherOrOverlay = Content.Load<Texture2D>("EitherOrOverlay");
            SpanOverlay = Content.Load<Texture2D>("Span");
            LeftOfIcon = Content.Load<Texture2D>("NextTo");
        }

        static void LoadStaticParts(ContentManager Content)
        {
            GoldBarVertical = Content.Load<Texture2D>("GoldBarVertical");
            GoldBarHorizontal = Content.Load<Texture2D>("GoldBarHorizontal");
            TransGray = Content.Load<Texture2D>("TransGrey");
            TransparentBox = Content.Load<Texture2D>("TransparentGray");
            ScrollArrow = Content.Load<Texture2D>("ScrollArrow");
            ScrollBar = Content.Load<Texture2D>("ScrollBar");
            ScrollCursor = Content.Load<Texture2D>("ScrollCursor");
            SliderBar = Content.Load<Texture2D>("SliderBar");
            SliderCursor = Content.Load<Texture2D>("SliderCursor");
            GoldArrowLeft = Content.Load<Texture2D>("GoldArrowLeft");
            GoldArrowRight = Content.Load<Texture2D>("GoldArrowRight");
            HelpBackground = Content.Load<Texture2D>("HelpBackground");
            GoldCoin = Content.Load<Texture2D>("GoldCoin");
            CheckBox = Content.Load<Texture2D>("CheckBox");
            Check = Content.Load<Texture2D>("Check");

            IconMap["GoldCoin"] = GoldCoin;
        }

        static void LoadFonts(ContentManager Content)
        {
            MenuFont = Content.Load<SpriteFont>("MenuFont");
            DialogFont = Content.Load<SpriteFont>("DialogFont");
            HelpFont = Content.Load<SpriteFont>("HelpFont");
        }

        static void LoadTowers(ContentManager content)
        {
            Towers = new Texture2D[4];
            Towers[0] = content.Load<Texture2D>("Towers/Tower1");
            Towers[1] = content.Load<Texture2D>("Towers/Tower2");
            Towers[2] = content.Load<Texture2D>("Towers/Tower3");
            Towers[3] = content.Load<Texture2D>("Towers/Tower4");
        }

        static void LoadSignInIcons(ContentManager content)
        {
            Facebook = content.Load<Texture2D>("facebook");
            Google = content.Load<Texture2D>("google");
            Email = content.Load<Texture2D>("email");
        }

        static void LoadMusic(ContentManager content)
        {
            MenuSong = content.Load<Song>("Audio/jenkees/02_-_Ronald_Jenkees_-_Neptune");            

            GameSongs = new Song[11];
            GameSongs[0] = content.Load<Song>("Audio/jenkees/01_-_Ronald_Jenkees_-_Derty");
            GameSongs[1] = content.Load<Song>("Audio/jenkees/03_-_Ronald_Jenkees_-_Canon_in_D_Remix");
            GameSongs[2] = content.Load<Song>("Audio/jenkees/04_-_Ronald_Jenkees_-_Clutter");
            GameSongs[3] = content.Load<Song>("Audio/jenkees/05_-_Ronald_Jenkees_-_Super-Fun");
            GameSongs[4] = content.Load<Song>("Audio/jenkees/06_-_Ronald_Jenkees_-_The_Rocky_Song_Remixed");
            GameSongs[5] = content.Load<Song>("Audio/jenkees/07_-_Ronald_Jenkees_-_Snap");
            GameSongs[6] = content.Load<Song>("Audio/jenkees/08_-_Ronald_Jenkees_-_The_Sunfish_Song");
            GameSongs[7] = content.Load<Song>("Audio/jenkees/09_-_Ronald_Jenkees_-_Loui");
            GameSongs[8] = content.Load<Song>("Audio/jenkees/10_-_Ronald_Jenkees_-_Gold_Spinners");
            GameSongs[9] = content.Load<Song>("Audio/jenkees/11_-_Ronald_Jenkees_-_Remix_To_A_Remix");
            GameSongs[10] = content.Load<Song>("Audio/jenkees/12_-_Ronald_Jenkees_-_Almost_Undamaged");
        }

        static void LoadSounds(ContentManager content)
        {
            MenuNavigate = content.Load<SoundEffect>("Audio/menu_navigate");
            MenuAccept = content.Load<SoundEffect>("Audio/menu_accept");
            GameLoad = content.Load<SoundEffect>("Audio/game_load");
            SliderMove = content.Load<SoundEffect>("Audio/slider_move");
            GameSave = content.Load<SoundEffect>("Audio/game_save");
            UnhideClues = content.Load<SoundEffect>("Audio/unhide_clues");
            GameAction1 = content.Load<SoundEffect>("Audio/game_action1");
            GameAction2 = content.Load<SoundEffect>("Audio/game_action2");
            GameAction3 = content.Load<SoundEffect>("Audio/game_action3");
            GameAction4 = content.Load<SoundEffect>("Audio/game_action4");
            GameAction5 = content.Load<SoundEffect>("Audio/game_action5");
            GameAction6 = content.Load<SoundEffect>("Audio/game_action6");
            PuzzleFailed = content.Load<SoundEffect>("Audio/puzzle_failed");
            PuzzleComplete = content.Load<SoundEffect>("Audio/puzzle_complete");
            HappinessWav = content.Load<SoundEffect>("Audio/Happiness");
        }
    }
}
