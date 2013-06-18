using SharpDX.D3DCompiler;
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
        
        
    }
}
