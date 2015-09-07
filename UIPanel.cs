using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Happiness
{
    class UIPanel
    {
        protected Happiness m_Game;
        protected Rectangle m_Rect;

        public UIPanel(Happiness game)
        {
            m_Game = game;
        }

        public virtual void Click(int x, int y) { }
        public virtual void DragBegin(DragArgs args) { }
        public virtual void Drag(DragArgs args) { }
        public virtual void DragEnd(DragArgs args) { }

        public virtual bool Contains(int x, int y) { return m_Rect.Contains(x, y); }

        #region Accessors
        public Rectangle Rect
        {
            get { return m_Rect; }
        }
        #endregion
    }
}
