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
        //internal Buffer _vertices;
        //internal Buffer _verticesColor;
        //internal Buffer _trianglesIndex;

        public RenderContextImp(IRenderCanvasImp renderCanvas)
        {

            RenderCanvasImp dxCanvas = (RenderCanvasImp) renderCanvas;
            _context = dxCanvas._context;
            _device = dxCanvas._device;
            _swapChain = dxCanvas._swapChain;
            _renderView = dxCanvas._renderView;
            Viewport(0,0,dxCanvas.Width,dxCanvas.Height);
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
        public ITexture CreateTexture(ImageData img)
        {
           
            ITexture texID = new Texture { handle = 0 };
            return texID;

        }


        public IEnumerable<ShaderParamInfo> GetShaderParamList(IShaderProgramImp shaderProgram)
        {
            throw new NotImplementedException();
        }

        public IShaderParam GetShaderParam(IShaderProgramImp shaderProgram, string paramName)
        {
            int pos=0;
            for (int i = 0; i < _pShaderDescription.ConstantBuffers; ++i)
            {
                //ShaderParameterDescription paramDesc = _pRefelector.GetInputParameterDescription(i);
                ConstantBuffer cbTemp = _pReflector.GetConstantBuffer(i);
                ConstantBufferDescription cbDesc = cbTemp.Description;
                //cbDEsc.Name Liefert den Namen der Struktur

                for (int j = 0; j < cbDesc.VariableCount; j++)
                {
                    ShaderReflectionVariable shaderRefVar = cbTemp.GetVariable(j);
                    ShaderReflectionType shaderRefType = cbTemp.GetVariable(j).GetVariableType();
                    ShaderTypeDescription shaderRefTypeDesc = shaderRefType.Description;
                    //shaderRefVar.GetVariableType().GetMemberTypeName(i);
                    //shaderRefVar.GetVariableType().GetMemberType(i);
                    //MessageBox.Show("cBuffer name " + cbDesc.Name);
                    //MessageBox.Show("Variable count " + cbDesc.VariableCount.ToString());
                    //MessageBox.Show("Variable name " + shaderRefVar.Description.Name);
                    //MessageBox.Show("Variable size " + shaderRefVar.Description.Size.ToString());
                    //MessageBox.Show("Position in Bytes " + shaderRefVar.Description.StartOffset.ToString());
                    //MessageBox.Show("Flags " + shaderRefVar.Description.Flags.ToString());
                    //MessageBox.Show("Variable Type" + shaderRefTypeDesc.Type.ToString());
                    //8 werte 

                    pos = shaderRefVar.Description.StartOffset;
                   
                    
                }
            }
            return new ShaderParam { position = pos};//,size=size_,shaderType=sType
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
            foreach (var daten in _sDxShaderParams)
            {
                if (daten._varSize == 4)
                {

                    
                    var data = new DataStream(4, true, true);
                    data.Write(val);
                    data.Position = daten._varPositionB;
                    _context.UpdateSubresource(new DataBox(data.PositionPointer, 0, 0), daten._sdxBuffer, 0);
                    _context.PixelShader.SetConstantBuffer(0, daten._sdxBuffer);
                }
                else
                {
                    
                }
            }

            
        }

        public void SetShaderParam(IShaderParam param, float2 val)
        {
            foreach (var daten in _sDxShaderParams)
            {
                if (daten._varSize == 8)
                {


                    var data = new DataStream(8, true, true);
                    data.Write(val);
                    data.Position = daten._varPositionB;
                    _context.UpdateSubresource(new DataBox(data.PositionPointer, 0, 0), daten._sdxBuffer, 0);
                    _context.PixelShader.SetConstantBuffer(0, daten._sdxBuffer);
                }
                else
                {

                }
            }
        }

        public void SetShaderParam(IShaderParam param, float3 val)
        {
            foreach (var daten in _sDxShaderParams)
            {
                if (daten._varSize == 12)
                {


                    var data = new DataStream(12, true, true);
                    data.Write(val);
                    data.Position = daten._varPositionB;
                    _context.UpdateSubresource(new DataBox(data.PositionPointer, 0, 0), daten._sdxBuffer, 0);
                    _context.PixelShader.SetConstantBuffer(0, daten._sdxBuffer);
                }
                else
                {

                }
            }
        }

        public void SetShaderParam(IShaderParam param, float4 val)
        {
            //data.Position = ((ShaderParam).param).position;
            foreach (var daten in _sDxShaderParams)
            {
                if (daten._varSize == 16)
                {


                    var data = new DataStream(16, true, true);
                    data.Write(val);
                    data.Position = daten._varPositionB;
                    _context.UpdateSubresource(new DataBox(data.PositionPointer, 0, 0), daten._sdxBuffer, 0);
                    _context.PixelShader.SetConstantBuffer(0, daten._sdxBuffer);
                }
                else
                {

                }
            }
        }

        // TODO add vector implementations

        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
           
        }

        public void SetShaderParam(IShaderParam param, int val)
        {
           
        }

        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texId">An ITexture probably returned from CreateTexture method</param>
        public void SetShaderParamTexture(IShaderParam param, ITexture texId)
        {
            
        }

        public float4x4 ModelView
        {
            get { throw new NotImplementedException(); }
            set {  }
        }

        public float4x4 Projection
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        public float4 ClearColor
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        public float ClearDepth
        {
            get { throw new NotImplementedException(); }
            set {  }
        }

        public IShaderProgramImp CreateShader(string vs, string ps)
        {
            

            var vertexShaderByteCode = ShaderBytecode.Compile(vs, "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None);
            _vertexShader = new VertexShader(_device, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.Compile(ps, "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);
            _pixelShader = new PixelShader(_device, pixelShaderByteCode);

            

            _layout = new InputLayout(
                    _device,
                    ShaderSignature.GetInputSignature(vertexShaderByteCode),
                    new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 0, 1)
                    });


            _pReflector = new ShaderReflection(pixelShaderByteCode);
            _vReflector = new ShaderReflection(vertexShaderByteCode);
            _pShaderDescription = _pReflector.Description;
            _vShaderDescription = _vReflector.Description;
            _sDxShaderParams = new List<SharpDxShaderParamInfo>();
            _sDxShaderBuffers = new Dictionary<string, Buffer>();
            for (int i = 0; i < _pShaderDescription.ConstantBuffers; ++i)
            {

                ConstantBuffer cbTemp = _pReflector.GetConstantBuffer(i);
                ConstantBufferDescription cbDEsc = cbTemp.Description;
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
                    _sDxShaderParams.Add(_psparams);

                }
            }

            for (int i = 0; i < _vShaderDescription.ConstantBuffers; ++i)
            {

                ConstantBuffer cbTemp = _pReflector.GetConstantBuffer(i);
                ConstantBufferDescription cbDEsc = cbTemp.Description;
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
                SharpDxShaderParamInfo _vparams = new SharpDxShaderParamInfo();
                for (int j = 0; j < cbDEsc.VariableCount; j++)
                {
                    ShaderReflectionVariable shaderRefVar = cbTemp.GetVariable(j);
                    ShaderReflectionType shaderRefType = cbTemp.GetVariable(j).GetVariableType();
                    ShaderTypeDescription shaderRefTypeDesc = shaderRefType.Description;
                    
                    _vparams._vs = _vertexShader;
                    _vparams._vsByteCode = vertexShaderByteCode;
                    _vparams._buffername = cbDEsc.Name;
                    _vparams._sdxBuffer = _buffer;
                    _vparams._flags = shaderRefVar.Description.Flags;
                    _vparams._varCount = cbDEsc.VariableCount;
                    _vparams._varName = shaderRefVar.Description.Name;
                    _vparams._varSize = shaderRefVar.Description.Size;
                    _vparams._varPositionB = shaderRefVar.Description.StartOffset;
                    _vparams._flags = shaderRefVar.Description.Flags;
                    _vparams._varPositionI = j;
                    _sDxShaderParams.Add(_vparams);

                }
            }
            return new ShaderProgramImp {};
        }


        public void SetShader(IShaderProgramImp program)
        {
            _context.VertexShader.Set(_vertexShader);
            _context.PixelShader.Set(_pixelShader);
        }

        public void Clear(ClearFlags flags)
        {
            //hat keinen Effekt

            _context.ClearRenderTargetView(_renderView, SharpDX.Color.CadetBlue);
           
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
            ((MeshImp) mr).NElements = triangleIndices.Length;

        }

        public void Render(IMeshImp mr)
        {
            var maxCount = 4;
            if (((MeshImp) mr).VertexBufferBindingObject == null)
            {
                List<VertexBufferBinding> binding = new List<VertexBufferBinding>(4);
                if (((MeshImp)mr).VertexBufferObject != null)
                {
                     binding.Add(new VertexBufferBinding(((MeshImp)mr).VertexBufferObject,12, 0)); 
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

                    binding.Add(new VertexBufferBinding(((MeshImp) mr).NormalBufferObject, 12, 0));
                }
                ((MeshImp)mr).VertexBufferBindingObject = binding.ToArray();

            }
            _context.InputAssembler.SetVertexBuffers(0,((MeshImp)mr).VertexBufferBindingObject);


            if (((MeshImp)mr).ElementBufferObject != null)
            {
                _context.InputAssembler.InputLayout = _layout;
                _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

                _context.InputAssembler.SetIndexBuffer(((MeshImp)mr).ElementBufferObject, Format.R16_UInt, 0);
                _context.DrawIndexed(((MeshImp)mr).NElements, 0, 0);
            }
       
            
            

        }

        public IMeshImp CreateMeshImp()
        {
            return new MeshImp();
        }

        public void Viewport(int x, int y, int width, int height)
        {
            _context.Rasterizer.SetViewports(new Viewport(x,y,width,height, 0.0f,1.0f));
        }

        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            
        }

        public void Frustum(double left, double right, double bottom, double top, double zNear, double zFar)
        {
           
        }
    }
}