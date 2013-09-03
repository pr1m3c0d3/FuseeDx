
using SharpDX.Direct3D11;

namespace Fusee.Engine
{
    /// <summary>
    /// Texture Implementation for OpenTK, an integer value is used as a handle
    /// </summary>
    class Texture : ITexture
    {
        internal ShaderResourceView handle;
    }
}
