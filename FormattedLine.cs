using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    public class FormattedLine
    {
        public class PieceInfo
        {
            public string m_Str;
            public Texture2D m_Icon;

            public PieceInfo(string str)
            {
                m_Str = str;
                m_Icon = null;
            }

            public PieceInfo(Texture2D icon)
            {
                m_Str = null;
                m_Icon = icon;
            }
        }

        List<PieceInfo> m_Pieces;

        public FormattedLine()
        {
            m_Pieces = new List<PieceInfo>();
        }

        FormattedLine(FormattedLine other)
        {
            List<PieceInfo> pieces = new List<PieceInfo>();
            foreach (PieceInfo p in other.m_Pieces)
            {
                if (p.m_Str != null)
                    pieces.Add(new PieceInfo(p.m_Str));
                else
                    pieces.Add(new PieceInfo(p.m_Icon));
            }
            m_Pieces = pieces;
        }

        public FormattedLine(string line)
        {
            List<PieceInfo> pieces = new List<PieceInfo>();

            string[] icons = line.Split('{');
            foreach (string iconStr in icons)
            {
                int idx = iconStr.IndexOf('}');
                if (idx >= 0)
                {
                    string iconInfo = iconStr.Substring(0, idx++);
                    string remaining = iconStr.Substring(idx, iconStr.Length - idx);

                    string[] iconInfoPieces = iconInfo.Split(':');
                    if (iconInfoPieces[0] == "icon")
                    {
                        pieces.Add(new PieceInfo(Assets.IconMap[iconInfoPieces[1]]));
                    }

                    if (remaining.Length > 0)
                        pieces.Add(new PieceInfo(remaining));
                }
                else
                {
                    pieces.Add(new PieceInfo(iconStr));
                }
            }

            m_Pieces = pieces;
        }

        public void Draw(SpriteBatch sb, SpriteFont font, Vector2 position, Color color)
        {
            Vector2 fontSize = font.MeasureString("qQ");
            int iconSize = (int)fontSize.Y;
            float fX = position.X;
            float fY = position.Y;

            foreach (PieceInfo piece in m_Pieces)
            {
                if (piece.m_Str != null)
                {
                    Happiness.ShadowString(sb, font, piece.m_Str, new Vector2(fX, fY), color);
                    fX += font.MeasureString(piece.m_Str).X;
                }
                if (piece.m_Icon != null)
                {
                    sb.Draw(piece.m_Icon, new Rectangle((int)fX, (int)fY, iconSize, iconSize), Color.White);
                    fX += iconSize;
                }
            }
        }

        public Vector2 Size(SpriteFont font)
        {
            Vector2 fontSize = font.MeasureString("qQ");
            float width = 0;
            foreach (PieceInfo piece in m_Pieces)
            {
                if (piece.m_Str != null)
                {
                    width += font.MeasureString(piece.m_Str).X;
                }
                if (piece.m_Icon != null)
                {
                    width += fontSize.Y;
                }
            }
            return new Vector2(width, fontSize.Y);
        }

        public PieceInfo[] Split()
        {
            List<PieceInfo> words = new List<PieceInfo>();

            foreach (PieceInfo piece in m_Pieces)
            {
                if (piece.m_Str != null)
                {
                    string[] splitWords = piece.m_Str.Split(' ');
                    foreach (string word in splitWords)
                        words.Add(new PieceInfo(word));
                }
                if (piece.m_Icon != null)
                    words.Add(piece);
            }

            return words.ToArray();
        }

        public void Add(PieceInfo piece)
        {
            if( m_Pieces.Count > 0 )
                m_Pieces.Add(new PieceInfo(" "));
            m_Pieces.Add(piece);
        }

        public void Merge()
        {
            List<PieceInfo> merged = new List<PieceInfo>();
            PieceInfo pending = null;
            foreach (PieceInfo piece in m_Pieces)
            {
                if (piece.m_Str != null)
                {
                    if (pending != null)
                    {
                        pending.m_Str += piece.m_Str;
                    }
                    else
                    {
                        pending = piece;
                    }
                }
                else
                {
                    if (pending != null)
                    {
                        merged.Add(pending);
                        pending = null;
                    }
                    merged.Add(piece);
                }
            }
            if( pending != null )
                merged.Add(pending);
            m_Pieces = merged;
        }

        public static FormattedLine operator +(FormattedLine a, PieceInfo piece)
        {
            FormattedLine nl = new FormattedLine(a);
            nl.Add(piece);
            return nl;
        }
    }
}
