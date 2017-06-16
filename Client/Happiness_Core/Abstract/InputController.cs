using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happiness
{
    public abstract class InputController
    {
        public event EventHandler<DragArgs> OnDragBegin;
        public event EventHandler<DragArgs> OnDrag;
        public event EventHandler<DragArgs> OnDragEnd;
        public event EventHandler<DragArgs> OnClick;
        public event EventHandler<KeyArgs> OnKeyDown;
        public event EventHandler<KeyArgs> OnKeyRepeat;
        public event EventHandler<KeyArgs> OnKeyUp;
        public event Action<int> OnScroll;

        public abstract void Update(Happiness theGame, double time);

        protected static InputController s_IC;
        public static InputController IC
        {
            get { return s_IC; }
        }

        public static void Update(double time)
        {
            IC.Update(null, time);
        }

        protected void Invoke_OnDragBegin(object sender, DragArgs args) { OnDragBegin?.Invoke(sender, args); }
        protected void Invoke_OnDrag(object sender, DragArgs args) { OnDrag?.Invoke(sender, args); }
        protected void Invoke_OnDragEnd(object sender, DragArgs args) { OnDragEnd?.Invoke(sender, args); }
        protected void Invoke_OnClick(object sender, DragArgs args) { OnClick?.Invoke(sender, args); }
        protected void Invoke_OnKeyDown(object sender, KeyArgs args) { OnKeyDown?.Invoke(sender, args); }
        protected void Invoke_OnKeyRepeat(object sender, KeyArgs args) { OnKeyRepeat?.Invoke(sender, args); }
        protected void Invoke_OnKeyUp(object sender, KeyArgs args) { OnKeyUp?.Invoke(sender, args); }
        protected void Invoke_OnScroll(int delta) { OnScroll?.Invoke(delta); }
    }

    public class DragArgs : EventArgs 
    {
        public int StartX;
        public int StartY;
        public int CurrentX;
        public int CurrentY;
        public bool Abort;
    }

    public class KeyArgs : EventArgs
    {
        public Keys Key;
        public bool Shift;

        public KeyArgs(Keys key, bool shift)
        {
            Key = key;
            Shift = shift;
        }
    }
}
