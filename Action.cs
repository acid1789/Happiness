using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicMatrix; 
using System.IO;

namespace Happiness
{
    public enum eActionType
    {
        eAT_SetFinalIcon,
        eAT_EliminateIcon,
        eAT_RestoreIcon,
    };

    public class Action
    {
        eActionType m_Type;
        int m_iRow;
        int m_iCol;
        int m_iIcon;

        bool[,,] m_SavedPuzzle;

        private Action()
        {
        }

        public Action(eActionType type, int iRow, int iCol, int iIcon, Puzzle P)
        {
            m_Type = type;
            m_iRow = iRow;
            m_iCol = iCol;
            m_iIcon = iIcon;

            SavePuzzle(P);
        }

        private void SavePuzzle(Puzzle P)
        {
            m_SavedPuzzle = new bool[P.m_iSize, P.m_iSize, P.m_iSize];
            for (int iRow = 0; iRow < P.m_iSize; iRow++)
            {
                for (int iCol = 0; iCol < P.m_iSize; iCol++)
                {
                    for (int i = 0; i < P.m_iSize; i++)
                    {
                        m_SavedPuzzle[iRow, iCol, i] = P.m_Rows[iRow].m_Cells[iCol].m_bValues[i];
                    }
                }
            }
        }

        public void Perform(Puzzle P)
        {
            switch (m_Type)
            {
                case eActionType.eAT_EliminateIcon:
                    P.EliminateIcon(m_iRow, m_iCol, m_iIcon);
                    break;
                case eActionType.eAT_SetFinalIcon:
                    P.SetFinalIcon(m_iRow, m_iCol, m_iIcon);
                    break;
                case eActionType.eAT_RestoreIcon:
                    P.m_Rows[m_iRow].m_Cells[m_iCol].m_bValues[m_iIcon] = true;
                    P.m_Rows[m_iRow].m_Cells[m_iCol].m_iFinalIcon = P.m_Rows[m_iRow].m_Cells[m_iCol].GetRemainingIcon();
                    break;
            }
        }

        public void Revert(Puzzle P)
        {
            for (int iRow = 0; iRow < P.m_iSize; iRow++)
            {
                for (int iCol = 0; iCol < P.m_iSize; iCol++)
                {
                    for (int iIcon = 0; iIcon < P.m_iSize; iIcon++)
                    {
                        P.m_Rows[iRow].m_Cells[iCol].m_bValues[iIcon] = m_SavedPuzzle[iRow, iCol, iIcon];
                    }
                    P.m_Rows[iRow].m_Cells[iCol].m_iFinalIcon = P.m_Rows[iRow].m_Cells[iCol].GetRemainingIcon();
                }
            }
        }

        public void Save(BinaryWriter bw, int puzzleSize)
        {
            bw.Write((int)m_Type);
            bw.Write(m_iRow);
            bw.Write(m_iCol);
            bw.Write(m_iIcon);

            for (int iRow = 0; iRow < puzzleSize; iRow++)
            {
                for (int iCol = 0; iCol < puzzleSize; iCol++)
                {
                    for (int iIcon = 0; iIcon < puzzleSize; iIcon++)
                    {
                        bw.Write(m_SavedPuzzle[iRow, iCol, iIcon]);
                    }
                }
            }
        }

        public static Action Load(BinaryReader br, int puzzleSize)
        {
            Action a = new Action();
            a.m_Type = (eActionType)br.ReadInt32();
            a.m_iRow = br.ReadInt32();
            a.m_iCol = br.ReadInt32();
            a.m_iIcon = br.ReadInt32();
            a.m_SavedPuzzle = new bool[puzzleSize, puzzleSize, puzzleSize];

            for (int iRow = 0; iRow < puzzleSize; iRow++)
            {
                for (int iCol = 0; iCol < puzzleSize; iCol++)
                {
                    for (int iIcon = 0; iIcon < puzzleSize; iIcon++)
                    {
                        a.m_SavedPuzzle[iRow, iCol, iIcon] = br.ReadBoolean();
                    }
                }
            }

            return a;
        }
    }
}
