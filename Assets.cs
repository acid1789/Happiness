using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

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

        // Markers
        public static AnimatedSprite HintSprite;
        public static Texture2D SelectionIconWide;
        public static Texture2D SelectionIconTall;

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
        public static Texture2D Apple;


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
        }

        static void LoadMarkers(ContentManager Content)
        {
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
            GoldArrowLeft = Content.Load<Texture2D>("GoldArrowLeft");
            GoldArrowRight = Content.Load<Texture2D>("GoldArrowRight");
            HelpBackground = Content.Load<Texture2D>("HelpBackground");
            GoldCoin = Content.Load<Texture2D>("GoldCoin");
            CheckBox = Content.Load<Texture2D>("CheckBox");
            Check = Content.Load<Texture2D>("Check");
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
            Apple = content.Load<Texture2D>("apple_id");
        }
    }
}
