using System;


namespace Fusee.Engine
{
    public class InputImp : IInputImp
    {
        //protected GameWindow _gameWindow;
        internal Keymapper _keyMapper;

        public InputImp(IRenderCanvasImp renderCanvas)
        {
            
        }

        public void FrameTick(double time)
        {
            // Do Nothing
        }

        public Point GetMousePos()
        {
            return new Point{x = 10, y = 10};
        }

        public int GetMouseWheelPos()
        {
            return 20;
        }

        public event EventHandler<MouseEventArgs> MouseButtonDown;

        protected void OnGameWinMouseDown(object sender, MouseButtonEventArgs mouseArgs)
        {
           
        }

        public event EventHandler<MouseEventArgs> MouseButtonUp;

        protected void OnGameWinMouseUp(object sender, MouseButtonEventArgs mouseArgs)
        {
            
        }
      
        public event EventHandler<KeyEventArgs> KeyDown;

        protected void OnGameWinKeyDown(object sender, KeyboardKeyEventArgs key)
        {
           
        }

        public event EventHandler<KeyEventArgs> KeyUp;

        protected void OnGameWinKeyUp(object sender, KeyboardKeyEventArgs key)
        {
           
        }
    }

    public class KeyboardKeyEventArgs
    {
    }

    public class MouseButtonEventArgs
    {
    }
}
