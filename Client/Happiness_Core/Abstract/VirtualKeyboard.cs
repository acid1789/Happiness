using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness
{
    public abstract class VirtualKeyboard
    {
        protected static VirtualKeyboard s_Instance;

        protected abstract void InternalShowKeyboard(UIInputField inputField);
        protected abstract void InternalHide();

        public static void ShowKeyboard(UIInputField inputField) { s_Instance.InternalShowKeyboard(inputField); }
        public static void Hide() { s_Instance.InternalHide(); }
    }
}
