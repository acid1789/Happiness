using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Happiness_Shared
{
    class InputController_XNA : Happiness.InputController
    {
        MouseState m_LastMouseState;
        KeyboardState m_LastKeyboardState;

        const int m_DragThreshold = 5;
        int m_MousePressedX;
        int m_MousePressedY;
        bool m_bDragging;
        Happiness.DragArgs m_DragArgs;

        Dictionary<Keys, Happiness.Keys> m_XNAToHappinessKeyMap;

        public InputController_XNA()
        {
            s_IC = this;
            m_DragArgs = new Happiness.DragArgs();

            m_LastMouseState = Mouse.GetState();
            m_LastKeyboardState = Keyboard.GetState();

            m_XNAToHappinessKeyMap = new Dictionary<Keys, Happiness.Keys>();
            m_XNAToHappinessKeyMap[Keys.Space] = Happiness.Keys.Space;
            m_XNAToHappinessKeyMap[Keys.Back] = Happiness.Keys.Backspace;
            m_XNAToHappinessKeyMap[Keys.OemPeriod] = Happiness.Keys.OemPeriod;
            m_XNAToHappinessKeyMap[Keys.OemQuestion] = Happiness.Keys.OemQuestion;
            m_XNAToHappinessKeyMap[Keys.OemComma] = Happiness.Keys.OemComma;
            m_XNAToHappinessKeyMap[Keys.OemPipe] = Happiness.Keys.OemPipe;
            m_XNAToHappinessKeyMap[Keys.OemCloseBrackets] = Happiness.Keys.OemCloseBrackets;
            m_XNAToHappinessKeyMap[Keys.OemOpenBrackets] = Happiness.Keys.OemOpenBrackets;
            m_XNAToHappinessKeyMap[Keys.OemPlus] = Happiness.Keys.OemPlus;
            m_XNAToHappinessKeyMap[Keys.OemMinus] = Happiness.Keys.OemMinus;
            m_XNAToHappinessKeyMap[Keys.OemTilde] = Happiness.Keys.OemTilde;
            m_XNAToHappinessKeyMap[Keys.Enter] = Happiness.Keys.Space;
            m_XNAToHappinessKeyMap[Keys.Tab] = Happiness.Keys.Tab;
            m_XNAToHappinessKeyMap[Keys.Escape] = Happiness.Keys.Escape;
        }

        public override void Update(Happiness.Happiness theGame, double time)
        {
            UpdateMouse();
            UpdateTouch();
            UpdateKeyboard();
        }

        void UpdateTouch()
        {
            TouchCollection touches = TouchPanel.GetState();
            if (touches.Count > 0)
            {                
                TouchLocation tl = touches[0];

                Console.WriteLine("Touch (0 / {0}): {1}", touches.Count, tl.Position);
                m_DragArgs.CurrentX = (int)tl.Position.X;
                m_DragArgs.CurrentY = (int)tl.Position.Y;
                switch (tl.State)
                {
                    case TouchLocationState.Pressed:
                        m_MousePressedX = m_DragArgs.CurrentX;
                        m_MousePressedY = m_DragArgs.CurrentY;
                        m_bDragging = false;
                        break;
                    case TouchLocationState.Moved:
                        break;
                    case TouchLocationState.Released:
                        break;
                }
                if (tl.State == TouchLocationState.Released)
                {
                    if (m_bDragging)
                    {
                        Invoke_OnDragEnd(this, m_DragArgs);
                        m_bDragging = false;
                    }
                    else
                    {
                        Invoke_OnClick(this, m_DragArgs);
                    }
                }
                else if (tl.State == TouchLocationState.Moved)
                {
                    int deltaX = m_DragArgs.CurrentX - m_MousePressedX;
                    int deltaY = m_DragArgs.CurrentY - m_MousePressedY;
                    if (Math.Abs(deltaX) > m_DragThreshold || Math.Abs(deltaY) > m_DragThreshold)
                    {
                        if (!m_bDragging)
                        {
                            // Start dragging
                            m_DragArgs.StartX = m_MousePressedX;
                            m_DragArgs.StartY = m_MousePressedY;

                            Invoke_OnDragBegin(this, m_DragArgs);
                            m_bDragging = true;
                        }
                        else
                        {
                            // Still dragging
                            Invoke_OnDrag(this, m_DragArgs);
                        }
                    }
                }
            }
        }

        void UpdateMouse()
        {
            if (!Happiness.Platform.Instance.ApplicationIsActivated())
                return;

            MouseState state = Mouse.GetState();

            if (state.LeftButton == ButtonState.Pressed && m_LastMouseState.LeftButton == ButtonState.Released)
            {
                // Left button press
                m_MousePressedX = state.X;
                m_MousePressedY = state.Y;
                m_bDragging = false;
            }
            else if (state.LeftButton == ButtonState.Released && m_LastMouseState.LeftButton == ButtonState.Pressed)
            {
                // Left button release
                m_DragArgs.CurrentX = state.X;
                m_DragArgs.CurrentY = state.Y;
                m_DragArgs.Abort = false;
                if (m_bDragging)
                {
                    Invoke_OnDragEnd(this, m_DragArgs);
                    m_bDragging = false;
                }
                else
                {
                    // Normal click
                    Invoke_OnClick(this, m_DragArgs);
                }
            }
            else if (state.LeftButton == ButtonState.Pressed)
            {
                // Left button still pressed
                int deltaX = state.X - m_MousePressedX;
                int deltaY = state.Y - m_MousePressedY;
                if (Math.Abs(deltaX) > m_DragThreshold || Math.Abs(deltaY) > m_DragThreshold)
                {
                    m_DragArgs.CurrentX = state.X;
                    m_DragArgs.CurrentY = state.Y;
                    m_DragArgs.Abort = false;
                    if (!m_bDragging)
                    {
                        // Start dragging
                        m_DragArgs.StartX = m_MousePressedX;
                        m_DragArgs.StartY = m_MousePressedY;

                        Invoke_OnDragBegin(this, m_DragArgs);
                        m_bDragging = true;
                    }
                    else
                    {
                        // Still dragging
                        Invoke_OnDrag(this, m_DragArgs);
                    }
                }
            }

            if (state.ScrollWheelValue != 0 && state.ScrollWheelValue != m_LastMouseState.ScrollWheelValue)
            {
                Invoke_OnScroll(state.ScrollWheelValue - m_LastMouseState.ScrollWheelValue);
            }

            m_LastMouseState = state;
        }

        void UpdateKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            foreach (Keys key in m_LastKeyboardState.GetPressedKeys())
            {
                if (!state.IsKeyDown(key))
                {
                    // Key was down, now isnt
                    Happiness.Keys hk = XNAKeyToHappinessKey(key);
                    if(hk != Happiness.Keys.None )
                        Invoke_OnKeyUp(this, new Happiness.KeyArgs(hk, m_LastKeyboardState.IsKeyDown(Keys.LeftShift) | m_LastKeyboardState.IsKeyDown(Keys.RightShift)));
                }
            }

            foreach (Keys key in state.GetPressedKeys())
            {
                if (m_LastKeyboardState.IsKeyDown(key))
                {
                    // Key is down, key was down
                    Happiness.Keys hk = XNAKeyToHappinessKey(key);
                    if (hk != Happiness.Keys.None)
                        Invoke_OnKeyRepeat(this, new Happiness.KeyArgs(hk, state.IsKeyDown(Keys.LeftShift) | state.IsKeyDown(Keys.RightShift)));
                }
                else
                {
                    // Key is donw now, wasnt down before
                    Happiness.Keys hk = XNAKeyToHappinessKey(key);
                    if (hk != Happiness.Keys.None)
                        Invoke_OnKeyDown(this, new Happiness.KeyArgs(hk, state.IsKeyDown(Keys.LeftShift) | state.IsKeyDown(Keys.RightShift)));
                }
            }

            m_LastKeyboardState = state;
        }

        public Happiness.Keys XNAKeyToHappinessKey(Keys k)
        {
            if ( m_XNAToHappinessKeyMap.ContainsKey(k) )
                return m_XNAToHappinessKeyMap[k];
            return Happiness.Keys.None;
        }
    }
}