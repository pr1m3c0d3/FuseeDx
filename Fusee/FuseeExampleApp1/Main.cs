using System;
using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.FuseeExampleApp1
{
    public class FuseeExampleApp1 : RenderCanvas
    {
        private Mesh _myMesh;

        private IShaderParam _vColorParam;
        private IShaderParam _vTextureParam;

        private Quaternion _aRotation;

        public override void Init()
        {
            _aRotation = Quaternion.Identity;

            ShaderProgram sParam = MoreShaders.GetShader("simple", RC);
            RC.SetShader(sParam);

            _vTextureParam = sParam.GetShaderParam("vTexture");
            _vColorParam = sParam.GetShaderParam("vColor");
            _myMesh = new Mesh();

            var sqrt3Half = 100 * (float)Math.Sqrt(3) / 2;

            var myVertices = new float3[4];
            myVertices[0] = new float3(-50f, 0, -sqrt3Half / 3);
            myVertices[1] = new float3(50f, 0, -sqrt3Half / 3);
            myVertices[2] = new float3(0, 0, +sqrt3Half / 3 * 2);
            myVertices[3] = new float3(0, sqrt3Half, 0);

            var myNormals = new float3[4];
            myNormals[0] = new float3(-1, 0, -1);
            myNormals[1] = new float3(1, 0, -1);
            myNormals[2] = new float3(0, 0, +1);
            myNormals[3] = new float3(0, +1, 0);

            var myTriangles = new short[12];

            myTriangles[0] = 0;
            myTriangles[1] = 1;
            myTriangles[2] = 2;

            myTriangles[3] = 2;
            myTriangles[4] = 1;
            myTriangles[5] = 3;

            myTriangles[6] = 1;
            myTriangles[7] = 0;
            myTriangles[8] = 3;

            myTriangles[9] = 0;
            myTriangles[10] = 2;
            myTriangles[11] = 3;

            _myMesh.Vertices = myVertices;
            _myMesh.Normals = myNormals;
            _myMesh.Triangles = myTriangles;

            RC.ClearColor = new float4(1, 1, 1, 1);
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.SetShaderParamTexture(_vTextureParam, RC.DisableTexture());
            RC.SetShaderParam(_vColorParam, new float4(0.5f, 0.8f, 0, 1));

            var mtxCam = float4x4.LookAt(0, 200, 300, 0, 0, 0, 0, 1, 0);

            var rotVektor = new float3((float)Time.Instance.DeltaTime, 0, 0);
            _aRotation *= Quaternion.EulerToQuaternion(rotVektor);
            _aRotation.Normalize();

            RC.ModelView = Quaternion.QuaternionToMatrix(_aRotation) * mtxCam;
            RC.Render(_myMesh);

            Present();
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new FuseeExampleApp1();
            app.Run();
        }

    }
}