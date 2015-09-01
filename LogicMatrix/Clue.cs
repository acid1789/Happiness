using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace LogicMatrix
{
    public enum eClueType
    {
        Given,
        Vertical,
        Horizontal,
    };

    public enum eVerticalType
    {
        Two,
        Three,
        EitherOr,
        TwoNot,
        ThreeTopNot,
        ThreeMidNot,
        ThreeBotNot,
    };

    public enum eHorizontalType
    {
        NextTo,
        NotNextTo,
        LeftOf,
        NotLeftOf,
        Span,
        SpanNotLeft,
        SpanNotMid,
        SpanNotRight,
    };

    public class Clue : IComparable
    {
        public eClueType m_Type;
        public int m_iUseCount;
        public int m_iRow;
        public int m_iRow2;
        public int m_iRow3;
        public int m_iCol;
        public int m_iCol2;
        public int m_iCol3;
        public int m_iHorizontal1;
        public int m_iNotCell;
        public eVerticalType m_VerticalType;
        public eHorizontalType m_HorizontalType;

        public int CompareTo(object obj)
        {
            if (obj is Clue)
            {
                Clue OtherClue = (Clue)obj;
                return OtherClue.m_iUseCount.CompareTo(m_iUseCount);
            }
            else
            {
                throw new ArgumentException("Can only compare with other clues");
            }
        }

        public Clue(Puzzle p, Random rand)
        {
            GenerateClue(p, rand);
        }

        private void GenerateClue(Puzzle p, Random rand)
        {
            // Pick the clue type randomly
            PickClueType(p, rand);

            // Generate the clue
            switch (m_Type)
            {
                case eClueType.Given:
                    GenerateGiven(p, rand);
                    break;
                case eClueType.Vertical:
                    GenerateVertical(p, rand);
                    break;
                case eClueType.Horizontal:
                    GenerateHorizontal(p, rand);
                    break;
            }
        }

        private void PickClueType(Puzzle P, Random rand)
        {
            double dfVal = rand.NextDouble();
            if (dfVal < 0.1 && P.GetNumGivenClues() < 5)
                m_Type = eClueType.Given;
            else if (dfVal < 0.35)
                m_Type = eClueType.Vertical;
            else
                m_Type = eClueType.Horizontal;
        }

        private void GenerateGiven(Puzzle P, Random rand)
        {
            bool bGoodRowCol = false;

            while (!bGoodRowCol)
            {
                // Pick a random row/col to give
                m_iRow = rand.Next(0, P.m_iSize);
                m_iCol = rand.Next(0, P.m_iSize);

                // Check to see if we should give this one
                if (P.m_Rows[m_iRow].m_Cells[m_iCol].m_iFinalIcon < 0)
                    bGoodRowCol = true;
            }
        }

        private void GenerateVertical(Puzzle P, Random rand)
        {
            // Pick a random column that isnt complete
            bool bGoodColumn = false;
            while (!bGoodColumn)
            {
                m_iCol = rand.Next(0, P.m_iSize);
                for (int i = 0; i < P.m_iSize; i++)
                {
                    if (P.m_Rows[i].m_Cells[m_iCol].m_iFinalIcon < 0)
                    {
                        // This has a cell that isnt completed yet
                        bGoodColumn = true;
                        break;
                    }
                }
            }

            // Pick the vertical type
            double dfVal = rand.NextDouble();
            if (dfVal <= 0.142)
            {
                m_VerticalType = eVerticalType.Two;

                GenerateTwoRowColumn(P, rand);
            }
            else if (dfVal <= 0.284)
            {
                m_VerticalType = eVerticalType.Three;

                GenerateThreeRowColumn(P, rand);
            }
            else if (dfVal <= 0.426)
            {
                m_VerticalType = eVerticalType.EitherOr;

                int iTries = 0;
                bool bGoodRows = false;
                while (!bGoodRows)
                {
                    iTries++;
                    // Pick 3 random rows
                    m_iRow3 = m_iRow2 = m_iRow = rand.Next(0, P.m_iSize);
                    while (m_iRow2 == m_iRow)
                    {
                        m_iRow2 = rand.Next(0, P.m_iSize);
                    }
                    while (m_iRow3 == m_iRow2 || m_iRow3 == m_iRow)
                    {
                        m_iRow3 = rand.Next(0, P.m_iSize);
                    }

                    int iTemp1 = m_iRow;
                    int iTemp2 = m_iRow2;
                    int iTemp3 = m_iRow3;
                    m_iRow = Math.Min(iTemp1, Math.Min(iTemp2, iTemp3));
                    m_iRow3 = Math.Max(iTemp1, Math.Max(iTemp2, iTemp3));
                    m_iRow2 = (iTemp1 != m_iRow && iTemp1 != m_iRow3) ? iTemp1 : (iTemp2 != m_iRow && iTemp2 != m_iRow3) ? iTemp2 : iTemp3;

                    // Pick one to be the not
                    dfVal = rand.NextDouble();
                    if (dfVal <= 0.5)
                        m_iNotCell = m_iRow2;
                    else
                        m_iNotCell = m_iRow3;

                    // Pick an icon to show as the not
                    while (true)
                    {
                        m_iHorizontal1 = rand.Next(0, P.m_iSize);
                        if (P.m_Solution[m_iNotCell, m_iCol] != m_iHorizontal1)
                            break;
                    }

                    // Make sure the either or isnt both already
                    bGoodRows = (P.m_Rows[m_iRow2].m_Cells[m_iCol].m_iFinalIcon < 0 ||
                                 P.m_Rows[m_iRow3].m_Cells[m_iCol].m_iFinalIcon < 0);

                    if( iTries > 5 )
                    {
                        GenerateClue(P, rand);
                        return;
                    }
                }
            }
            else if (dfVal <= 0.668)
            {
                m_VerticalType = eVerticalType.TwoNot;

                GenerateTwoRowColumn(P, rand);

                bool bGoodCell = false;
                while (!bGoodCell)
                {
                    m_iNotCell = rand.Next(0, P.m_iSize);
                    bGoodCell = (m_iNotCell != P.m_Solution[m_iRow2, m_iCol]);
                }
            }
            else if (dfVal <= 0.71)
            {
                m_VerticalType = eVerticalType.ThreeTopNot;

                GenerateThreeRowColumn(P, rand);

                // Make sure I picke a cell that is actually not the solution
                bool bGoodCell = false;
                while (!bGoodCell)
                {
                    m_iNotCell = rand.Next(0, P.m_iSize);
                    bGoodCell = (m_iNotCell != P.m_Solution[m_iRow, m_iCol]);
                }
            }
            else if( dfVal <= 0.852)
            {
                m_VerticalType = eVerticalType.ThreeMidNot;

                GenerateThreeRowColumn(P, rand);

                // Make sure I picke a cell that is actually not the solution
                bool bGoodCell = false;
                while (!bGoodCell)
                {
                    m_iNotCell = rand.Next(0, P.m_iSize);
                    bGoodCell = (m_iNotCell != P.m_Solution[m_iRow2, m_iCol]);
                }
            }
            else
            {
                m_VerticalType = eVerticalType.ThreeBotNot;

                GenerateThreeRowColumn(P, rand);

                // Make sure I picke a cell that is actually not the solution
                bool bGoodCell = false;
                while (!bGoodCell)
                {
                    m_iNotCell = rand.Next(0, P.m_iSize);
                    bGoodCell = (m_iNotCell != P.m_Solution[m_iRow3, m_iCol]);
                }
            }
        }

        private void GenerateTwoRowColumn(Puzzle P, Random rand)
        {
            bool bGoodRows = false;
            while (!bGoodRows)
            {
                // Pick 2 random rows
                m_iRow2 = m_iRow = rand.Next(0, P.m_iSize);
                while (m_iRow2 == m_iRow)
                {
                    m_iRow2 = rand.Next(0, P.m_iSize);
                }

                // Make sure one of the rows is at least useful
                bGoodRows = (P.m_Rows[m_iRow].m_Cells[m_iCol].m_iFinalIcon < 0 ||
                             P.m_Rows[m_iRow2].m_Cells[m_iCol].m_iFinalIcon < 0);
            }

            int iTemp1 = m_iRow;
            int iTemp2 = m_iRow2;
            m_iRow = Math.Min(iTemp1, iTemp2);
            m_iRow2 = Math.Max(iTemp1, iTemp2);
        }

        private void GenerateThreeRowColumn(Puzzle P, Random rand)
        {
            bool bGoodRows = false;
            while (!bGoodRows)
            {
                // Pick 3 random rows
                m_iRow3 = m_iRow2 = m_iRow = rand.Next(0, P.m_iSize);
                while (m_iRow2 == m_iRow)
                {
                    m_iRow2 = rand.Next(0, P.m_iSize);
                }
                while (m_iRow3 == m_iRow2 || m_iRow3 == m_iRow)
                {
                    m_iRow3 = rand.Next(0, P.m_iSize);
                }

                // Make sure one of the rows is at least useful
                bGoodRows = (P.m_Rows[m_iRow].m_Cells[m_iCol].m_iFinalIcon < 0 ||
                             P.m_Rows[m_iRow2].m_Cells[m_iCol].m_iFinalIcon < 0 ||
                             P.m_Rows[m_iRow3].m_Cells[m_iCol].m_iFinalIcon < 0);
            }

            int iTemp1 = m_iRow;
            int iTemp2 = m_iRow2;
            int iTemp3 = m_iRow3;
            m_iRow = Math.Min(iTemp1, Math.Min(iTemp2, iTemp3));
            m_iRow3 = Math.Max(iTemp1, Math.Max(iTemp2, iTemp3));
            m_iRow2 = (iTemp1 != m_iRow && iTemp1 != m_iRow3) ? iTemp1 : (iTemp2 != m_iRow && iTemp2 != m_iRow3) ? iTemp2 : iTemp3;
        }

        private void GenerateHorizontal(Puzzle P, Random rand)
        {
            // Generate the horizontal type
            double dfVal = rand.NextDouble();
            if (dfVal <= 0.125)
            {
                m_HorizontalType = eHorizontalType.NextTo;

                while (true)
                {
                    // Pick first icon randomly
                    m_iRow = rand.Next(0, P.m_iSize);
                    m_iCol = rand.Next(0, P.m_iSize);

                    // Pick neighboring column
                    if (m_iCol == 0)
                        m_iCol2 = 1;
                    else if (m_iCol == (P.m_iSize - 1))
                        m_iCol2 = m_iCol - 1;
                    else
                    {
                        dfVal = rand.NextDouble();
                        if (dfVal <= 0.5f)
                            m_iCol2 = m_iCol - 1;
                        else
                            m_iCol2 = m_iCol + 1;
                    }

                    // Pick a neighboring row
                    m_iRow2 = rand.Next(0, P.m_iSize);

                    // Make sure this clue is useful
                    if (P.m_Rows[m_iRow].m_Cells[m_iCol].m_iFinalIcon < 0 ||
                        P.m_Rows[m_iRow2].m_Cells[m_iCol2].m_iFinalIcon < 0)
                        break;
                }
                m_iRow3 = m_iRow;
            }
            else if (dfVal <= 0.250)
            {
                m_HorizontalType = eHorizontalType.NotNextTo;

                // Pick first icon randomly
                m_iRow = rand.Next(0, P.m_iSize);
                m_iCol = rand.Next(0, P.m_iSize);

                // Pick a neighboring row
                m_iRow2 = rand.Next(0, P.m_iSize);

                // Pick an icon that is not in m_iRow2 on either side of m_iCol
                int iTries = 0;
                while (true)
                {
                    if (iTries++ > 25)
                    {
                        GenerateHorizontal(P, rand);
                        return;
                    }

                    // Pick one randomly
                    m_iHorizontal1 = rand.Next(0, P.m_iSize);

                    // Make sure its not the same as the first icon
                    if (m_iRow2 == m_iRow && P.m_Solution[m_iRow, m_iCol] == m_iHorizontal1)
                        continue;

                    // Make sure its not on the left of the first column
                    if (m_iCol > 0 && P.m_Solution[m_iRow2, m_iCol - 1] == m_iHorizontal1)
                        continue;
                    
                    // Make sure its not on the right of the first column
                    if (m_iCol < (P.m_iSize - 1) && P.m_Solution[m_iRow2, m_iCol + 1] == m_iHorizontal1)
                        continue;

                    // This one looks fine
                    break;
                }
                m_iRow3 = m_iRow;
            }
            else if (dfVal <= 0.375)
            {
                m_HorizontalType = eHorizontalType.LeftOf;

                while (true)
                {
                    // Pick first icon randomly
                    m_iCol = rand.Next(0, P.m_iSize - 1);
                    m_iRow = rand.Next(0, P.m_iSize);

                    // Pick second icon
                    m_iCol2 = rand.Next(m_iCol + 1, P.m_iSize);
                    m_iRow2 = rand.Next(0, P.m_iSize);

                    // Make sure this is useful
                    if (P.m_Rows[m_iRow].m_Cells[m_iCol].m_iFinalIcon < 0 ||
                        P.m_Rows[m_iRow2].m_Cells[m_iCol2].m_iFinalIcon < 0)
                        break;
                }
            }
            else if (dfVal <= 0.500)
            {
                m_HorizontalType = eHorizontalType.NotLeftOf;

                int iTries = 0;
                while (true)
                {
                    // Pick first icon randomly
                    m_iCol = rand.Next(1, P.m_iSize);
                    m_iRow = rand.Next(0, P.m_iSize);

                    // Pick second icon
                    m_iCol2 = rand.Next(0, m_iCol);
                    m_iRow2 = rand.Next(0, P.m_iSize);

                    // Make sure this is useful
                    if (P.m_Rows[m_iRow].m_Cells[m_iCol].m_iFinalIcon < 0 ||
                        P.m_Rows[m_iRow2].m_Cells[m_iCol2].m_iFinalIcon < 0)
                        break;

                    iTries++;
                    if (iTries > 5)
                    {
                        GenerateClue(P, rand);
                        return;
                    }
                }
            }
            else if (dfVal <= 0.625)
            {
                m_HorizontalType = eHorizontalType.Span;

                GenerateHorizontalSpan(P, rand);                
            }
            else if (dfVal <= 0.750)
            {
                m_HorizontalType = eHorizontalType.SpanNotLeft;

                GenerateHorizontalSpan(P, rand);

                // Find an icon that is not the correct icon for m_iCol
                int iTries = 0;
                while (true)
                {
                    if (iTries++ > 25)
                    {
                        GenerateHorizontal(P, rand);
                        return;
                    }

                    m_iHorizontal1 = rand.Next(0, P.m_iSize);

                    if (P.m_Solution[m_iRow, m_iCol] == m_iHorizontal1)
                        continue;

                    if (P.m_Solution[m_iRow2, m_iCol2] == m_iHorizontal1)
                        continue;

                    if (P.m_Solution[m_iRow3, m_iCol3] == m_iHorizontal1)
                        continue;                        
                        
                    break;
                }
            }
            else if (dfVal <= 0.875)
            {
                m_HorizontalType = eHorizontalType.SpanNotMid;

                GenerateHorizontalSpan(P, rand);

                // Find an icon that is not the correct icon for m_iCol2
                int iTries = 0;
                while (true)
                {
                    if (iTries++ > 25)
                    {
                        GenerateHorizontal(P, rand);
                        return;
                    }

                    m_iHorizontal1 = rand.Next(0, P.m_iSize);

                    if (P.m_Solution[m_iRow, m_iCol] == m_iHorizontal1)
                        continue;

                    if (P.m_Solution[m_iRow2, m_iCol2] == m_iHorizontal1)
                        continue;

                    if (P.m_Solution[m_iRow3, m_iCol3] == m_iHorizontal1)
                        continue;

                    break;
                }
            }
            else
            {
                m_HorizontalType = eHorizontalType.SpanNotRight;

                GenerateHorizontalSpan(P, rand);

                // Find an icon that is not the correct icon for m_iCol3
                int iTries = 0;
                while (true)
                {
                    if (iTries++ > 25)
                    {
                        GenerateHorizontal(P, rand);
                        return;
                    }

                    m_iHorizontal1 = rand.Next(0, P.m_iSize);

                    if (P.m_Solution[m_iRow, m_iCol] == m_iHorizontal1)
                        continue;

                    if (P.m_Solution[m_iRow2, m_iCol2] == m_iHorizontal1)
                        continue;

                    if (P.m_Solution[m_iRow3, m_iCol3] == m_iHorizontal1)
                        continue;

                    break;
                }
            }
        }

        private void GenerateHorizontalSpan(Puzzle P, Random rand)
        {
            while (true)
            {
                // Pick the first icon
                m_iCol = rand.Next(0, P.m_iSize);

                // Pick the second & third columns
                if (m_iCol + 2 < P.m_iSize)
                {
                    m_iCol2 = m_iCol + 1;
                    m_iCol3 = m_iCol2 + 1;
                }
                else if (m_iCol - 2 >= 0)
                {
                    m_iCol2 = m_iCol - 1;
                    m_iCol3 = m_iCol2 - 1;
                }
                else
                    continue;

                // Generate the rows randomly
                m_iRow = rand.Next(0, P.m_iSize);
                m_iRow2 = rand.Next(0, P.m_iSize);
                m_iRow3 = rand.Next(0, P.m_iSize);

                // Make sure the clue is useful
                if (P.m_Rows[m_iRow].m_Cells[m_iCol].m_iFinalIcon < 0 ||
                    P.m_Rows[m_iRow2].m_Cells[m_iCol2].m_iFinalIcon < 0 ||
                    P.m_Rows[m_iRow3].m_Cells[m_iCol3].m_iFinalIcon < 0)
                    break;
            }
        }

        public void Analyze(Puzzle P)
        {
            switch (m_Type)
            {
                case eClueType.Given:
                    AnalyzeGiven(P);
                    break;
                case eClueType.Vertical:
                    AnalyzeVertical(P);
                    break;
                case eClueType.Horizontal:
                    AnalyzeHorizontal(P);
                    break;
            }
        }

        private void AnalyzeGiven(Puzzle P)
        {
            P.SetFinalIcon(this, m_iRow, m_iCol, P.m_Solution[m_iRow, m_iCol]);
            m_iUseCount += 25;
        }

        private void AnalyzeVertical(Puzzle P)
        {
            switch (m_VerticalType)
            {
                case eVerticalType.Two:
                    AnalyzeVerticalTwo(P);
                    break;
                case eVerticalType.Three:
                    AnalyzeVerticalThree(P);
                    break;
                case eVerticalType.EitherOr:
                    AnalyzeVerticalEitherOr(P);
                    break;
                case eVerticalType.TwoNot:
                    AnalyzeVerticalTwoNot(P);
                    break;
                case eVerticalType.ThreeTopNot:
                    AnalyzeVerticalThreeTopNot(P);
                    break;
                case eVerticalType.ThreeMidNot:
                    AnalyzeVerticalThreeMidNot(P);
                    break;
                case eVerticalType.ThreeBotNot:
                    AnalyzeVerticalThreeBotNot(P);
                    break;
            }
        }

        private void AnalyzeHorizontal(Puzzle P)
        {
            switch (m_HorizontalType)
            {
                case eHorizontalType.NextTo:
                    AnalyzeHorizontalNextTo(P);
                    break;
                case eHorizontalType.NotNextTo:
                    AnalyzeHorizontalNotNextTo(P);
                    break;
                case eHorizontalType.LeftOf:
                    AnalyzeHorizontalLeftOf(P);
                    break;
                case eHorizontalType.NotLeftOf:
                    AnalyzeHorizontalNotLeftOf(P);
                    break;
                case eHorizontalType.Span:
                    AnalyzeHorizontalSpan(P);
                    break;
                case eHorizontalType.SpanNotLeft:
                    AnalyzeHorizontalSpanNotLeft(P);
                    break;
                case eHorizontalType.SpanNotMid:
                    AnalyzeHorizontalSpanNotMid(P);
                    break;
                case eHorizontalType.SpanNotRight:
                    AnalyzeHorizontalSpanNotRight(P);
                    break;
            }
        }

        private void AnalyzeVerticalTwo(Puzzle P)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol];
            for (int i = 0; i < P.m_iSize; i++)
            {
                if (!P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                {
                    P.EliminateIcon(this, m_iRow2, i, iIcon2);                   
                }
                else if (!P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                {
                    P.EliminateIcon(this, m_iRow, i, iIcon1);                   
                }

                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    P.SetFinalIcon(this, m_iRow2, i, iIcon2);
                    return;
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    P.SetFinalIcon(this, m_iRow, i, iIcon1);
                    return;
                }
            }
        }

        private void AnalyzeVerticalThree(Puzzle P)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol];
            int iIcon3 = P.m_Solution[m_iRow3, m_iCol];
            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    P.SetFinalIcon(this, m_iRow2, i, iIcon2);
                    P.SetFinalIcon(this, m_iRow3, i, iIcon3);
                    return;
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    P.SetFinalIcon(this, m_iRow, i, iIcon1);
                    P.SetFinalIcon(this, m_iRow3, i, iIcon3);
                    return;
                }
                else if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon == iIcon3)
                {
                    P.SetFinalIcon(this, m_iRow, i, iIcon1);
                    P.SetFinalIcon(this, m_iRow2, i, iIcon2);
                    return;
                }

                if (!P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                {
                    P.EliminateIcon(this, m_iRow2, i, iIcon2);
                    P.EliminateIcon(this, m_iRow3, i, iIcon3);
                }
                else if (!P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                {
                    P.EliminateIcon(this, m_iRow, i, iIcon1);
                    P.EliminateIcon(this, m_iRow3, i, iIcon3);
                }
                else if (!P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                {
                    P.EliminateIcon(this, m_iRow, i, iIcon1);
                    P.EliminateIcon(this, m_iRow2, i, iIcon2);
                }
            }
        }

        private void AnalyzeVerticalEitherOr(Puzzle P)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = (m_iRow2 == m_iNotCell) ? m_iHorizontal1 : P.m_Solution[m_iRow2, m_iCol];
            int iIcon3 = (m_iRow3 == m_iNotCell) ? m_iHorizontal1 : P.m_Solution[m_iRow3, m_iCol];

            int iIcon2Col = -1;
            int iIcon3Col = -1;
            for( int i = 0; i < P.m_iSize; i++ )
            {
                if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    P.EliminateIcon(this, m_iRow3, i, iIcon3);
                    iIcon2Col = i;                   
                }
                if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon == iIcon3)
                {
                    P.EliminateIcon(this, m_iRow2, i, iIcon2);
                    iIcon3Col = i;                   
                }
                if (!P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2] &&
                    !P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                {
                    // If neither icon is in this column, icon1 cant be here either
                    P.EliminateIcon(this, m_iRow, i, iIcon1);
                }

                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    for (int j = 0; j < P.m_iSize; j++)
                    {
                        if( j == i )
                            continue;

                        if (P.m_Rows[m_iRow2].m_Cells[j].m_iFinalIcon == iIcon2)
                        {
                            P.SetFinalIcon(this, m_iRow3, i, iIcon3);                           
                        }
                        else if (P.m_Rows[m_iRow3].m_Cells[j].m_iFinalIcon == iIcon3)
                        {
                            P.SetFinalIcon(this, m_iRow2, i, iIcon2);                           
                        }
                    }
                }
            }

            if (iIcon2Col >= 0 && iIcon3Col >= 0)
            {
                for (int i = 0; i < P.m_iSize; i++)
                {
                    if (i != iIcon2Col && i != iIcon3Col)
                    {
                        P.EliminateIcon(this, m_iRow, i, iIcon1);                       
                    }
                }
            }
        }

        private void AnalyzeVerticalTwoNot(Puzzle P)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = m_iNotCell;

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    P.EliminateIcon(this, m_iRow2, i, iIcon2);
                   
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    P.EliminateIcon(this, m_iRow, i, iIcon1);                   
                }
            }
        }

        private void AnalyzeVerticalThreeTopNot(Puzzle P)
        {
            int iIcon1 = m_iNotCell;
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol];
            int iIcon3 = P.m_Solution[m_iRow3, m_iCol];

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    P.EliminateIcon(this, m_iRow2, i, iIcon2);
                    P.EliminateIcon(this, m_iRow3, i, iIcon3);                   
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    P.EliminateIcon(this, m_iRow, i, iIcon1);
                    P.SetFinalIcon(this, m_iRow3, i, iIcon3);
                    return;
                }
                else if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon == iIcon3)
                {
                    P.EliminateIcon(this, m_iRow, i, iIcon1);
                    P.SetFinalIcon(this, m_iRow2, i, iIcon2);
                    return;
                }
                else if (!P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                {
                    P.EliminateIcon(this, m_iRow3, i, iIcon3);
                }
                else if (!P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                {
                    P.EliminateIcon(this, m_iRow2, i, iIcon2);
                }
            }
        }

        private void AnalyzeVerticalThreeMidNot(Puzzle P)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = m_iNotCell;
            int iIcon3 = P.m_Solution[m_iRow3, m_iCol];

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    P.EliminateIcon(this, m_iRow, i, iIcon1);
                    P.EliminateIcon(this, m_iRow3, i, iIcon3);
                   
                }
                else if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    P.EliminateIcon(this, m_iRow2, i, iIcon2);
                    P.SetFinalIcon(this, m_iRow3, i, iIcon3);
                    return;
                }
                else if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon == iIcon3)
                {
                    P.EliminateIcon(this, m_iRow2, i, iIcon2);
                    P.SetFinalIcon(this, m_iRow, i, iIcon1);                    
                    return;
                }
                else if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon >= 0)
                {
                    P.EliminateIcon(this, m_iRow3, i, iIcon3);
                }
                else if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon >= 0)
                {
                    P.EliminateIcon(this, m_iRow, i, iIcon1);
                }
            }
        }

        private void AnalyzeVerticalThreeBotNot(Puzzle P)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol];
            int iIcon3 = m_iNotCell;

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon == iIcon3)
                {
                    P.EliminateIcon(this, m_iRow2, i, iIcon2);
                    P.EliminateIcon(this, m_iRow, i, iIcon1);
                   
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    P.EliminateIcon(this, m_iRow3, i, iIcon3);
                    P.SetFinalIcon(this, m_iRow, i, iIcon1);
                    return;
                }
                else if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    P.EliminateIcon(this, m_iRow3, i, iIcon3);
                    P.SetFinalIcon(this, m_iRow2, i, iIcon2);
                    return;
                }
                else if (!P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                {
                    P.EliminateIcon(this, m_iRow, i, iIcon1);
                }
                else if (!P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                {
                    P.EliminateIcon(this, m_iRow2, i, iIcon2);
                }
            }
        }

        private void AnalyzeHorizontalNextTo(Puzzle P)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol2];

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    if (i == 0)
                    {
                        P.SetFinalIcon(this, m_iRow2, 1, iIcon2);
                       
                    }
                    else if (i == P.m_iSize - 1)
                    {
                        P.SetFinalIcon(this, m_iRow2, i - 1, iIcon2);
                       
                    }
                    else
                    {
                        for (int j = 0; j < P.m_iSize; j++)
                        {
                            if (j == (i - 1) || j == (i + 1))
                                continue;
                            P.EliminateIcon(this, m_iRow2, j, iIcon2);
                           
                        }
                    }
                    break;
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    if (i == 0)
                    {
                        P.SetFinalIcon(this, m_iRow, 1, iIcon1);
                       
                    }
                    else if (i == P.m_iSize - 1)
                    {
                        P.SetFinalIcon(this, m_iRow, i - 1, iIcon1);
                       
                    }
                    else
                    {
                        for (int j = 0; j < P.m_iSize; j++)
                        {
                            if (j == (i - 1) || j == (i + 1))
                                continue;
                            P.EliminateIcon(this, m_iRow, j, iIcon1);
                           
                        }
                    }
                    break;
                }
                else
                {
                    if (i == 0)
                    {
                        if (!P.m_Rows[m_iRow2].m_Cells[i + 1].m_bValues[iIcon2])
                        {
                            P.EliminateIcon(this, m_iRow, i, iIcon1);
                           
                        }
                        if (!P.m_Rows[m_iRow].m_Cells[i + 1].m_bValues[iIcon1])
                        {
                            P.EliminateIcon(this, m_iRow2, i, iIcon2);
                           
                        }
                    }
                    else if (i == P.m_iSize - 1)
                    {
                        if (!P.m_Rows[m_iRow2].m_Cells[i - 1].m_bValues[iIcon2])
                        {
                            P.EliminateIcon(this, m_iRow, i, iIcon1);
                           
                        }
                        if (!P.m_Rows[m_iRow].m_Cells[i - 1].m_bValues[iIcon1])
                        {
                            P.EliminateIcon(this, m_iRow2, i, iIcon2);
                           
                        }
                    }
                    else
                    {
                        if (!P.m_Rows[m_iRow2].m_Cells[i + 1].m_bValues[iIcon2] &&
                            !P.m_Rows[m_iRow2].m_Cells[i - 1].m_bValues[iIcon2])
                        {
                            P.EliminateIcon(this, m_iRow, i, iIcon1);
                           
                        }
                        if (!P.m_Rows[m_iRow].m_Cells[i + 1].m_bValues[iIcon1] &&
                            !P.m_Rows[m_iRow].m_Cells[i - 1].m_bValues[iIcon1])
                        {
                            P.EliminateIcon(this, m_iRow2, i, iIcon2);
                           
                        }
                    }
                }
            }
        }

        private void AnalyzeHorizontalNotNextTo(Puzzle P)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = m_iHorizontal1;

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    if (i == 0)
                    {
                        P.EliminateIcon(this, m_iRow2, i + 1, iIcon2);
                       
                    }
                    else if (i == P.m_iSize - 1)
                    {
                        P.EliminateIcon(this, m_iRow2, i - 1, iIcon2);
                       
                    }
                    else
                    {
                        P.EliminateIcon(this, m_iRow2, i - 1, iIcon2);
                        P.EliminateIcon(this, m_iRow2, i + 1, iIcon2);
                       
                    }
                    break;
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    if (i == 0)
                    {
                        P.EliminateIcon(this, m_iRow, i + 1, iIcon1);
                       
                    }
                    else if (i == P.m_iSize - 1)
                    {
                        P.EliminateIcon(this, m_iRow, i - 1, iIcon1);
                       
                    }
                    else
                    {
                        P.EliminateIcon(this, m_iRow, i - 1, iIcon1);
                        P.EliminateIcon(this, m_iRow, i + 1, iIcon1);
                       
                    }
                    break;
                }
            }
        }

        private void AnalyzeHorizontalLeftOf(Puzzle P)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol2];

            int iFirstPossibleLeft = 0;
            for( iFirstPossibleLeft = 0; iFirstPossibleLeft < P.m_iSize; iFirstPossibleLeft++ )
            {
                if( P.m_Rows[m_iRow].m_Cells[iFirstPossibleLeft].m_bValues[iIcon1] )
                    break;
            }

            int iFirstPossibleRight = P.m_iSize - 1;
            for (iFirstPossibleRight = P.m_iSize - 1; iFirstPossibleRight >= 0; iFirstPossibleRight--)
            {
                if (P.m_Rows[m_iRow2].m_Cells[iFirstPossibleRight].m_bValues[iIcon2])
                    break;
            }

            if (iFirstPossibleLeft + 1 == iFirstPossibleRight)
            {
                // we have a solution for this clue
                P.SetFinalIcon(this, m_iRow, iFirstPossibleLeft, iIcon1);
                P.SetFinalIcon(this, m_iRow2, iFirstPossibleRight, iIcon2);
            }
            else
            {
                // Remove all icon2's from the left side of the first possible left
                for (int i = 0; i <= iFirstPossibleLeft; i++)
                {
                    P.EliminateIcon(this, m_iRow2, i, iIcon2);
                }

                // Remove all the icon1's from the right side of the first possible right
                for (int i = iFirstPossibleRight; i < P.m_iSize; i++)
                {
                    P.EliminateIcon(this, m_iRow, i, iIcon1);
                }
            }            
        }

        private void AnalyzeHorizontalNotLeftOf(Puzzle P)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol2];

            // If both icons are on the same row
            if (m_iRow == m_iRow2)
            {
                // Icon1 cant be in the zero column, because that would ensure that it is always left of Icon2
                P.EliminateIcon(this, m_iRow, 0, iIcon1);

                // Icon2 cant be in the last column, because that would ensure that it is always right of Icon1
                P.EliminateIcon(this, m_iRow2, P.m_iSize - 1, iIcon2);
               
            }

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    // Icon1 is known, remove all instances of icon2 to the right
                    for (int j = i + 1; j < P.m_iSize; j++)
                    {
                        P.EliminateIcon(this, m_iRow2, j, iIcon2);
                       
                    }
                    break;
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    // Icon2 is known, remove all instances of icon1 to the left
                    for (int j = 0; j < i; j++)
                    {
                        P.EliminateIcon(this, m_iRow, j, iIcon1);
                       
                    }
                    break;
                }
            }
        }

        private void AnalyzeHorizontalSpan(Puzzle P)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol2];
            int iIcon3 = P.m_Solution[m_iRow3, m_iCol3];

            // Icon2 cant be on either end
            P.EliminateIcon(this, m_iRow2, 0, iIcon2);
            P.EliminateIcon(this, m_iRow2, P.m_iSize - 1, iIcon2);
           

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (SolveSpan(i, m_iRow, iIcon1, false, m_iRow2, iIcon2, false, m_iRow3, iIcon3, false, P))
                    return;
                if (SolveSpan(i, m_iRow3, iIcon3, false, m_iRow2, iIcon2, false, m_iRow, iIcon1, false, P))
                    return;
            }
        }

        private bool SolveSpan(int iCol, int iRow1, int iIcon1, bool bNot1, int iRow2, int iIcon2, bool bNot2, int iRow3, int iIcon3, bool bNot3, Puzzle P)
        {
            int iFinal1 = P.m_Rows[iRow1].m_Cells[iCol].m_iFinalIcon;
            if (iFinal1 == iIcon1)
            {
                if (iCol < 2)
                {
                    if (bNot1)
                    {
                        int iFinal2Right = P.m_Rows[iRow2].m_Cells[iCol + 1].m_iFinalIcon;
                        if (iFinal2Right == iIcon2)
                        {
                            P.SetFinalIcon(this, iRow3, iCol, iIcon3);
                           
                        }
                    }
                    else
                    {
                        if (bNot2)
                        {
                            P.EliminateIcon(this, iRow2, iCol + 1, iIcon2);
                           
                        }
                        else
                        {
                            P.SetFinalIcon(this, iRow2, iCol + 1, iIcon2);
                           
                        }

                        if (bNot3)
                        {
                            P.EliminateIcon(this, iRow3, iCol + 2, iIcon3);
                           
                        }
                        else
                        {
                            P.SetFinalIcon(this, iRow3, iCol + 2, iIcon3);
                           
                        }
                        return true;
                    }
                }
                else if (iCol > P.m_iSize - 3)
                {
                    if (bNot1)
                    {
                        int iFinal2Left = P.m_Rows[iRow2].m_Cells[iCol - 1].m_iFinalIcon;
                        if (iFinal2Left == iIcon2)
                        {
                            P.SetFinalIcon(this, iRow3, iCol, iIcon3);
                           
                        }
                    }
                    else
                    {
                        if (bNot2)
                            P.EliminateIcon(this, iRow2, iCol - 1, iIcon2);
                        else
                            P.SetFinalIcon(this, iRow2, iCol - 1, iIcon2);

                        if (bNot3)
                            P.EliminateIcon(this, iRow3, iCol - 2, iIcon3);
                        else
                            P.SetFinalIcon(this, iRow3, iCol - 2, iIcon3);
                       
                        return true;
                    }
                }
                else
                {
                    int iFinal2Left = P.m_Rows[iRow2].m_Cells[iCol - 1].m_iFinalIcon;
                    int iFinal2Right = P.m_Rows[iRow2].m_Cells[iCol + 1].m_iFinalIcon;
                    if (iFinal2Left == iIcon2)
                    {
                        if (bNot1)
                        {
                            P.SetFinalIcon(this, iRow3, iCol, iIcon3);
                           
                        }
                        else if (bNot2)
                        {
                            P.SetFinalIcon(this, iRow3, iCol + 2, iIcon3);
                           
                        }
                        else if (bNot3)
                        {
                            P.EliminateIcon(this, iRow3, iCol - 2, iIcon3);
                           
                        }
                        else
                        {
                            P.SetFinalIcon(this, iRow3, iCol - 2, iIcon3);
                           
                        }
                        return true;
                    }
                    else if (iFinal2Right == iIcon2)
                    {
                        if (bNot1)
                        {
                            P.SetFinalIcon(this, iRow3, iCol, iIcon3);
                           
                        }
                        else if (bNot2)
                        {
                            P.SetFinalIcon(this, iRow3, iCol - 2, iIcon3);
                           
                        }
                        else if (bNot3)
                        {
                            P.EliminateIcon(this, iRow3, iCol + 2, iIcon3);
                           
                        }
                        else
                        {
                            P.SetFinalIcon(this, iRow3, iCol + 2, iIcon3);
                           
                        }
                        return true;
                    }
                    else if (!P.m_Rows[iRow2].m_Cells[iCol - 1].m_bValues[iIcon2] && !bNot1 && !bNot2)
                    {
                        P.SetFinalIcon(this, iRow2, iCol + 1, iIcon2);
                        if (bNot3)
                            P.EliminateIcon(this, iRow3, iCol + 2, iIcon3);
                        else
                            P.SetFinalIcon(this, iRow3, iCol + 2, iIcon3);
                       
                        return true;
                    }
                    else if (!P.m_Rows[iRow2].m_Cells[iCol + 1].m_bValues[iIcon2] && !bNot1 && !bNot2)
                    {
                        P.SetFinalIcon(this, iRow2, iCol - 1, iIcon2);
                        if (bNot3)
                            P.EliminateIcon(this, iRow3, iCol - 2, iIcon3);
                        else
                            P.SetFinalIcon(this, iRow3, iCol - 2, iIcon3);
                       
                        return true;
                    }
                    else if (!bNot3)
                    {
                        int iFinal3Left = P.m_Rows[iRow3].m_Cells[iCol - 2].m_iFinalIcon;
                        int iFinal3Right = P.m_Rows[iRow3].m_Cells[iCol + 2].m_iFinalIcon;
                        if (iFinal3Left == iIcon3)
                        {
                            if (bNot1)
                            {
                                P.SetFinalIcon(this, iRow2, iCol - 3, iIcon2);
                               
                                return true;
                            }
                            else if (bNot2)
                            {
                                P.EliminateIcon(this, iRow2, iCol - 1, iIcon2);
                               
                            }
                            else
                            {
                                P.SetFinalIcon(this, iRow2, iCol - 1, iIcon2);
                               
                                return true;
                            }
                        }
                        else if (iFinal3Right == iIcon3)
                        {
                            if (bNot1)
                            {
                                P.SetFinalIcon(this, iRow2, iCol + 3, iIcon2);
                               
                                return true;
                            }
                            else if (bNot2)
                            {
                                P.EliminateIcon(this, iRow2, iCol + 1, iIcon2);
                               
                            }
                            else
                            {
                                P.SetFinalIcon(this, iRow2, iCol + 1, iIcon2);
                               
                                return true;
                            }
                        }
                        else if (!P.m_Rows[iRow3].m_Cells[iCol - 2].m_bValues[iIcon3] && !bNot1 && !bNot2)
                        {
                            P.SetFinalIcon(this, iRow2, iCol + 1, iIcon2);
                            P.SetFinalIcon(this, iRow3, iCol + 2, iIcon3);
                           
                            return true;
                        }
                        else if (!P.m_Rows[iRow3].m_Cells[iCol + 2].m_bValues[iIcon3] && !bNot1 && !bNot2)
                        {
                            P.SetFinalIcon(this, iRow2, iCol - 1, iIcon2);
                            P.SetFinalIcon(this, iRow3, iCol - 2, iIcon3);
                           
                            return true;
                        }
                        else if( !bNot1 )
                        {
                            for (int j = 0; j < P.m_iSize; j++)
                            {
                                if (!bNot2 && j != (iCol - 1) && j != (iCol + 1))
                                {
                                    P.EliminateIcon(this, iRow2, j, iIcon2);
                                   
                                }
                                if (j != (iCol - 2) && j != (iCol + 2))
                                {
                                    P.EliminateIcon(this, iRow3, j, iIcon3);
                                   
                                }
                            }
                        }
                    }
                }
            }
            else if (iFinal1 >= 0 && !bNot1 && !bNot2 && !bNot3)
            {
                int iFinal3 = P.m_Rows[iRow3].m_Cells[iCol].m_iFinalIcon;
                if (iFinal3 != iIcon3 && iFinal3 >= 0)
                {
                    if (iCol == 0)
                    {
                        P.EliminateIcon(this, iRow2, iCol + 1, iIcon2);
                       
                    }
                    else if (iCol == P.m_iSize - 1)
                    {
                        P.EliminateIcon(this, iRow2, iCol - 1, iIcon2);
                       
                    }
                    else if (iCol == P.m_iSize - 3)
                    {
                        P.EliminateIcon(this, iRow1, iCol + 2, iIcon1);
                        P.EliminateIcon(this, iRow2, iCol + 1, iIcon2);
                        P.EliminateIcon(this, iRow3, iCol + 2, iIcon3);
                       
                    }
                    else if (iCol == 2)
                    {
                        P.EliminateIcon(this, iRow1, iCol - 2, iIcon1);
                        P.EliminateIcon(this, iRow2, iCol - 1, iIcon2);
                        P.EliminateIcon(this, iRow3, iCol - 2, iIcon3);
                       
                    }
                }
            }
            if (!P.m_Rows[iRow1].m_Cells[iCol].m_bValues[iIcon1] && !bNot1)
            {
                if (!bNot3)
                {
                    if (iCol + 4 < P.m_iSize)
                    {
                        if (!P.m_Rows[iRow1].m_Cells[iCol + 4].m_bValues[iIcon1])
                        {
                            P.EliminateIcon(this, iRow3, iCol + 2, iIcon3);
                        }
                    }
                    else if (iCol + 2 < P.m_iSize)
                    {
                        P.EliminateIcon(this, iRow3, iCol + 2, iIcon3);
                    }
                    if (iCol - 4 >= 0)
                    {
                        if (!P.m_Rows[iRow1].m_Cells[iCol - 4].m_bValues[iIcon1])
                        {
                            P.EliminateIcon(this, iRow3, iCol - 2, iIcon3);
                        }
                    }
                    else if (iCol - 2 >= 0)
                    {
                        P.EliminateIcon(this, iRow3, iCol - 2, iIcon3);
                    }
                }
                if (!bNot2)
                {
                    if (iCol + 2 < P.m_iSize)
                    {
                        if (!P.m_Rows[iRow1].m_Cells[iCol + 2].m_bValues[iIcon1])
                        {
                            P.EliminateIcon(this, iRow2, iCol + 1, iIcon2);
                        }
                    }
                    if (iCol - 2 >= 0)
                    {
                        if (!P.m_Rows[iRow1].m_Cells[iCol - 2].m_bValues[iIcon1])
                        {
                            P.EliminateIcon(this, iRow2, iCol - 1, iIcon2);
                        }
                    }
                }
            }
            if (P.m_Rows[iRow2].m_Cells[iCol].m_iFinalIcon == iIcon2 && !bNot2)
            {
                // Middle icon is known, eliminate impossible end icons
                for (int i = 0; i < P.m_iSize; i++)
                {
                    if (i != iCol - 1 && i != iCol + 1)
                    {
                        if (!bNot1)
                            P.EliminateIcon(this, iRow1, i, iIcon1);
                        if (!bNot3)
                            P.EliminateIcon(this, iRow3, i, iIcon3);
                    }
                }
            }

            return false;
        }

        private void AnalyzeHorizontalSpanNotLeft(Puzzle P)
        {
            int iIcon1 = m_iHorizontal1;
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol2];
            int iIcon3 = P.m_Solution[m_iRow3, m_iCol3];

            // Icon2 cant be on either end
            P.EliminateIcon(this, m_iRow2, 0, iIcon2);
            P.EliminateIcon(this, m_iRow2, P.m_iSize - 1, iIcon2);
           

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (SolveSpan(i, m_iRow, iIcon1, true, m_iRow2, iIcon2, false, m_iRow3, iIcon3, false, P))
                    return;
                if (SolveSpan(i, m_iRow3, iIcon3, false, m_iRow2, iIcon2, false, m_iRow, iIcon1, true, P))
                    return;
            }
        }

        private void AnalyzeHorizontalSpanNotMid(Puzzle P)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = m_iHorizontal1;
            int iIcon3 = P.m_Solution[m_iRow3, m_iCol3];

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (SolveSpan(i, m_iRow, iIcon1, false, m_iRow2, iIcon2, true, m_iRow3, iIcon3, false, P))
                    return;
                if (SolveSpan(i, m_iRow3, iIcon3, false, m_iRow2, iIcon2, true, m_iRow, iIcon1, false, P))
                    return;
            }
        }

        private void AnalyzeHorizontalSpanNotRight(Puzzle P)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol2];
            int iIcon3 = m_iHorizontal1;

            // Icon2 cant be on either end
            P.EliminateIcon(this, m_iRow2, 0, iIcon2);
            P.EliminateIcon(this, m_iRow2, P.m_iSize - 1, iIcon2);
           

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (SolveSpan(i, m_iRow, iIcon1, false, m_iRow2, iIcon2, false, m_iRow3, iIcon3, true, P))
                    return;
                if (SolveSpan(i, m_iRow3, iIcon3, true, m_iRow2, iIcon2, false, m_iRow, iIcon1, false, P))
                    return;
            }
        }

        public int[] GetRows()
        {
            int[] ret = new int[3];
            ret[0] = m_iRow;
            ret[1] = m_iRow2;
            ret[2] = m_iRow3;
            return ret;
        }

        public int GetIcons(Puzzle P, int[] iIcons)
        {
            switch (m_Type)
            {
                case eClueType.Given:
                    break;
                case eClueType.Vertical:
                    {
                        switch (m_VerticalType)
                        {
                            case eVerticalType.Two:
                                iIcons[0] = P.m_Solution[m_iRow, m_iCol];
                                iIcons[1] = P.m_Solution[m_iRow2, m_iCol];
                                return 2;
                            case eVerticalType.Three:
                                iIcons[0] = P.m_Solution[m_iRow, m_iCol];
                                iIcons[1] = P.m_Solution[m_iRow2, m_iCol];
                                iIcons[2] = P.m_Solution[m_iRow3, m_iCol];
                                return 3;
                            case eVerticalType.EitherOr:
                                iIcons[0] = P.m_Solution[m_iRow, m_iCol];
                                iIcons[1] = (m_iRow2 == m_iNotCell) ? m_iHorizontal1 : P.m_Solution[m_iRow2, m_iCol];
                                iIcons[2] = (m_iRow3 == m_iNotCell) ? m_iHorizontal1 : P.m_Solution[m_iRow3, m_iCol];
                                return 3;
                            case eVerticalType.TwoNot:
                                iIcons[0] = P.m_Solution[m_iRow, m_iCol];
                                iIcons[1] = m_iNotCell;
                                return 2;
                            case eVerticalType.ThreeTopNot:
                                iIcons[0] = m_iNotCell;
                                iIcons[1] = P.m_Solution[m_iRow2, m_iCol];
                                iIcons[2] = P.m_Solution[m_iRow3, m_iCol];
                                return 3;
                            case eVerticalType.ThreeMidNot:
                                iIcons[0] = P.m_Solution[m_iRow, m_iCol];
                                iIcons[1] = m_iNotCell;
                                iIcons[2] = P.m_Solution[m_iRow3, m_iCol];
                                return 3;
                            case eVerticalType.ThreeBotNot:
                                iIcons[0] = P.m_Solution[m_iRow, m_iCol];
                                iIcons[1] = P.m_Solution[m_iRow2, m_iCol];
                                iIcons[2] = m_iNotCell;
                                return 3;
                        }
                    }
                    break;
                case eClueType.Horizontal:
                    {
                        switch (m_HorizontalType)
                        {
                            case eHorizontalType.NextTo:
                                iIcons[0] = P.m_Solution[m_iRow, m_iCol];
                                iIcons[1] = P.m_Solution[m_iRow2, m_iCol2];
                                iIcons[2] = P.m_Solution[m_iRow, m_iCol];
                                return 3;
                            case eHorizontalType.NotNextTo:
                                iIcons[0] = P.m_Solution[m_iRow, m_iCol];
                                iIcons[1] = m_iHorizontal1;
                                iIcons[2] = P.m_Solution[m_iRow, m_iCol];
                                return 3;
                            case eHorizontalType.LeftOf:
                            case eHorizontalType.NotLeftOf:
                                iIcons[0] = P.m_Solution[m_iRow, m_iCol];
                                iIcons[1] = P.m_Solution[m_iRow2, m_iCol2];
                                return 2;
                            case eHorizontalType.Span:
                                iIcons[0] = P.m_Solution[m_iRow, m_iCol];
                                iIcons[1] = P.m_Solution[m_iRow2, m_iCol2];
                                iIcons[2] = P.m_Solution[m_iRow3, m_iCol3];
                                return 3;
                            case eHorizontalType.SpanNotLeft:
                                iIcons[0] = m_iHorizontal1;
                                iIcons[1] = P.m_Solution[m_iRow2, m_iCol2];
                                iIcons[2] = P.m_Solution[m_iRow3, m_iCol3];
                                return 3;
                            case eHorizontalType.SpanNotMid:
                                iIcons[0] = P.m_Solution[m_iRow, m_iCol];
                                iIcons[1] = m_iHorizontal1;
                                iIcons[2] = P.m_Solution[m_iRow3, m_iCol3];
                                return 3;
                            case eHorizontalType.SpanNotRight:
                                iIcons[0] = P.m_Solution[m_iRow, m_iCol];
                                iIcons[1] = P.m_Solution[m_iRow2, m_iCol2];
                                iIcons[2] = m_iHorizontal1;
                                return 3;
                        }
                    }
                    break;
            }
            return 0;
        }

        public void Dump(int iIndex, Puzzle P)
        {
            Debug.Write(string.Format("Clue({0}): ", iIndex));
            switch (m_Type)
            {
                case eClueType.Given:
                    Debug.WriteLine(string.Format("Type: Given ({0}, {1}, {2})", m_iRow, m_iCol, P.m_Solution[m_iRow, m_iCol]));
                    break;
                case eClueType.Vertical:
                    {
                        Debug.Write("Type: Vertical  VType: ");
                        switch (m_VerticalType)
                        {
                            case eVerticalType.Two:
                                Debug.WriteLine(string.Format("Two ([{0}]:{1}, [{2}]:{3})", m_iRow, P.m_Solution[m_iRow, m_iCol], m_iRow2, P.m_Solution[m_iRow2, m_iCol]));
                                break;
                            case eVerticalType.Three:
                                Debug.WriteLine(string.Format("Three ([{0}]:{1}, [{2}]:{3}, [{4}]:{5})", m_iRow, P.m_Solution[m_iRow, m_iCol], m_iRow2, P.m_Solution[m_iRow2, m_iCol], m_iRow3, P.m_Solution[m_iRow3, m_iCol]));
                                break;
                            case eVerticalType.EitherOr:
                                Debug.WriteLine(string.Format("EitherOr ([{0}]:{1}, [{2}]:{3}, [{4}]:{5})", m_iRow, P.m_Solution[m_iRow, m_iCol], m_iRow2, (m_iRow2 == m_iNotCell) ? m_iHorizontal1 : P.m_Solution[m_iRow2, m_iCol], m_iRow3, (m_iRow3 == m_iNotCell) ? m_iHorizontal1 : P.m_Solution[m_iRow3, m_iCol]));
                                break;
                            case eVerticalType.TwoNot: 
                                Debug.WriteLine(string.Format("TwoNot ([{0}]:{1}, [{2}]:{3})", m_iRow, P.m_Solution[m_iRow, m_iCol], m_iRow2, m_iNotCell));
                                break;
                            case eVerticalType.ThreeTopNot:
                                Debug.WriteLine(string.Format("ThreeTopNot ([{0}]:{1}, [{2}]:{3}, [{4}]:{5})", m_iRow, m_iNotCell, m_iRow2, P.m_Solution[m_iRow2, m_iCol], m_iRow3, P.m_Solution[m_iRow3, m_iCol]));
                                break;
                            case eVerticalType.ThreeMidNot:
                                Debug.WriteLine(string.Format("ThreeMidNot ([{0}]:{1}, [{2}]:{3}, [{4}]:{5})", m_iRow, P.m_Solution[m_iRow, m_iCol], m_iRow2, m_iNotCell, m_iRow3, P.m_Solution[m_iRow3, m_iCol]));
                                break;
                            case eVerticalType.ThreeBotNot:
                                Debug.WriteLine(string.Format("ThreeBotNot ([{0}]:{1}, [{2}]:{3}, [{4}]:{5})", m_iRow, P.m_Solution[m_iRow, m_iCol], m_iRow2, P.m_Solution[m_iRow2, m_iCol], m_iRow3, m_iNotCell));
                                break;
                        }
                    }
                    break;
                case eClueType.Horizontal:
                    {
                        Debug.Write("Type: Horizontal  HType: ");
                        switch (m_HorizontalType)
                        {
                            case eHorizontalType.NextTo: 
                                Debug.WriteLine(string.Format("NextTo ([{0}]:{1}, [{2}]:{3})", m_iRow, P.m_Solution[m_iRow, m_iCol], m_iRow2, P.m_Solution[m_iRow2, m_iCol2]));
                                break;
                            case eHorizontalType.NotNextTo:
                                Debug.WriteLine(string.Format("NotNextTo ([{0}]:{1}, [{2}]:{3})", m_iRow, P.m_Solution[m_iRow, m_iCol], m_iRow2, m_iHorizontal1));
                                break;
                            case eHorizontalType.LeftOf:
                                Debug.WriteLine(string.Format("LeftOf ([{0}]:{1}, [{2}]:{3})", m_iRow, P.m_Solution[m_iRow, m_iCol], m_iRow2, P.m_Solution[m_iRow2, m_iCol2]));
                                break;
                            case eHorizontalType.NotLeftOf:
                                Debug.WriteLine(string.Format("NotLeftOf ([{0}]:{1}, [{2}]:{3})", m_iRow, P.m_Solution[m_iRow, m_iCol], m_iRow2, P.m_Solution[m_iRow2, m_iCol2]));
                                break;
                            case eHorizontalType.Span:
                                Debug.WriteLine(string.Format("Span ([{0}]:{1}, [{2}]:{3}, [{4}]:{5})", m_iRow, P.m_Solution[m_iRow, m_iCol], m_iRow2, P.m_Solution[m_iRow2, m_iCol2], m_iRow3, P.m_Solution[m_iRow3, m_iCol3]));
                                break;
                            case eHorizontalType.SpanNotLeft:
                                Debug.WriteLine(string.Format("SpanNotLeft ([{0}]:{1}, [{2}]:{3}, [{4}]:{5})", m_iRow, m_iHorizontal1, m_iRow2, P.m_Solution[m_iRow2, m_iCol2], m_iRow3, P.m_Solution[m_iRow3, m_iCol3]));
                                break;
                            case eHorizontalType.SpanNotMid:
                                Debug.WriteLine(string.Format("SpanNotMid ([{0}]:{1}, [{2}]:{3}, [{4}]:{5})", m_iRow, P.m_Solution[m_iRow, m_iCol], m_iRow2, m_iHorizontal1, m_iRow3, P.m_Solution[m_iRow3, m_iCol3]));
                                break;
                            case eHorizontalType.SpanNotRight:
                                Debug.WriteLine(string.Format("SpanNotRight ([{0}]:{1}, [{2}]:{3}, [{4}]:{5})", m_iRow, P.m_Solution[m_iRow, m_iCol], m_iRow2, P.m_Solution[m_iRow2, m_iCol2], m_iRow3, m_iHorizontal1));
                                break;
                        }
                    }
                    break;
            }
        }

        #region Hint
        public bool GetHintAction(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            switch (m_Type)
            {
                case eClueType.Horizontal:
                    switch (m_HorizontalType)
                    {
                        case eHorizontalType.NextTo:
                            return GetHintActionHorizontalNextTo(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        case eHorizontalType.NotNextTo:
                            return GetHintActionHorizontalNotNextTo(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        case eHorizontalType.LeftOf:
                            return GetHintActionHorizontalLeftOf(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        case eHorizontalType.NotLeftOf:
                            return GetHintActionHorizontalNotLeftOf(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        case eHorizontalType.Span:
                            return GetHintActionHorizontalSpan(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        case eHorizontalType.SpanNotLeft:
                            return GetHintActionHorizontalSpanNotLeft(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        case eHorizontalType.SpanNotMid:
                            return GetHintActionHorizontalSpanNotMid(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        case eHorizontalType.SpanNotRight:
                            return GetHintActionHorizontalSpanNotRight(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        default:
                            return false;
                    }
                case eClueType.Vertical:
                    switch (m_VerticalType)
                    {
                        case eVerticalType.Two:
                            return GetHintActionVerticalTwo(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        case eVerticalType.Three:
                            return GetHintActionVerticalThree(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        case eVerticalType.EitherOr:
                            return GetHintActionVerticalEitherOr(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        case eVerticalType.TwoNot:
                            return GetHintActionVerticalTwoNot(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        case eVerticalType.ThreeTopNot:
                            return GetHintActionVerticalThreeTopNot(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        case eVerticalType.ThreeMidNot:
                            return GetHintActionVerticalThreeMidNot(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        case eVerticalType.ThreeBotNot:
                            return GetHintActionVerticalThreeBotNot(P, out bSetFinalIcon, out iRow, out iCol, out iIcon);
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }

        #region Hint_Vertical
        private bool GetHintActionVerticalTwo(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol];
            for (int i = 0; i < P.m_iSize; i++)
            {
                if (!P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                {
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                }
                else if (!P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                {
                    if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                }

                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon != iIcon2)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon != iIcon1)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                }
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }

        private bool GetHintActionVerticalThree(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol];
            int iIcon3 = P.m_Solution[m_iRow3, m_iCol];
            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon != iIcon2)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                    else if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon != iIcon3)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow3;
                        iCol = i;
                        iIcon = iIcon3;
                        return true;
                    }
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    if (P.m_Rows[m_iRow ].m_Cells[i].m_iFinalIcon != iIcon1)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow ;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                    else if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon != iIcon3)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow3;
                        iCol = i;
                        iIcon = iIcon3;
                        return true;
                    }
                }
                else if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon == iIcon3)
                {
                    if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon != iIcon1)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                    else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon != iIcon2)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                }

                if (!P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                {
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                    else if (P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow3;
                        iCol = i;
                        iIcon = iIcon3;
                        return true;
                    }
                }
                else if (!P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                {                    
                    if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                    else if (P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow3;
                        iCol = i;
                        iIcon = iIcon3;
                        return true;
                    }
                }
                else if (!P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                {
                    if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    } 
                    else if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                }
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }

        private bool GetHintActionVerticalEitherOr(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = (m_iRow2 == m_iNotCell) ? m_iHorizontal1 : P.m_Solution[m_iRow2, m_iCol];
            int iIcon3 = (m_iRow3 == m_iNotCell) ? m_iHorizontal1 : P.m_Solution[m_iRow3, m_iCol];

            int iIcon2Col = -1;
            int iIcon3Col = -1;
            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    if (P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow3;
                        iCol = i;
                        iIcon = iIcon3;
                        return true;
                    }
                    iIcon2Col = i;
                }
                if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon == iIcon3)
                {
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                    iIcon3Col = i;
                }

                if (!P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2] &&
                    !P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                {
                    if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                }

                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    for (int j = 0; j < P.m_iSize; j++)
                    {
                        if (j == i)
                            continue;

                        if (P.m_Rows[m_iRow2].m_Cells[j].m_iFinalIcon == iIcon2)
                        {
                            if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iRow = m_iRow3;
                                iCol = i;
                                iIcon = iIcon3;
                                return true;
                            }
                        }
                        else if (P.m_Rows[m_iRow3].m_Cells[j].m_iFinalIcon == iIcon3)
                        {
                            if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon != iIcon2)
                            {
                                bSetFinalIcon = true;
                                iRow = m_iRow2;
                                iCol = i;
                                iIcon = iIcon2;
                                return true;
                            }
                        }
                    }
                }
            }

            if (iIcon2Col >= 0 && iIcon3Col >= 0)
            {
                for (int i = 0; i < P.m_iSize; i++)
                {
                    if (i != iIcon2Col && i != iIcon3Col)
                    {
                        if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                        {
                            bSetFinalIcon = false;
                            iRow = m_iRow;
                            iCol = i;
                            iIcon = iIcon1;
                            return true;
                        }
                    }
                }
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }

        private bool GetHintActionVerticalTwoNot(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = m_iNotCell;

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                }
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }

        private bool GetHintActionVerticalThreeTopNot(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = m_iNotCell;
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol];
            int iIcon3 = P.m_Solution[m_iRow3, m_iCol];

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                    else if (P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow3;
                        iCol = i;
                        iIcon = iIcon3;
                        return true;
                    }
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }

                    if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon != iIcon3)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow3;
                        iCol = i;
                        iIcon = iIcon3;
                        return true;
                    }
                }
                else if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon == iIcon3)
                {
                    if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon != iIcon2)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                }
                else if (!P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                {
                    if (P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow3;
                        iCol = i;
                        iIcon = iIcon3;
                        return true;
                    }
                }
                else if (!P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                {
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                }
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }

        private bool GetHintActionVerticalThreeMidNot(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = m_iNotCell;
            int iIcon3 = P.m_Solution[m_iRow3, m_iCol];

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                    else if (P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow3;
                        iCol = i;
                        iIcon = iIcon3;
                        return true;
                    }
                }
                else if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                    if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon != iIcon3)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow3;
                        iCol = i;
                        iIcon = iIcon3;
                        return true;
                    }
                }
                else if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon == iIcon3)
                {
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                    if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon != iIcon1)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                }
                else if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon >= 0)
                {
                    if (P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow3;
                        iCol = i;
                        iIcon = iIcon3;
                        return true;
                    }
                }
                else if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon >= 0)
                {
                    if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                }
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }

        private bool GetHintActionVerticalThreeBotNot(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol];
            int iIcon3 = m_iNotCell;

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow3].m_Cells[i].m_iFinalIcon == iIcon3)
                {
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                    else if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    if (P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow3;
                        iCol = i;
                        iIcon = iIcon3;
                        return true;
                    }
                    if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon != iIcon1)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                }
                else if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    if (P.m_Rows[m_iRow3].m_Cells[i].m_bValues[iIcon3])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow3;
                        iCol = i;
                        iIcon = iIcon3;
                        return true;
                    }
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon != iIcon2)
                    {
                        bSetFinalIcon = true;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                }
                else if (!P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                {
                    if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                }
                else if (!P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                {
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                }
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }
        #endregion

        #region Hint_Horizontal
        private bool GetHintActionHorizontalNextTo(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol2];

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    if (i == 0)
                    {
                        if (P.m_Rows[m_iRow2].m_Cells[1].m_iFinalIcon != iIcon2)
                        {
                            bSetFinalIcon = true;
                            iRow = m_iRow2;
                            iCol = 1;
                            iIcon = iIcon2;
                            return true;
                        }
                    }
                    else if (i == P.m_iSize - 1)
                    {
                        if (P.m_Rows[m_iRow2].m_Cells[i - 1].m_iFinalIcon != iIcon2)
                        {
                            bSetFinalIcon = true;
                            iRow = m_iRow2;
                            iCol = i - 1;
                            iIcon = iIcon2;
                            return true;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < P.m_iSize; j++)
                        {
                            if (j == (i - 1) || j == (i + 1))
                                continue;
                            if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                            {
                                bSetFinalIcon = false;
                                iRow = m_iRow2;
                                iCol = i;
                                iIcon = iIcon2;
                                return true;
                            }

                        }
                    }
                    break;
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    if (i == 0)
                    {
                        if (P.m_Rows[m_iRow].m_Cells[1].m_iFinalIcon != iIcon1)
                        {
                            bSetFinalIcon = true;
                            iRow = m_iRow;
                            iCol = 1;
                            iIcon = iIcon1;
                            return true;
                        }
                    }
                    else if (i == P.m_iSize - 1)
                    {
                        if (P.m_Rows[m_iRow].m_Cells[i - 1].m_iFinalIcon != iIcon1)
                        {
                            bSetFinalIcon = true;
                            iRow = m_iRow;
                            iCol = i - 1;
                            iIcon = iIcon1;
                            return true;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < P.m_iSize; j++)
                        {
                            if (j == (i - 1) || j == (i + 1))
                                continue;
                            if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                            {
                                bSetFinalIcon = false;
                                iRow = m_iRow;
                                iCol = i;
                                iIcon = iIcon1;
                                return true;
                            }
                        }
                    }
                    break;
                }
                else
                {
                    if (i == 0)
                    {
                        if (!P.m_Rows[m_iRow2].m_Cells[i + 1].m_bValues[iIcon2])
                        {
                            if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                            {
                                bSetFinalIcon = false;
                                iRow = m_iRow;
                                iCol = i;
                                iIcon = iIcon1;
                                return true;
                            }
                        }
                        if (!P.m_Rows[m_iRow].m_Cells[i + 1].m_bValues[iIcon1])
                        {
                            if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                            {
                                bSetFinalIcon = false;
                                iRow = m_iRow2;
                                iCol = i;
                                iIcon = iIcon2;
                                return true;
                            }
                        }
                    }
                    else if (i == P.m_iSize - 1)
                    {
                        if (!P.m_Rows[m_iRow2].m_Cells[i - 1].m_bValues[iIcon2])
                        {
                            if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                            {
                                bSetFinalIcon = false;
                                iRow = m_iRow;
                                iCol = i;
                                iIcon = iIcon1;
                                return true;
                            }
                        }
                        if (!P.m_Rows[m_iRow].m_Cells[i - 1].m_bValues[iIcon1])
                        {
                            if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                            {
                                bSetFinalIcon = false;
                                iRow = m_iRow2;
                                iCol = i;
                                iIcon = iIcon2;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (!P.m_Rows[m_iRow2].m_Cells[i + 1].m_bValues[iIcon2] &&
                            !P.m_Rows[m_iRow2].m_Cells[i - 1].m_bValues[iIcon2])
                        {
                            if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                            {
                                bSetFinalIcon = false;
                                iRow = m_iRow;
                                iCol = i;
                                iIcon = iIcon1;
                                return true;
                            }
                        }
                        if (!P.m_Rows[m_iRow].m_Cells[i + 1].m_bValues[iIcon1] &&
                            !P.m_Rows[m_iRow].m_Cells[i - 1].m_bValues[iIcon1])
                        {
                            if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                            {
                                bSetFinalIcon = false;
                                iRow = m_iRow2;
                                iCol = i;
                                iIcon = iIcon2;
                                return true;
                            }
                        }
                    }
                }
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }

        private bool GetHintActionHorizontalNotNextTo(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = m_iHorizontal1;

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    if (i == 0)
                    {
                        if (P.m_Rows[m_iRow2].m_Cells[i + 1].m_bValues[iIcon2])
                        {
                            bSetFinalIcon = false;
                            iRow = m_iRow2;
                            iCol = i + 1;
                            iIcon = iIcon2;
                            return true;
                        }
                    }
                    else if (i == P.m_iSize - 1)
                    {
                        if (P.m_Rows[m_iRow2].m_Cells[i - 1].m_bValues[iIcon2])
                        {
                            bSetFinalIcon = false;
                            iRow = m_iRow2;
                            iCol = i - 1;
                            iIcon = iIcon2;
                            return true;
                        }
                    }
                    else
                    {
                        if (P.m_Rows[m_iRow2].m_Cells[i - 1].m_bValues[iIcon2])
                        {
                            bSetFinalIcon = false;
                            iRow = m_iRow2;
                            iCol = i - 1;
                            iIcon = iIcon2;
                            return true;
                        }
                        else if (P.m_Rows[m_iRow2].m_Cells[i + 1].m_bValues[iIcon2])
                        {
                            bSetFinalIcon = false;
                            iRow = m_iRow2;
                            iCol = i + 1;
                            iIcon = iIcon2;
                            return true;
                        }
                    }
                    break;
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    if (i == 0)
                    {
                        if (P.m_Rows[m_iRow].m_Cells[i + 1].m_bValues[iIcon1])
                        {
                            bSetFinalIcon = false;
                            iRow = m_iRow;
                            iCol = i + 1;
                            iIcon = iIcon1;
                            return true;
                        }
                    }
                    else if (i == P.m_iSize - 1)
                    {
                        if (P.m_Rows[m_iRow].m_Cells[i - 1].m_bValues[iIcon1])
                        {
                            bSetFinalIcon = false;
                            iRow = m_iRow;
                            iCol = i - 1;
                            iIcon = iIcon1;
                            return true;
                        }
                    }
                    else
                    {
                        if (P.m_Rows[m_iRow].m_Cells[i - 1].m_bValues[iIcon1])
                        {
                            bSetFinalIcon = false;
                            iRow = m_iRow;
                            iCol = i - 1;
                            iIcon = iIcon1;
                            return true;
                        }
                        else if (P.m_Rows[m_iRow].m_Cells[i + 1].m_bValues[iIcon1])
                        {
                            bSetFinalIcon = false;
                            iRow = m_iRow;
                            iCol = i + 1;
                            iIcon = iIcon1;
                            return true;
                        }
                    }
                    break;
                }
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }

        private bool GetHintActionHorizontalLeftOf(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol2];

            int iFirstPossibleLeft = 0;
            for (iFirstPossibleLeft = 0; iFirstPossibleLeft < P.m_iSize; iFirstPossibleLeft++)
            {
                if (P.m_Rows[m_iRow].m_Cells[iFirstPossibleLeft].m_bValues[iIcon1])
                    break;
            }

            int iFirstPossibleRight = P.m_iSize - 1;
            for (iFirstPossibleRight = P.m_iSize - 1; iFirstPossibleRight >= 0; iFirstPossibleRight--)
            {
                if (P.m_Rows[m_iRow2].m_Cells[iFirstPossibleRight].m_bValues[iIcon2])
                    break;
            }

            if (iFirstPossibleLeft + 1 == iFirstPossibleRight)
            {
                // we have a solution for this clue
                if (P.m_Rows[m_iRow].m_Cells[iFirstPossibleLeft].m_iFinalIcon != iIcon1)
                {
                    bSetFinalIcon = true;
                    iRow = m_iRow;
                    iCol = iFirstPossibleLeft;
                    iIcon = iIcon1;
                    return true;
                }
                else if (P.m_Rows[m_iRow2].m_Cells[iFirstPossibleRight].m_iFinalIcon != iIcon2)
                {
                    bSetFinalIcon = true;
                    iRow = m_iRow2;
                    iCol = iFirstPossibleRight;
                    iIcon = iIcon2;
                    return true;
                }
            }
            else
            {
                // Remove all icon2's from the left side of the first possible left
                for (int i = 0; i <= iFirstPossibleLeft; i++)
                {
                    if (P.m_Rows[m_iRow2].m_Cells[i].m_bValues[iIcon2])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow2;
                        iCol = i;
                        iIcon = iIcon2;
                        return true;
                    }
                }

                // Remove all the icon1's from the right side of the first possible right
                for (int i = iFirstPossibleRight; i < P.m_iSize; i++)
                {
                    if (P.m_Rows[m_iRow].m_Cells[i].m_bValues[iIcon1])
                    {
                        bSetFinalIcon = false;
                        iRow = m_iRow;
                        iCol = i;
                        iIcon = iIcon1;
                        return true;
                    }
                }
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }

        private bool GetHintActionHorizontalNotLeftOf(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol2];

            // If both icons are on the same row
            if (m_iRow == m_iRow2)
            {
                if (P.m_Rows[m_iRow].m_Cells[0].m_bValues[iIcon1])
                {
                    bSetFinalIcon = false;
                    iRow = m_iRow;
                    iCol = 0;
                    iIcon = iIcon1;
                    return true;
                }

                if (P.m_Rows[m_iRow2].m_Cells[P.m_iSize - 1].m_bValues[iIcon2])
                {
                    bSetFinalIcon = false;
                    iRow = m_iRow2;
                    iCol = P.m_iSize - 1;
                    iIcon = iIcon2;
                    return true;
                }
            }

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (P.m_Rows[m_iRow].m_Cells[i].m_iFinalIcon == iIcon1)
                {
                    // Icon1 is known, remove all instances of icon2 to the right
                    for (int j = i + 1; j < P.m_iSize; j++)
                    {
                        if (P.m_Rows[m_iRow2].m_Cells[j].m_bValues[iIcon2])
                        {
                            bSetFinalIcon = false;
                            iRow = m_iRow2;
                            iCol = j;
                            iIcon = iIcon2;
                            return true;
                        }
                    }
                    break;
                }
                else if (P.m_Rows[m_iRow2].m_Cells[i].m_iFinalIcon == iIcon2)
                {
                    // Icon2 is known, remove all instances of icon1 to the left
                    for (int j = 0; j < i; j++)
                    {
                        if (P.m_Rows[m_iRow].m_Cells[j].m_bValues[iIcon1])
                        {
                            bSetFinalIcon = false;
                            iRow = m_iRow;
                            iCol = j;
                            iIcon = iIcon1;
                            return true;
                        }
                    }
                    break;
                }
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }

        private bool GetHintActionHorizontalSpan(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol2];
            int iIcon3 = P.m_Solution[m_iRow3, m_iCol3];

            // Icon2 cant be on either end
            if (P.m_Rows[m_iRow2].m_Cells[0].m_bValues[iIcon2])
            {
                bSetFinalIcon = false;
                iRow = m_iRow2;
                iCol = 0;
                iIcon = iIcon2;
                return true;
            }
            else if (P.m_Rows[m_iRow2].m_Cells[P.m_iSize - 1].m_bValues[iIcon2])
            {
                bSetFinalIcon = false;
                iRow = m_iRow2;
                iCol = P.m_iSize - 1;
                iIcon = iIcon2;
                return true;
            }


            for (int i = 0; i < P.m_iSize; i++)
            {
                if (GetHintActionSpan(i, m_iRow, iIcon1, false, m_iRow2, iIcon2, false, m_iRow3, iIcon3, false, P, out bSetFinalIcon, out iRow, out iCol, out iIcon))
                    return true;
                if (GetHintActionSpan(i, m_iRow3, iIcon3, false, m_iRow2, iIcon2, false, m_iRow, iIcon1, false, P, out bSetFinalIcon, out iRow, out iCol, out iIcon))
                    return true;
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }

        private bool GetHintActionSpan(int iCol, int iRow1, int iIcon1, bool bNot1, int iRow2, int iIcon2, bool bNot2, int iRow3, int iIcon3, bool bNot3, Puzzle P, out bool bSetFinalIcon, out int iOutRow, out int iOutCol, out int iOutIcon)
        {
            int iFinal1 = P.m_Rows[iRow1].m_Cells[iCol].m_iFinalIcon;
            if (iFinal1 == iIcon1)
            {
                if (iCol < 2)
                {
                    if (bNot1)
                    {
                        int iFinal2Right = P.m_Rows[iRow2].m_Cells[iCol + 1].m_iFinalIcon;
                        if (iFinal2Right == iIcon2)
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (bNot2)
                        {
                            if (P.m_Rows[iRow2].m_Cells[iCol + 1].m_bValues[iIcon2])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow2;
                                iOutCol = iCol + 1;
                                iOutIcon = iIcon2;
                                return true;
                            }
                        }
                        else
                        {
                            if (P.m_Rows[iRow2].m_Cells[iCol + 1].m_iFinalIcon != iIcon2)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow2;
                                iOutCol = iCol + 1;
                                iOutIcon = iIcon2;
                                return true;
                            }
                        }

                        if (bNot3)
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol + 2].m_bValues[iIcon3])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow3;
                                iOutCol = iCol + 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                        else
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol + 2].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol + 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                    }
                }
                else if (iCol > P.m_iSize - 3)
                {
                    if (bNot1)
                    {
                        int iFinal2Left = P.m_Rows[iRow2].m_Cells[iCol - 1].m_iFinalIcon;
                        if (iFinal2Left == iIcon2)
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (bNot2)
                        {
                            if (P.m_Rows[iRow2].m_Cells[iCol - 1].m_bValues[iIcon2])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow2;
                                iOutCol = iCol - 1;
                                iOutIcon = iIcon2;
                                return true;
                            }
                        }
                        else
                        {
                            if (P.m_Rows[iRow2].m_Cells[iCol - 1].m_iFinalIcon != iIcon2)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow2;
                                iOutCol = iCol - 1;
                                iOutIcon = iIcon2;
                                return true;
                            }
                        }

                        if (bNot3)
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol - 2].m_bValues[iIcon3])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow3;
                                iOutCol = iCol - 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                        else
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol - 2].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol - 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    int iFinal2Left = P.m_Rows[iRow2].m_Cells[iCol - 1].m_iFinalIcon;
                    int iFinal2Right = P.m_Rows[iRow2].m_Cells[iCol + 1].m_iFinalIcon;
                    if (iFinal2Left == iIcon2)
                    {
                        if (bNot1)
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                        else if (bNot2)
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol + 2].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol + 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                        else if (bNot3)
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol - 2].m_bValues[iIcon3])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow3;
                                iOutCol = iCol - 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                        else
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol - 2].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol - 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                    }
                    else if (iFinal2Right == iIcon2)
                    {
                        if (bNot1)
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                        else if (bNot2)
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol - 2].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol - 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                        else if (bNot3)
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol + 2].m_bValues[iIcon3])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow3;
                                iOutCol = iCol + 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                        else
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol + 2].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol + 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                    }
                    else if (!P.m_Rows[iRow2].m_Cells[iCol - 1].m_bValues[iIcon2] && !bNot1 && !bNot2)
                    {
                        if (P.m_Rows[iRow2].m_Cells[iCol + 1].m_iFinalIcon != iIcon2)
                        {
                            bSetFinalIcon = true;
                            iOutRow = iRow2;
                            iOutCol = iCol + 1;
                            iOutIcon = iIcon2;
                            return true;
                        }

                        if (bNot3)
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol + 2].m_bValues[iIcon3])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow3;
                                iOutCol = iCol + 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                        else
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol + 2].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol + 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                    }
                    else if (!P.m_Rows[iRow2].m_Cells[iCol + 1].m_bValues[iIcon2] && !bNot1 && !bNot2)
                    {
                        if (P.m_Rows[iRow2].m_Cells[iCol - 1].m_iFinalIcon != iIcon2)
                        {
                            bSetFinalIcon = true;
                            iOutRow = iRow2;
                            iOutCol = iCol - 1;
                            iOutIcon = iIcon2;
                            return true;
                        }

                        if (bNot3)
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol - 2].m_bValues[iIcon3])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow3;
                                iOutCol = iCol - 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                        else
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol - 2].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol - 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                    }
                    else if (!bNot3)
                    {
                        int iFinal3Left = P.m_Rows[iRow3].m_Cells[iCol - 2].m_iFinalIcon;
                        int iFinal3Right = P.m_Rows[iRow3].m_Cells[iCol + 2].m_iFinalIcon;
                        if (iFinal3Left == iIcon3)
                        {
                            if (bNot1)
                            {
                                if (P.m_Rows[iRow2].m_Cells[iCol - 3].m_iFinalIcon != iIcon2)
                                {
                                    bSetFinalIcon = true;
                                    iOutRow = iRow2;
                                    iOutCol = iCol - 3;
                                    iOutIcon = iIcon2;
                                    return true;
                                }
                            }
                            else if (bNot2)
                            {
                                if (P.m_Rows[iRow2].m_Cells[iCol - 1].m_bValues[iIcon2])
                                {
                                    bSetFinalIcon = false;
                                    iOutRow = iRow2;
                                    iOutCol = iCol - 1;
                                    iOutIcon = iIcon2;
                                    return true;
                                }
                            }
                            else
                            {
                                if (P.m_Rows[iRow2].m_Cells[iCol - 1].m_iFinalIcon != iIcon2)
                                {
                                    bSetFinalIcon = true;
                                    iOutRow = iRow2;
                                    iOutCol = iCol - 1;
                                    iOutIcon = iIcon2;
                                    return true;
                                }
                            }
                        }
                        else if (iFinal3Right == iIcon3)
                        {
                            if (bNot1)
                            {
                                if (P.m_Rows[iRow2].m_Cells[iCol + 3].m_iFinalIcon != iIcon2)
                                {
                                    bSetFinalIcon = true;
                                    iOutRow = iRow2;
                                    iOutCol = iCol + 3;
                                    iOutIcon = iIcon2;
                                    return true;
                                }
                            }
                            else if (bNot2)
                            {
                                if (P.m_Rows[iRow2].m_Cells[iCol + 1].m_bValues[iIcon2])
                                {
                                    bSetFinalIcon = false;
                                    iOutRow = iRow2;
                                    iOutCol = iCol + 1;
                                    iOutIcon = iIcon2;
                                    return true;
                                }
                            }
                            else
                            {
                                if (P.m_Rows[iRow2].m_Cells[iCol + 1].m_iFinalIcon != iIcon2)
                                {
                                    bSetFinalIcon = true;
                                    iOutRow = iRow2;
                                    iOutCol = iCol + 1;
                                    iOutIcon = iIcon2;
                                    return true;
                                }
                            }
                        }
                        else if (!P.m_Rows[iRow3].m_Cells[iCol - 2].m_bValues[iIcon3] && !bNot1 && !bNot2)
                        {
                            if (P.m_Rows[iRow2].m_Cells[iCol + 1].m_iFinalIcon != iIcon2)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow2;
                                iOutCol = iCol + 1;
                                iOutIcon = iIcon2;
                                return true;
                            }
                            else if (P.m_Rows[iRow3].m_Cells[iCol + 2].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol + 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                        else if (!P.m_Rows[iRow3].m_Cells[iCol + 2].m_bValues[iIcon3] && !bNot1 && !bNot2)
                        {
                            if (P.m_Rows[iRow2].m_Cells[iCol - 1].m_iFinalIcon != iIcon2)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow2;
                                iOutCol = iCol - 1;
                                iOutIcon = iIcon2;
                                return true;
                            }
                            else if (P.m_Rows[iRow3].m_Cells[iCol - 2].m_iFinalIcon != iIcon3)
                            {
                                bSetFinalIcon = true;
                                iOutRow = iRow3;
                                iOutCol = iCol - 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                        else if (!bNot1)
                        {
                            for (int j = 0; j < P.m_iSize; j++)
                            {
                                if (!bNot2 && j != (iCol - 1) && j != (iCol + 1))
                                {
                                    if (P.m_Rows[iRow2].m_Cells[j].m_bValues[iIcon2])
                                    {
                                        bSetFinalIcon = false;
                                        iOutRow = iRow2;
                                        iOutCol = j;
                                        iOutIcon = iIcon2;
                                        return true;
                                    }
                                }
                                if (j != (iCol - 2) && j != (iCol + 2))
                                {
                                    if (P.m_Rows[iRow3].m_Cells[j].m_bValues[iIcon3])
                                    {
                                        bSetFinalIcon = false;
                                        iOutRow = iRow3;
                                        iOutCol = j;
                                        iOutIcon = iIcon3;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (iFinal1 >= 0 && !bNot1 && !bNot2 && !bNot3)
            {
                int iFinal3 = P.m_Rows[iRow3].m_Cells[iCol].m_iFinalIcon;
                if (iFinal3 != iIcon3 && iFinal3 >= 0)
                {
                    if (iCol == 0)
                    {
                        if (P.m_Rows[iRow2].m_Cells[iCol + 1].m_bValues[iIcon2])
                        {
                            bSetFinalIcon = false;
                            iOutRow = iRow2;
                            iOutCol = iCol + 1;
                            iOutIcon = iIcon2;
                            return true;
                        }
                    }
                    else if (iCol == P.m_iSize - 1)
                    {
                        if (P.m_Rows[iRow2].m_Cells[iCol - 1].m_bValues[iIcon2])
                        {
                            bSetFinalIcon = false;
                            iOutRow = iRow2;
                            iOutCol = iCol - 1;
                            iOutIcon = iIcon2;
                            return true;
                        }
                    }
                    else if (iCol == P.m_iSize - 3)
                    {
                        if (P.m_Rows[iRow1].m_Cells[iCol + 2].m_bValues[iIcon1])
                        {
                            bSetFinalIcon = false;
                            iOutRow = iRow1;
                            iOutCol = iCol + 2;
                            iOutIcon = iIcon1;
                            return true;
                        }
                        if (P.m_Rows[iRow2].m_Cells[iCol + 1].m_bValues[iIcon2])
                        {
                            bSetFinalIcon = false;
                            iOutRow = iRow2;
                            iOutCol = iCol + 1;
                            iOutIcon = iIcon2;
                            return true;
                        }
                        if (P.m_Rows[iRow3].m_Cells[iCol + 2].m_bValues[iIcon3])
                        {
                            bSetFinalIcon = false;
                            iOutRow = iRow3;
                            iOutCol = iCol + 2;
                            iOutIcon = iIcon3;
                            return true;
                        }
                    }
                    else if (iCol == 2)
                    {
                        if (P.m_Rows[iRow1].m_Cells[iCol - 2].m_bValues[iIcon1])
                        {
                            bSetFinalIcon = false;
                            iOutRow = iRow1;
                            iOutCol = iCol - 2;
                            iOutIcon = iIcon1;
                            return true;
                        }
                        if (P.m_Rows[iRow2].m_Cells[iCol - 1].m_bValues[iIcon2])
                        {
                            bSetFinalIcon = false;
                            iOutRow = iRow2;
                            iOutCol = iCol - 1;
                            iOutIcon = iIcon2;
                            return true;
                        }
                        if (P.m_Rows[iRow3].m_Cells[iCol - 2].m_bValues[iIcon3])
                        {
                            bSetFinalIcon = false;
                            iOutRow = iRow3;
                            iOutCol = iCol - 2;
                            iOutIcon = iIcon3;
                            return true;
                        }
                    }
                }
            }
            if (!P.m_Rows[iRow1].m_Cells[iCol].m_bValues[iIcon1] && !bNot1)
            {
                if (!bNot3)
                {
                    if (iCol + 4 < P.m_iSize)
                    {
                        if (!P.m_Rows[iRow1].m_Cells[iCol + 4].m_bValues[iIcon1])
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol + 2].m_bValues[iIcon3])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow3;
                                iOutCol = iCol + 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                    }
                    else if (iCol + 2 < P.m_iSize)
                    {
                        if (P.m_Rows[iRow3].m_Cells[iCol + 2].m_bValues[iIcon3])
                        {
                            bSetFinalIcon = false;
                            iOutRow = iRow3;
                            iOutCol = iCol + 2;
                            iOutIcon = iIcon3;
                            return true;
                        }
                    }
                    if (iCol - 4 >= 0)
                    {
                        if (!P.m_Rows[iRow1].m_Cells[iCol - 4].m_bValues[iIcon1])
                        {
                            if (P.m_Rows[iRow3].m_Cells[iCol - 2].m_bValues[iIcon3])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow3;
                                iOutCol = iCol - 2;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                    }
                    else if (iCol - 2 >= 0)
                    {
                        if (P.m_Rows[iRow3].m_Cells[iCol - 2].m_bValues[iIcon3])
                        {
                            bSetFinalIcon = false;
                            iOutRow = iRow3;
                            iOutCol = iCol - 2;
                            iOutIcon = iIcon3;
                            return true;
                        }
                    }
                }
                if (!bNot2)
                {
                    if (iCol + 2 < P.m_iSize)
                    {
                        if (!P.m_Rows[iRow1].m_Cells[iCol + 2].m_bValues[iIcon1])
                        {
                            if (P.m_Rows[iRow2].m_Cells[iCol + 1].m_bValues[iIcon2])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow2;
                                iOutCol = iCol + 1;
                                iOutIcon = iIcon2;
                                return true;
                            }
                        }
                    }
                    if (iCol - 2 >= 0)
                    {
                        if (!P.m_Rows[iRow1].m_Cells[iCol - 2].m_bValues[iIcon1])
                        {
                            if (P.m_Rows[iRow2].m_Cells[iCol - 1].m_bValues[iIcon2])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow2;
                                iOutCol = iCol - 1;
                                iOutIcon = iIcon2;
                                return true;
                            }
                        }
                    }
                }
            }
            if (P.m_Rows[iRow2].m_Cells[iCol].m_iFinalIcon == iIcon2 && !bNot2)
            {
                // Middle icon is known, eliminate impossible end icons
                for (int i = 0; i < P.m_iSize; i++)
                {
                    if (i != iCol - 1 && i != iCol + 1)
                    {
                        if (!bNot1)
                        {
                            if (P.m_Rows[iRow1].m_Cells[i].m_bValues[iIcon1])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow1;
                                iOutCol = i;
                                iOutIcon = iIcon1;
                                return true;
                            }
                        }
                        if (!bNot3)
                        {
                            if (P.m_Rows[iRow3].m_Cells[i].m_bValues[iIcon3])
                            {
                                bSetFinalIcon = false;
                                iOutRow = iRow3;
                                iOutCol = i;
                                iOutIcon = iIcon3;
                                return true;
                            }
                        }
                    }
                }
            }

            bSetFinalIcon = false;
            iOutRow = -1;
            iOutCol = -1;
            iOutIcon = -1;
            return false;
        }

        private bool GetHintActionHorizontalSpanNotLeft(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = m_iHorizontal1;
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol2];
            int iIcon3 = P.m_Solution[m_iRow3, m_iCol3];

            // Icon2 cant be on either end
            if (P.m_Rows[m_iRow2].m_Cells[0].m_bValues[iIcon2])
            {
                bSetFinalIcon = false;
                iRow = m_iRow2;
                iCol = 0;
                iIcon = iIcon2;
                return true;
            }
            else if (P.m_Rows[m_iRow2].m_Cells[P.m_iSize - 1].m_bValues[iIcon2])
            {
                bSetFinalIcon = false;
                iRow = m_iRow2;
                iCol = P.m_iSize - 1;
                iIcon = iIcon2;
                return true;
            }


            for (int i = 0; i < P.m_iSize; i++)
            {
                if (GetHintActionSpan(i, m_iRow, iIcon1, true, m_iRow2, iIcon2, false, m_iRow3, iIcon3, false, P, out bSetFinalIcon, out iRow, out iCol, out iIcon))
                    return true;
                if (GetHintActionSpan(i, m_iRow3, iIcon3, false, m_iRow2, iIcon2, false, m_iRow, iIcon1, true, P, out bSetFinalIcon, out iRow, out iCol, out iIcon))
                    return true;
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }

        private bool GetHintActionHorizontalSpanNotMid(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = m_iHorizontal1;
            int iIcon3 = P.m_Solution[m_iRow3, m_iCol3];

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (GetHintActionSpan(i, m_iRow, iIcon1, false, m_iRow2, iIcon2, true, m_iRow3, iIcon3, false, P, out bSetFinalIcon, out iRow, out iCol, out iIcon))
                    return true;
                if (GetHintActionSpan(i, m_iRow3, iIcon3, false, m_iRow2, iIcon2, true, m_iRow, iIcon1, false, P, out bSetFinalIcon, out iRow, out iCol, out iIcon))
                    return true;
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }

        private bool GetHintActionHorizontalSpanNotRight(Puzzle P, out bool bSetFinalIcon, out int iRow, out int iCol, out int iIcon)
        {
            int iIcon1 = P.m_Solution[m_iRow, m_iCol];
            int iIcon2 = P.m_Solution[m_iRow2, m_iCol2];
            int iIcon3 = m_iHorizontal1;

            // Icon2 cant be on either end
            if (P.m_Rows[m_iRow2].m_Cells[0].m_bValues[iIcon2])
            {
                bSetFinalIcon = false;
                iRow = m_iRow2;
                iCol = 0;
                iIcon = iIcon2;
                return true;
            }
            else if (P.m_Rows[m_iRow2].m_Cells[P.m_iSize - 1].m_bValues[iIcon2])
            {
                bSetFinalIcon = false;
                iRow = m_iRow2;
                iCol = P.m_iSize - 1;
                iIcon = iIcon2;
                return true;
            }

            for (int i = 0; i < P.m_iSize; i++)
            {
                if (GetHintActionSpan(i, m_iRow, iIcon1, false, m_iRow2, iIcon2, false, m_iRow3, iIcon3, true, P, out bSetFinalIcon, out iRow, out iCol, out iIcon))
                    return true;
                if (GetHintActionSpan(i, m_iRow3, iIcon3, true, m_iRow2, iIcon2, false, m_iRow, iIcon1, false, P, out bSetFinalIcon, out iRow, out iCol, out iIcon))
                    return true;
            }

            bSetFinalIcon = false;
            iRow = -1;
            iCol = -1;
            iIcon = -1;
            return false;
        }
        #endregion
        #endregion
    }
}
