using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Fusee.Math;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Color = System.Drawing.Color;
using Device = SharpDX.Direct3D11.Device;

using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

//
namespace Fusee.Engine
{
    public class RenderContextImp : IRenderContextImp
    {

       
        internal Device _device;
        internal SwapChain _swapChain;
        internal DeviceContext _context;
        internal RenderTargetView _renderView;
        //internal VertexShader _vertexShader;
        //internal PixelShader _pixelShader;
        internal InputLayout _layout;
        //internal ShaderReflection _pReflector;
        //internal ShaderReflection _vReflector;
        //internal ShaderDescription _pShaderDescription;
        //internal ShaderDescription _vShaderDescription;
        internal List<SharpDxShaderParamInfo> _sDxShaderParams;
        //internal Dictionary<string, Buffer> _sDxShaderBuffers = new Dictionary<string, Buffer>();
        internal Buffer _buff;
        //internal ShaderResourceView _textureView;
        //internal Buffer _cbUniforms;
        internal SamplerState _sampler;
        internal DepthStencilView _depthView;
        internal float4 _cColor;
        internal float _depth;
        internal bool _layoutCreated;
        //internal List<int> oldPos = new List<int>();
        internal int _pos;
      
        //internal Buffer _vertices;
        //internal Buffer _verticesColor;
        //internal Buffer _trianglesIndex;

       



        public RenderContextImp(IRenderCanvasImp renderCanvas)
        {

            RenderCanvasImp dxCanvas = (RenderCanvasImp)renderCanvas;
            _context = dxCanvas._context;
            _device = dxCanvas._device;
            _swapChain = dxCanvas._swapChain;
            _renderView = dxCanvas._renderView;
            //_textureView = dxCanvas._textureView;
            _sampler = dxCanvas._sampler;
            _depthView = dxCanvas._depthView;
            Viewport(0, 0, dxCanvas.Width, dxCanvas.Height);
            //            _context.Rasterizer.State.Description = new  = CullMode.None;
            var rasterDesc = new RasterizerStateDescription() //CULLMODE!!
            {
                IsAntialiasedLineEnabled = false,
                CullMode = CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = .0f,
                IsDepthClipEnabled = true,
                FillMode = FillMode.Solid,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = false,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0.0f
            };

            // Create the rasterizer state from the description we just filled out.
            RasterizerState rasterState = new RasterizerState(_device, rasterDesc);
            
            // Now set the rasterizer state.
            _context.Rasterizer.State = rasterState;
        }

        /// <summary>
        /// Creates a new Bitmap-Object from an image file,
        /// locks the bits in the memory and makes them available
        /// for furher action (e.g. creating a texture).
        /// Method must be called before creating a texture to get the necessary
        /// ImageData struct.
        /// </summary>
        /// <param name="filename">Path to the image file you would like to use as texture.</param>
        /// <returns>An ImageData object with all necessary information for the texture-binding process.</returns>
        public ImageData LoadImage(String filename)
        {
            Bitmap bmp = new Bitmap(filename);
            //Flip y-axis, otherwise texture would be upside down
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int strideAbs = (bmpData.Stride < 0) ? -bmpData.Stride : bmpData.Stride;
            int bytes = (strideAbs) * bmp.Height;


            ImageData ret = new ImageData()
            {
                PixelData = new byte[bytes],
                Height = bmpData.Height,
                Width = bmpData.Width,
                Stride = bmpData.Stride

            };


            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, ret.PixelData, 0, bytes);

            bmp.UnlockBits(bmpData);
            return ret;
        }

        /// <summary>
        /// Creates a new Image with a specified size and color.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bgColor">The color of the image. Value must be JS compatible.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing.</returns>
        public ImageData CreateImage(int width, int height, String bgColor)
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics gfx = Graphics.FromImage(bmp);
            Color color = Color.FromName(bgColor);
            gfx.Clear(color);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int strideAbs = (bmpData.Stride < 0) ? -bmpData.Stride : bmpData.Stride;
            int bytes = (strideAbs) * bmp.Height;


            ImageData ret = new ImageData()
            {
                PixelData = new byte[bytes],
                Height = bmpData.Height,
                Width = bmpData.Width,
                Stride = bmpData.Stride

            };


            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, ret.PixelData, 0, bytes);

            bmp.UnlockBits(bmpData);
            return ret;
        }

        /// <summary>
        /// Maps a specified text with on an image.
        /// </summary>
        /// <param name="imgData">The ImageData struct with the PixelData from the image.</param>
        /// <param name="fontName">The name of the text-font.</param>
        /// <param name="fontSize">The size of the text-font.</param>
        /// <param name="text">The text that sould be mapped on the iamge.</param>
        /// <param name="textColor">The color of the text-font.</param>
        /// <param name="startPosX">The horizontal start-position of the text on the image.</param>
        /// <param name="startPosY">The vertical start-position of the text on the image.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing</returns>
        public ImageData TextOnImage(ImageData imgData, String fontName, float fontSize, String text, String textColor, float startPosX, float startPosY)
        {

            GCHandle arrayHandle = GCHandle.Alloc(imgData.PixelData,
                                   GCHandleType.Pinned);
            Bitmap bmp = new Bitmap(imgData.Width, imgData.Height, imgData.Stride, PixelFormat.Format32bppArgb,
                                    arrayHandle.AddrOfPinnedObject());
            Color color = Color.FromName(textColor);
            Font font = new Font(fontName, fontSize, FontStyle.Regular, GraphicsUnit.World);


            Graphics gfx = Graphics.FromImage(bmp);
            gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
            gfx.DrawString(text, font, new SolidBrush(color), startPosX, startPosY);

            //Flip y-axis, otherwise texture would be upside down
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int strideAbs = (bmpData.Stride < 0) ? -bmpData.Stride : bmpData.Stride;
            int bytes = (strideAbs) * bmp.Height;

            imgData.PixelData = new byte[bytes];
            imgData.Height = bmpData.Height;
            imgData.Width = bmpData.Width;
            imgData.Stride = bmpData.Stride;

            Marshal.Copy(bmpData.Scan0, imgData.PixelData, 0, bytes);


            bmp.UnlockBits(bmpData);
            return imgData;

        }

        /// <summary>
        /// Creates a new Texture and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITexture that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITexture CreateTexture(ImageData img)
        {
            ITexture texID;
            Texture2DDescription texDesc = new Texture2DDescription();
            texDesc.Height = img.Height;
            texDesc.Width = img.Width;
            texDesc.SampleDescription = new SampleDescription(1, 0);
            texDesc.Format = Format.B8G8R8X8_UNorm;
            texDesc.ArraySize = 1;
            texDesc.MipLevels = 1;
            texDesc.OptionFlags = ResourceOptionFlags.None;
            texDesc.BindFlags = BindFlags.ShaderResource;
            texDesc.Usage = ResourceUsage.Default;


            //var texture = Texture2D.FromFile<Texture2D>(_device, "D:/Thesis/FuseeDx-DevDx/FuseeDx-DevDx/Fusee/DreieckDx11/Assets/GeneticaMortarlessBlocks.jpg");
            unsafe
            {


                fixed (byte* pMemory = img.PixelData)
                {


                    DataRectangle imgDataRec = new DataRectangle((IntPtr)pMemory, img.Stride);
                    var tex = new Texture2D(_device, texDesc, imgDataRec);
                    //var tex = Texture2D.FromMemory<Texture2D>(_device, img.PixelData);

                    //var textview = new ShaderResourceView(_device, tex);
                    ShaderResourceView textureView;
                    textureView = new ShaderResourceView(_device, tex);
                    texID = new Texture { handle = textureView };
                }
            }
            return texID;

        }


        public IList<ShaderParamInfo> GetShaderParamList(IShaderProgramImp shaderProgram)
        {
            throw new NotImplementedException();
        }

        public IShaderParam GetShaderParam(IShaderProgramImp shaderProgram, string paramName)
        {

            for (int i = 0; i < ((ShaderProgramImp)shaderProgram)._sDXShaderParams.Count; i++)
            {
                if (paramName == ((ShaderProgramImp)shaderProgram)._sDXShaderParams[i]._varName)
                {
                    if (((ShaderProgramImp)shaderProgram)._sDXShaderParams[i]._ps != null)
                    {
                        return new ShaderParam { position = i, shaderType = ShaderType.PixelShader, name = paramName, size = ((ShaderProgramImp)shaderProgram)._sDXShaderParams[i]._varSize, _bufferParams = ((ShaderProgramImp)shaderProgram)._sDXShaderParams[i]._bufferParams, _sdxBuffer = ((ShaderProgramImp)shaderProgram)._sDXShaderParams[i]._sdxBuffer, _varPositionB = ((ShaderProgramImp)shaderProgram)._sDXShaderParams[i]._varPositionB};
                    }
                    else
                    {
                        return new ShaderParam { position = i, shaderType = ShaderType.VertexShader, name = paramName, size = ((ShaderProgramImp)shaderProgram)._sDXShaderParams[i]._varSize, _bufferParams = ((ShaderProgramImp)shaderProgram)._sDXShaderParams[i]._bufferParams, _sdxBuffer = ((ShaderProgramImp)shaderProgram)._sDXShaderParams[i]._sdxBuffer, _varPositionB = ((ShaderProgramImp)shaderProgram)._sDXShaderParams[i]._varPositionB };
                    }
                }

            }

            return null;
        }

        public float GetParamValue(IShaderProgramImp program, IShaderParam handle)
        {


            return 10.0f;
        }

        //public IEnumerable<ShaderParamInfo> GetShaderParamList(IShaderProgramImp shaderProgram)
        //{

        //}



        public void SetShaderParam(IShaderParam param, float val)
        {
            /*
             * Ist das hier
             *  var cb = new Variables();
             *  cb.R = 1.0f;
             *  cb.G = 0.0f;
             *  cb.B = 0.0f;
             *  cb.A = 1.0f;
             */
            //var shaderParamInfo = _sDxShaderParams[((ShaderParam)param).position];
            if (((ShaderParam)param).size == 4)
            {
                ((ShaderParam)param)._bufferParams.Position = ((ShaderParam)param)._varPositionB;
                ((ShaderParam)param)._bufferParams.Write(val);
                ((ShaderParam)param)._bufferParams.Position = 0;
                _context.UpdateSubresource(new DataBox(((ShaderParam)param)._bufferParams.PositionPointer, 0, 0),
                                              ((ShaderParam)param)._sdxBuffer, 0);
                if (((ShaderParam)param).shaderType == ShaderType.PixelShader)
                {
                    _context.PixelShader.SetConstantBuffer(0, ((ShaderParam)param)._sdxBuffer);
                }
                else
                {
                    _context.VertexShader.SetConstantBuffer(0, ((ShaderParam)param)._sdxBuffer);
                }
            }


        }

        public void SetShaderParam(IShaderParam param, float2 val)
        {
            //var shaderParamInfo = _sDxShaderParams[((ShaderParam)param).position];
            if (((ShaderParam)param).size == 8)
            {
                ((ShaderParam)param)._bufferParams.Position = ((ShaderParam)param)._varPositionB;
                ((ShaderParam)param)._bufferParams.Write(val);
                ((ShaderParam)param)._bufferParams.Position = 0;
                _context.UpdateSubresource(new DataBox(((ShaderParam)param)._bufferParams.PositionPointer, 0, 0),
                                              ((ShaderParam)param)._sdxBuffer, 0);
                if (((ShaderParam)param).shaderType == ShaderType.PixelShader)
                {
                    _context.PixelShader.SetConstantBuffer(0, ((ShaderParam)param)._sdxBuffer);
                }
                else
                {
                    _context.VertexShader.SetConstantBuffer(0, ((ShaderParam)param)._sdxBuffer);
                }
            }
        }

        public void SetShaderParam(IShaderParam param, float3 val)
        {
            //var shaderParamInfo = _sDxShaderParams[((ShaderParam)param).position];
            if (((ShaderParam)param).size == 12)
            {
                ((ShaderParam)param)._bufferParams.Position = ((ShaderParam)param)._varPositionB;
                ((ShaderParam)param)._bufferParams.Write(val);
                ((ShaderParam)param)._bufferParams.Position = 0;
                _context.UpdateSubresource(new DataBox(((ShaderParam)param)._bufferParams.PositionPointer, 0, 0),
                                              ((ShaderParam)param)._sdxBuffer, 0);
                if (((ShaderParam)param).shaderType == ShaderType.PixelShader)
                {
                    _context.PixelShader.SetConstantBuffer(0, ((ShaderParam)param)._sdxBuffer);
                }
                else
                {
                    _context.VertexShader.SetConstantBuffer(0, ((ShaderParam)param)._sdxBuffer);
                }
            }
        }

        public void SetShaderParam(IShaderParam param, float4 val)
        {
            //data.Position = ((ShaderParam).param).position;
            //var shaderParamInfo = _sDxShaderParams[((ShaderParam)param).position];
            //if (shaderParamInfo._varSize == 16)
            //{
            //    shaderParamInfo._bufferParams.Position = shaderParamInfo._varPositionB;
            //    shaderParamInfo._bufferParams.Write(val);
            //    shaderParamInfo._bufferParams.Position = 0;
            //    _context.UpdateSubresource(new DataBox(shaderParamInfo._bufferParams.PositionPointer, 0, 0),
            //                                  shaderParamInfo._sdxBuffer, 0);
            //    if (((ShaderParam)param).shaderType == ShaderType.PixelShader)
            //    {
            //        _context.PixelShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
            //    }
            //    else
            //    {
            //        _context.VertexShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
            //    }
            //}

            
            if (((ShaderParam)param).size == 16)
            {
               ((ShaderParam) param)._bufferParams.Position = ((ShaderParam) param)._varPositionB;
               ((ShaderParam)param)._bufferParams.Write(val);
                ((ShaderParam) param)._bufferParams.Position = 0;

                _context.UpdateSubresource(new DataBox(((ShaderParam)param)._bufferParams.PositionPointer, 0, 0),
                                              ((ShaderParam)param)._sdxBuffer, 0);
                if (((ShaderParam)param).shaderType == ShaderType.PixelShader)
                {
                    _context.PixelShader.SetConstantBuffer(0, ((ShaderParam)param)._sdxBuffer);
                }
                else
                {
                    _context.VertexShader.SetConstantBuffer(0, ((ShaderParam)param)._sdxBuffer);
                }
            }

        }

        // TODO add vector implementations

        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
            //Transponieren der Matrix damit diese in DirectX richtig dargestellt wird und sie nicht über den HLSL transponiert werden muss.
            val.Transpose();
            unsafe
            {
                float* mF = (float*)(&val);
                //var shaderParamInfo = _sDxShaderParams[((ShaderParam)param).position];
                if (((ShaderParam)param).size == 64)
                {
                    ((ShaderParam)param)._bufferParams.Position = ((ShaderParam)param)._varPositionB;
                    ((ShaderParam)param)._bufferParams.Write((IntPtr)mF, 0, 64);
                    ((ShaderParam)param)._bufferParams.Position = 0;
                    _context.UpdateSubresource(new DataBox(((ShaderParam)param)._bufferParams.PositionPointer, 0, 0),
                                               ((ShaderParam)param)._sdxBuffer, 0);
                    if (((ShaderParam)param).shaderType == ShaderType.PixelShader)
                    {
                        _context.PixelShader.SetConstantBuffer(0, ((ShaderParam)param)._sdxBuffer);

                    }
                    else
                    {
                        _context.VertexShader.SetConstantBuffer(0, ((ShaderParam)param)._sdxBuffer);

                    }
                }
            }
        }

        public void SetShaderParam(IShaderParam param, int val)
        {
            //var shaderParamInfo = _sDxShaderParams[((ShaderParam)param).position];
            if (((ShaderParam)param).size == 4)
            {
                ((ShaderParam)param)._bufferParams.Position = ((ShaderParam)param)._varPositionB;
                ((ShaderParam)param)._bufferParams.Write(val);
                ((ShaderParam)param)._bufferParams.Position = 0;
                _context.UpdateSubresource(new DataBox(((ShaderParam)param)._bufferParams.PositionPointer, 0, 0),
                                              ((ShaderParam)param)._sdxBuffer, 0);
                if (((ShaderParam)param).shaderType == ShaderType.PixelShader)
                {
                    _context.PixelShader.SetConstantBuffer(0, ((ShaderParam)param)._sdxBuffer);
                }
                else
                {
                    _context.VertexShader.SetConstantBuffer(0, ((ShaderParam)param)._sdxBuffer);
                }
            }
        }

        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texId">An ITexture probably returned from CreateTexture method</param>
        public void SetShaderParamTexture(IShaderParam param, ITexture texId)
        {
           
            _context.PixelShader.SetSampler(0, _sampler);
            _context.PixelShader.SetShaderResource(0, ((Texture)texId).handle);

        }

        public float4x4 ModelView
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        public float4x4 Projection
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        public float4 ClearColor
        {
            get
            {
                return _cColor;
            }
            set { _cColor = value; }
        }

        public float ClearDepth
        {
            get { return _depth; }
            set { _depth = value; }
        }


        private void createLayout(ShaderBytecode vertexShaderByteCode)
        {
            _layout = new InputLayout(
               _device,
               ShaderSignature.GetInputSignature(vertexShaderByteCode),
               new[]
                        {
                            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                            new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 0, 1),
                            new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 0, 2),
                            new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, 0, 3)
                        });
            _layoutCreated = true;
           
        }

        public IShaderProgramImp CreateShader(string vs, string ps)
        {
           

            var vertexShaderByteCode = ShaderBytecode.Compile(vs, "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None);
            VertexShader vertexShader = new VertexShader(_device, vertexShaderByteCode);

            
            
            var pixelShaderByteCode = ShaderBytecode.Compile(ps, "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);
            PixelShader pixelShader = new PixelShader(_device, pixelShaderByteCode);
            



            ShaderProgramImp shaderProgram = new ShaderProgramImp();
            shaderProgram._ps = pixelShader;
            shaderProgram._vs = vertexShader;
            _sDxShaderParams = new List<SharpDxShaderParamInfo>();

            shaderProgram._sDXShaderParams = new List<SharpDxShaderParamInfo>();

            //if (oldPos.Count==0)
            //{
            //    oldPos.Add(_pos);
            //    shaderProgram.pos = _pos;
            //}
            //else
            //{
            //    _pos = oldPos[oldPos.Count - 1] +1;
            //    shaderProgram.pos = _pos;
            //};


            //_layout = new InputLayout(
            //        _device,
            //        ShaderSignature.GetInputSignature(vertexShaderByteCode),
            //        new[]
            //        {
            //            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
            //            new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 0, 1)
            //        });

           
           //_sDxShaderBuffers = new Dictionary<string, Buffer>();

            ShaderReflection pReflector = new ShaderReflection(pixelShaderByteCode);
            ShaderReflection vReflector = new ShaderReflection(vertexShaderByteCode);
            ShaderDescription pShaderDescription = pReflector.Description;
            ShaderDescription vShaderDescription = vReflector.Description;

            /*
             * 
             * Get SamplerState Texture2D Names (PixelShader)
             * 
             */
            int ResourceCountP = pReflector.Description.BoundResources;
            InputBindingDescription descPT;
            for (int i = 0; i < ResourceCountP; i++)
            {
                descPT = pReflector.GetResourceBindingDescription(i);
                SharpDxShaderParamInfo psparams = new SharpDxShaderParamInfo();
                psparams._varName = descPT.Name;
                _sDxShaderParams.Add(psparams);
                shaderProgram._sDXShaderParams.Add(psparams);
            }
            /*
             * 
             * Get SamplerState Texture2D Names (VertexShader)
             * 
             */
            //int ResourceCountV = _vReflector.Description.BoundResources;
            //InputBindingDescription descVT;
            //for (int i = 0; i < ResourceCountV; i++)
            //{
            //    descVT = _pReflector.GetResourceBindingDescription(i);


            //}

            //bool color = true;
            //bool norms = false;
            //for (int i = 0; i < pShaderDescription.InputParameters; i++)
            //{
            //    ShaderParameterDescription paramDesc = pReflector.GetInputParameterDescription(i);
            //    if (paramDesc.SemanticName == "COLOR")
            //    {
            //        color = true;
            //    }
            //    if (paramDesc.SemanticName == "TEXCOORD")
            //    {
            //        color = false;
            //    }
            //    if (paramDesc.SemanticName == "NORMAL")
            //    {
            //        norms = true;
            //    }
            //    else
            //    {
            //        norms = false;
            //    }
            //}
            if (_layoutCreated != true)
            {
                
                createLayout(vertexShaderByteCode);
            }
          
            //if (color && norms == false)
            //{
            //    _layout = new InputLayout(
            //            _device,
            //            ShaderSignature.GetInputSignature(vertexShaderByteCode),
            //            new[]
            //            {
            //                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
            //                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 0, 1)
            //            });
            //}
            //else if (color && norms)
            //{
            //_layout = new InputLayout(
            //       _device,
            //       ShaderSignature.GetInputSignature(vertexShaderByteCode),
            //       new[]
            //            {
            //                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
            //                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 0, 1),
            //                new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 0, 2),
            //                new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, 0, 3)
            //            });
            //}
            //else if (norms && color == false)
            //{
            //    _layout = new InputLayout(
            //            _device,
            //            ShaderSignature.GetInputSignature(vertexShaderByteCode),
            //            new[]
            //            {
            //                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
            //                new InputElement("TEXCOORD", 0, Format.R32G32B32_Float, 0, 1),
            //                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 0, 2)
            //            });
            //}
            //else
            //{

            //    _layout = new InputLayout(
            //            _device,
            //            ShaderSignature.GetInputSignature(vertexShaderByteCode),
            //            new[]
            //            {
            //                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
            //                new InputElement("TEXCOORD", 0, Format.R32G32B32_Float, 0, 1),
            //                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 0, 2)
            //            });
            //}
            /*--------------------------------------------------------------------------------------------------
             * 
             * PixelShader-ConstantBuffers
             * 
             * -------------------------------------------------------------------------------------------------
             */
            
            for (int i = 0; i < pShaderDescription.ConstantBuffers; ++i)
            {

                ConstantBuffer cbTemp = pReflector.GetConstantBuffer(i);
                ConstantBufferDescription cbDEsc = cbTemp.Description;
                //if (_sDxShaderBuffers.ContainsKey(cbDEsc.Name))
                //{
                //    _sDxShaderBuffers.TryGetValue(cbDEsc.Name, out _buff);
                //    _sDxShaderBuffers.Add(cbDEsc.Name, _buff);

                    

                //    SharpDxShaderParamInfo psparams = new SharpDxShaderParamInfo();
                //    for (int j = 0; j < cbDEsc.VariableCount; j++)
                //    {
                //        ShaderReflectionVariable shaderRefVar = cbTemp.GetVariable(j);
                //        ShaderReflectionType shaderRefType = cbTemp.GetVariable(j).GetVariableType();
                //        ShaderTypeDescription shaderRefTypeDesc = shaderRefType.Description;
                //        psparams._ps = pixelShader;
                //        psparams._psByteCode = pixelShaderByteCode;
                //        psparams._buffername = cbDEsc.Name;

                //        psparams._sdxBuffer = _buff;
                //        psparams._flags = shaderRefVar.Description.Flags;
                //        psparams._varCount = cbDEsc.VariableCount;
                //        psparams._varName = shaderRefVar.Description.Name;

                //        psparams._varSize = shaderRefVar.Description.Size;
                //        psparams._varPositionB = shaderRefVar.Description.StartOffset;
                //        psparams._flags = shaderRefVar.Description.Flags;
                //        psparams._varPositionI = j;
                //        psparams._varType = shaderRefType.Description.Type;
                //        psparams._bufferParams.Write(shaderRefVar.Description.DefaultValue);
                //        _sDxShaderParams.Add(psparams);
                        
                //    }
                //}
                //else
                //{
                    //cbDEsc.Name Liefert den Namen der Struktur
                    var bufferSize = cbDEsc.Size;
                    var buffer = new Buffer(_device, new BufferDescription
                    {
                        Usage = ResourceUsage.Default,
                        SizeInBytes = bufferSize,
                        BindFlags = BindFlags.ConstantBuffer,
                        CpuAccessFlags = 0
                    });

                    

                   

                    SharpDxShaderParamInfo psparams = new SharpDxShaderParamInfo();
                    psparams._sdxBuffer = buffer;
                    psparams._bufferParams = new DataStream(cbDEsc.Size, true, true);
                    for (int j = 0; j < cbDEsc.VariableCount; j++)
                    {
                        ShaderReflectionVariable shaderRefVar = cbTemp.GetVariable(j);
                        ShaderReflectionType shaderRefType = cbTemp.GetVariable(j).GetVariableType();
                        ShaderTypeDescription shaderRefTypeDesc = shaderRefType.Description;
                        psparams._ps = pixelShader;
                        psparams._psByteCode = pixelShaderByteCode;
                        psparams._buffername = cbDEsc.Name;
                        psparams._sdxBuffer = buffer;
                        psparams._flags = shaderRefVar.Description.Flags;
                        psparams._varCount = cbDEsc.VariableCount;
                        psparams._varName = shaderRefVar.Description.Name;

                        psparams._varSize = shaderRefVar.Description.Size;
                        psparams._varPositionB = shaderRefVar.Description.StartOffset;
                        psparams._flags = shaderRefVar.Description.Flags;
                        psparams._varPositionI = j;
                        psparams._varType = shaderRefType.Description.Type;
                        psparams._bufferParams.Write(shaderRefVar.Description.DefaultValue);
                        _sDxShaderParams.Add(psparams);
                        shaderProgram._sDXShaderParams.Add(psparams);
                    }
               //}
            }


            /*--------------------------------------------------------------------------------------------------
             * 
             * VertexShader-ConstantBuffers
             * 
             * -------------------------------------------------------------------------------------------------
             */
            
            for (int i = 0; i < vShaderDescription.ConstantBuffers; ++i)
            {

                ConstantBuffer cbTemp = vReflector.GetConstantBuffer(i);
                ConstantBufferDescription cbDEsc = cbTemp.Description;
                //if (_sDxShaderBuffers.ContainsKey(cbDEsc.Name))
                //{
                //    _sDxShaderBuffers.TryGetValue(cbDEsc.Name, out _buff);
                //    _sDxShaderBuffers.Add(cbDEsc.Name, _buff);

                    

                //    SharpDxShaderParamInfo vsparams = new SharpDxShaderParamInfo();
                //    for (int j = 0; j < cbDEsc.VariableCount; j++)
                //    {
                //        ShaderReflectionVariable shaderRefVar = cbTemp.GetVariable(j);
                //        ShaderReflectionType shaderRefType = cbTemp.GetVariable(j).GetVariableType();
                //        ShaderTypeDescription shaderRefTypeDesc = shaderRefType.Description;
                //        vsparams._vs = vertexShader;
                //        vsparams._vsByteCode = vertexShaderByteCode;
                //        vsparams._buffername = cbDEsc.Name;
                //        vsparams._sdxBuffer = _buff;
                //        vsparams._flags = shaderRefVar.Description.Flags;
                //        vsparams._varCount = cbDEsc.VariableCount;
                //        vsparams._varName = shaderRefVar.Description.Name;

                //        vsparams._varSize = shaderRefVar.Description.Size;

                //        vsparams._varPositionB = shaderRefVar.Description.StartOffset;
                //        vsparams._flags = shaderRefVar.Description.Flags;
                //        vsparams._varPositionI = j;
                //        vsparams._varType = shaderRefType.Description.Type;

                //        vsparams._bufferParams.Write(shaderRefVar.Description.DefaultValue);
                //        _sDxShaderParams.Add(vsparams);

                //    }
                //}
                //else
                //{
                    //cbDEsc.Name Liefert den Namen der Struktur
                    var bufferSize = cbDEsc.Size;
                    var buffer = new Buffer(_device, new BufferDescription
                    {
                        Usage = ResourceUsage.Default,
                        SizeInBytes = bufferSize,
                        BindFlags = BindFlags.ConstantBuffer,
                        CpuAccessFlags = 0
                    });

                    SharpDxShaderParamInfo vsparams = new SharpDxShaderParamInfo();
                    vsparams._sdxBuffer = buffer;
                    vsparams._bufferParams = new DataStream(cbDEsc.Size, true, true);
                    for (int j = 0; j < cbDEsc.VariableCount; j++)
                    {
                        ShaderReflectionVariable shaderRefVar = cbTemp.GetVariable(j);
                        ShaderReflectionType shaderRefType = cbTemp.GetVariable(j).GetVariableType();
                        ShaderTypeDescription shaderRefTypeDesc = shaderRefType.Description;
                        vsparams._vs = vertexShader;
                        vsparams._vsByteCode = vertexShaderByteCode;
                        vsparams._buffername = cbDEsc.Name;
                        vsparams._sdxBuffer = buffer;
                        vsparams._flags = shaderRefVar.Description.Flags;
                        vsparams._varCount = cbDEsc.VariableCount;
                        vsparams._varName = shaderRefVar.Description.Name;

                        vsparams._varSize = shaderRefVar.Description.Size;
                        vsparams._varPositionB = shaderRefVar.Description.StartOffset;

                        vsparams._flags = shaderRefVar.Description.Flags;
                        vsparams._varPositionI = j;
                        vsparams._varType = shaderRefType.Description.Type;
                        vsparams._bufferParams.Write(shaderRefVar.Description.DefaultValue);

                        _sDxShaderParams.Add(vsparams);
                        shaderProgram._sDXShaderParams.Add(vsparams);
                    }
                //}
            }
            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();



            return shaderProgram;
        }


        public void SetShader(IShaderProgramImp program)
        {


          
           //int posi =  ((ShaderProgramImp)program).pos;
            _context.PixelShader.Set(((ShaderProgramImp)program)._ps);
            _context.VertexShader.Set(((ShaderProgramImp)program)._vs);
           
            //_context.VertexShader.Set(_vertexShader);
            //_context.PixelShader.Set(_pixelShader);
        }

        public void Clear(ClearFlags flags)
        {
            //hat keinen Effekt

            if ((flags & ClearFlags.Color) != (ClearFlags)0)
            {
                Color4 clearColor = new Color4();
                clearColor.Red = _cColor.x;
                clearColor.Green = _cColor.y;
                clearColor.Blue = _cColor.z;
                clearColor.Alpha = _cColor.w;
                _context.ClearRenderTargetView(_renderView, clearColor);
            }
            if ((flags & ClearFlags.Depth) != (ClearFlags)0)
            {
                _context.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth, _depth, 0);//tiefenwert durch benutzer ändern
            }


        }


        public void SetVertices(IMeshImp mr, float3[] vertices)
        {
            if (vertices == null || vertices.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            ((MeshImp)mr).VertexBufferObject = Buffer.Create(_device, BindFlags.VertexBuffer, vertices);

        }


        public void SetNormals(IMeshImp mr, float3[] normals)
        {
            if (normals == null || normals.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            ((MeshImp)mr).NormalBufferObject = Buffer.Create(_device, BindFlags.VertexBuffer, normals);
        }

        public void SetUVs(IMeshImp mr, float2[] uvs)
        {
            if (uvs == null || uvs.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            ((MeshImp)mr).UVBufferObject = Buffer.Create(_device, BindFlags.VertexBuffer, uvs);
        }

        public void SetColors(IMeshImp mr, float4[] colors)
        {


            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            ((MeshImp)mr).ColorBufferObject = Buffer.Create(_device, BindFlags.VertexBuffer, colors);

        }


        public void SetTriangles(IMeshImp mr, short[] triangleIndices)
        {
            if (triangleIndices == null || triangleIndices.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            ((MeshImp)mr).ElementBufferObject = Buffer.Create(_device, BindFlags.IndexBuffer, triangleIndices);
            ((MeshImp)mr).NElements = triangleIndices.Length;

        }

        public void Render(IMeshImp mr)
        {
          
            if (((MeshImp)mr).VertexBufferBindingObject == null)
            {
                List<VertexBufferBinding> binding = new List<VertexBufferBinding>(4);
                if (((MeshImp)mr).VertexBufferObject != null)
                {
                    binding.Add(new VertexBufferBinding(((MeshImp)mr).VertexBufferObject, 12, 0));
                }

                if (((MeshImp)mr).ColorBufferObject != null)
                {
                    binding.Add(new VertexBufferBinding(((MeshImp)mr).ColorBufferObject, 16, 0));
                }

                if (((MeshImp)mr).UVBufferObject != null)
                {
                    binding.Add(new VertexBufferBinding(((MeshImp)mr).UVBufferObject, 8, 0));
                }

                if (((MeshImp)mr).NormalBufferObject != null)
                {

                    binding.Add(new VertexBufferBinding(((MeshImp)mr).NormalBufferObject, 12, 0));
                }
                ((MeshImp)mr).VertexBufferBindingObject = binding.ToArray();

            }
            _context.InputAssembler.SetVertexBuffers(0, ((MeshImp)mr).VertexBufferBindingObject);


            if (((MeshImp)mr).ElementBufferObject != null)
            {

                _context.InputAssembler.InputLayout = _layout;
                _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

                _context.InputAssembler.SetIndexBuffer(((MeshImp)mr).ElementBufferObject, Format.R16_UInt, 0);



                _context.DrawIndexed(((MeshImp)mr).NElements, 0, 0);
               
            }       
        }
        /*
         * 
         * 
         * 
         */
        //NEU
        public void DebugLine(float3 start, float3 end, float4 color)
        {

        }


        public IMeshImp CreateMeshImp()
        {
            return new MeshImp();
        }

        public void Viewport(int x, int y, int width, int height)
        {
               
            _context.Rasterizer.SetViewport(x,y,width,height,0.0f,1.0f);
        }

        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {

        }


    }
}