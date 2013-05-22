﻿using Fusee.Engine;
using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// Provides the abillity to create or interact directly with the point data.
    /// </summary>
    /// <remarks>For an example how you can use it, see <see cref="Cube"/>.</remarks>
    public class Mesh
    {
        internal IMeshImp _meshImp;
        private float3[] _vertices;
        /// <summary>
        /// Gets or sets the vertices.
        /// </summary>
        /// <value>
        /// The vertices.
        /// </value>
        public float3[] Vertices
        {
            get { return _vertices; }
            set { if (_meshImp!= null) _meshImp.InvalidateVertices(); _vertices = value; }
        }
        /// <summary>
        /// Gets a value indicating whether vertices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if vertices are set; otherwise, <c>false</c>.
        /// </value>
        public bool VerticesSet { get { return (_meshImp!= null) && _meshImp.VerticesSet; } }

        private float4[] _colors;
        /// <summary>
        /// Gets or sets the color of a single vertex.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public float4[] Colors
        {
            get { return _colors; }
            set { if (_meshImp != null) _meshImp.InvalidateColors(); _colors = value; }
        }
        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a colore is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet { get { return (_meshImp != null) && _meshImp.ColorsSet; } }

        private float3[] _normals;
        /// <summary>
        /// Gets or sets the normals.
        /// </summary>
        /// <value>
        /// The normals..
        /// </value>
        public float3[] Normals
        {
            get { return _normals; }
            set { if (_meshImp != null) _meshImp.InvalidateNormals(); _normals = value; }
        }
        /// <summary>
        /// Gets a value indicating whether normals are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if normals are set; otherwise, <c>false</c>.
        /// </value>
        public bool NormalsSet { get { return (_meshImp != null) && _meshImp.NormalsSet; } }

        private float2[] _uvs;
        /// <summary>
        /// Gets or sets the UV-coordinates.
        /// </summary>
        /// <value>
        /// The UV-coordinates.
        /// </value>
        public float2[] UVs
        {
            get { return _uvs; }
            set { if (_meshImp != null) _meshImp.InvalidateUVs(); _uvs = value; }
        }
        /// <summary>
        /// Gets a value indicating whether UVs are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if UVs are set; otherwise, <c>false</c>.
        /// </value>
        public bool UVsSet { get { return (_meshImp != null) && _meshImp.UVsSet; } }

        private short[] _triangles;
        /// <summary>
        /// Gets or sets the triangles.
        /// </summary>
        /// <value>
        /// The triangles.
        /// </value>
        public short[] Triangles
        {
            get { return _triangles; }
            set { if (_meshImp != null) _meshImp.InvalidateTriangles(); _triangles = value; }
        }
        /// <summary>
        /// Gets a value indicating whether triangles are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if triangles are set; otherwise, <c>false</c>.
        /// </value>
        public bool TrianglesSet { get { return (_meshImp != null) && _meshImp.TrianglesSet; } }


    }
}



