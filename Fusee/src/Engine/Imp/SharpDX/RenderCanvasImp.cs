using System;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;


//using MathHelper = OpenTK.MathHelper;

namespace Fusee.Engine
{
      
    public class RenderCanvasImp : IRenderCanvasImp
    {
        private  Device device;
        private  SwapChain swapChain;
        
        public int Width { get { return _renderForm.Width; } }
       
        public int Height { get { return _renderForm.Height; } }
       
     // Device device;
        public double DeltaTime
        {
            get
            {
                return 0.01; // _gameWindow.DeltaTime;
            }
        }

        internal RenderForm _renderForm;// RenderForm (Dx)
        
        public RenderCanvasImp()
        {
            _renderForm = new RenderForm("DX_WINDOW");
            // SwapChain description
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription =
                    new ModeDescription(_renderForm.ClientSize.Width, _renderForm.ClientSize.Height,
                                        new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = _renderForm.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);
           
        }

        public void Present()
        {

        }

        public void Run()
        {
            RenderLoop.Run(_renderForm, () => swapChain.Present(0, PresentFlags.None));
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

    
}