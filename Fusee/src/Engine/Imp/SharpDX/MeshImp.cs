using SharpDX.Direct3D11;

namespace Fusee.Engine
{
    public class MeshImp : IMeshImp
    {
        internal Buffer VertexBufferObject;
        internal Buffer NormalBufferObject;
        internal Buffer ColorBufferObject;
        internal Buffer UVBufferObject;
        internal Buffer ElementBufferObject;
        internal VertexBufferBinding[] VertexBufferBindingObject;
        internal int NElements;

        public void InvalidateVertices()
        {
            
        }
        public bool VerticesSet { get { return VertexBufferObject != null; } }

        public void InvalidateNormals()
        {
            
        }
        public bool NormalsSet { get { return NormalBufferObject != null; ; } }

        public void InvalidateColors()
        {
           
        }
        public bool ColorsSet { get { return ColorBufferObject != null; ; } }

        public bool UVsSet { get { return UVBufferObject != null; ; } }
        public void InvalidateUVs()
        {
           
        }

        public void InvalidateTriangles()
        {
            NElements = 0;
        }
        public bool TrianglesSet { get { return ElementBufferObject != null; } }
    }
}
