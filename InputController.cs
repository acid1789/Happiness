using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Happiness
{
    public class InputController
    {
        public bool m_bLeftButtonPressed;
        public bool m_bRightButtonPressed;
        private int m_iDragX;
        private int m_iDragY;
        private MouseState m_LastMouseState;
        private KeyboardState m_LastKeyboardState;
        TouchCollection m_LastTouchState;

        public int m_iMouseX;
        public int m_iMouseY;

        const int m_DragThreshold = 5;
        int m_MousePressedX;
        int m_MousePressedY;
        bool m_bDragging;
        DragArgs m_DragArgs;

        public event EventHandler<DragArgs> OnDragBegin;
        public event EventHandler<DragArgs> OnDrag;
        public event EventHandler<DragArgs> OnDragEnd;
        public event EventHandler<DragArgs> OnClick;

        public InputController()
        {
            m_bLeftButtonPressed = false;
            m_bRightButtonPressed = false;

            m_DragArgs = new DragArgs();

            m_LastKeyboardState = Keyboard.GetState();
            m_LastMouseState = Mouse.GetState();
        }

        public void Update(Happiness theGame, GameTime time, PlayerIndex Primary)
        {
            if (theGame.IsActive)
            {
                //UpdateKeyboardMouse(theGame);
                UpdateMouse();
                UpdateTouch();
            }
        }

        void UpdateTouch()
        {
            TouchCollection touches = TouchPanel.GetState();
            if( touches.Count > 0 )
            {
                TouchLocation tl = touches[0];
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
                        OnDragEnd(this, m_DragArgs);
                        m_bDragging = false;
                    }
                    else
                    {
                        OnClick(this, m_DragArgs);
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

                            OnDragBegin(this, m_DragArgs);
                            m_bDragging = true;
                        }
                        else
                        {
                            // Still dragging
                            OnDrag(this, m_DragArgs);
                        }
                    }
                }
            }
        }

        void UpdateMouse()
        {
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
                if (m_bDragging)
                {
                    OnDragEnd(this, m_DragArgs);
                    m_bDragging = false;
                }
                else
                {
                    // Normal click
                    OnClick(this, m_DragArgs);
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
                    if (!m_bDragging)
                    {
                        // Start dragging
                        m_DragArgs.StartX = m_MousePressedX;
                        m_DragArgs.StartY = m_MousePressedY;

                        OnDragBegin(this, m_DragArgs);
                        m_bDragging = true;
                    }
                    else
                    {
                        // Still dragging
                        OnDrag(this, m_DragArgs);
                    }                    
                }
            }

            m_LastMouseState = state;
        }

        private void UpdateKeyboardMouse(Happiness theGame)
        {
            MouseState state = Mouse.GetState();
            m_iMouseX = state.X;
            m_iMouseY = state.Y;

            if (state.LeftButton == ButtonState.Pressed)
            {
                if (!m_bLeftButtonPressed)
                {
                    m_iDragX = state.X;
                    m_iDragY = state.Y;
                }
                else
                {
                    if (Math.Abs(state.X - m_iDragX) < 10 && Math.Abs(state.Y - m_iDragY) < 10)
                    {
                        theGame.DragFrom(m_iDragX, m_iDragY);
                    }
                }

                m_bLeftButtonPressed = true;
            }
            if (state.RightButton == ButtonState.Pressed)
            {
                if (!m_bRightButtonPressed)
                {
                    m_iDragX = state.X;
                    m_iDragY = state.Y;
                }

                m_bRightButtonPressed = true;
            }

            if (state.LeftButton == ButtonState.Released && m_bLeftButtonPressed)
            {
                theGame.ClearDragIcon();
                if (Math.Abs(state.X - m_iDragX) < 10 && Math.Abs(state.Y - m_iDragY) < 10)
                {
                    // Click in one location
                    theGame.LeftClick(state.X, state.Y);
                }
                else
                {
                    // Drag somewhere else
                    theGame.DragIcon(m_iDragX, m_iDragY, state.X, state.Y);
                }
                m_bLeftButtonPressed = false;
            }
            if (state.RightButton == ButtonState.Released && m_bRightButtonPressed)
            {
                theGame.RightClick(state.X, state.Y);
                m_bRightButtonPressed = false;
            }

            int iScrollDelta = state.ScrollWheelValue - m_LastMouseState.ScrollWheelValue;
            if (iScrollDelta > 0)
            {
                theGame.ScrollUp();
            }
            else if (iScrollDelta < 0)
            {
                theGame.ScrollDown();
            }


            KeyboardState kstate = Keyboard.GetState();
            Keys[] oldPressed = m_LastKeyboardState.GetPressedKeys();
            Keys[] newPressed = kstate.GetPressedKeys();
            for (int i = 0; i < oldPressed.Length; i++)
            {
                // See if this key is still pressed
                bool bStillPressed = false;
                for (int j = 0; j < newPressed.Length; j++)
                {
                    if (oldPressed[i] == newPressed[j])
                    {
                        bStillPressed = true;
                        break;
                    }
                }
                if (!bStillPressed)
                {
                    // Key was released, handle it
                    HandleKeyRelease(theGame, oldPressed[i]);
                }
            }
            m_LastMouseState = state;
            m_LastKeyboardState = kstate;
        }

        private void HandleKeyRelease(Happiness theGame, Keys key)
        {
            switch (key)
            {
                case Keys.Escape:
                    theGame.Pause();
                    break;
                case Keys.S:
                    theGame.SavePuzzle();
                    break;
                case Keys.H:        // Hint
                    theGame.ShowHint();
                    break;
                case Keys.U:        // Unhide clues
                    theGame.UnHideAllClues();
                    break;
                case Keys.Z:        // Undo/Redo
                    if (m_LastKeyboardState.IsKeyDown(Keys.LeftControl) || m_LastKeyboardState.IsKeyDown(Keys.RightControl))
                    {
                        if (m_LastKeyboardState.IsKeyDown(Keys.LeftShift) || m_LastKeyboardState.IsKeyDown(Keys.RightShift))
                            theGame.Redo();
                        else
                            theGame.Undo();
                    }
                    break;
                case Keys.Up:       // Previous Selection
                    theGame.NavigateUp();
                    break;
                case Keys.Down:     // Next Selection
                    theGame.NavigateDown();
                    break;
                case Keys.Left:     // Navigate Left
                    theGame.NavigateLeft();
                    break;
                case Keys.Right:    // Navigate Right
                    theGame.NavigateRight();
                    break;
                case Keys.Enter:
                case Keys.Space:
                    theGame.SelectItem();
                    break;
            }
        }
    }

    public class DragArgs : EventArgs 
    {
        public int StartX;
        public int StartY;
        public int CurrentX;
        public int CurrentY;
    }    
}
