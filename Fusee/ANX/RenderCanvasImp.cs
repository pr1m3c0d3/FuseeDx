using System;
using System.Drawing;
using System.Windows.Forms;


using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D9;

using SlimDX.DXGI;
using SlimDX.Windows;

using Device = SlimDX.Direct3D9.Device;
using Resource = SlimDX.Direct3D9.Resource;
//using ANX.Framework;
//using SharpDX;

//using ANX.Framework.Graphics;
//using ANX.Framework.Design;
//using ANX.Framework.Content;
//using ANX.Framework.Graphics;
//using ANX.Framework.Media;
////using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//using MathHelper = OpenTK.MathHelper;

namespace Fusee.Engine
{
      
    public class RenderCanvasImp : IRenderCanvasImp
    {
        public int Width { get { return _width; } }
        internal int _width;
        public int Height { get { return _height; } }
        internal int _height;
      Device device;
        public double DeltaTime
        {
            get
            {
                return _gameWindow.DeltaTime;
            }
        }

        internal RenderCanvasGameWindow _gameWindow;

        public RenderCanvasImp()
        {
            try
            {
                _gameWindow = new RenderCanvasGameWindow(this, true);
            }
            catch
            {
                _gameWindow = new RenderCanvasGameWindow(this, false);
            }
        }

        public void Present()
        {
            if (_gameWindow != null)
                // _gameWindow.SwapBuffers();

              //SlimDX.Direct3D9.SwapEffect.Discard;//HMMMMMMMMMMMMMMMMM????????
               //alternative?    
                   device.Dispose(); 
        }

        public void Run()
        {
            if (_gameWindow != null)
                
                device.Present();
            

        }

        public event EventHandler<InitEventArgs> Init;
        public event EventHandler<InitEventArgs> UnLoad;

        public event EventHandler<RenderEventArgs> Render;
        public event EventHandler<ResizeEventArgs> Resize;

        internal void DoInit()
        {
            if (Init != null)
                Init(this, new InitEventArgs());
        }

        internal void DoUnLoad()
        {
            if (UnLoad != null)
                UnLoad(this, new InitEventArgs());
        }

        internal void DoRender()
        {
            if (Render != null)
                Render(this, new RenderEventArgs());
        }

        internal void DoResize()
        {
            if (Resize != null)
                Resize(this, new ResizeEventArgs());
        }
    }

    class RenderCanvasGameWindow 
    {

        Device device;
       
        private RenderCanvasImp _renderCanvasImp;
        private double _deltaTime;
        public double DeltaTime
        {
            get { return _deltaTime; }
        }

        public RenderCanvasGameWindow(RenderCanvasImp renderCanvasImp, bool antiAliasing)
        //    : base(1280, 720, new GraphicsMode(32,24,0,(antiAliasing) ? 8 : 0) /*GraphicsMode.Default*/, "Fusee Engine")
        //   : base(1280, 720, new Color4(0.32f, 0.24f, 0.0f), (antiAliasing) ? MultisampleType.EightSamples : MultisampleType.None , "Fusee Engine") FALSCH 
        {
           
           /** FALSCHE Lösung
           var viewport = new Viewport(0, 0, 1280, 720);
           new Color4(0.32f, 0.24f, 0.0f);
           if(antiAliasing==true) {
               
           MultisampleType   aA=  MultisampleType.EightSamples;
           }
            else
           {
           MultisampleType   aA= MultisampleType.None;
           }
           **/
            _renderCanvasImp = renderCanvasImp;
        }

        protected override void OnLoad(EventArgs e)
        {
   
            // Check for necessary capabilities:
           /*
            //  string version = GL.GetString(StringName.Version);
   
            int major = (int)version[0];
            // int minor = (int)version[2];
            if (major < 2)
            {
                MessageBox.Show("You need at least OpenGL 2.0 to run this example. Aborting.", "GLSL not supported",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Exit();
            }
            */
            // Obere Code durch CheckVersion ersetzt, sollte passen
           // D3DX.CheckVersion();
            
            //

            
            device.Clear(ClearFlags.Target, Color.MidnightBlue, 1.0f, 0);

            //GL.ClearColor(Color.MidnightBlue);
            //GL.Enable(EnableCap.DepthTest);

            _renderCanvasImp.DoInit();
        }

        protected override void OnUnload(EventArgs e)
        {
            _renderCanvasImp.DoUnLoad();
            // if (_renderCanvasImp != null)
            //     _renderCanvasImp.Dispose();      
        }


        protected override void OnResize(EventArgs e)
        {
            if (_renderCanvasImp != null)
            {
                //_renderCanvasImp._width = Width;
                //_renderCanvasImp._height = Height;
                _renderCanvasImp.DoResize();
            }

            /*
            GL.Viewport(0, 0, Width, Height);

            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
             * */
        }

        //protected override void OnUpdateFrame(FrameEventArgs e)
        //{
        //    if (Keyboard[OpenTK.Input.Key.Escape])
        //        this.Exit();

        //    if (Keyboard[OpenTK.Input.Key.F11])
        //        if (WindowState != WindowState.Fullscreen)
        //            WindowState = WindowState.Fullscreen;
        //        else
        //            WindowState = WindowState.Normal;
        //}

        //protected override void OnRenderFrame(FrameEventArgs e)
        //{
        //    _deltaTime = e.Time;
        //    if (_renderCanvasImp != null)
        //    {
        //        _renderCanvasImp.DoRender();
        //    }
        //}
    }
}