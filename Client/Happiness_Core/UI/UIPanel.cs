using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness
{
    public class UIPanel
    {
        protected Scene m_Scene;
        protected Rectangle m_Rect;

        public UIPanel(Scene scene)
        {
            m_Scene = scene;
        }

        public virtual void Click(int x, int y) { }
        public virtual void DragBegin(DragArgs args) { }
        public virtual void Drag(DragArgs args) { }
        public virtual void DragEnd(DragArgs args) { }
        
        public virtual bool Contains(int x, int y) { return m_Rect.Contains(x, y); }

        public virtual void Draw(Renderer sb) { }

        #region Accessors
        public Rectangle Rect
        {
            get { return m_Rect; }
        }

        public Scene Scene
        {
            get { return m_Scene; }
        }
        #endregion
    }
}
