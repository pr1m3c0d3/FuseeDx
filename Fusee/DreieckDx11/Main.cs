using System.IO;
using System.Windows.Forms;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.DreieckDx11
{

    public class DreieckDx11 : RenderCanvas
    {
        private Mesh _myMesh;
        private Mesh _myMesh_;
        protected IShaderParam VColorParam;
        protected IShaderParam VColorParam_;
        public override void Init()
        {
            // is called on startup
            string Vs = @"cbuffer Variables : register(b0){
 float4 TestFarbe;
float4 TestF;
} 
struct VS_IN
                    {
	                    float4 pos : POSITION;
	                    float4 col : COLOR;
                    };
                    struct PS_IN
                    {
	                    float4 pos : SV_POSITION;
	                    float4 col : COLOR;
                    };

                    PS_IN VS( VS_IN input )
                    {
	                    PS_IN output = (PS_IN)0;
	
	                    output.pos = input.pos;
	                    output.col = input.col;
	
	                    return output;
                    }

                    float4 PS( PS_IN input ) : SV_Target
                    {
	                     return TestFarbe;
                    }";
            string Ps = @"cbuffer Variables : register(b0){
 float4 TestFarbe;
float4 TestF;
} 
                    struct VS_IN
                    {
	                    float4 pos : POSITION;
	                    float4 col : COLOR;
                    };

                    struct PS_IN
                    {
	                    float4 pos : SV_POSITION;
	                    float4 col : COLOR;
                    };

                    PS_IN VS( VS_IN input )
                    {
	                    PS_IN output = (PS_IN)0;
	
	                    output.pos = input.pos;
	                    output.col = input.col;
	
	                    return output;
                    }

                    float4 PS( PS_IN input ) : SV_Target
                    {
                       
	                     return TestFarbe;
                    }";
            ShaderProgram sp = RC.CreateShader(Vs, Ps);
            RC.SetShader(sp);
            VColorParam = sp.GetShaderParam("TestF");
            VColorParam_ = sp.GetShaderParam("TestFarbe");
            _myMesh = new Mesh();
            _myMesh_ = new Mesh();
            
            var myVertices = new float3[]
            {
                new float3(0.0f, 0.5f, 0.5f),
                new float3(0.5f, 0.0f, 0.5f),
                new float3(-0.5f, 0.0f, 0.5f),
                //new float3(0.0f, -0.5f, 0.5f)
            };


            var myVert = new float3[]
                {
                    new float3(0.5f, 0.0f, 0.5f),
                    new float3(-0.5f, 0.0f, 0.5f),
                    new float3(0.2f, -0.8f, 0.5f)
               
                };

            //myVertices[0] = new float3(0.0f, 0.5f, 0.5f);
            //myVertices[1] = new float3(0.5f, -0.5f, 0.5f);
            //myVertices[2] = new float3(-0.5f, -0.5f, 0.5f);
            //myVertices[3] = new float3(-0.5f, -2.5f, 0.5f);

            var myColors = new float4[4];
            myColors[0] = new float4(1.0f, 0.0f, 0.0f,1.0f);
            myColors[1] = new float4(0.0f, 1.0f, 0.0f, 1.0f);
            myColors[2] = new float4(0.0f, 0.0f, 1.0f, 1.0f);
            myColors[3] = new float4(1.0f, 1.0f, 1.0f, 1.0f);

            var triangles = new short[]
                {
                    0,
                    1,
                    2

                };


            var triangles_ = new short[]
                {
                    2,
                    1,
                    0
                };

            _myMesh.Vertices = myVertices;
            //_myMesh.Colors = myColors;
            //_myMesh_.Vertices = myVert;
            _myMesh.Triangles = triangles;
            //_myMesh_.Triangles = triangles_;
            //RC.ClearColor = new float4(1,1,1,1);



        }

        public override void RenderAFrame()
        {
            // is called once a frame
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            if (_myMesh != null)
            {
                
                RC.SetShaderParam(VColorParam_, new float4(1.0f, 1.0f, 1.0f, 1.0f));
                RC.Render(_myMesh);
                
                    
                //RC.SetShaderParam(VColorParam, new float4(1.0f, 0.0f, 0.0f, 1.0f));
                //RC.Render(_myMesh_);
            }
            else
            {
                MessageBox.Show("myMesh is null");

            }


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
