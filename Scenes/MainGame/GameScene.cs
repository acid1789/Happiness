using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LogicMatrix;

namespace Happiness
{
    public class GameScene : Scene
    {
        // Dialogs
        EndPuzzleScreen m_EndScreen;
        ReconnectScreen m_ReconnectScreen;

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

        public GameScene(Happiness hgame) : base(hgame)
        {
            // Init Input
            InputController.IC.OnDragBegin += M_Input_OnDragBegin;
            InputController.IC.OnDrag += M_Input_OnDrag;
            InputController.IC.OnDragEnd += M_Input_OnDragEnd;
            InputController.IC.OnClick += M_Input_OnClick;

            m_PuzzleStart = DateTime.Now;
            m_PauseSeconds = 0;
        }

        #region Initialization
        public void Initialize(int puzzleIndex, int puzzleSize)
        {
            // Create the puzzle
            m_iPuzzleIndex = puzzleIndex;
            m_Puzzle = new Puzzle(puzzleIndex, puzzleSize, 1);            
            double elapsed = SaveGame.LoadPuzzle(m_Puzzle, puzzleSize, puzzleIndex);
            if( elapsed > 0 )
                m_PuzzleStart.AddSeconds(-elapsed);

            // Initialize the UI
            int buttonPanelWidth = (int)(Constants.ButtonPanel_Width * Game.ScreenWidth);
            int helpPanelHeight = (int)(Constants.HelpPanel_Height * Game.ScreenHeight);
            m_HorizontalCluePanel = new HorizontalCluePanel(this, m_Puzzle.HorizontalClues);
            m_VerticalCluePanel = new VerticalCluePanel(this, m_Puzzle.VerticalClues, Game.ScreenWidth - m_HorizontalCluePanel.Rect.Width);
            m_GamePanel = new GamePanel(this, new Rectangle(buttonPanelWidth, helpPanelHeight, Game.ScreenWidth - (m_HorizontalCluePanel.Rect.Width + buttonPanelWidth), Game.ScreenHeight - (m_VerticalCluePanel.Rect.Height + helpPanelHeight)));
            m_HelpPanel = new HelpPanel(this, new Rectangle(buttonPanelWidth, 0, m_GamePanel.Rect.Width, m_GamePanel.Rect.Top));
            m_ButtonPanel = new ButtonPanel(this, new Rectangle(0, 0, buttonPanelWidth, m_VerticalCluePanel.Rect.Top));

            m_UIPanels = new List<UIPanel>();
            m_UIPanels.Add(m_GamePanel);
            m_UIPanels.Add(m_HorizontalCluePanel);
            m_UIPanels.Add(m_VerticalCluePanel);
            m_UIPanels.Add(m_HelpPanel);
            m_UIPanels.Add(m_ButtonPanel);

            // Init Icons
            InitIcons();
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
        }
        #endregion

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
        
        public void SavePuzzle()
        {
            SaveGame.SavePuzzle(m_Puzzle, m_iPuzzleIndex, ElapsedTime);
            //m_SoundManager.PlayGameSave();            
        }

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (m_Puzzle.IsCompleted())
            {
                if (m_ReconnectScreen != null)
                {
                    m_ReconnectScreen.Update(gameTime);
                }
                else if (!NetworkManager.Net.Connected)
                {
                    m_ReconnectScreen = new ReconnectScreen();
                }
                else
                {
                    if (m_EndScreen == null)
                    {
                        m_EndScreen = new EndPuzzleScreen();
                        m_EndScreen.m_bSuccess = m_Puzzle.IsSolved();
                        if (m_EndScreen.m_bSuccess)
                        {
                            //m_SoundManager.PlayPuzzleComplete();
                            NetworkManager.Net.PuzzleComplete(m_Puzzle.m_iSize - 3, m_iPuzzleIndex, ElapsedTime);
                        }
                        else
                        {
                            //m_SoundManager.PlayPuzzleFailed();
                        }
                    }
                }
            }
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
            //m_aHistory.Add(a);
            //m_aFuture.Clear();
            //HideHint(false);
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
            //if (m_Hint != null && m_Hint.ShouldDraw(c))
            //    bHintClue = true;
            return bHintClue;
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
            int iX = e.CurrentX;
            int iY = e.CurrentY;

            if (m_EndScreen != null)
            {
                if (!m_EndScreen.HandleClick(iX, iY))
                {
                    switch (m_EndScreen.m_iSelection)
                    {
                        case 0: // Next Puzzle
                            Initialize(m_iPuzzleIndex + 1, m_Puzzle.m_iSize);
                            break;
                        case 1: // Restart Puzzle
                            m_Puzzle.Reset();
                            UnHideAllClues();
                            break;
                        case 2: // Main Menu
                            Game.GotoScene(new HubScene(Game));
                            break;
                    }
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
                        case 4: // Save & Exit
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
        #endregion
    }
}
