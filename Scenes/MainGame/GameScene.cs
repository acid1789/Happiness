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

            HappinessNetwork.VipDataArgs vip = NetworkManager.Net.VipData;
            m_ButtonPanel.SetHintCount(vip.Hints - m_iHintCount, vip.Hints);
            m_ButtonPanel.SetMegaHintCount(vip.MegaHints - m_iMegaHintCount, vip.MegaHints);
            m_ButtonPanel.SetUndoCount(m_History.Count, vip.UndoSize);
            m_ButtonPanel.SetCoins(NetworkManager.Net.HardCurrency);

            m_UIPanels = new List<UIPanel>();
            m_UIPanels.Add(m_GamePanel);
            m_UIPanels.Add(m_HorizontalCluePanel);
            m_UIPanels.Add(m_VerticalCluePanel);
            m_UIPanels.Add(m_HelpPanel);
            m_UIPanels.Add(m_ButtonPanel);

            // Init Icons
            InitIcons();

            //m_EndScreen = new EndPuzzleScreen(true, Game.ScreenWidth, Game.ScreenHeight);
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
        string PuzzleSaveName(int puzzleSize, int puzzleIndex)
        {
            string saveName = string.Format("{0}/{1}_{2}.save", NetworkManager.Net.DisplayName, puzzleSize, puzzleIndex);
            if( !Directory.Exists(NetworkManager.Net.DisplayName) )
                Directory.CreateDirectory(NetworkManager.Net.DisplayName);
            return saveName;
        }

        public void SavePuzzle()
        {
            string saveName = PuzzleSaveName(m_Puzzle.m_iSize, m_iPuzzleIndex);
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
            //m_SoundManager.PlayGameSave();            
        }

        public void LoadPuzzle()
        {
            string saveName = PuzzleSaveName(m_Puzzle.m_iSize, m_iPuzzleIndex);
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
            string saveName = PuzzleSaveName(m_Puzzle.m_iSize, m_iPuzzleIndex);
            if (File.Exists(saveName))
            {
                File.Delete(saveName);
            }
        }
        #endregion

        #region Game Functions
        public void DoHint(bool verified = false)
        {
            if (m_Hint == null)
            {
                if (NetworkManager.Net.HardCurrency < 2)
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
                    NetworkManager.Net.SpendCoins(2, 1);

                    // Show the hint
                    List<Clue> visibleClues = new List<Clue>();
                    visibleClues.AddRange(m_HorizontalCluePanel.Clues);
                    visibleClues.AddRange(m_VerticalCluePanel.Clues);
                    m_Hint = m_Puzzle.GenerateHint(visibleClues.ToArray());

                    // Modify the count
                    m_iHintCount++;
                    int maxHints = NetworkManager.Net.VipData.Hints;
                    m_ButtonPanel.SetHintCount(maxHints - m_iHintCount, maxHints);

                    SavePuzzle();

                    // Play the sound
                    // m_SoundManager.PlayGameHint();
                }
            }
        }

        public void DoMegaHint(bool verified = false)
        {
            if (NetworkManager.Net.HardCurrency < 50)
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
                NetworkManager.Net.SpendCoins(50, 2);

                // Modify the count
                m_iMegaHintCount++;
                int maxHints = NetworkManager.Net.VipData.MegaHints;
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

                m_ButtonPanel.SetUndoCount(m_History.Count, NetworkManager.Net.VipData.UndoSize);
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

            m_ButtonPanel.SetCoins(NetworkManager.Net.HardCurrency);
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
            else if (!NetworkManager.Net.Connected && !NetworkManager.Net.Disabled)
            {
                m_ReconnectScreen = new ReconnectScreen();
            }
            else
            {
                if (m_EndScreen == null)
                {
                    DeleteSavedPuzzle();
                    m_EndScreen = new EndPuzzleScreen(m_Puzzle.IsSolved(), m_Puzzle.m_iSize, ElapsedTime, Game.ScreenWidth, Game.ScreenHeight);
                    m_EndScreen.OnNextPuzzle += M_EndScreen_OnNextPuzzle;
                    m_EndScreen.OnMainMenu += M_EndScreen_OnMainMenu;
                    m_EndScreen.OnRestartPuzzle += M_EndScreen_OnRestartPuzzle;
                    if (m_Puzzle.IsSolved())
                    {
                        //m_SoundManager.PlayPuzzleComplete();
                        NetworkManager.Net.PuzzleComplete(m_Puzzle.m_iSize - 3, m_iPuzzleIndex, ElapsedTime);
                    }
                    else
                    {
                        //m_SoundManager.PlayPuzzleFailed();
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

            int undoSize = NetworkManager.Net.VipData.UndoSize;
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
            get { return !m_Puzzle.IsSolved(); }
        }
        #endregion
    }
}
