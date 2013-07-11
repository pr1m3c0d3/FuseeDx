﻿using System;

namespace Fusee.Engine
{
    /// <summary>
    /// To be implemented on certain platforms
    /// </summary>
    public interface IInputImp
    {
        /// <summary>
        /// Implement this to receive callbacks once a frame if your implementation needs
        /// regular updates.
        /// </summary>
        /// <param name="time">The elapsed time since the last frame.</param>
        void FrameTick(double time);

        /// <summary>
        /// Implement this to deliver mouse information.
        /// </summary>
        /// <returns>The point containing window X and Y values.</returns>
        Point GetMousePos();

        /// <summary>
        /// Implement this to return the absolute mouse wheel position
        /// </summary>
        /// <returns>The mouse wheel position.</returns>
        int GetMouseWheelPos();

        /// <summary>
        /// Trigger this event on any mouse button pressed down (and held).
        /// </summary>
        event EventHandler<MouseEventArgs> MouseButtonDown;

        /// <summary>
        /// Trigger this event on any mouse button release.
        /// </summary>
        event EventHandler<MouseEventArgs> MouseButtonUp;

        /// <summary>
        /// Trigger this event once a key on the keyboard is pressed down.
        /// </summary>
        event EventHandler<KeyEventArgs> KeyDown;

        /// <summary>
        /// Trigger this event in your implementation once a key on the keyboard is released.
        /// </summary>
        event EventHandler<KeyEventArgs> KeyUp;      
    }
}