using System.IO;
using System.Windows.Forms;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.DreieckDx11
{

    public class DreieckDx11 : RenderCanvas
    {
        private Mesh _myMesh;
        protected IShaderParam _vTextureParam;
        protected ImageData _imgData1;
        protected ITexture _iTex1;
        public override void Init()
        {
            // is called on startup
            string Vs = @"cbuffer Variables : register(b0){
 float4 TestFarbe;
 float4 TestF;
float4x4 FUSEE_MVP;
} 
SamplerState pictureSampler;
Texture2D imageFG;

struct VS_IN
{
	float4 pos : POSITION;
	float4 tex : TEXCOORD;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 tex : TEXCOORD;

};

PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;
	input.pos.w = 1.0f;
	output.pos = mul(input.pos,FUSEE_MVP);
	// output.col = TestFarbe;
		
	output.tex = input.tex;
	
	return output;
}

float4 PS( PS_IN input ) : SV_Target
{
	/*return input.col;*/
	/*return TestFarbe;*/
	return  imageFG.Sample(pictureSampler,input.tex);
}
";
            string Ps = @"cbuffer Variables : register(b0){
 float4 TestFarbe;
 float4 TestF;
 float4x4 FUSEE_MVP;
} 
SamplerState pictureSampler;
Texture2D imageFG;

struct VS_IN
{
	float4 pos : POSITION;
	float4 tex : TEXCOORD;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 tex : TEXCOORD;

};

PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;
	input.pos.w = 1.0f;
	output.pos = mul(input.pos,FUSEE_MVP);
	// output.col = TestFarbe;
    
	output.tex = input.tex;
	
	return output;
}

float4 PS( PS_IN input ) : SV_Target
{
	/*return input.col;*/
	/*return TestFarbe;*/
	return  imageFG.Sample(pictureSampler,input.tex);
}
";
           Geometry geo2 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Dreieck.obj.model"));
           _myMesh = geo2.ToMesh();
           // _myMesh = new Mesh();
            ShaderProgram sp = RC.CreateShader(Vs, Ps);
            RC.SetShader(sp);
            _vTextureParam = sp.GetShaderParam("");
           
            
            

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
            myColors[0] = new float4(1.0f, 0.0f, 0.0f, 1.0f);
            myColors[1] = new float4(0.0f, 1.0f, 0.0f, 1.0f);
            myColors[2] = new float4(0.0f, 0.0f, 1.0f, 1.0f);
            myColors[3] = new float4(1.0f, 1.0f, 1.0f, 1.0f);
            var myUvs = new float2[3];
            myUvs[0] = new float2(1.0f, 0.0f);
            myUvs[1] = new float2(0.0f, 1.0f);
            myUvs[2] = new float2(0.0f, 0.0f);

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

            _imgData1 = RC.LoadImage("Assets/cube_tex.jpg");
            _iTex1 = RC.CreateTexture(_imgData1);

            //_myMesh.Vertices = myVertices;
            //_myMesh.Colors = myColors;
            //_myMesh.UVs = myUvs;
            //_myMesh_.Vertices = myVert;
            //_myMesh.Triangles = triangles;

            RC.ClearColor = new float4(0.5f, 0.5f, 0.5f, 1);
            RC.ClearDepth = 1.0f;
        }

        public override void RenderAFrame()
        {
            // is called once a frame
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            if (_myMesh != null)
            {
                float4x4 mtxRot = float4x4.CreateRotationY(5) * float4x4.CreateRotationX(10);
                float4x4 mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);

                RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;
                RC.SetShaderParamTexture(_vTextureParam, _iTex1);
                RC.Render(_myMesh);

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
