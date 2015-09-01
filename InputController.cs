using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
        //private XboxController[] m_aControllers;

        public int m_iMouseX;
        public int m_iMouseY;

        public InputController()
        {
            m_bLeftButtonPressed = false;
            m_bRightButtonPressed = false;

            m_LastKeyboardState = Keyboard.GetState();

            //m_aControllers = new XboxController[4];
            //m_aControllers[0] = new XboxController(PlayerIndex.One);
            //m_aControllers[1] = new XboxController(PlayerIndex.Two);
            //m_aControllers[2] = new XboxController(PlayerIndex.Three);
            //m_aControllers[3] = new XboxController(PlayerIndex.Four);
        }

        public bool IsControllerConnected()
        {
            //for (int i = 0; i < m_aControllers.Length; i++)
            //{
            //    if (m_aControllers[i].IsConnected())
            //        return true;
            //}
            return false;
        }

        public void SetRepeatDelay(double dfDelay)
        {
            //for (int i = 0; i < m_aControllers.Length; i++)
            //{
            //    m_aControllers[i].m_dfRepeatDelay = dfDelay;
            //}
        }

        public bool IsAControllerHoldingLeft()
        {
            //for (int i = 0; i < m_aControllers.Length; i++)
            //{
            //    if (m_aControllers[i].m_bHoldingLeft)
            //        return true;
            //}
            return false;
        }

        public bool IsAControllerHoldingRight()
        {
            //for (int i = 0; i < m_aControllers.Length; i++)
            //{
            //    if (m_aControllers[i].m_bHoldingRight)
            //        return true;
            //}
            return false;
        }

        public void Update(Happiness theGame, GameTime time, PlayerIndex Primary)
        {
            if (theGame.IsActive)
            {
                UpdateKeyboardMouse(theGame);

                //for (int i = 0; i < m_aControllers.Length; i++)
                //{
                //    if (!m_aControllers[i].Update(theGame, time, Primary))
                //    {
                        // Primary Controller disconnected
                //        theGame.Pause();
                //    }
                //}
            }
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
}
