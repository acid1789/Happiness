using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Diagnostics;

namespace LogicMatrix
{
    public class Puzzle
    {
        public int m_iSeed;
        public int m_iSize;
        public int m_iDifficulty;
        public int[,] m_Solution;
        public PuzzleRow[] m_Rows;
        public PuzzleRow[] m_MarkerRows;
        public Random m_Rand;
        public ArrayList m_Clues;
        public List<Clue> m_GivenClues;
        public List<Clue> m_VeritcalClues;
        public List<Clue> m_HorizontalClues;

        public Puzzle(int iSeed, int iSize, int iDifficulty)
        {
            m_iSeed = iSeed;
            m_iSize = iSize;
            m_iDifficulty = iDifficulty;

            m_Rand = new Random(iSeed);
            m_Rows = new PuzzleRow[m_iSize];
            for (int i = 0; i < iSize; i++)
            {
                m_Rows[i] = new PuzzleRow(iSize);
            }

            GenerateSolution();
            GenerateClues();
        }

        public int GetNumGivenClues()
        {
            int iCount = 0;
            for (int i = 0; i < m_Clues.Count; i++)
            {
                Clue c = (Clue)m_Clues[i];
                if (c.m_Type == eClueType.Given)
                    iCount++;
            }
            return iCount;
        }

        public static void RandomDistribution(Random rand, int[] rands)
        {
            for (int i = 0; i < rands.Length; i++)
            {
                int iRand = rand.Next(rands.Length);
                bool bGoodRandom = true;
                for (int j = 0; j < i; j++)
                {
                    if (rands[j] == iRand)
                    {
                        bGoodRandom = false;
                        break;
                    }
                }
                if (bGoodRandom)
                {
                    rands[i] = iRand;
                }
                else
                {
                    i--;
                }
            }
        }

        private void GenerateSolution()
        {
            m_Solution = new int[m_iSize, m_iSize];
            int[] rands = new int[m_iSize];

            // Generate a random distribution for each row
            for (int i = 0; i < m_iSize; i++)
            {                
                RandomDistribution(m_Rand, rands);
                for( int j = 0; j < m_iSize; j++ )
                {
                    m_Solution[i, j] = rands[j];
                }
            }
        }

        private void GenerateClues()
        {
            m_Clues = new ArrayList();
            while (!IsSolved())
            {
                Clue c = new Clue(this, m_Rand);
                if (ValidateClue(c))
                {
                    //Debug.Write("Adding Clue: ");
                    //Debug.WriteLine(m_Clues.Count);
                    m_Clues.Insert(0, c);

                    AnalyzeAllClues();
                }
            }

            Reset();

            Debug.WriteLine("Clue Count after initial generation: " + m_Clues.Count);

            // Optimize the clues
            OptimizeClues();

            Debug.WriteLine("Clue Count after optimization: " + m_Clues.Count);

            // Scramble All the clues
            ScrambleClues();

            // Reset the puzzle for actual play
            Reset();
        }

        private bool IsDuplicateClue(Clue testClue)
        {
            for (int i = 0; i < m_Clues.Count; i++)
            {
                Clue C = (Clue)m_Clues[i];
                if (testClue.m_Type == C.m_Type)
                {
                    if (C.m_Type == eClueType.Vertical)
                    {
                        if (C.m_VerticalType == testClue.m_VerticalType)
                        {
                            if (C.m_iCol == testClue.m_iCol)
                            {
                                switch (C.m_VerticalType)
                                {
                                    case eVerticalType.Two:
                                        if (C.m_iRow == testClue.m_iRow && C.m_iRow2 == testClue.m_iRow2)
                                            return true;
                                        break;
                                    case eVerticalType.Three:
                                        if (C.m_iRow == testClue.m_iRow && C.m_iRow2 == testClue.m_iRow2 && C.m_iRow3 == testClue.m_iRow3)
                                            return true;
                                        break;
                                    case eVerticalType.TwoNot:
                                        if (C.m_iRow == testClue.m_iRow && C.m_iRow2 == testClue.m_iRow2 && C.m_iNotCell == testClue.m_iNotCell)
                                            return true;
                                        break;
                                    case eVerticalType.EitherOr:
                                    case eVerticalType.ThreeTopNot:
                                    case eVerticalType.ThreeMidNot:
                                    case eVerticalType.ThreeBotNot:
                                        if (C.m_iRow == testClue.m_iRow && C.m_iRow2 == testClue.m_iRow2 && C.m_iRow3 == testClue.m_iRow3 && C.m_iNotCell == testClue.m_iNotCell)
                                            return true;
                                        break;
                                }
                            }
                        }
                    }
                    else if (C.m_Type == eClueType.Horizontal)
                    {
                        if (C.m_HorizontalType == testClue.m_HorizontalType)
                        {
                            switch (C.m_HorizontalType)
                            {
                                case eHorizontalType.NextTo:
                                case eHorizontalType.LeftOf:
                                case eHorizontalType.NotLeftOf:
                                    if (C.m_iRow == testClue.m_iRow && C.m_iCol == testClue.m_iCol && C.m_iRow2 == testClue.m_iRow2 && C.m_iCol2 == testClue.m_iCol2)
                                        return true;
                                    break;
                                case eHorizontalType.NotNextTo:
                                    if (C.m_iRow == testClue.m_iRow && C.m_iCol == testClue.m_iCol && C.m_iRow2 == testClue.m_iRow2 && C.m_iHorizontal1 == testClue.m_iHorizontal1)
                                        return true;
                                    break;
                                case eHorizontalType.Span:
                                    if (C.m_iRow == testClue.m_iRow && C.m_iCol == testClue.m_iCol && C.m_iRow2 == testClue.m_iRow2 && C.m_iCol2 == testClue.m_iCol2 && C.m_iRow3 == testClue.m_iRow3 && C.m_iCol3 == testClue.m_iCol3)
                                        return true;
                                    break;
                                case eHorizontalType.SpanNotLeft:
                                case eHorizontalType.SpanNotMid:
                                case eHorizontalType.SpanNotRight:
                                    if (C.m_iRow == testClue.m_iRow && C.m_iCol == testClue.m_iCol && C.m_iRow2 == testClue.m_iRow2 && C.m_iCol2 == testClue.m_iCol2 && C.m_iRow3 == testClue.m_iRow3 && C.m_iCol3 == testClue.m_iCol3 && C.m_iHorizontal1 == testClue.m_iHorizontal1)
                                        return true;
                                    break;
                            }
                        }
                    }
                    else // Given
                    {
                        if (C.m_iCol == testClue.m_iCol && C.m_iRow == testClue.m_iRow)
                            return true;
                    }
                }
            }
            return false;
        }

        private bool ValidateClue(Clue C)
        {
            if (IsDuplicateClue(C))
                return false;

            if (C.m_Type == eClueType.Vertical)
            {
                int iRow1 = -1;
                int iRow2 = -1;
                int iCol = C.m_iCol;
                switch (C.m_VerticalType)
                {
                    case eVerticalType.Two:
                    case eVerticalType.ThreeBotNot:
                        iRow1 = C.m_iRow;
                        iRow2 = C.m_iRow2;
                        break;
                    case eVerticalType.ThreeMidNot:
                        iRow1 = C.m_iRow;
                        iRow2 = C.m_iRow3;
                        break;
                    case eVerticalType.ThreeTopNot:
                        iRow1 = C.m_iRow2;
                        iRow2 = C.m_iRow3;
                        break;
                }
                if (iRow1 >= 0)
                {
                    for (int i = 0; i < m_Clues.Count; i++)
                    {
                        Clue cTest = (Clue)m_Clues[i];
                        if (cTest.m_Type == eClueType.Vertical)
                        {
                            switch (cTest.m_VerticalType)
                            {
                                case eVerticalType.Two:
                                case eVerticalType.ThreeBotNot:
                                    if (iRow1 == cTest.m_iRow && iRow2 == cTest.m_iRow2)
                                        return false;
                                    break;
                                case eVerticalType.ThreeMidNot:
                                    if (iRow1 == cTest.m_iRow && iRow2 == cTest.m_iRow3)
                                        return false;
                                    break;
                                case eVerticalType.ThreeTopNot:
                                    if (iRow1 == cTest.m_iRow2 && iRow2 == cTest.m_iRow3)
                                        return false;
                                    break;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private void AnalyzeAllClues()
        {
            // Go through each clue and use it to try to solve the puzzle
            for (int i = 0; i < m_Clues.Count; i++)
            {
                Clue c = (Clue)m_Clues[i];
                c.Analyze(this);
            }
        }

        private void BuildClueLists()
        {
            m_GivenClues = new List<Clue>();
            m_VeritcalClues = new List<Clue>();
            m_HorizontalClues = new List<Clue>();
            for (int i = 0; i < m_Clues.Count; i++)
            {
                Clue c = (Clue)m_Clues[i];
                switch (c.m_Type)
                {
                    case eClueType.Given:
                        m_GivenClues.Add(c);
                        break;
                    case eClueType.Vertical:
                        m_VeritcalClues.Add(c);
                        break;
                    case eClueType.Horizontal:
                        m_HorizontalClues.Add(c);
                        break;
                }
            }
        }

        private void OptimizeClues()
        {
            // Build the seperate clue lists
            BuildClueLists();

            // Reset the puzzle & apply all the givens
            Reset();

            // Solve again with the new clue order
            int iPass = 0;
            while (!IsSolved())
            {
                for (int i = 0; i < m_VeritcalClues.Count; i++)
                {
                    Clue c = m_VeritcalClues[i];
                    c.Analyze(this);
                }

                for (int i = 0; i < m_HorizontalClues.Count; i++)
                {
                    Clue c = m_HorizontalClues[i];
                    c.Analyze(this);
                }

                iPass++;                
                if (iPass > 100)
                {
                    Debug.WriteLine("Unsolvable in the optimize stage?");
                    DumpPuzzle();
                    DebugError();
                    return;
                }
            }
            Debug.Write("Passes: ");
            Debug.WriteLine(iPass++);

            // Sort the list based on most used
            m_Clues.Sort();

            // Remove any zero use count clues
            for (int i = m_Clues.Count - 1; i > 0; i--)
            {
                Clue C = (Clue)m_Clues[i];
                if (C.m_iUseCount > 0)
                    break;

                m_Clues.RemoveAt(i);
            }            

            // Resolve sorted
            Debug.WriteLine("Clue Count Before 2nd stage optimization: " + m_Clues.Count);
            BuildClueLists();
            Reset();
            iPass = 0;
            while (!IsSolved())
            {
                for (int i = 0; i < m_VeritcalClues.Count; i++)
                {
                    Clue c = m_VeritcalClues[i];
                    c.Analyze(this);
                }

                for (int i = 0; i < m_HorizontalClues.Count; i++)
                {
                    Clue c = m_HorizontalClues[i];
                    c.Analyze(this);
                }

                iPass++;
                if (iPass > 100)
                {
                    Debug.WriteLine("Unsolvable in the optimize stage?");
                    DumpPuzzle();
                    DebugError();
                    return;
                }
            }
            Debug.WriteLine("Passes: " + iPass);

            // Sort the list based on most used
            m_Clues.Sort();

            // Remove any zero use count clues
            for (int i = m_Clues.Count - 1; i > 0; i--)
            {
                Clue C = (Clue)m_Clues[i];
                if (C.m_iUseCount > 0)
                    break;

                m_Clues.RemoveAt(i);
            }

            // Add/Remove clues for difficulty
            Reset();
            if (m_iDifficulty == 0)
            {
                // Add some clues
                int iCluesToAdd = m_Rand.Next(4) + 1;
                int iNewClueCount = m_Clues.Count + iCluesToAdd;
                while (m_Clues.Count < iNewClueCount)
                {
                    Clue c = new Clue(this, m_Rand);
                    if (ValidateClue(c))
                    {
                        m_Clues.Add(c);
                    }
                }
            }
            else if (m_iDifficulty == 2)
            {
                // Remove some clues
                int iCluesToRemove = m_Rand.Next(m_iSize / 3) + 2;
                int iUseCount = 1;
                while (iCluesToRemove > 0)
                {
                    int iClueCount = m_Clues.Count;
                    for (int i = 0; i < m_Clues.Count; i++)
                    {
                        Clue C = (Clue)m_Clues[i];
                        if (C.m_iUseCount <= iUseCount)
                        {
                            iCluesToRemove--;
                            m_Clues.Remove(C);
                            break;
                        }
                    }
                    if (iClueCount == m_Clues.Count)
                        iUseCount++;
                }
            }

            // Rebuild the clues list
            BuildClueLists();

            // Reset the puzzle again
            Reset();
        }

        private void DumpPuzzle()
        {
            for (int i = 0; i < m_iSize; i++)
            {
                for (int j = 0; j < m_iSize; j++)
                {
                    Debug.Write("[");
                    for (int k = 0; k < m_iSize; k++)
                    {
                        if (!m_Rows[i].m_Cells[j].m_bValues[k])
                            Debug.Write(" ");
                        else
                            Debug.Write(k);
                        if (k < m_iSize - 1)
                            Debug.Write(",");
                    }
                    if (j < m_iSize - 1)
                        Debug.Write("], ");
                    else
                        Debug.WriteLine("]");
                }
            }

            Debug.WriteLine("");
            Debug.WriteLine("");
        }

        public void DumpSolution()
        {
            for (int i = 0; i < m_iSize; i++)
            {
                for (int j = 0; j < m_iSize; j++)
                {
                    Debug.Write("[");
                    Debug.Write(m_Solution[i, j]);
                    
                    if( j < m_iSize - 1)
                        Debug.Write("], ");
                    else
                        Debug.WriteLine("]");
                }
            }

            Debug.WriteLine("");
            Debug.WriteLine("");
        }

        private void DumpClues()
        {
            for (int i = 0; i < m_Clues.Count; i++)
            {
                Clue c = (Clue)m_Clues[i];
                c.Dump(i, this);
            }
        }

        private void ScrambleClues()
        {
            int iNumClues = m_Clues.Count;
            ArrayList copy = new ArrayList(m_Clues);
            int[] iScramble = new int[iNumClues];
            RandomDistribution(m_Rand, iScramble);

            for (int i = 0; i < iNumClues; i++)
            {
                m_Clues[i] = copy[iScramble[i]];
            }

            BuildClueLists();
        }

        private void ApplyAllGiven()
        {
            if (m_GivenClues != null)
            {
                for (int i = 0; i < m_GivenClues.Count; i++)
                {
                    Clue c = (Clue)m_GivenClues[i];
                    c.Analyze(this);
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < m_iSize; i++)
            {
                m_Rows[i].Reset();
            }

            for (int i = 0; i < m_Clues.Count; i++)
            {
                Clue c = (Clue)m_Clues[i];
                c.m_iUseCount = 0;
            }

            ApplyAllGiven();
        }

        public void ResetRow(int iRow)
        {
            for (int i = 0; i < m_iSize; i++)
            {
                m_Rows[iRow].m_Cells[i].Reset();                
            }
            ApplyAllGiven();
        }

        public bool IsCompleted()
        {
            ReEnforceFinalIcons();
            for (int i = 0; i < m_iSize; i++)
            {
                if (!m_Rows[i].IsCompleted())
                    return false;
            }
            return true;
        }

        public bool IsSolved()
        {
            if (IsCompleted())
            {
                // Check the puzzle against the solution
                for (int i = 0; i < m_iSize; i++)
                {
                    for (int j = 0; j < m_iSize; j++)
                    {
                        int iFinal = m_Rows[i].m_Cells[j].m_iFinalIcon;
                        if (iFinal != m_Solution[i, j])
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

        public bool IsCorrect(int iRow, int iCol)
        {
            int iFinal = m_Rows[iRow].m_Cells[iCol].m_iFinalIcon;
            return (iFinal >= 0 && iFinal == m_Solution[iRow, iCol]);
        }

        public int SolutionIcon(int iRow, int iCol)
        {
            return m_Solution[iRow, iCol];
        }

        private void DebugError()
        {
            Reset();
            DumpSolution();
            DumpPuzzle();
            DumpClues();
        }

        public void SetFinalIcon(int iRow, int iCol, int iIcon)
        {
            SetFinalIcon(null, iRow, iCol, iIcon);
        }

        public void SetFinalIcon(Clue theClue, int iRow, int iCol, int iIcon)
        {
            int iFinal = m_Rows[iRow].m_Cells[iCol].m_iFinalIcon;
            if (iFinal >= 0 && iFinal != iIcon)
            {
                Debug.WriteLine(string.Format("SetFinalIcon({0}, {1}, {2}) changing final icon from: {3}", iRow, iCol, iIcon, iFinal));

                DebugError();
            }

            if (!m_Rows[iRow].m_Cells[iCol].m_bValues[iIcon])
            {
                Debug.WriteLine(string.Format("SetFinalIcon({0}, {1}, {2}) setting final icon to an already eliminated icon", iRow, iCol, iIcon));
                DebugError();
            }

            if (iFinal != iIcon)
            {
                if (theClue != null)
                    theClue.m_iUseCount += 5;
                
                // Set the final icon
                m_Rows[iRow].m_Cells[iCol].m_iFinalIcon = iIcon;

                // Eliminate all the other icons in this cell
                for (int i = 0; i < m_iSize; i++)
                {
                    if (i != iIcon)
                        EliminateIcon(theClue, iRow, iCol, i);
                }

                // Eliminate the newly set icon from every other column on this row
                for (int i = 0; i < m_iSize; i++)
                {
                    if (i != iCol)
                        EliminateIcon(theClue, iRow, i, iIcon);
                }
            }
        }

        public void EliminateIcon(int iRow, int iCol, int iIcon)
        {
            EliminateIcon(null, iRow, iCol, iIcon);
        }

        public void EliminateIcon(Clue theClue, int iRow, int iCol, int iIcon)
        {
            int iFinal = m_Rows[iRow].m_Cells[iCol].m_iFinalIcon;
            if (iFinal == iIcon)
            {
                Debug.WriteLine(string.Format("EliminateIcon({0}, {1}, {2}) eliminating the final icon", iRow, iCol, iIcon));
                DebugError();
            }

            if (iIcon < 0)
            {
                Debug.WriteLine(string.Format("EliminateIcon({0}, {1}, {2}) eliminating a negative icon", iRow, iCol, iIcon));
                DebugError();
            }

            if (m_Rows[iRow].m_Cells[iCol].m_bValues[iIcon])
            {
                // If the icon hasnt been eliminated already and there is a clue driving this, mark the clue as used
                if (theClue != null)
                    theClue.m_iUseCount++;            

                // Eliminate the icon
                m_Rows[iRow].m_Cells[iCol].m_bValues[iIcon] = false;
                                
                if (m_Rows[iRow].m_Cells[iCol].m_iFinalIcon < 0)
                {
                    // Check to see if there is only one icon left in this cell
                    int iRemaining = m_Rows[iRow].m_Cells[iCol].GetRemainingIcon();
                    if (iRemaining >= 0)
                    {
                        // Only one icon remaining and the icon hasnt been 'set' yet
                        SetFinalIcon(theClue, iRow, iCol, iRemaining);
                    }
                }

                // Check to see if there is only one of this icon left on this row
                int iCount = 0;
                int iColumn = 0;
                for (int i = 0; i < m_iSize; i++)
                {
                    if (m_Rows[iRow].m_Cells[i].m_bValues[iIcon])
                    {
                        iCount++;
                        iColumn = i;
                    }
                }
                if (iCount == 1 && m_Rows[iRow].m_Cells[iColumn].m_iFinalIcon < 0)
                {
                    // There is only one of this icon left and it hasnt been 'set' in its column yet
                    SetFinalIcon(theClue, iRow, iColumn, iIcon);
                }              
            }
        }

        private void ReEnforceFinalIcons()
        {
            for (int iRow = 0; iRow < m_iSize; iRow++)
            {
                for (int iCol = 0; iCol < m_iSize; iCol++)
                {
                    int iFinal = m_Rows[iRow].m_Cells[iCol].m_iFinalIcon;
                    if (iFinal >= 0)
                    {
                        for (int j = 0; j < m_iSize; j++)
                        {
                            if( j != iCol )
                                m_Rows[iRow].m_Cells[j].m_bValues[iFinal] = false;
                        }
                    }
                }
            }
        }

        private void SetMarker()
        {
            m_MarkerRows = new PuzzleRow[m_Rows.Length];
            for (int i = 0; i < m_MarkerRows.Length; i++)
            {
                m_MarkerRows[i] = new PuzzleRow(m_Rows[i]);
            }
        }

        private void RestoreMarker()
        {
            m_Rows = new PuzzleRow[m_MarkerRows.Length];
            for (int i = 0; i < m_Rows.Length; i++)
            {
                m_Rows[i] = new PuzzleRow(m_MarkerRows[i]);
            }
        }

        public Hint GenerateHint(Clue[] VisibleClues)
        {
            Hint hRet = null;

            // Pick a clue that we could use for a hint
            for (int i = 0; i < VisibleClues.Length; i++)
            {
                SetMarker();
                if (VisibleClues[i] != null)
                {
                    int iUseCount = VisibleClues[i].m_iUseCount;
                    VisibleClues[i].Analyze(this);
                    RestoreMarker();

                    if (VisibleClues[i].m_iUseCount > iUseCount)
                    {
                        // This clue can do something, use it for the hint
                        hRet = new Hint();
                        if (!hRet.Init(this, VisibleClues[i]))
                        {
                            continue;
                        }
                        break;
                    }
                }
            }

            return hRet;
        }

        #region Accessors
        public Clue[] HorizontalClues
        {
            get { return m_HorizontalClues.ToArray(); }
        }

        public Clue[] VerticalClues
        {
            get { return m_VeritcalClues.ToArray(); }
        }
        #endregion
    }
}
