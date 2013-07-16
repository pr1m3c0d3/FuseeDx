using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
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

        private int _currentTextureUnit;
        private Dictionary<int, int> _shaderParam2TexUnit;
        internal Device _device;
        internal SwapChain _swapChain;
        internal DeviceContext _context;
        internal RenderTargetView _renderView;
        internal VertexShader _vertexShader;
        internal PixelShader _pixelShader;
        internal InputLayout _layout;
        internal ShaderReflection _pReflector;
        internal ShaderReflection _vReflector;
        internal ShaderDescription _pShaderDescription;
        internal ShaderDescription _vShaderDescription;
        internal List<SharpDxShaderParamInfo> _sDxShaderParams = new List<SharpDxShaderParamInfo>();
        internal Dictionary<string, Buffer> _sDxShaderBuffers = new Dictionary<string, Buffer>();
        internal Buffer _buff;
        internal ShaderResourceView _textureView;
        internal Buffer _cbUniforms;
        internal SamplerState _sampler;
        internal DepthStencilView _depthView;
        internal float4 _cColor;
        internal float _depth;
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
            _textureView = dxCanvas._textureView;
            _sampler = dxCanvas._sampler;
            _depthView = dxCanvas._depthView;
            Viewport(0, 0, dxCanvas.Width, dxCanvas.Height);

            // CreateShader("", "");



            // // Instantiate Vertex buiffer from vertex data
            // var vertices = Buffer.Create(_device, BindFlags.VertexBuffer, new[]
            //                       {
            //                           new Vector4(-0.9f, -0.2f, 0.0f, 1.0f),
            //                           new Vector4(0.0f, 0.5f, 0.5f, 1.0f),
            //                           new Vector4(0.5f, -0.5f, 0.5f, 1.0f), 
            //                           new Vector4(-0.5f, -0.5f, 0.5f, 1.0f)

            //                       });
            // // Instantiate Vertex buiffer from vertex data
            // var verticesColors = Buffer.Create(_device, BindFlags.VertexBuffer, new[]
            //                       {
            //                           new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            //                           new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
            //                           new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
            //                           new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
            //                       });
            // var triangles = Buffer.Create(_device, BindFlags.IndexBuffer, new[]
            //                       {
            //                           1,
            //                           2,
            //                           3
            //                       });


            // // Prepare All the stages
            // _context.InputAssembler.InputLayout = _layout;
            // _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            // _context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding[]
            // {
            //   new VertexBufferBinding(vertices,16, 0),
            //   new VertexBufferBinding(verticesColors,16, 0)
            // }

            // );
            // _context.InputAssembler.SetIndexBuffer(triangles, Format.R32_UInt, 0);
            // _context.VertexShader.Set(_vertexShader);
            //Viewport(0,0,dxCanvas.Width,dxCanvas.Height);
            // _context.PixelShader.Set(_pixelShader);
            // _context.OutputMerger.SetTargets(_renderView);

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
        public  ITexture CreateTexture(ImageData img)
        {
            ITexture texID;
            Texture2DDescription texDesc = new Texture2DDescription();
            texDesc.Height = img.Height;
            texDesc.Width = img.Width;
            texDesc.SampleDescription = new SampleDescription(1,0);
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
                
            
                DataRectangle imgDataRec = new DataRectangle((IntPtr) pMemory,img.Stride);
                var tex = new Texture2D(_device,texDesc,imgDataRec);
                //var tex = Texture2D.FromMemory<Texture2D>(_device, img.PixelData);
            
                //var textview = new ShaderResourceView(_device, tex);
                _textureView = new ShaderResourceView(_device, tex);
                texID  = new Texture { handle = 0 };
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

            for (int i = 0; i < _sDxShaderParams.Count; i++)
            {
                if (paramName == _sDxShaderParams[i]._varName)
                {
                    if (_sDxShaderParams[i]._ps != null)
                    {
                        return new ShaderParam {position = i, shaderType = ShaderType.PixelShader, name = paramName};
                    }
                    else
                    {
                        return new ShaderParam { position = i, shaderType = ShaderType.VertexShader, name = paramName };
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
            var shaderParamInfo = _sDxShaderParams[((ShaderParam)param).position];
            if (shaderParamInfo._varSize == 4)
            {
                shaderParamInfo._bufferParams.Position = shaderParamInfo._varPositionB;
                shaderParamInfo._bufferParams.Write(val);
                shaderParamInfo._bufferParams.Position = 0;
                _context.UpdateSubresource(new DataBox(shaderParamInfo._bufferParams.PositionPointer, 0, 0),
                                              shaderParamInfo._sdxBuffer, 0);
                if (((ShaderParam)param).shaderType == ShaderType.PixelShader)
                {
                    _context.PixelShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
                }
                else
                {
                    _context.VertexShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
                }
            }


        }

        public void SetShaderParam(IShaderParam param, float2 val)
        {
            var shaderParamInfo = _sDxShaderParams[((ShaderParam)param).position];
            if (shaderParamInfo._varSize == 8)
            {
                shaderParamInfo._bufferParams.Position = shaderParamInfo._varPositionB;
                shaderParamInfo._bufferParams.Write(val);
                shaderParamInfo._bufferParams.Position = 0;
                _context.UpdateSubresource(new DataBox(shaderParamInfo._bufferParams.PositionPointer, 0, 0),
                                              shaderParamInfo._sdxBuffer, 0);
                if (((ShaderParam)param).shaderType == ShaderType.PixelShader)
                {
                    _context.PixelShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
                }
                else
                {
                    _context.VertexShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
                }
            }
        }

        public void SetShaderParam(IShaderParam param, float3 val)
        {
            var shaderParamInfo = _sDxShaderParams[((ShaderParam)param).position];
            if (shaderParamInfo._varSize == 12)
            {
                shaderParamInfo._bufferParams.Position = shaderParamInfo._varPositionB;
                shaderParamInfo._bufferParams.Write(val);
                shaderParamInfo._bufferParams.Position = 0;
                _context.UpdateSubresource(new DataBox(shaderParamInfo._bufferParams.PositionPointer, 0, 0),
                                              shaderParamInfo._sdxBuffer, 0);
                if (((ShaderParam)param).shaderType == ShaderType.PixelShader)
                {
                    _context.PixelShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
                }
                else
                {
                    _context.VertexShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
                }
            }
        }

        public void SetShaderParam(IShaderParam param, float4 val)
        {
            //data.Position = ((ShaderParam).param).position;
            var shaderParamInfo = _sDxShaderParams[((ShaderParam)param).position];
            if (shaderParamInfo._varSize == 16)
            {
                shaderParamInfo._bufferParams.Position = shaderParamInfo._varPositionB;
                shaderParamInfo._bufferParams.Write(val);
                shaderParamInfo._bufferParams.Position = 0;
                _context.UpdateSubresource(new DataBox(shaderParamInfo._bufferParams.PositionPointer, 0, 0),
                                              shaderParamInfo._sdxBuffer, 0);
                if (((ShaderParam)param).shaderType == ShaderType.PixelShader)
                {
                    _context.PixelShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
                }
                else
                {
                    _context.VertexShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
                }
            }
        }

        // TODO add vector implementations

        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
            unsafe
            {
                float mF = *(float*) (&val);
                var shaderParamInfo = _sDxShaderParams[((ShaderParam) param).position];
                if (shaderParamInfo._varSize == 64)
                {
                    shaderParamInfo._bufferParams.Position = shaderParamInfo._varPositionB;
                    shaderParamInfo._bufferParams.Write( mF);
                    shaderParamInfo._bufferParams.Position = 0;
                    _context.UpdateSubresource(new DataBox(shaderParamInfo._bufferParams.PositionPointer, 0, 0),
                                               shaderParamInfo._sdxBuffer, 0);
                    if (((ShaderParam) param).shaderType == ShaderType.PixelShader)
                    {
                        _context.PixelShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
                       
                    }
                    else
                    {
                        _context.VertexShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
                       
                    }
                }
            }
        }

        public void SetShaderParam(IShaderParam param, int val)
        {
            var shaderParamInfo = _sDxShaderParams[((ShaderParam)param).position];
            if (shaderParamInfo._varSize == 4)
            {
                shaderParamInfo._bufferParams.Position = shaderParamInfo._varPositionB;
                shaderParamInfo._bufferParams.Write(val);
                shaderParamInfo._bufferParams.Position = 0;
                _context.UpdateSubresource(new DataBox(shaderParamInfo._bufferParams.PositionPointer, 0, 0),
                                              shaderParamInfo._sdxBuffer, 0);
                if (((ShaderParam)param).shaderType == ShaderType.PixelShader)
                {
                    _context.PixelShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
                }
                else
                {
                    _context.VertexShader.SetConstantBuffer(0, shaderParamInfo._sdxBuffer);
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
            _cbUniforms = new Buffer(_device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default,
                                            BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            _context.VertexShader.SetConstantBuffer(0, _cbUniforms);
            _context.PixelShader.SetSampler(0, _sampler);
            _context.PixelShader.SetShaderResource(0, _textureView);
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

        public IShaderProgramImp CreateShader(string vs, string ps)
        {


            var vertexShaderByteCode = ShaderBytecode.Compile(vs, "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None);
            _vertexShader = new VertexShader(_device, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.Compile(ps, "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);
            _pixelShader = new PixelShader(_device, pixelShaderByteCode);

            

            //_layout = new InputLayout(
            //        _device,
            //        ShaderSignature.GetInputSignature(vertexShaderByteCode),
            //        new[]
            //        {
            //            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
            //            new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 0, 1)
            //        });


            _pReflector = new ShaderReflection(pixelShaderByteCode);
            _vReflector = new ShaderReflection(vertexShaderByteCode);
            _pShaderDescription = _pReflector.Description;
            _vShaderDescription = _vReflector.Description;

            /*
             * 
             * Get SamplerState Texture2D Names (PixelShader)
             * 
             */
            int ResourceCountP = _pReflector.Description.BoundResources;
            InputBindingDescription descPT;
            for (int i = 0; i < ResourceCountP; i++)
            {
                descPT = _pReflector.GetResourceBindingDescription(i);
                MessageBox.Show(descPT.Name);
                MessageBox.Show(descPT.Type.ToString());
            }
            /*
             * 
             * Get SamplerState Texture2D Names (VertexShader)
             * 
             */
            int ResourceCountV = _vReflector.Description.BoundResources;
            InputBindingDescription descVT;
            for (int i = 0; i < ResourceCountP; i++)
            {
                descVT = _pReflector.GetResourceBindingDescription(i);
                MessageBox.Show(descVT.Name);
                MessageBox.Show(descVT.Type.ToString());
            }
            
         
            _sDxShaderParams = new List<SharpDxShaderParamInfo>();
            _sDxShaderBuffers = new Dictionary<string, Buffer>();
            bool color = true;
            bool norms = false;
            for (int i = 0; i < _pShaderDescription.InputParameters;i++ )
            {
                ShaderParameterDescription paramDesc = _pReflector.GetInputParameterDescription(i);
                if (paramDesc.SemanticName=="COLOR")
                {
                    color = true;
                }
                if (paramDesc.SemanticName=="TEXCOORD")
                {
                    color = false;
                }
                if (paramDesc.SemanticName == "NORMAL")
                {
                    norms = true;
                }
                else
                {
                    norms = false;
                }
            }
            if (color && norms!=true)
            {
                _layout = new InputLayout(
                        _device,
                        ShaderSignature.GetInputSignature(vertexShaderByteCode),
                        new[]
                        {
                            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                            new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 0, 1)
                        });   
            }else if (color && norms)
            {
                _layout = new InputLayout(
                       _device,
                       ShaderSignature.GetInputSignature(vertexShaderByteCode),
                       new[]
                        {
                            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                            new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 0, 1),
                            new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 0, 2),
                            new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, 0, 2)
                        }); 
            }
            else if (norms && color != true)
            {
                _layout = new InputLayout(
                        _device,
                        ShaderSignature.GetInputSignature(vertexShaderByteCode),
                        new[]
                        {
                            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                            new InputElement("TEXCOORD", 0, Format.R32G32B32_Float, 0, 1),
                            new InputElement("NORMAL", 0, Format.R32G32B32_Float, 0, 2)
                        });
            }
            else
            {
                
                _layout = new InputLayout(
                        _device,
                        ShaderSignature.GetInputSignature(vertexShaderByteCode),
                        new[]
                        {
                            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                            new InputElement("TEXCOORD", 0, Format.R32G32B32_Float, 0, 1)
                        });
            }
           /*--------------------------------------------------------------------------------------------------
            * 
            * PixelShader-ConstantBuffers
            * 
            * -------------------------------------------------------------------------------------------------
            */
            for (int i = 0; i < _pShaderDescription.ConstantBuffers; ++i)
            {

                ConstantBuffer cbTemp = _pReflector.GetConstantBuffer(i);
                ConstantBufferDescription cbDEsc = cbTemp.Description;
                if (_sDxShaderBuffers.ContainsKey(cbDEsc.Name))
                {
                    _sDxShaderBuffers.TryGetValue(cbDEsc.Name, out _buff);
                    _sDxShaderBuffers.Add(cbDEsc.Name, _buff);
                    SharpDxShaderParamInfo _psparams = new SharpDxShaderParamInfo();
                    for (int j = 0; j < cbDEsc.VariableCount; j++)
                    {
                        ShaderReflectionVariable shaderRefVar = cbTemp.GetVariable(j);
                        ShaderReflectionType shaderRefType = cbTemp.GetVariable(j).GetVariableType();
                        ShaderTypeDescription shaderRefTypeDesc = shaderRefType.Description;
                        _psparams._ps = _pixelShader;
                        _psparams._psByteCode = pixelShaderByteCode;
                        _psparams._buffername = cbDEsc.Name;

                        _psparams._sdxBuffer = _buff;
                        _psparams._flags = shaderRefVar.Description.Flags;
                        _psparams._varCount = cbDEsc.VariableCount;
                        _psparams._varName = shaderRefVar.Description.Name;
                        
                        _psparams._varSize = shaderRefVar.Description.Size;
                        _psparams._varPositionB = shaderRefVar.Description.StartOffset;
                        _psparams._flags = shaderRefVar.Description.Flags;
                        _psparams._varPositionI = j;
                        _psparams._varType = shaderRefType.Description.Type;
                        _psparams._bufferParams.Write(shaderRefVar.Description.DefaultValue);
                        _sDxShaderParams.Add(_psparams);

                    }
                }
                else
                {
                    //cbDEsc.Name Liefert den Namen der Struktur
                    var bufferSize = cbDEsc.Size;
                    var _buffer = new Buffer(_device, new BufferDescription
                    {
                        Usage = ResourceUsage.Default,
                        SizeInBytes = bufferSize,
                        BindFlags = BindFlags.ConstantBuffer,
                        CpuAccessFlags = 0
                    });

                    _sDxShaderBuffers.Add(cbDEsc.Name, _buffer);
                    SharpDxShaderParamInfo _psparams = new SharpDxShaderParamInfo();
                    _psparams._bufferParams = new DataStream(cbDEsc.Size, true, true);
                    for (int j = 0; j < cbDEsc.VariableCount; j++)
                    {
                        ShaderReflectionVariable shaderRefVar = cbTemp.GetVariable(j);
                        ShaderReflectionType shaderRefType = cbTemp.GetVariable(j).GetVariableType();
                        ShaderTypeDescription shaderRefTypeDesc = shaderRefType.Description;
                        _psparams._ps = _pixelShader;
                        _psparams._psByteCode = pixelShaderByteCode;
                        _psparams._buffername = cbDEsc.Name;
                        _psparams._sdxBuffer = _buffer;
                        _psparams._flags = shaderRefVar.Description.Flags;
                        _psparams._varCount = cbDEsc.VariableCount;
                        _psparams._varName = shaderRefVar.Description.Name;
                        
                        _psparams._varSize = shaderRefVar.Description.Size;
                        _psparams._varPositionB = shaderRefVar.Description.StartOffset;
                        _psparams._flags = shaderRefVar.Description.Flags;
                        _psparams._varPositionI = j;
                        _psparams._varType = shaderRefType.Description.Type;
                        _psparams._bufferParams.Write(shaderRefVar.Description.DefaultValue);
                        _sDxShaderParams.Add(_psparams);

                    }
                }
            }


            /*--------------------------------------------------------------------------------------------------
             * 
             * VertexShader-ConstantBuffers
             * 
             * -------------------------------------------------------------------------------------------------
             */
            for (int i = 0; i < _vShaderDescription.ConstantBuffers; ++i)
            {

                ConstantBuffer cbTemp = _vReflector.GetConstantBuffer(i);
                ConstantBufferDescription cbDEsc = cbTemp.Description;
                if (_sDxShaderBuffers.ContainsKey(cbDEsc.Name))
                {
                    _sDxShaderBuffers.TryGetValue(cbDEsc.Name, out _buff);
                    _sDxShaderBuffers.Add(cbDEsc.Name, _buff);
                    SharpDxShaderParamInfo _vsparams = new SharpDxShaderParamInfo();
                    for (int j = 0; j < cbDEsc.VariableCount; j++)
                    {
                        ShaderReflectionVariable shaderRefVar = cbTemp.GetVariable(j);
                        ShaderReflectionType shaderRefType = cbTemp.GetVariable(j).GetVariableType();
                        ShaderTypeDescription shaderRefTypeDesc = shaderRefType.Description;
                        _vsparams._vs = _vertexShader;
                        _vsparams._vsByteCode = vertexShaderByteCode;
                        _vsparams._buffername = cbDEsc.Name;
                        _vsparams._sdxBuffer = _buff;
                        _vsparams._flags = shaderRefVar.Description.Flags;
                        _vsparams._varCount = cbDEsc.VariableCount;
                        _vsparams._varName = shaderRefVar.Description.Name;
                   
                        _vsparams._varSize = shaderRefVar.Description.Size;
                     
                        _vsparams._varPositionB = shaderRefVar.Description.StartOffset;
                        _vsparams._flags = shaderRefVar.Description.Flags;
                        _vsparams._varPositionI = j;
                        _vsparams._varType = shaderRefType.Description.Type;
              
                        _vsparams._bufferParams.Write(shaderRefVar.Description.DefaultValue);
                        _sDxShaderParams.Add(_vsparams);

                    }
                }
                else
                {
                    //cbDEsc.Name Liefert den Namen der Struktur
                    var bufferSize = cbDEsc.Size;
                    var _buffer = new Buffer(_device, new BufferDescription
                    {
                        Usage = ResourceUsage.Default,
                        SizeInBytes = bufferSize,
                        BindFlags = BindFlags.ConstantBuffer,
                        CpuAccessFlags = 0
                    });

                    _sDxShaderBuffers.Add(cbDEsc.Name, _buffer);
                    SharpDxShaderParamInfo _vsparams = new SharpDxShaderParamInfo();
                    _vsparams._bufferParams = new DataStream(cbDEsc.Size, true, true);
                    for (int j = 0; j < cbDEsc.VariableCount; j++)
                    {
                        ShaderReflectionVariable shaderRefVar = cbTemp.GetVariable(j);
                        ShaderReflectionType shaderRefType = cbTemp.GetVariable(j).GetVariableType();
                        ShaderTypeDescription shaderRefTypeDesc = shaderRefType.Description;
                        _vsparams._vs = _vertexShader;
                        _vsparams._vsByteCode = vertexShaderByteCode;
                        _vsparams._buffername = cbDEsc.Name;
                        _vsparams._sdxBuffer = _buffer;
                        _vsparams._flags = shaderRefVar.Description.Flags;
                        _vsparams._varCount = cbDEsc.VariableCount;
                        _vsparams._varName = shaderRefVar.Description.Name;
                  
                        _vsparams._varSize = shaderRefVar.Description.Size;
                        _vsparams._varPositionB = shaderRefVar.Description.StartOffset;
      
                        _vsparams._flags = shaderRefVar.Description.Flags;
                        _vsparams._varPositionI = j;
                        _vsparams._varType = shaderRefType.Description.Type;
                        _vsparams._bufferParams.Write(shaderRefVar.Description.DefaultValue);
        
                        _sDxShaderParams.Add(_vsparams);

                    }
                }
            }
            //for (int i = 0; i < _vShaderDescription.ConstantBuffers; ++i)
            //{

            //    ConstantBuffer cbTemp = _pReflector.GetConstantBuffer(i);
            //    ConstantBufferDescription cbDEsc = cbTemp.Description;
            //    //cbDEsc.Name Liefert den Namen der Struktur
            //    var bufferSize = cbDEsc.Size;
            //    var _buffer = new Buffer(_device, new BufferDescription
            //    {
            //        Usage = ResourceUsage.Default,
            //        SizeInBytes = bufferSize,
            //        BindFlags = BindFlags.ConstantBuffer,
            //        CpuAccessFlags = 0
            //    });

            //    _sDxShaderBuffers.Add(cbDEsc.Name, _buffer);
            //    SharpDxShaderParamInfo _vparams = new SharpDxShaderParamInfo();
            //    for (int j = 0; j < cbDEsc.VariableCount; j++)
            //    {
            //        ShaderReflectionVariable shaderRefVar = cbTemp.GetVariable(j);
            //        ShaderReflectionType shaderRefType = cbTemp.GetVariable(j).GetVariableType();
            //        ShaderTypeDescription shaderRefTypeDesc = shaderRefType.Description;

            //        _vparams._vs = _vertexShader;
            //        _vparams._vsByteCode = vertexShaderByteCode;
            //        _vparams._buffername = cbDEsc.Name;
            //        _vparams._sdxBuffer = _buffer;
            //        _vparams._flags = shaderRefVar.Description.Flags;
            //        _vparams._varCount = cbDEsc.VariableCount;
            //        _vparams._varName = shaderRefVar.Description.Name;
            //        _vparams._varSize = shaderRefVar.Description.Size;
            //        _vparams._varPositionB = shaderRefVar.Description.StartOffset;
            //        _vparams._flags = shaderRefVar.Description.Flags;
            //        _vparams._varPositionI = j;
            //        _sDxShaderParams.Add(_vparams);

            //    }
            //}
            return new ShaderProgramImp { };
        }


        public void SetShader(IShaderProgramImp program)
        {
            _context.VertexShader.Set(_vertexShader);
            _context.PixelShader.Set(_pixelShader);
        }

        public void Clear(ClearFlags flags)
        {
            //hat keinen Effekt

            if ((flags & ClearFlags.Color) != (ClearFlags) 0)
            {
                Color4 clearColor = new Color4();
                clearColor.Red = _cColor.x;
                clearColor.Green = _cColor.y;
                clearColor.Blue = _cColor.z;
                clearColor.Alpha = _cColor.w;
                _context.ClearRenderTargetView(_renderView, clearColor);
            }
            if ((flags & ClearFlags.Depth) != (ClearFlags) 0)
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
            var maxCount = 4;
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
            _context.Rasterizer.SetViewports(new Viewport(x, y, width, height, 0.0f, 1.0f));
        }

        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {

        }


    }
}