using System.IO;

using Fusee.Engine;
using Fusee.Math;

namespace Examples.DreieckDx11
{
    public class DreieckDx11 : RenderCanvas
    {
        private Mesh _myMesh;
        public override void Init()
        {
            // is called on startup
            string Vs = "";
            string Ps = "";
            ShaderProgram sp = RC.CreateShader(Vs, Ps);
            RC.SetShader(sp);

            _myMesh = new Mesh();
            var myVertices = new float3[4];

            myVertices[0] = new float3(-50f, 0, -1 / 3);
            myVertices[1] = new float3(50f, 0, -2 / 3);
            myVertices[2] = new float3(0, 0, +4 / 3 * 2);
            myVertices[3] = new float3(0, 3, 0);

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
        }

        public override void RenderAFrame()
        {
            // is called once a frame
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
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
            var app = new DreieckDx11();
            app.Run();
        }

    }
}
