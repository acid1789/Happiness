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
        private MouseState m_LastMouseState;

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
            m_DragArgs = new DragArgs();
            
            m_LastMouseState = Mouse.GetState();
        }

        public void Update(Happiness theGame, GameTime time, PlayerIndex Primary)
        {
            UpdateMouse();
            UpdateTouch();
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
                    if( OnDragEnd != null)
                        OnDragEnd(this, m_DragArgs);
                    m_bDragging = false;
                }
                else
                {
                    // Normal click
                    if( OnClick != null )
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

                        if( OnDragBegin != null )
                            OnDragBegin(this, m_DragArgs);
                        m_bDragging = true;
                    }
                    else
                    {
                        // Still dragging
                        if( OnDrag != null )
                            OnDrag(this, m_DragArgs);
                    }                    
                }
            }

            m_LastMouseState = state;
        }

        /*
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
            }
        }
        */

        static InputController s_IC;
        public static InputController IC
        {
            get { if (s_IC == null) s_IC = new InputController(); return s_IC; }
        }

        public static void Update(GameTime time)
        {
            IC.Update(null, time, PlayerIndex.One);
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
