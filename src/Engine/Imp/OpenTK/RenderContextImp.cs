﻿using System;
using Fusee.Math;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Fusee.Engine
{
    public class RenderContextImp : IRenderContextImp
    {
        public RenderContextImp(IRenderCanvasImp renderCanvas)
        {
        }

        public IShaderParam GetShaderParam(IShaderProgramImp shaderProgram, string paramName)
        {
            int h = GL.GetUniformLocation(((ShaderProgramImp)shaderProgram).Program, paramName);
            return (h == -1) ? null : new ShaderParam {handle = h};
        }

        public void SetShaderParam(IShaderParam param, float val)
        {
            GL.Uniform1(((ShaderParam)param).handle, val);
        }

        public void SetShaderParam(IShaderParam param, float2 val)
        {
            GL.Uniform2(((ShaderParam)param).handle, val.x, val.y);
        }

        public void SetShaderParam(IShaderParam param, float3 val)
        {
            GL.Uniform3(((ShaderParam)param).handle, val.x, val.y, val.z);
        }

        public void SetShaderParam(IShaderParam param, float4 val)
        {
            GL.Uniform4(((ShaderParam)param).handle, val.x, val.y, val.z, val.w);
        }

        // TODO add vector implementations

        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
            unsafe
            {
                float* mF = (float*) (&val);
                GL.UniformMatrix4(((ShaderParam)param).handle, 1, false, mF);
            }
        }

        public float4x4 ModelView
        {
            get 
            { throw new NotImplementedException(); }
            set 
            {
                GL.MatrixMode(MatrixMode.Modelview);
                unsafe {GL.LoadMatrix((float*)(&value));}
            }
        }

        public float4x4 Projection
        {
            get
            { throw new NotImplementedException(); }
            set
            {
                GL.MatrixMode(MatrixMode.Projection);
                unsafe { GL.LoadMatrix((float*)(&value)); }
            }
        }

        public float4 ClearColor
        {
            get
            {
                Vector4 ret; 
                GL.GetFloat(GetPName.ColorClearValue, out ret);
                return new float4(ret.X, ret.Y, ret.Z, ret.W);
            }
            set
            {
                GL.ClearColor(value.x, value.y, value.z, value.w);
            }
        }

        public float ClearDepth
        {
            get
            {
                float ret;
                GL.GetFloat(GetPName.DepthClearValue, out ret);
                return ret;
            }
            set
            {
                GL.ClearDepth(value);
            }
        }

        public IShaderProgramImp CreateShader(string vs, string ps)
        {
            int statusCode;
            string info;

            int vertexObject = GL.CreateShader(ShaderType.VertexShader);
            int fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(vertexObject, vs);
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, out info);
            GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            // Compile pixel shader
            GL.ShaderSource(fragmentObject, ps);
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(fragmentObject, out info);
            GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            int program = GL.CreateProgram();
            GL.AttachShader(program, fragmentObject);
            GL.AttachShader(program, vertexObject);

            // enable GLSL (ES) shaders to use fuVertex, fuColor and fuNormal attributes
            GL.BindAttribLocation(program, Helper.VertexAttribLocation, Helper.VertexAttribName);
            GL.BindAttribLocation(program, Helper.ColorAttribLocation, Helper.ColorAttribName);
            GL.BindAttribLocation(program, Helper.NormalAttribLocation, Helper.NormalAttribName);

            GL.LinkProgram(program); // AAAARRRRRGGGGHHHH!!!! Must be called AFTER BindAttribLocation
            return new ShaderProgramImp {Program = program};
        }


        public void SetShader(IShaderProgramImp program)
        {
            GL.UseProgram(((ShaderProgramImp)program).Program);
        }

        public void Clear(ClearFlags flags)
        {
            GL.Clear((ClearBufferMask)flags);
        }


        public void SetVertices(IMeshImp mr, float3[] vertices)
        {
            if (vertices == null || vertices.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            int vboBytes;
            int vertsBytes = vertices.Length * 3 * sizeof(float);
            if (((MeshImp)mr).VertexBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).VertexBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertsBytes), vertices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }


        public void SetNormals(IMeshImp mr, float3[] normals)
        {
            if (normals == null || normals.Length == 0)
            {
                throw new ArgumentException("Normals must not be null or empty");
            }

            int vboBytes;
            int normsBytes = normals.Length * 3 * sizeof(float);
            if (((MeshImp)mr).NormalBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).NormalBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normsBytes), normals, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != normsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.",
                    normsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void SetColors(IMeshImp mr, uint[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("colors must not be null or empty");
            }

            int vboBytes;
            int colsBytes = colors.Length * sizeof(uint);
            if (((MeshImp)mr).ColorBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).ColorBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).ColorBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colsBytes), colors, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != colsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.",
                    colsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }


        public void SetTriangles(IMeshImp mr, short[] triangleIndices)
        {
            if (triangleIndices == null || triangleIndices.Length == 0)
            {
                throw new ArgumentException("triangleIndices must not be null or empty");
            }
            ((MeshImp)mr).NElements = triangleIndices.Length;
            int vboBytes;
            int trisBytes = triangleIndices.Length * sizeof(short);

            if (((MeshImp)mr).ElementBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).ElementBufferObject);
            // Upload the   index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(trisBytes), triangleIndices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != trisBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (offsets). Tried to upload {0} bytes, uploaded {1}.",
                    trisBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Render(IMeshImp mr)
        {
            if (((MeshImp)mr).VertexBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.VertexAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
                GL.VertexAttribPointer(Helper.VertexAttribLocation, 3, VertexAttribPointerType.Float , false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).ColorBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.ColorAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).ColorBufferObject);
                GL.VertexAttribPointer(Helper.ColorAttribLocation, 4, VertexAttribPointerType.UnsignedByte, true, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).NormalBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.NormalAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
                GL.VertexAttribPointer(Helper.NormalAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).ElementBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);
                GL.DrawElements(BeginMode.Triangles, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                //GL.DrawArrays(GL.Enums.BeginMode.POINTS, 0, shape.Vertices.Length);
            }
            if (((MeshImp)mr).VertexBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.VertexAttribLocation);
            }
            if (((MeshImp)mr).ColorBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.ColorAttribLocation);
            }
            if (((MeshImp)mr).NormalBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.NormalAttribLocation);
            }
        }

        public IMeshImp CreateMeshImp()
        {
            return new MeshImp();
        }

        public void Viewport(int x, int y, int width, int height)
        {
            GL.Viewport(x, y, width, height);
        }
    }
}