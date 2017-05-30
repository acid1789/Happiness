using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LogicMatrix;
using System.IO;

namespace Happiness
{
    public class GameScene : Scene
    {
        public enum MessageBoxContext
        {
            None,
            InsufficientFunds_Hint,
            InusfficientFunds_MegaHint
        }

        // Dialogs
        EndPuzzleScreen m_EndScreen;
        ReconnectScreen m_ReconnectScreen;
        MessageBox m_MessageBox;

        // Panels
        HorizontalCluePanel m_HorizontalCluePanel;
        VerticalCluePanel m_VerticalCluePanel;
        GamePanel m_GamePanel;
        HelpPanel m_HelpPanel;
        ButtonPanel m_ButtonPanel;
        List<UIPanel> m_UIPanels;

        // Puzzle
        Puzzle m_Puzzle;
        int m_iPuzzleIndex;
        DateTime m_PuzzleStart;

        // Pause stuff
        PauseMenu m_PauseMenu;
        double m_PauseSeconds;

        // Icons
        Texture2D[,] m_aIcons;

        // Undos
        List<Action> m_History;

        // Hint
        bool m_bVerifyHintPurchase;
        int m_iHintCount;
        Hint m_Hint;

        // Mega Hint
        bool m_bVerifyMegaHintPurchase;
        int m_iMegaHintCount;

        public GameScene(Happiness hgame) : base(hgame)
        {
            // Init Input
            InputController.IC.OnDragBegin += M_Input_OnDragBegin;
            InputController.IC.OnDrag += M_Input_OnDrag;
            InputController.IC.OnDragEnd += M_Input_OnDragEnd;
            InputController.IC.OnClick += M_Input_OnClick;
            InputController.IC.OnKeyUp += M_Input_OnKeyUp;
        }

        #region Initialization
        public void Initialize(int puzzleIndex, int puzzleSize, bool load)
        {
            m_PuzzleStart = DateTime.Now;
            m_PauseSeconds = 0;
            m_bVerifyHintPurchase = true;
            m_bVerifyMegaHintPurchase = true;
            m_iHintCount = 0;
            m_iMegaHintCount = 0;
            m_History = new List<Action>();
            
            Game.SoundManager.PlayGameMusic();

            // Create the puzzle
            m_iPuzzleIndex = puzzleIndex;
            m_Puzzle = new Puzzle(puzzleIndex, puzzleSize, 1);
            if( load )
                LoadPuzzle();

            // Initialize the UI
            int buttonPanelWidth = (int)(Constants.ButtonPanel_Width * Game.ScreenWidth);
            int helpPanelHeight = (int)(Constants.HelpPanel_Height * Game.ScreenHeight);
            m_HorizontalCluePanel = new HorizontalCluePanel(this, m_Puzzle.HorizontalClues);
            m_VerticalCluePanel = new VerticalCluePanel(this, m_Puzzle.VerticalClues, Game.ScreenWidth - m_HorizontalCluePanel.Rect.Width);
            m_GamePanel = new GamePanel(this, new Rectangle(buttonPanelWidth, helpPanelHeight, Game.ScreenWidth - (m_HorizontalCluePanel.Rect.Width + buttonPanelWidth), Game.ScreenHeight - (m_VerticalCluePanel.Rect.Height + helpPanelHeight)));
            m_HelpPanel = new HelpPanel(this, new Rectangle(buttonPanelWidth, 0, m_GamePanel.Rect.Width, m_GamePanel.Rect.Top));
            m_ButtonPanel = new ButtonPanel(this, new Rectangle(0, 0, buttonPanelWidth, m_VerticalCluePanel.Rect.Top));

            HappinessNetwork.VipDataArgs vip = Game.m_GameInfo.VipData;
            m_ButtonPanel.SetHintCount(vip.Hints - m_iHintCount, vip.Hints);
            m_ButtonPanel.SetMegaHintCount(vip.MegaHints - m_iMegaHintCount, vip.MegaHints);
            m_ButtonPanel.SetUndoCount(m_History.Count, vip.UndoSize);
            m_ButtonPanel.SetCoins(Game.m_GameInfo.HardCurrency);

            m_UIPanels = new List<UIPanel>();
            m_UIPanels.Add(m_GamePanel);
            m_UIPanels.Add(m_HorizontalCluePanel);
            m_UIPanels.Add(m_VerticalCluePanel);
            m_UIPanels.Add(m_HelpPanel);
            m_UIPanels.Add(m_ButtonPanel);

            // Init Icons
            InitIcons();
            
            int gamePanelCenterX = m_GamePanel.Rect.Left + (m_GamePanel.Rect.Width / 6);
            Vector2 clueArrow = new Vector2(m_HorizontalCluePanel.Rect.Left, m_HorizontalCluePanel.Rect.Top + (m_HorizontalCluePanel.ClueHeight >> 1));
            Rectangle bottomTutorialRect = new Rectangle(gamePanelCenterX + Game.Tutorial.ArrowHeight, m_GamePanel.Rect.Bottom + 10, m_GamePanel.Rect.Width - ((m_GamePanel.Rect.Width / 6) + (Game.Tutorial.ArrowHeight >> 1)), 0);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.GameStart, new Vector2(gamePanelCenterX, m_GamePanel.Rect.Bottom), Constants.ArrowUp,
                                                                               bottomTutorialRect, "The object of the game is to figure out which icon belongs in each cell of the puzzle grid area.\nThe puzzle is solved when all of the icons are in the correct places.",
                                                                               TutorialSystem.TutorialPiece.HorizontalClueArea, Rectangle.Empty, true);
            Rectangle clue1Area = new Rectangle(m_HorizontalCluePanel.Rect.Left, m_HorizontalCluePanel.Rect.Top, m_HorizontalCluePanel.Rect.Width, m_HorizontalCluePanel.ClueHeight);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.HorizontalClueArea, clueArrow, 0,
                bottomTutorialRect, "This area is for the horizontal clues.\nHorizontal clues give hints about the icons from left to right or right to left.\n\nTap this first Horizontal Clue.", 
                TutorialSystem.TutorialPiece.SpanExplanation, clue1Area);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.SpanExplanation, clueArrow, 0, 
                bottomTutorialRect, "The arrow across the top of the clue indicates that this is a span clue.\nA span clue describes a series of 3 neighboring columns. The span can go from left to right or right to left but the center column will always be inbetween the two outer columns.",
                TutorialSystem.TutorialPiece.ClueHelp, Rectangle.Empty, true);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.ClueHelp, new Vector2(m_HelpPanel.Rect.Left, m_HelpPanel.Rect.Bottom), Constants.ArrowDiagonalUpRight,
                bottomTutorialRect, "This area gives a brief description of the currently highlighted clue.\n\nSince this clue is a span type clue, it is showing that in a group of 3 columns {icon:Simpsons[2]} is on one edge of the group and {icon:Superheros[3]} is on the other edge.\nSince there is a NOT icon in the center of this span, in the center column can not have the {icon:Hubble[4]}.", 
                TutorialSystem.TutorialPiece.SpanHelp1, Rectangle.Empty, true);

            Rectangle centerGridArea = new Rectangle(m_GamePanel.Rect.Left + m_GamePanel.CellWidth, m_GamePanel.Rect.Top + m_GamePanel.CellHeight, m_GamePanel.CellWidth, m_GamePanel.CellHeight);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.SpanHelp1, m_GamePanel.IconPosition(1, 1, 1), Constants.ArrowDiagonalUpRight,
                bottomTutorialRect, "This is a 3x3 puzzle so the center column of this span has to be the center column of this puzzle.\nWe can eliminate the {icon:Hubble[4]} from the center column.\n\nTap the center grid area.",
                TutorialSystem.TutorialPiece.EliminateRedNebula, centerGridArea);

            Rectangle centerBottomGridArea = new Rectangle(m_GamePanel.Rect.Left + m_GamePanel.CellWidth, m_GamePanel.Rect.Top + (m_GamePanel.CellHeight * 2), m_GamePanel.CellWidth, m_GamePanel.CellHeight);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.BartMan1, m_GamePanel.IconPosition(2, 1, 1), Constants.ArrowDown,
                bottomTutorialRect, "The {icon:Simpsons[2]} has to be on one of the edges of the column group so it can not be in the center column.\n\nTap the bottom grid area of the center column.",
                TutorialSystem.TutorialPiece.BartMan2, centerBottomGridArea);

            Rectangle centerTopGridArea = new Rectangle(m_GamePanel.Rect.Left + m_GamePanel.CellWidth, m_GamePanel.Rect.Top, m_GamePanel.CellWidth, m_GamePanel.CellHeight);
            Vector2 iconBotOffset = new Vector2(m_GamePanel.IconSize >> 1, 0);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Hulk1, Vector2.Add(m_GamePanel.IconPosition(0, 1, 0), iconBotOffset), Constants.ArrowUp,
                bottomTutorialRect, "The {icon:Superheros[3]} has to be on one of the edges of the column group so it can not be in the center column.\n\nTap the top grid area of the center column.",
                TutorialSystem.TutorialPiece.Hulk2, centerTopGridArea);

            Vector2 clue2Position = new Vector2(m_HorizontalCluePanel.Rect.Left, m_HorizontalCluePanel.Rect.Top + (m_HorizontalCluePanel.ClueHeight) + (m_HorizontalCluePanel.ClueHeight >> 1));
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.HorizontalClue2a, clue2Position, 0, 
                bottomTutorialRect, "We have done all we can right now with the first clue.\nLets move on to the next clue.\n\nTap the second horizontal clue.",
                TutorialSystem.TutorialPiece.HorizontalClue2b, new Rectangle(m_HorizontalCluePanel.Rect.Left, m_HorizontalCluePanel.Rect.Top + (m_HorizontalCluePanel.ClueHeight), m_HorizontalCluePanel.Rect.Width, m_HorizontalCluePanel.ClueHeight));

            Vector2 iconTopOffset = new Vector2(m_GamePanel.IconSize >> 1, -m_GamePanel.IconSize);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.HorizontalClue2b, Vector2.Add(m_GamePanel.IconPosition(1, 1, 2), iconTopOffset), Constants.ArrowDown,
                bottomTutorialRect, "This clue is also a span type clue with the {icon:Hubble[2]} in the center column, the {icon:Superheros[2]} on one edge and NOT the {icon:Hubble[3]} on the other edge.\nSince this is a 3x3 puzzle, we know the center of the span is the center column of the puzzle so we now know where the {icon:Hubble[2]} goes.\n\nTap the center grid area.",
                TutorialSystem.TutorialPiece.CrabNebula1, centerGridArea);

            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.HorizontalClue2c, Vector2.Add(m_GamePanel.IconPosition(0, 1, 2), iconTopOffset), Constants.ArrowDown, bottomTutorialRect,
                "We know that the {icon:Superheros[2]} has to be on one of the outer edges, so it cant be in the center column.\n\nTap the top grid area of the center column.", TutorialSystem.TutorialPiece.GreenLantern1, centerTopGridArea);

            Vector2 iconSideOffset = new Vector2(0, -(m_GamePanel.IconSize >> 1));
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.HorizontalClue2d, m_GamePanel.IconPosition(0, 1, 0), 0, bottomTutorialRect,
                "Notice that the {icon:Superheros[4]} is confirmed in the center column now. This is because it was the only posibility left after eliminating the {icon:Superheros[3]} and the {icon:Superheros[2]}.",
                TutorialSystem.TutorialPiece.HorizontalClue3a, Rectangle.Empty, true);

            Vector2 clueHeightOffset = new Vector2(0, m_HorizontalCluePanel.ClueHeight);
            Vector2 clue3Position = Vector2.Add(clue2Position, clueHeightOffset);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.HorizontalClue3a, clue3Position, 0, bottomTutorialRect, "There is nothing else we can do with this clue for now so lets move on to the next clue.\n\nTap the third horizontal clue.\n", TutorialSystem.TutorialPiece.HorizontalClue3b,
                new Rectangle(m_HorizontalCluePanel.Rect.Left, m_HorizontalCluePanel.Rect.Top + (m_HorizontalCluePanel.ClueHeight * 2), m_HorizontalCluePanel.Rect.Width, m_HorizontalCluePanel.ClueHeight));

            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.HorizontalClue3b, m_GamePanel.IconPosition(2, 1, 0), Constants.ArrowDiagonalUpRight, bottomTutorialRect, 
                "This is another span type clue. Most of what it is telling us, we already know. It is however telling us that the {icon:Simpsons[3]} is in the center column.\n\nTap the bottom grid area of the center column.",
                TutorialSystem.TutorialPiece.Homer1, centerBottomGridArea);

            Vector2 clue4Position = Vector2.Add(clue3Position, clueHeightOffset);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.HorizontalClue4, clue4Position, 0, bottomTutorialRect, "The fourth clue doesn't have any useful information for us right now since we already know the {icon:Superheros[4]} is in the center column and we dont know where either the {icon:Hubble[4]} or the {icon:Superheros[2]} are.", TutorialSystem.TutorialPiece.HorizontalClue5a, Rectangle.Empty, true);

            Vector2 clue5Position = Vector2.Add(clue4Position, clueHeightOffset);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.HorizontalClue5a, clue5Position, 0, bottomTutorialRect, "Tap the fifth clue to select it.", TutorialSystem.TutorialPiece.HorizontalClue5b,
                new Rectangle(m_HorizontalCluePanel.Rect.Left, m_HorizontalCluePanel.Rect.Top + (m_HorizontalCluePanel.ClueHeight * 4), m_HorizontalCluePanel.Rect.Width, m_HorizontalCluePanel.ClueHeight));

            Rectangle rightTopGridArea = new Rectangle(m_GamePanel.Rect.Right - m_GamePanel.CellWidth, m_GamePanel.Rect.Top, m_GamePanel.CellWidth, m_GamePanel.CellHeight);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.HorizontalClue5b, Vector2.Add(iconSideOffset, m_GamePanel.IconPosition(0, 2, 2)), 0, bottomTutorialRect, 
                "This clue tells us that the {icon:Superheros[2]} is in a column somewhere to the right of the {icon:Simpsons[3]}.\nSince we already know where the {icon:Simpsons[3]} is, and there is only one column to the right of it, we know where the {icon:Superheros[2]} is.\n\nTap the top right grid cell.", TutorialSystem.TutorialPiece.GreenLantern4, rightTopGridArea);
            
            bool online = true;

            Vector2 hideClueTarget = new Vector2(m_ButtonPanel.HideClueRect.Right, m_ButtonPanel.HideClueRect.Top + (m_ButtonPanel.HideClueRect.Height >> 1));
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.HideClue1, hideClueTarget, Constants.ArrowLeft, bottomTutorialRect,
                "We have learned all we possibly can from this clue since we know where both the {icon:Simpsons[3]} and the {icon:Superheros[2]} are.\nWe can now hide this clue so we can more easily see the relevant clues.\nYou can unhide all the clues anytime you want in the pause menu.\n\nTap the 'HC' button to hide this clue.",
                online ? TutorialSystem.TutorialPiece.Hint1 : TutorialSystem.TutorialPiece.Hint2, m_ButtonPanel.HideClueRect);
            
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Hint1, new Vector2(m_ButtonPanel.HintRect.Right, m_ButtonPanel.HintRect.Top + (m_ButtonPanel.HintRect.Height >> 1)), Constants.ArrowLeft, bottomTutorialRect,
                "If you get stuck on a puzzle you can spend {icon:GoldCoin}2 to get a hint of what to do next.\nThere are a maximum number of hints you can use per puzzle, this number increases with your VIP level.\n\nTap the 'H' button to get a hint.", TutorialSystem.TutorialPiece.Hint2, m_ButtonPanel.HintRect);
            
            if (online)
            {
                m_MessageBox = new MessageBox("Would you like to spend 2 coins for a hint?", MessageBoxButtons.YesNo, (int)MessageBoxContext.InsufficientFunds_Hint, Game.ScreenWidth, Game.ScreenHeight, "Don't ask again");
                Rectangle hintYes = m_MessageBox.GetButtonRect(0);
                m_MessageBox = null;
                Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Hint2, m_GamePanel.IconPosition(2, 1, 0), Constants.ArrowDiagonalUpRight, bottomTutorialRect,
                    "When you tap the hint button, you are asked for confirmation before proceding. You can skip this confirmation for the rest of the puzzle by tapping the checkbox. If the box remains unchecked, you will be asked to confirm each hint used in this puzzle.\n\nTap the 'Yes' button to confirm we want a hint.",
                    TutorialSystem.TutorialPiece.Hint3, hintYes);

                Rectangle rightBottomGridArea = new Rectangle(m_GamePanel.Rect.Right - m_GamePanel.CellWidth, m_GamePanel.Rect.Bottom - m_GamePanel.CellHeight, m_GamePanel.CellWidth, m_GamePanel.CellHeight);
                Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Hint3, Vector2.Add(iconTopOffset, m_GamePanel.IconPosition(2, 2, 2)), Constants.ArrowDown, bottomTutorialRect,
                    "The hint is showing us that this clue should effect the {icon:Simpsons[2]} in the third column.\nThis span clue has the {icon:Superheros[3]} on one edge of the span and the {icon:Simpsons[2]} on the other edge. Since we know the {icon:Superheros[3]} is in the first column, the {icon:Simpsons[2]} belongs in the third column.\n\nTap the bottom right grid cell.",
                    TutorialSystem.TutorialPiece.Bartman5, rightBottomGridArea);
            }
            else
            {
                Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Hint2, clueArrow, 0, bottomTutorialRect,
                    "Tap the first clue to select it again.",
                    TutorialSystem.TutorialPiece.Hint3, clue1Area);

                Rectangle rightBottomGridArea = new Rectangle(m_GamePanel.Rect.Right - m_GamePanel.CellWidth, m_GamePanel.Rect.Bottom - m_GamePanel.CellHeight, m_GamePanel.CellWidth, m_GamePanel.CellHeight);
                Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Hint3, Vector2.Add(iconTopOffset, m_GamePanel.IconPosition(2, 2, 2)), Constants.ArrowDown, bottomTutorialRect,
                    "This span clue has the {icon:Superheros[3]} on one edge of the span and the {icon:Simpsons[2]} on the other edge. Since we know the {icon:Superheros[3]} is in the first column, the {icon:Simpsons[2]} belongs in the third column.\n\nTap the bottom right grid cell.",
                    TutorialSystem.TutorialPiece.Bartman5, rightBottomGridArea);
            }

            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Undo, new Vector2(m_ButtonPanel.UndoRect.Right, m_ButtonPanel.UndoRect.Top + (m_ButtonPanel.UndoRect.Height >> 1)), Constants.ArrowLeft, bottomTutorialRect,
                "If you make a mistake, you can undo previous actions by tapping the 'U' button.\nThere are a limited number of actions that you can undo indicated by the number under the 'U' button. The number on the left is the number of actions currently in the history. The number on the right is the maximum number of actions stored in the history. The maximum increases with your VIP level.", 
                TutorialSystem.TutorialPiece.HorizontalClue4b, Rectangle.Empty, true);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.HorizontalClue4b, clue4Position, 0, bottomTutorialRect,
                "There are only two cells left to solve. We need to figure out which cells the {icon:Hubble[4]} and the {icon:Hubble[3]} belong in.\nLets go back to the fourth horizontal clue.\n\nTap the fourth horizontal clue.\n",
                TutorialSystem.TutorialPiece.HorizontalClue4c, new Rectangle(m_HorizontalCluePanel.Rect.Left, m_HorizontalCluePanel.Rect.Top + (m_HorizontalCluePanel.ClueHeight * 3), m_HorizontalCluePanel.Rect.Width, m_HorizontalCluePanel.ClueHeight));

            Rectangle rightCenterGridArea = new Rectangle(m_GamePanel.Rect.Right - m_GamePanel.CellWidth, m_GamePanel.Rect.Top + m_GamePanel.CellHeight, m_GamePanel.CellWidth, m_GamePanel.CellHeight);
            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.HorizontalClue4c, m_GamePanel.IconPosition(1, 2, 1), Constants.ArrowDiagonalUpRight, bottomTutorialRect,
                "This clue is telling us that the {icon:Superheros[4]} has the {icon:Hubble[4]} on one side and NOT the {icon:Superheros[3]} on the other side.\nSince the {icon:Superheros[3]} is in the first column, the {icon:Hubble[4]} can not be in the third column.\n\nTap the middle grid cell in the third column.",
                TutorialSystem.TutorialPiece.RedNebula4, rightCenterGridArea);

            if (!Game.Tutorial.IsPieceSetup(TutorialSystem.TutorialPiece.EndScreen1))
            {
                // Create a dummy end screen here just so that the tutorial data gets setup correctly for later
                EndPuzzleScreen tempEnd = new EndPuzzleScreen(false, 3, 0, Game.ScreenWidth, Game.ScreenHeight, Game);
            }

            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Puzzle2, new Vector2(-1000, -1000), 0, 
                new Rectangle(Game.ScreenWidth - 170, Game.ScreenHeight - 80, 150, 0), "Solve this puzzle", TutorialSystem.TutorialPiece.EndScreen4, new Rectangle(0, 0, Game.ScreenWidth, Game.ScreenHeight));

            Game.Tutorial.SetPieceData(TutorialSystem.TutorialPiece.Horizontal_NextTo, clue2Position, 0, bottomTutorialRect,
                "This clue is a Next To type clue. It tells us that the {icon:Superheros[5]} is in a column directly next to the column with the {icon:Flowers[2]}.\n\nThis doesn't help you any right now, but it will come in handy as you learn more about the puzzle.", TutorialSystem.TutorialPiece.None, Rectangle.Empty, true);

            Game.Tutorial.TriggerPiece(TutorialSystem.TutorialPiece.GameStart);
        }

        public void InitIcons()
        {
            Random rand = m_Puzzle.m_Rand;
            int[] iScatterArray = new int[8];

            Puzzle.RandomDistribution(rand, iScatterArray);
            Texture2D[] aAllIcons = new Texture2D[64];
            for (int i = 0; i < 8; i++)
            {
                aAllIcons[i] = Assets.Cars[iScatterArray[i]];
                aAllIcons[8 + i] = Assets.Cats[iScatterArray[i]];
                aAllIcons[16 + i] = Assets.Flowers[iScatterArray[i]];
                aAllIcons[24 + i] = Assets.Hubble[iScatterArray[i]];
                aAllIcons[32 + i] = Assets.Princesses[iScatterArray[i]];
                aAllIcons[40 + i] = Assets.Puppies[iScatterArray[i]];
                aAllIcons[48 + i] = Assets.Simpsons[iScatterArray[i]];
                aAllIcons[56 + i] = Assets.Superheros[iScatterArray[i]];
            }

            Puzzle.RandomDistribution(rand, iScatterArray);
            m_aIcons = new Texture2D[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    m_aIcons[i, j] = aAllIcons[(iScatterArray[i] * 8) + j];
                }
            }
        }

        public override void Shutdown()
        {
            base.Shutdown();

            // Remove input handlers
            InputController.IC.OnDragBegin -= M_Input_OnDragBegin;
            InputController.IC.OnDrag -= M_Input_OnDrag;
            InputController.IC.OnDragEnd -= M_Input_OnDragEnd;
            InputController.IC.OnClick -= M_Input_OnClick;
            InputController.IC.OnKeyUp -= M_Input_OnKeyUp;
        }
        #endregion

        #region Pausing
        public void Pause()
        {
            if (m_PauseMenu == null)
            {
                m_PauseMenu = new PauseMenu(Game.ScreenWidth, Game.ScreenHeight);
                //m_SoundManager.PlayMenuAccept();
            }
        }

        void UnPause()
        {
            if (m_PauseMenu != null)
            {
                m_PauseSeconds += m_PauseMenu.Elapsed;
                m_PauseMenu = null;
            }
        }
        #endregion

        #region Puzzle File
        

        public void SavePuzzle()
        {
            string saveName = Happiness.PuzzleSaveName(m_Puzzle.m_iSize, m_iPuzzleIndex);
            FileStream fs = File.Open(saveName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
                        
            bw.Write(ElapsedTime);
            bw.Write(m_iHintCount);
            bw.Write(m_iMegaHintCount);
            bw.Write(m_History.Count);
            foreach (Action a in m_History)
            {
                a.Save(bw, m_Puzzle.m_iSize);
            }
            for (int iRow = 0; iRow < m_Puzzle.m_iSize; iRow++)
            {
                for (int iCol = 0; iCol < m_Puzzle.m_iSize; iCol++)
                {
                    for (int iIcon = 0; iIcon < m_Puzzle.m_iSize; iIcon++)
                    {
                        bw.Write(m_Puzzle.m_Rows[iRow].m_Cells[iCol].m_bValues[iIcon]);
                    }
                }
            }
            bw.Close();          
        }

        public void LoadPuzzle()
        {
            string saveName = Happiness.PuzzleSaveName(m_Puzzle.m_iSize, m_iPuzzleIndex);
            if (File.Exists(saveName))
            {
                FileStream fs = File.OpenRead(saveName);
                BinaryReader br = new BinaryReader(fs);

                double elapsed = br.ReadDouble();
                m_PuzzleStart = m_PuzzleStart.AddSeconds(-elapsed);
                m_iHintCount = br.ReadInt32();
                m_iMegaHintCount = br.ReadInt32();
                int historyCount = br.ReadInt32();
                m_History = new List<Action>();
                for (int i = 0; i < historyCount; i++)
                {
                    m_History.Add(Action.Load(br, m_Puzzle.m_iSize));                   
                }
                for (int iRow = 0; iRow < m_Puzzle.m_iSize; iRow++)
                {
                    for (int iCol = 0; iCol < m_Puzzle.m_iSize; iCol++)
                    {
                        for (int iIcon = 0; iIcon < m_Puzzle.m_iSize; iIcon++)
                        {
                            m_Puzzle.m_Rows[iRow].m_Cells[iCol].m_bValues[iIcon] = br.ReadBoolean();
                        }
                        m_Puzzle.m_Rows[iRow].m_Cells[iCol].m_iFinalIcon = m_Puzzle.m_Rows[iRow].m_Cells[iCol].GetRemainingIcon();
                    }
                }
                br.Close();
            }
        }

        void DeleteSavedPuzzle()
        {
            string saveName = Happiness.PuzzleSaveName(m_Puzzle.m_iSize, m_iPuzzleIndex);
            if (File.Exists(saveName))
            {
                File.Delete(saveName);
            }
        }
        #endregion

        #region Game Functions
        public void DoHint(bool verified = false)
        {
            Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Hint2);
            Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.Hint1);

            if (m_Hint == null)
            {
                if (Game.m_GameInfo.HardCurrency < 2)
                {
                    m_MessageBox = new MessageBox("You don't have enough coins for a hint (2)", MessageBoxButtons.BuyCoinsCancel, (int)MessageBoxContext.InsufficientFunds_Hint, Game.ScreenWidth, Game.ScreenHeight);
                }
                else if (m_bVerifyHintPurchase && !verified)
                {
                    m_MessageBox = new MessageBox("Would you like to spend 2 coins for a hint?", MessageBoxButtons.YesNo, (int)MessageBoxContext.InsufficientFunds_Hint, Game.ScreenWidth, Game.ScreenHeight, "Don't ask again");
                }
                else
                {
                    // Subtract the coins
                    Game.SpendCoins(2, 1);

                    // Show the hint
                    List<Clue> visibleClues = new List<Clue>();
                    visibleClues.AddRange(m_HorizontalCluePanel.Clues);
                    visibleClues.AddRange(m_VerticalCluePanel.Clues);
                    m_Hint = m_Puzzle.GenerateHint(visibleClues.ToArray());

                    // Modify the count
                    m_iHintCount++;
                    int maxHints = Game.m_GameInfo.VipData.Hints;
                    m_ButtonPanel.SetHintCount(maxHints - m_iHintCount, maxHints);

                    SavePuzzle();

                    // Play the sound
                    // m_SoundManager.PlayGameHint();
                }
            }
        }

        public void DoMegaHint(bool verified = false)
        {
            if (Game.m_GameInfo.HardCurrency < 50)
            {
                m_MessageBox = new MessageBox("You don't have enough coins for a Mega Hint (50)", MessageBoxButtons.BuyCoinsCancel, (int)MessageBoxContext.InusfficientFunds_MegaHint, Game.ScreenWidth, Game.ScreenHeight);
            }
            else if (m_bVerifyMegaHintPurchase && !verified)
            {
                m_MessageBox = new MessageBox("Would you like to spend 50 coins for a Mega Hint?", MessageBoxButtons.YesNo, (int)MessageBoxContext.InusfficientFunds_MegaHint, Game.ScreenWidth, Game.ScreenHeight, "Don't ask again");
            }
            else
            {
                // Subtract the coins
                Game.SpendCoins(50, 2);

                // Modify the count
                m_iMegaHintCount++;
                int maxHints = Game.m_GameInfo.VipData.MegaHints;
                m_ButtonPanel.SetMegaHintCount(maxHints - m_iMegaHintCount, maxHints);

                // Show the hint
                Random rand = new Random();
                while (true)
                {
                    int row = rand.Next(m_Puzzle.m_iSize);
                    int col = rand.Next(m_Puzzle.m_iSize);
                    if (m_Puzzle.m_Rows[row].m_Cells[col].m_iFinalIcon != m_Puzzle.m_Solution[row, col])
                    {
                        // Found one we can give
                        DoAction(eActionType.eAT_SetFinalIcon, row, col, m_Puzzle.m_Solution[row, col]);
                        break;
                    }
                }

                // Play the sound
                // m_SoundManager.PlayGameHint();
            }
        }

        public void DoUndo()
        {
            if (m_History.Count > 0)
            {
                int index = m_History.Count - 1;
                Action a = m_History[index];
                m_History.RemoveAt(index);

                a.Revert(m_Puzzle);

                m_ButtonPanel.SetUndoCount(m_History.Count, Game.m_GameInfo.VipData.UndoSize);
            }
        }

        public void DoBuyCoins(MessageBoxContext context)
        {

        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if( Game.CurrentScene != this )
                return;

            m_ButtonPanel.SetCoins(Game.m_GameInfo.HardCurrency);
            if (m_Hint != null)
            {
                if( m_Hint.ShouldHide(m_Puzzle) )
                    m_Hint = null;
            }

            if (m_Puzzle.IsCompleted())
            {
                DoEndScreenUpdate(gameTime);
            }
        }
        #endregion

        #region EndScreen
        void DoEndScreenUpdate(GameTime gameTime)
        {
            if (m_ReconnectScreen != null)
            {
                m_ReconnectScreen.Update(gameTime);
            }           
            else
            {
                if (m_EndScreen == null)
                {
                    DeleteSavedPuzzle();
                    m_EndScreen = new EndPuzzleScreen(m_Puzzle.IsSolved(), m_Puzzle.m_iSize, ElapsedTime, Game.ScreenWidth, Game.ScreenHeight, Game);
                    m_EndScreen.OnNextPuzzle += M_EndScreen_OnNextPuzzle;
                    m_EndScreen.OnMainMenu += M_EndScreen_OnMainMenu;
                    m_EndScreen.OnRestartPuzzle += M_EndScreen_OnRestartPuzzle;
                    if (m_Puzzle.IsSolved())
                    {
                        Game.SoundManager.PlaySound(SoundManager.SEInst.GamePuzzleComplete);
                        Game.SavePuzzleData(m_Puzzle.m_iSize - 3, m_iPuzzleIndex, ElapsedTime, m_EndScreen.OnServerDataComplete);
                    }
                    else
                    {
                        Game.SoundManager.PlaySound(SoundManager.SEInst.GamePuzzleFailed);
                    }
                }
                else
                    m_EndScreen.Update(gameTime);
            }
        }

        private void M_EndScreen_OnRestartPuzzle(object sender, EventArgs e)
        {
            Initialize(m_iPuzzleIndex, m_Puzzle.m_iSize, false);
        }

        private void M_EndScreen_OnMainMenu(object sender, EventArgs e)
        {
            Game.GotoScene(new HubScene(Game));
        }

        private void M_EndScreen_OnNextPuzzle(object sender, EventArgs e)
        {
            Initialize(m_iPuzzleIndex + 1, m_Puzzle.m_iSize, true);
        }
        #endregion

        #region Drawing
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw all the UI Pannels
            for (int i = m_UIPanels.Count - 1; i >= 0; i--)
                m_UIPanels[i].Draw(spriteBatch);

            if (m_ReconnectScreen != null)
                m_ReconnectScreen.Draw(spriteBatch, Game.ScreenWidth, Game.ScreenHeight);

            if (m_EndScreen != null)
                m_EndScreen.Draw(spriteBatch, Game.ScreenWidth, Game.ScreenHeight);

            if( m_PauseMenu != null )
                m_PauseMenu.Draw(spriteBatch);

            if( m_MessageBox != null )
                m_MessageBox.Draw(spriteBatch);
        }
        #endregion

        #region Actions
        public void DoAction(eActionType type, int iRow, int iCol, int iIcon)
        {
            switch (type)
            {
                case eActionType.eAT_EliminateIcon:
                    //m_SoundManager.PlayGameEliminateIcon();
                    break;
                case eActionType.eAT_RestoreIcon:
                    //m_SoundManager.PlayGameRestoreIcon();
                    break;
                case eActionType.eAT_SetFinalIcon:
                    //m_SoundManager.PlayGameSetFinalIcon();
                    break;
            }

            Action a = new Action(type, iRow, iCol, iIcon, m_Puzzle);
            a.Perform(m_Puzzle);            

            int undoSize = Game.m_GameInfo.VipData.UndoSize;
            while (m_History.Count >= undoSize)
            {
                m_History.RemoveAt(0);
            }
            m_History.Add(a);
            m_ButtonPanel.SetUndoCount(m_History.Count, undoSize);
            //HideHint(false);

            SavePuzzle();
        }
        #endregion

        #region Clue Management
        public Texture2D GetIcon(int row, int col)
        {
            return m_aIcons[row, col];
        }

        public bool ShouldShowHint(Clue c)
        {
            bool bHintClue = false;
            if (m_Hint != null && m_Hint.ShouldDraw(c))
                bHintClue = true;
            return bHintClue;
        }

        public bool ShouldDrawHint(int row, int col, int icon)
        {
            return ( m_Hint != null && m_Hint.ShouldDraw(row, col, icon) );
        }

        public void SelectClue(Clue clue, UIPanel panel)
        {
            if (panel == m_HorizontalCluePanel)
            {
                m_VerticalCluePanel.ClearSelected();
            }
            else
            {
                m_HorizontalCluePanel.ClearSelected();
            }
            m_HelpPanel.SelectedClue = clue;
            m_ButtonPanel.HideClueEnabled = clue != null;
        }

        public void HideSelectedClue()
        {
            m_VerticalCluePanel.HideSelectedClue();
            m_HorizontalCluePanel.HideSelectedClue();
            m_HelpPanel.SelectedClue = null;

            Game.Tutorial.FinishPiece(TutorialSystem.TutorialPiece.HideClue1);
        }

        public void UnHideAllClues()
        {
            m_VerticalCluePanel.UnhideClues(m_Puzzle.VerticalClues);
            m_HorizontalCluePanel.UnhideClues(m_Puzzle.HorizontalClues);
            m_HelpPanel.SelectedClue = null;
        }
        #endregion

        #region Input
        private void M_Input_OnClick(object sender, DragArgs e)
        {
            if( e.Abort )
                return;

            int iX = e.CurrentX;
            int iY = e.CurrentY;

            if (m_MessageBox != null)
            {
                MessageBoxResult res = m_MessageBox.HandleClick(iX, iY);
                if (res != MessageBoxResult.NoResult)
                {
                    DoMessageBoxResult(res);
                }
            }         
            else if (m_EndScreen != null)
            {
                if (!m_EndScreen.HandleClick(iX, iY))
                {
                    m_EndScreen = null;
                }
            }
            else if (m_PauseMenu != null)
            {
                if (!m_PauseMenu.HandleClick(iX, iY))
                {
                    switch (m_PauseMenu.m_iSelection)
                    {
                        case 0: // Resume Game
                            UnPause();
                            break;
                        case 1: // Reset Puzzle
                            m_Puzzle.Reset();
                            UnHideAllClues();
                            UnPause();
                            break;
                        case 2: // Unhide Clues
                            UnHideAllClues();
                            UnPause();
                            break;
                        case 3: // Buy Coins
                            break;
                        case 4: // Options
                            break;
                        case 5: // Save & Exit
                            UnPause();
                            SavePuzzle();
                            Game.GotoScene(new HubScene(Game));
                            break;
                    }
                }
            }            
            else
            {
                foreach (UIPanel p in m_UIPanels)
                {
                    if (p.Contains(e.CurrentX, e.CurrentY))
                    {
                        p.Click(e.CurrentX, e.CurrentY);
                        break;
                    }
                }
            }


        }

        private void M_Input_OnDragEnd(object sender, DragArgs e)
        {
            if (m_PauseMenu != null)
            {
                m_PauseMenu.OnDragEnd(e);
            }
            else if (m_UIPanels != null)
            {
                foreach (UIPanel p in m_UIPanels)
                {
                    if (p.Rect.Contains(e.StartX, e.StartY))
                    {
                        p.DragEnd(e);
                        break;
                    }
                }
            }
        }

        private void M_Input_OnDrag(object sender, DragArgs e)
        {
            if (m_PauseMenu != null)
            {
                m_PauseMenu.OnDrag(e);
            }
            else if (m_UIPanels != null)
            {
                foreach (UIPanel p in m_UIPanels)
                {
                    if (p.Rect.Contains(e.StartX, e.StartY))
                    {
                        p.Drag(e);
                        break;
                    }
                }
            }
        }

        private void M_Input_OnDragBegin(object sender, DragArgs e)
        {
            if (m_PauseMenu != null)
            {
                m_PauseMenu.OnDragBegin(e);
            }
            else if (m_UIPanels != null)
            {
                foreach (UIPanel p in m_UIPanels)
                {
                    if (p.Rect.Contains(e.StartX, e.StartY))
                    {
                        p.DragBegin(e);
                        break;
                    }
                }
            }
        }

        void M_Input_OnKeyUp(object sender, KeyArgs e)
        {
            if( e.Key == Microsoft.Xna.Framework.Input.Keys.Escape )
                Pause();
        }

        void DoMessageBoxResult(MessageBoxResult res)
        {
            MessageBoxContext context = (MessageBoxContext)m_MessageBox.Context;
            switch (context)
            {
                case MessageBoxContext.InsufficientFunds_Hint:
                case MessageBoxContext.InusfficientFunds_MegaHint:
                    // Either result, kill the message box
                    bool bChecked = m_MessageBox.Checkbox;
                    m_MessageBox = null;

                    if (res == MessageBoxResult.BuyCoins)
                    {
                        DoBuyCoins(context);
                    }
                    else if (res == MessageBoxResult.Yes)
                    {
                        if (context == MessageBoxContext.InsufficientFunds_Hint)
                        {
                            m_bVerifyHintPurchase = !bChecked;
                            DoHint(true);
                        }
                        else
                        {
                            m_bVerifyMegaHintPurchase = !bChecked;
                            DoMegaHint(true);
                        }
                    }
                    break;
            }
        }
        #endregion

        #region Accessors
        public Puzzle Puzzle
        {
            get { return m_Puzzle; }
        }

        public double ElapsedTime
        {
            get { return (DateTime.Now - m_PuzzleStart).TotalSeconds - m_PauseSeconds; }
        }

        public bool ClockRunning
        {
            get { return m_EndScreen == null; }
        }
        #endregion
    }
}
