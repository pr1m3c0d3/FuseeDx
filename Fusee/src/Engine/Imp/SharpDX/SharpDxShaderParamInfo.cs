using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

using SharpDX.DXGI;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace Fusee.Engine
{
    internal struct SharpDxShaderParamInfo
    {
        public PixelShader _ps;
        public VertexShader _vs;
        public ShaderBytecode _psByteCode;
        public ShaderBytecode _vsByteCode;
        public string _buffername;
        public int _varCount;
        public string _varName;
        public int _varSize;
        public int _varPositionB;
        public ShaderVariableFlags _flags;
        public int _varPositionI;
        public Buffer _sdxBuffer;
        public ShaderVariableType _varType;
        public DataStream _bufferParams;
        public ShaderReflectionVariable _varValue;
    }
    internal struct SharpDxShaderTexture
    {
        public int _position;
    }
}
