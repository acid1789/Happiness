using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness 
{
    static class Constants
    {
        public static float MarginLeft = 10f / 1280f;
        public static float MarginRight = 10f / 1280f;
        public static float MarginTop = 10f / 720f;
        public static float MarginBottom = 10f / 720f;

        public static float ButtonPanel_Width = 98f / 1280f;
        public static float HelpPanel_Height = 40f / 720f;

        public static float IconSize = 60f / 720f;
        public static float ClueSpace = 6f / 720f;

        public static float CellDialogWidth = 1000f / 1280f;
        public static float CellDialogHeight = 600f / 720f;
        public static float CellDialogSmallIconSize = 200f / 720f;
        public static float CellDialogLargeIconSize = 400f / 720f;
        public static float CellDialog_SmallButtonWidth = 60f / 1280f;
        public static float CellDialog_SmallButtonHeight = 50f / 720f;
        public static float CellDialog_ButtonWidth = 200f / 1280f;
        public static float CellDialog_ButtonHeight = 40f / 720f;

        public static float CellDialog_CancelButtonX = 34f / 1280f;
        public static float CellDialog_CancelButtonY = 38f / 720f; 
        
        public static float HelpClueIconSize = 30f / 720f;

        public static float ButtonPanel_ButtonSize = 60f / 720f;
        public static float ButtonPanel_ButtonSpace = 40f / 720f;
        public static float ButtonPanel_CoinSize = 25f / 720f;
        public static float ButtonPanel_CoinMargin = 5f / 720f;

        public static float PauseMenu_ButtonX = 50f / 1280f;
        public static float PauseMenu_ButtonY = 50f / 720f;
        public static float PauseMenu_ButtonWidth = 200f / 1280f;
        public static float PauseMenu_ButtonHeight = 40f / 720f;
        public static float PauseMenu_ButtonSpace = 20f / 720f;

        public static float PauseMenu_ButtonAreaBGX = 30f / 1280f;
        public static float PauseMenu_ButtonAreaBGY = 25f / 720f;
        public static float PauseMenu_ButtonAreaBGW = 240f / 1280f;
        public static float PauseMenu_ButtonAreaBGH = 570f / 720f;

        public static float PauseMenu_HelpLeft = 300f / 1280f;
        public static float PauseMenu_HelpTextLeft = 327f / 1280f;
        public static float PauseMenu_HelpTextTop = 20f / 720f;
        public static float PauseMenu_IconSize = 30f / 720f;
        public static float PauseMenu_IconSizeSmall = 16f / 720f;

        public static float MessageBox_Width = 400f / 1280f;
        public static float MessageBox_ButtonWidth = 90f / 1280f;
        public static float MessageBox_ButtonHeight = 30f / 720f;
        public static float MessageBox_TopBottomMargin = 20f / 720f;
        public static float MessageBox_LeftRightMargin = 20f / 1280f;
        public static float MessageBox_LineSpace = 10f / 720f;
        public static float MessageBox_CheckboxSize = 20f / 720f;

        public static float SignInDialog_Width = 600f / 1280f;
        public static float SignInDialog_Height = 500f / 720f;
        public static float SignInDialog_Margin = 20f / 720f;
        public static float SignInDialog_InputWidth = 350f / 1280f;
        public static float SignInDialog_InputLeft = 180f / 1280f;
        public static float SignInDialog_InputTop = 100f / 720f;
        public static float SignInDialog_AuthGap = 10f / 720f;
        public static float SignInDialog_AuthSize = 60f / 720f;
        public static float SignInDialog_IconButtonWidth = 280f / 1280f;
        public static float SignInDialog_IconMarginLeftRight = 18f / 1280f;
        public static float SignInDialog_IconMarginTopBottom = 8f / 720f;
        public static float SignInDialog_IconButtonGap = 20f / 720f;
        public static float SignInDialog_StatusGap = 5f / 720f;
        public static float SignInDialog_ButtonGap = 80f / 720f;
        public static float SignInDialog_ButtonGapLeft = 40f / 1280f;
        public static float SignInDialog_ButtonWidthLarge = 200f / 1280f;
        public static float SignInDialog_ButtonWidthSmall = 80f / 1280f;
        public static float SignInDialog_ButtonHeight = 50f / 720f;
        public static float SignInDialog_WaitIconSize = 40f / 720f;
        public static float SignInDialog_WaitIconSpace = 10f / 720f;

        public static float OptionsDialog_Width = 700f / 1280f;
        public static float OptionsDialog_Height = 500f / 720f;
        public static float OptionsDialog_ButtonWidth = 180f / 1280f;
        public static float OptionsDialog_ButtonHeight = 30f / 720f;
        public static float OptionsDialog_TopBottomMargin = 30f / 720f;      
        public static float OptionsDialog_SliderWidth = 400f / 1280f;
        public static float OptionsDialog_SliderHeight = 16f / 720f;
        public static float OptionsDialog_SliderCursorWidth = 24f / 1280f;
        public static float OptionsDialog_SliderCursorHeight = 28f / 720f;

        public static float DisplayNameDialog_Width = 600f / 1280f;
        public static float DisplayNameDialog_Height = 175f / 720f;
        public static float DisplayNameDialog_MarginTopBottom = 20f / 720f;
        public static float DisplayNameDialog_LineSpace = 8f / 720f;
        public static float DisplayNameDialog_InputWidth = 400f / 1280f;
        public static float DisplayNameDialog_InputHeight = 22f / 720f;
        public static float DisplayNameDialog_ButtonWidth =  90f / 1280f;
        public static float DisplayNameDialog_ButtonHeight = 30f / 720f;
        
        public static float Startup_LogoSize = 400f / 720f;
        public static float Startup_CreditX = 40f / 1280f;
        public static float Startup_CreditY = 120f / 720f;
        public static float Startup_MusicCreditY = 60f / 720f;
        public static float Startup_WaitIconSize = 200f / 1280f;

        public static float EndScreen_Width = 600f / 1280f;
        public static float EndScreen_Height = 500f / 720f;
        public static float EndScreen_MarginTopBottom = 30f / 720f;
        public static float EndScreen_SuccessGap = 30f / 720f;
        public static float EndScreen_ButtonWidth = 150f / 1280f;
        public static float EndScreen_ButtonHeight = 30f / 720f;
        public static float EndScreen_ButtonSpace = 20f / 1280f;
        public static float EndScreen_TimeLabelGap = 50f / 720f;
        public static float EndScreen_TimeGap = 25f / 720f;
        public static float EndScreen_ScoreCenterGap = 15f / 1280f;
        public static float EndScreen_TimeScoreGap = 40f / 720f;
        public static float EndScreen_ScoreSpace = 25f / 720f;
        public static float EndScreen_ScoreBarWidth = 60f / 1280f;
        public static float EndScreen_ScoreBarHeight = 3f / 720f;
        public static float EndScreen_ScoreLevelGap = 40f / 720f;
        public static float EndScreen_ExpBarWidth = 300f / 1280f;
        public static float EndScreen_ExpBarHeight = 20f / 720f;
        public static float EndScreen_LevelUnlockGap = 30f / 720f;
        public static float EndScreen_WaitIconSize = 150f / 1280f;

        public static float HubScene_TowerSize = 180f / 720f;
        public static float HubScene_TowerAreaTop = 100f / 720f;
        public static float HubScene_ExpBarWidth = 150f / 720f;
        public static float HubScene_ExpBarHeight = 10f / 720f;
        public static float HubScene_MarginLeftRight = 20f / 1280f;
        public static float HubScene_MarginTopBottom = 10f / 720f;
        public static float HubScene_TutorialWidth = 170f / 1280f;
        public static float HubScene_ButtonWidth = 140f / 1280f;
        public static float HubScene_ButtonHeight = 30f / 720f;
        public static float HubScene_CoinsWidth = 100f / 1280f;

        public static float FloorSelectDialog_Width = 1000f / 1280f;
        public static float FloorSelectDialog_Height = 600 / 720f;
        public static float FloorSelectDialog_MarginTopBottom = 40f / 720f;
        public static float FloorSelectDialog_MarginLeftRight = 50f / 1280f;
        public static float FloorSelectDialog_NavButtonWidth = 60f / 1280f;
        public static float FloorSelectDialog_NavButtonHeight = 50f / 720f;
        public static float FloorSelectDialog_LBButtonWidth = 180f / 1280f;
        public static float FloorSelectDialog_LBButtonHeight = 50f / 720f;
        public static float FloorSelectDialog_FloorScrollWidth = 400f / 1280f;
        public static float FloorSelectDialog_WaitIconSize = 100f / 720f;
        public static float FloorSelectDialog_FloorSelectTutorialWidth = 180f / 1280f;
        public static float FloorSelectDialog_PlayTutorialWidth = 200f / 1280f;

        public static float TutorialSystem_ArrowTravelDistance = 8f / 1280f;
        public static float TutorialSystem_ArrowSpeed = 2.7f;
        public static float TutorialSystem_ArrowWidth = 120f / 1280f;
        public static float TutorialSystem_ArrowHeight = 90f / 720f;
        public static float TutorialSystem_TextMarginTopBottom = 10f / 720f;
        public static float TutorialSystem_TextMarginLeftRight = 5f / 1280f;
        public static float TutorialSystem_ButtonWidth = 100f / 1280f;
        public static float TutorialSystem_ButtonHeight = 30f / 720f;

        public static float BuyCreditsDialog_Width = 800f / 1280f;
        public static float BuyCreditsDialog_Height = 600f / 720f;
        public static float BuyCreditsDialog_WaitIconSize = 200f / 1280f;
        public static float BuyCreditsDialog_ButtonWidth = 180f / 1280f;
        public static float BuyCreditsDialog_ButtonHeight = 30f / 720f;
        public static float BuyCreditsDialog_TopBottomMargin = 30f / 720f;
        public static float BuyCreditsDialog_ProductWidth = 700 / 1280f;

        public static float ProductDisplay_CoinSize = 40 / 1280f;
        public static float ProductDisplay_CoinLeft = 50 / 1280f;
        public static float ProductDisplay_FrameLeft = 35 / 1280f;
        public static float ProductDisplay_FrameWidth = 600 / 1280f;   
        public static float ProductDisplay_ButtonWidth = 100 / 1280f;
        public static float ProductDisplay_ButtonHeight = 40 / 1280f;

        public static float VIPDialog_Width = 800f / 1280f;
        public static float VIPDialog_Height = 600f / 720f;
        public static float VIPDialog_ExpBarWidth = 600f / 1280f;
        public static float VIPDialog_ExpBarHeight = 30f / 720f;
        public static float VIPDialog_MarginTopBottom = 20f / 720f;
        public static float VIPDialog_LevelsAreaWidth = 500f / 1280f;
        public static float VIPDialog_LRButtonWidth = 60f / 1280f;
        public static float VIPDialog_LRButtonHeight = 50f / 720f;
        public static float VIPDialog_LRButtonSpace = 10f / 1280f;
        public static float VIPDialog_LineSpace = 5f / 720f;

        public static char[] Ascii0to9Shift = { ')', '!', '@', '#', '$', '%', '^', '&', '*', '(' };

        public static float ArrowUp = (float)-Math.PI / 2;
        public static float ArrowDown = (float)Math.PI / 2;
        public static float ArrowLeft = (float)Math.PI;
        public static float ArrowDiagonalUpRight = (float)-Math.PI / 4;
    }
}
