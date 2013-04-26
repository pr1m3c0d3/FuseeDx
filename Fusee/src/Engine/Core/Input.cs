﻿using System.Collections.Generic;

namespace Fusee.Engine
{
    /// <summary>
    /// The Input class provides takes care of al imputs. It is accessible from everywhere.
    /// E.g. : Input.Instance.IsButtonDown(MouseButtons.Left);
    /// </summary>
    public class Input
    {

        private static Input _instance;

        internal IInputImp InputImp
        {
            set 
            { 
                _inputImp = value;
                _inputImp.KeyDown += KeyDown;
                _inputImp.KeyUp += KeyUp;
                _inputImp.MouseButtonDown += ButtonDown;
                _inputImp.MouseButtonUp += ButtonUp;
                _axes = new float[(int)InputAxis.LastAxis];
                _axesPreviousAbsolute = new float[(int)InputAxis.LastAxis];
                //_keysPressed = new Dictionary<KeyCodes, bool>();
                //_buttonsPressed = new Dictionary<MouseButtons, bool>();
                _keysPressed = new Dictionary<int, bool>();
                _buttonsPressed = new Dictionary<int, bool>();
            }
        }

        private IInputImp _inputImp;
        private float[] _axes;
        private float[] _axesPreviousAbsolute;
        // private HashSet<KeyCodes> _keysPressed;
        // private HashSet<MouseButtons> _buttonsPressed;
        // private Dictionary<KeyCodes, bool> _keysPressed;
        // private Dictionary<MouseButtons, bool> _buttonsPressed;
        private Dictionary<int, bool> _keysPressed;
        private Dictionary<int, bool> _buttonsPressed;

        private void KeyDown(object sender, KeyEventArgs kea)
        {
            if (!_keysPressed.ContainsKey((int)kea.KeyCode))
                _keysPressed.Add((int)kea.KeyCode, true);
        }

        private void KeyUp(object sender, KeyEventArgs kea)
        {
            if (_keysPressed.ContainsKey((int)kea.KeyCode))
                _keysPressed.Remove((int)kea.KeyCode);
        }

        private void ButtonDown(object sender, MouseEventArgs mea)
        {
            if (!_buttonsPressed.ContainsKey((int)mea.Button))
                _buttonsPressed.Add((int)mea.Button, true);
        }

        private void ButtonUp(object sender, MouseEventArgs mea)
        {
            if (_buttonsPressed.ContainsKey((int)mea.Button))
                _buttonsPressed.Remove((int)mea.Button);
        }

        /// <summary>
        /// Create a new instance of the Input class and initialize it with an underlying InputImp instance.
        /// </summary>
        public Input()
        {
            
            
        }

        /// <summary>
        /// Returns the scalar value for the given axis. Typically these values are used as velocities.
        /// </summary>
        /// <param name="axis">The axis for which the value is to be returned.</param>
        /// <returns>
        /// The current deflection of the given axis.
        /// </returns>
        public float GetAxis(InputAxis axis)
        {
            return _axes[(int)axis];
        }

        /// <summary>
        /// Check if a given key is pressed during the current frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// true if the key is pressed. Otherwise false.
        /// </returns>
        public bool IsKeyDown(KeyCodes key)
        {
            return _keysPressed.ContainsKey((int)key);
        }

        /// <summary>
        /// Check if a given mouse button is pressed during the current frame.
        /// </summary>
        /// <param name="button">the button to check.</param>
        /// <returns>
        /// true if the button is pressed. Otherwise false.
        /// </returns>
        public bool IsButtonDown(MouseButtons button)
        {
            return _buttonsPressed.ContainsKey((int)button);
        }

        internal void OnUpdateFrame(double deltaTime)
        {
            Point p = _inputImp.GetMousePos();
            float curr = (float) p.x;
            _axes[(int)InputAxis.MouseX] = (curr - _axesPreviousAbsolute[(int)InputAxis.MouseX]) * ((float) deltaTime);
            _axesPreviousAbsolute[(int) InputAxis.MouseX] = curr;


            curr = (float) p.y;
            _axes[(int)InputAxis.MouseY] = (curr - _axesPreviousAbsolute[(int)InputAxis.MouseY]) * ((float) deltaTime);
           _axesPreviousAbsolute[(int) InputAxis.MouseY] = curr;

            curr = _inputImp.GetMouseWheelPos();
            _axes[(int)InputAxis.MouseWheel] = (curr - _axesPreviousAbsolute[(int)InputAxis.MouseWheel]) * ((float) deltaTime);
            _axesPreviousAbsolute[(int)InputAxis.MouseWheel] = curr;
        }

        /// <summary>
        /// Provides the Instance of the Input Class.
        /// </summary>
        public static Input Instance
        {
            get
            {
                if (_instance  == null)
                {
                    _instance = new Input();
                }
                return _instance;
            }
        }
    }
}
