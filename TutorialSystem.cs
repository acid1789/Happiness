using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class TutorialSystem
    {
        public enum TutorialPiece
        {
            ClickTower,
            FloorSelect,
            FloorPlay,
            GameStart,
            HorizontalClueArea,
            SpanExplanation,
            ClueHelp,
            SpanHelp1,
            EliminateRedNebula,
            EliminateRedNebula2,
            EliminateRedNebula3,
            BartMan1,
            BartMan2,
            BartMan3,
            BartMan4,
            Hulk1,
            Hulk2,
            Hulk3,
            Hulk4,
            HorizontalClue2a,
            HorizontalClue2b,
            CrabNebula1,
            CrabNebula2,
            CrabNebula3,
            HorizontalClue2c,
            GreenLantern1,
            GreenLantern2,
            GreenLantern3,
            HorizontalClue2d,
            HorizontalClue3a,
            HorizontalClue3b,
            Homer1,
            Homer2,
            Homer3,
            HorizontalClue4,
            HorizontalClue5a,
            HorizontalClue5b,
            GreenLantern4,
            GreenLantern5,
            GreenLantern6,
            HideClue1,
            Hint1,
            Hint2,
            Hint3,
            Bartman5,
            Bartman6,
            Bartman7,
            Undo,
            HorizontalClue4b,
            HorizontalClue4c,
            RedNebula4,
            RedNebula5,
            RedNebula6,
            EndScreen1,
            EndScreen2,
            EndScreen3,
            Puzzle2,
            EndScreen4,

            Horizontal_NextTo,
            Vertical_Two,
            Vertical_Three,
            Vertical_EitherOr,
            Vertical_Not,

            None
        }

        class PieceData
        {
            public Vector2 ArrowTarget;
            public float ArrowRotation;
            public Rectangle InstructionRect;
            public string Instructions;
            public bool OkButton;
            public TutorialPiece NextPiece;

            public bool Triggered;
            public bool Finished;
        }
        
        Dictionary<TutorialPiece, PieceData> m_Pieces;
        TutorialPiece m_CurrentPiece;

        #region Arrow Variables
        Vector2 m_vArrowTarget;
        Vector2 m_vArrowTravelEnd;
        Vector2 m_vArrowPosition;        
        float m_fArrowInterp;
        float m_fArrowSpeed;
        float m_fArrowTravelDistance;

        float m_fArrowWidth;
        float m_fArrowHeight;
        Vector2 m_vArrowScale;
        Vector2 m_vArrowOrigin;
        float m_fArrowRotation;
        #endregion

        #region Instructions Variables
        Rectangle m_InstructionRect;
        UIFrame m_InstructionFrame;

        FormattedLine[] m_Lines;
        Color m_TextColor;
        int m_iTextLineHeight;
        int m_iTextMarginTopBottom;
        int m_iTextMarginLeftRight;
        int m_iButtonWidth;
        int m_iButtonHeight;
        UIButton m_OKButton;
        #endregion

        public TutorialSystem(int screenWidth, int screenHeight)
        {
            m_Pieces = new Dictionary<TutorialPiece, PieceData>();
            m_CurrentPiece = TutorialPiece.None;

            m_fArrowTravelDistance = Constants.TutorialSystem_ArrowTravelDistance * screenWidth;
            m_fArrowSpeed = Constants.TutorialSystem_ArrowSpeed;
            m_fArrowWidth = Constants.TutorialSystem_ArrowWidth * screenWidth;
            m_fArrowHeight = Constants.TutorialSystem_ArrowHeight * screenHeight;
            m_vArrowScale = new Vector2(m_fArrowWidth / Assets.Arrow.Width, m_fArrowHeight / Assets.Arrow.Height);
            m_vArrowOrigin = new Vector2(Assets.Arrow.Width, Assets.Arrow.Height >> 1);

            m_TextColor = Color.CornflowerBlue;
            m_iTextLineHeight = (int)Assets.HelpFont.MeasureString("qQ").Y + 1;
            m_iTextMarginTopBottom = (int)(Constants.TutorialSystem_TextMarginTopBottom * screenHeight);
            m_iTextMarginLeftRight = (int)(Constants.TutorialSystem_TextMarginLeftRight * screenWidth);

            m_iButtonWidth = (int)(Constants.TutorialSystem_ButtonWidth * screenWidth);
            m_iButtonHeight = (int)(Constants.TutorialSystem_ButtonHeight * screenHeight);

            Load();
        }

        #region Persistance
        void Load()
        {
            int count = (int)TutorialPiece.None;
            for (int i = 0; i < count; i++)
            {
                m_Pieces[(TutorialPiece)i] = new PieceData();
            }
        }

        public void Load(ulong data)
        {
            for (int i = 0; i < (int)TutorialPiece.None; i++)
            {
                m_Pieces[(TutorialPiece)i].Finished = ((data & (1UL << i)) != 0);
            }
        }

        void Save()
        {
            ulong bitfield = 0;
            for (int i = 0; i < (int)TutorialPiece.None; i++)
            {
                if (m_Pieces[(TutorialPiece)i].Finished)
                {
                    ulong bit = (ulong)1 << i;
                    bitfield |= bit;
                }
            }

            NetworkManager.Net.SaveTutorialData(bitfield);
        }
        #endregion

        #region Input
        public bool HandleClick(int x, int y)
        {
            if (m_OKButton != null)
            {
                if (m_OKButton.Click(x, y))
                {
                    FinishPiece(m_CurrentPiece);
                }
            }

            // Return true if we are to eat all input
            return ( m_OKButton != null );
        }
        #endregion

        #region Updating
        public void Update(GameTime gt)
        {
            UpdateArrow(gt);
        }

        public void UpdateArrow(GameTime gt)
        {
            //m_fArrowRotation += 0.01f;

            m_fArrowInterp += m_fArrowSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
            if (m_fArrowInterp > 1)
            {
                m_fArrowInterp = 1;
                m_fArrowSpeed = -m_fArrowSpeed;
            }
            else if (m_fArrowInterp < 0)
            {
                m_fArrowInterp = 0;
                m_fArrowSpeed = -m_fArrowSpeed;
            }
            m_vArrowPosition = Vector2.Lerp(m_vArrowTarget, m_vArrowTravelEnd, m_fArrowInterp);
        }
        #endregion

        public bool IsPieceSetup(TutorialPiece piece)
        {
            if (m_Pieces.ContainsKey(piece))
            {
                return m_Pieces[piece].Instructions != null;
            }
            return false;
        }

        public void SetPieceData(TutorialPiece piece, Vector2 arrowTarget, float arrowRotation, Rectangle instructionRect, string instructions, TutorialPiece nextPiece, bool okButton = false)
        {
            m_Pieces[piece].ArrowTarget = arrowTarget;
            m_Pieces[piece].ArrowRotation = arrowRotation;
            m_Pieces[piece].InstructionRect = instructionRect;
            m_Pieces[piece].Instructions = instructions;
            m_Pieces[piece].OkButton = okButton;
            m_Pieces[piece].NextPiece = nextPiece;
        }

        public void TriggerPiece(TutorialPiece piece)
        {
            PieceData pd = m_Pieces[piece];
            if (!pd.Finished && !pd.Triggered)
            {
                pd.Triggered = true;
                SetArrow(pd.ArrowTarget, pd.ArrowRotation);
                SetInstructions(pd.InstructionRect, pd.Instructions, pd.OkButton);
                m_CurrentPiece = piece;
            }
            else if (!pd.Triggered)
            {
                // Set this as triggered and move down the chain
                pd.Triggered = true;
                if( pd.NextPiece != TutorialPiece.None )
                    TriggerPiece(pd.NextPiece);
            }
        }

        public void FinishPiece(TutorialPiece piece)
        {
            if (m_Pieces[piece].Triggered)
            {
                m_Pieces[piece].Finished = true;
                Save();
            }

            if (m_CurrentPiece == piece)
            {
                if (m_Pieces[piece].NextPiece != TutorialPiece.None)
                    TriggerPiece(m_Pieces[piece].NextPiece);
                else
                {
                    m_CurrentPiece = TutorialPiece.None;
                    m_OKButton = null;
                }
            }
        }

        public void CancelPiece()
        {
            if (m_CurrentPiece != TutorialPiece.None)
            {
                m_Pieces[m_CurrentPiece].Triggered = false;
                m_Pieces[m_CurrentPiece].Finished = false;
                m_CurrentPiece = TutorialPiece.None;
            }
        }

        public void SetArrow(Vector2 target, float rotation)
        {
            m_fArrowInterp = 0;
            m_vArrowTarget = target;

            m_fArrowRotation = rotation;
            Matrix rot = Matrix.CreateFromAxisAngle(Vector3.UnitZ, rotation);
            Vector3 direction3 = Vector3.Normalize(Vector3.Transform(Vector3.UnitX, rot));
            Vector2 direction = new Vector2(direction3.X, direction3.Y);
                    
            m_vArrowTarget = target;
            m_vArrowTravelEnd = Vector2.Add(m_vArrowTarget, Vector2.Multiply(Vector2.Negate(direction), m_fArrowTravelDistance));
        }

        public void SetInstructions(Rectangle rect, string text, bool okButton)
        {
            m_Lines = Happiness.FormatLines(rect.Width - (m_iTextMarginLeftRight * 2), text, Assets.HelpFont);

            int buttonHeight = okButton ? m_iButtonHeight + m_iTextMarginTopBottom : 0;
            int autoHeight = ((m_Lines.Length + 1) * m_iTextLineHeight) + buttonHeight;
            m_InstructionRect = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height == 0 ? autoHeight : rect.Height);
            m_InstructionFrame = new UIFrame(1, m_InstructionRect);

            if( okButton )
                m_OKButton = new UIButton(0, "OK", Assets.HelpFont, new Rectangle(m_InstructionRect.Left + (m_InstructionRect.Width >> 1) - (m_iButtonWidth >> 1), m_InstructionRect.Bottom - (m_iButtonHeight + m_iTextMarginTopBottom), m_iButtonWidth, m_iButtonHeight), Assets.ScrollBar);
            else
                m_OKButton = null;
        }

        #region Drawing
        public void Draw(SpriteBatch sb)
        {
            if (m_CurrentPiece != TutorialPiece.None)
            {
                // Draw Arrow
                DrawArrow(sb);

                // Draw Instructions
                DrawInstructions(sb);
            }
        }

        void DrawArrow(SpriteBatch sb)
        {
            sb.Draw(Assets.Arrow, m_vArrowPosition, null, null, m_vArrowOrigin, m_fArrowRotation, m_vArrowScale, Color.White, SpriteEffects.None, 0);
        }

        void DrawInstructions(SpriteBatch sb)
        {
            if (m_InstructionFrame != null)
            {
                // Draw background
                sb.Draw(Assets.TransGray, m_InstructionRect, Color.White);

                // Draw Frame
                m_InstructionFrame.Draw(sb);

                // Draw Text
                int iY = m_InstructionRect.Top + m_iTextMarginTopBottom;
                int iX = m_InstructionRect.Left + m_iTextMarginLeftRight;
                foreach (FormattedLine line in m_Lines)
                {
                    line.Draw(sb, Assets.HelpFont, new Vector2(iX, iY), m_TextColor);
                    iY += m_iTextLineHeight;
                }

                if( m_OKButton != null )
                    m_OKButton.Draw(sb);
            }
        }
        #endregion

        #region Accessors
        public int ArrowWidth
        {
            get { return (int)m_fArrowWidth; }
        }

        public int ArrowHeight
        {
            get { return (int)m_fArrowHeight; }
        }
        #endregion
    }
}
