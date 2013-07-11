using System.Collections.Generic;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace Fusee.Engine
{
    public class ShaderProgramImp : IShaderProgramImp
    {
        internal ShaderBytecode _vsByteCode;
        internal ShaderBytecode _psByteCode;
        internal VertexShader _vs;
        internal PixelShader _ps;
        internal List<SharpDxShaderParamInfo> _sDXShaderParams;
        internal Dictionary<string, Buffer> _sDXBuffers;
    }
}
