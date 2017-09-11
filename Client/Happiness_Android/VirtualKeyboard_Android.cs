using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;

using Happiness;

namespace Happiness_Android
{
    class VirtualKeyboard_Android : VirtualKeyboard
    {
        UIInputField m_CurrentInput;
        public VirtualKeyboard_Android()
        {
            s_Instance = this;
        }

        public static VirtualKeyboard_Android Instance { get { return (VirtualKeyboard_Android)s_Instance; } }

        protected override void InternalShowKeyboard(UIInputField inputField)
        {
            m_CurrentInput = inputField;
            InputMethodManager inputMethodManager = (InputMethodManager)Activity1.Instance.Application.GetSystemService(Context.InputMethodService);
            inputMethodManager.ShowSoftInput(Activity1.Instance.TheGame.Services.GetService<View>(), ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        protected override void InternalHide()
        {
            InputMethodManager inputMethodManager = (InputMethodManager)Activity1.Instance.Application.GetSystemService(Context.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(Activity1.Instance.TheGame.Services.GetService<View>().WindowToken, HideSoftInputFlags.None);
        }

        public void HandleEvent(KeyEventActions a, Keycode key, bool shift, string characters)
        {
            KeyArgs k = new KeyArgs(AndroidKeyToGenericKey(key), shift);
            if (k.Key != Keys.None)
                InputController.IC.ProcessExternalKey(k, AndroidActionToGenericAction(a));
        }

        InputController.InputType AndroidActionToGenericAction(KeyEventActions a)
        {
            switch (a)
            {
                default:
                case KeyEventActions.Down: return InputController.InputType.Down;
                case KeyEventActions.Multiple: return InputController.InputType.Repeat;
                case KeyEventActions.Up: return InputController.InputType.Up;
            }
        }

        Keys AndroidKeyToGenericKey(Keycode androidKey)
        {
            switch (androidKey)
            {
                case Keycode.A: return Keys.A;
                case Keycode.B: return Keys.B;
                case Keycode.C: return Keys.C;
                case Keycode.D: return Keys.D;
                case Keycode.E: return Keys.E;
                case Keycode.F: return Keys.F;
                case Keycode.G: return Keys.G;
                case Keycode.H: return Keys.H;
                case Keycode.I: return Keys.I;
                case Keycode.J: return Keys.J;
                case Keycode.K: return Keys.K;
                case Keycode.L: return Keys.L;
                case Keycode.M: return Keys.M;
                case Keycode.N: return Keys.N;
                case Keycode.O: return Keys.O;
                case Keycode.P: return Keys.P;
                case Keycode.Q: return Keys.Q;
                case Keycode.R: return Keys.R;
                case Keycode.S: return Keys.S;
                case Keycode.T: return Keys.T;
                case Keycode.U: return Keys.U;
                case Keycode.V: return Keys.V;
                case Keycode.W: return Keys.W;
                case Keycode.X: return Keys.X;
                case Keycode.Y: return Keys.Y;
                case Keycode.Z: return Keys.Z;
                case Keycode.Num0: return Keys.Num0;
                case Keycode.Num1: return Keys.Num1;
                case Keycode.Num2: return Keys.Num2;
                case Keycode.Num3: return Keys.Num3;
                case Keycode.Num4: return Keys.Num4;
                case Keycode.Num5: return Keys.Num5;
                case Keycode.Num6: return Keys.Num6;
                case Keycode.Num7: return Keys.Num7;
                case Keycode.Num8: return Keys.Num8;
                case Keycode.Num9: return Keys.Num9;
                case Keycode.Period: return Keys.OemPeriod;
                case Keycode.Comma: return Keys.OemComma;
                case Keycode.Plus: return Keys.OemPlus;
                case Keycode.Del: return Keys.Backspace;
                case Keycode.Enter: return Keys.Enter;
                case Keycode.Slash: return Keys.Slash;
                case Keycode.Backslash: return Keys.Backslash;
                case Keycode.Space: return Keys.Space;
                case Keycode.Equals: return Keys.Equals;
                case Keycode.Minus: return Keys.OemMinus;
                case Keycode.Apostrophe: return Keys.Apostrophe;
                case Keycode.Semicolon: return Keys.Semicolon;
                case Keycode.ShiftLeft: return Keys.None;
                case Keycode.Unknown: return Keys.None;


                default:
                    Console.WriteLine("Unhandled key: " + androidKey);
                    return Keys.None;
            }
        }
    }
}