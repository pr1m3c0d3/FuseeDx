using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using Fusee.Math;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Fusee.Engine
{
    public class RenderContextImp : IRenderContextImp
    {
        private int _currentTextureUnit;
        private Dictionary<int, int> _shaderParam2TexUnit;

        public RenderContextImp(IRenderCanvasImp renderCanvas)
        {
            SwapChain swapChain;
            
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
            

            var ret = new ImageData()
            {
                PixelData = new byte[2],
                Height = 20,
                Width = 20
                

            };

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
            var ret = new ImageData()
            {
                PixelData = new byte[2],
                Height = 20,
                Width = 20


            };

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
            
            return  new ShaderParam {  };
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
           
        }

        public void SetShaderParam(IShaderParam param, float2 val)
        {
            
        }

        public void SetShaderParam(IShaderParam param, float3 val)
        {
           
        }

        public void SetShaderParam(IShaderParam param, float4 val)
        {
            
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
            set {  }
        }

        public float ClearDepth
        {
            get { throw new NotImplementedException(); }
            set {  }
        }

        public IShaderProgramImp CreateShader(string vs, string ps)
        {
            
            return new ShaderProgramImp {  };
        }


        public void SetShader(IShaderProgramImp program)
        {
           
        }

        public void Clear(ClearFlags flags)
        {
           
        }


        public void SetVertices(IMeshImp mr, float3[] vertices)
        {
            
        }


        public void SetNormals(IMeshImp mr, float3[] normals)
        {
            
        }

        public void SetUVs(IMeshImp mr, float2[] uvs)
        {
            
        }

        public void SetColors(IMeshImp mr, uint[] colors)
        {
            
        }


        public void SetTriangles(IMeshImp mr, short[] triangleIndices)
        {
           
        }

        public void Render(IMeshImp mr)
        {
            
        }

        public IMeshImp CreateMeshImp()
        {
            return new MeshImp();
        }

        public void Viewport(int x, int y, int width, int height)
        {
            
        }

        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
          
        }

        public void Frustum(double left, double right, double bottom, double top, double zNear, double zFar)
        {
           
        }
    }
}