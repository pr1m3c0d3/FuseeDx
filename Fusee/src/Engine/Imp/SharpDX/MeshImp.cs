namespace Fusee.Engine
{
    public class MeshImp : IMeshImp
    {
        internal int VertexBufferObject;
        internal int NormalBufferObject;
        internal int ColorBufferObject;
        internal int UVBufferObject;
        internal int ElementBufferObject;
        internal int NElements;

        public void InvalidateVertices()
        {
            
        }
        public bool VerticesSet { get { return true; } }

        public void InvalidateNormals()
        {
            
        }
        public bool NormalsSet { get { return true; } }

        public void InvalidateColors()
        {
           
        }
        public bool ColorsSet { get { return true; } }

        public bool UVsSet { get { return true; } }
        public void InvalidateUVs()
        {
           
        }

        public void InvalidateTriangles()
        {
          
        }
        public bool TrianglesSet { get { return true; } }
    }
}
