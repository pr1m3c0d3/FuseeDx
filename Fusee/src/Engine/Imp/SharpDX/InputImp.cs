using System;
using System.Windows.Forms;
using SharpDX.DirectInput;
using SharpDX.Windows;

//constants for mouse buttons (NEW)


namespace Fusee.Engine
{

   
    public class InputImp : IInputImp
    {
        //protected GameWindow _gameWindow;
        internal Keymapper KeyMapper;
        internal RenderForm _renderForm;
        internal Keyboard _keyboard;
        internal Mouse _mouse;
        internal KeyboardState _keyboardState;
        internal MouseState _mouseState;
        internal string[] _mButtons = new string[8];
        internal int wheelDelta;

        public InputImp(IRenderCanvasImp renderCanvas)
        {
            if (renderCanvas == null)
                throw new ArgumentNullException("renderCanvas");

            if (!(renderCanvas is RenderCanvasImp))
                throw new ArgumentException("renderCanvas must be of type RenderCanvasImp", "renderCanvas");

            _renderForm = ((RenderCanvasImp)renderCanvas)._renderForm;
            _renderForm.KeyDown += OnGameWinKeyDown;
            _renderForm.KeyUp += OnGameWinKeyUp;
            _renderForm.MouseDown += OnGameWinMouseDown;
            _renderForm.MouseUp += OnGameWinMouseUp;
            var di = new DirectInput();
            _keyboard = new Keyboard(di);
            _keyboard.Acquire();

            _mouse = new Mouse(di);
            _mouse.Acquire();
            
            KeyMapper = new Keymapper();
        }

        public void FrameTick(double time)
        {
            // Do Nothing
        }

        public Point GetMousePos()
        {
            _mouseState = _mouse.GetCurrentState();
            return new Point{x = _mouseState.X, y = _mouseState.Y};
        }

        public int GetMouseWheelPos()
        {
            _mouseState=  _mouse.GetCurrentState();
            return _mouseState.Z;
        }

        public event EventHandler<MouseEventArgs> MouseButtonDown;

        protected void OnGameWinMouseDown(object sender, System.Windows.Forms.MouseEventArgs mouseEventArgs)
        {
            _mouseState=  _mouse.GetCurrentState();
            
            if (_mouseState.Buttons[1] == true || _mouseState.Buttons[0] == true || _mouseState.Buttons[2] == true)
            {
                if (MouseButtonDown != null)
                {
                    var mb = MouseButtons.Unknown;
                    switch (mouseEventArgs.Button)
                    {
                        case System.Windows.Forms.MouseButtons.Left:
                            mb = MouseButtons.Left;
                            break;
                        case System.Windows.Forms.MouseButtons.Right:
                            mb = MouseButtons.Right;
                            break;
                        case System.Windows.Forms.MouseButtons.Middle:
                            mb = MouseButtons.Middle;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("button");
                    }
                    MouseButtonDown(this, new MouseEventArgs
                        {
                            Button = mb,
                            Position = new Point { x = mouseEventArgs.X, y = mouseEventArgs.Y }
                        });
                }

            }
        }

        public event EventHandler<MouseEventArgs> MouseButtonUp;

        protected void OnGameWinMouseUp(object sender, System.Windows.Forms.MouseEventArgs mouseEventArgs)
        {
            _mouseState = _mouse.GetCurrentState();
            if (_mouseState.Buttons[1] == false && _mouseState.Buttons[0]==false && _mouseState.Buttons[2]==false)
            {
                if (MouseButtonUp != null)
                {
                    var mb = MouseButtons.Unknown;
                    switch (mouseEventArgs.Button)
                    {
                        case System.Windows.Forms.MouseButtons.Left:
                            mb = MouseButtons.Left;
                            break;
                        case System.Windows.Forms.MouseButtons.Right:
                            mb = MouseButtons.Right;
                            break;
                        case System.Windows.Forms.MouseButtons.Middle:
                            mb = MouseButtons.Middle;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("button");
                    }
                    MouseButtonUp(this, new MouseEventArgs
                    {
                        Button = mb,
                        Position = new Point { x = mouseEventArgs.X, y = mouseEventArgs.Y }
                    });
                }
            }
           
        }
      
        public event EventHandler<KeyEventArgs> KeyDown;

        protected void OnGameWinKeyDown(object sender, System.Windows.Forms.KeyEventArgs keyEventArgs)
        {
            _keyboardState = _keyboard.GetCurrentState();
            if (_keyboardState.PressedKeys != null && KeyMapper.ContainsKey((Key)keyEventArgs.KeyValue))
            {
                KeyDown(this, new KeyEventArgs()
                    {
                        Alt = false,
                        Control = false,
                        Shift = false,
                        KeyCode = (KeyCodes) keyEventArgs.KeyCode
                    });
            }
        }

       public event EventHandler<KeyEventArgs> KeyUp;

        protected void OnGameWinKeyUp(object sender, System.Windows.Forms.KeyEventArgs keyEventArgs)
        {
            _keyboardState = _keyboard.GetCurrentState();
            if (_keyboardState.PressedKeys != null && KeyMapper.ContainsKey((Key)keyEventArgs.KeyValue))
            {
                KeyUp(this, new KeyEventArgs()
                {
                    Alt = false,
                    Control = false,
                    Shift = false,
                    KeyCode = (KeyCodes) keyEventArgs.KeyCode
                });
            }
        }
    }
}

