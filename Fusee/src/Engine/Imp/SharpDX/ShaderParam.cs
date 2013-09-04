using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace Fusee.Engine
{
    enum ShaderType
    {
        PixelShader,
        VertexShader
    }
    public class ShaderParam : IShaderParam
    {
        internal int position;
        internal ShaderType shaderType;
        internal int size;
        internal uint flags; //??
        internal string name;
        internal DataStream _bufferParams;
        internal Buffer _sdxBuffer;
        public int _varPositionB;

    }
}
