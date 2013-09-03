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
        internal Device _device;
        internal SwapChain _swapChain;
        internal DeviceContext _context;
        internal RenderTargetView _renderView;
        internal Buffer _cbUniforms;
        internal Texture2D _depthBuffer;
        internal SamplerState _sampler;
        internal ShaderResourceView _textureView;
        internal DepthStencilView _depthView;
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

        /*
        * 
        * 
        * 
        */
        //NEU
        public bool VerticalSync
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        internal RenderForm _renderForm;// RenderForm (Dx)

        public RenderCanvasImp()
        {
            _renderForm = new RenderForm("DX_WINDOW");
            _renderForm.Resize += OnRenderFormOnResize;
            
            //Fullscreen
            //_renderForm.MaximizeBox = true; 
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
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out _device, out _swapChain);
            _context = _device.ImmediateContext;

           

            var factory = _swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(_renderForm.Handle, WindowAssociationFlags.IgnoreAll);
            var backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);
            _renderView = new RenderTargetView(_device, backBuffer);



            _depthBuffer = new Texture2D(_device, new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = _renderForm.ClientSize.Width,
                Height = _renderForm.ClientSize.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            _depthView = new DepthStencilView(_device, _depthBuffer);
            _sampler = new SamplerState(_device, new SamplerStateDescription()
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                BorderColor = Color.Black,
                ComparisonFunction = Comparison.Never,
                MaximumAnisotropy = 16,
                MipLodBias = 0,
                MinimumLod = 0,
                MaximumLod = 16,
            });
            _context.OutputMerger.SetTargets(_depthView, _renderView);

        }

        private void OnRenderFormOnResize(object sender, EventArgs args)
        {
            // Resize  depth bufer ?

        }

        public void Present()
        {
            //gehört in Clear von Render Context funktioniert aber dort nicht
            //context.ClearRenderTargetView(renderView, SharpDX.Color.CornflowerBlue);

            //Funktioniert nicht
            DoResize();
            _swapChain.Present(0, PresentFlags.None);
           
        }



        public void Run()
        {
            DoInit();
            // RenderLoop.Run(_renderForm, DoRender);
            RenderLoop.Run(_renderForm, DoRender);
          
        }

        public void Dispose()
        {

            _renderView.Dispose();
            _context.Dispose();
            _device.Dispose();
           _swapChain.Dispose();
            //context.ClearState();
            //context.Flush();
            //device.Dispose();
            //context.Dispose();
            //swapChain.Dispose();

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