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
        string Vs = @"cbuffer Variables : register(b0){
float4x4 Testfarbe;
float4x4 FUSEE_MVP;
} 
SamplerState pictureSampler;
Texture2D imageFG;

struct VS_IN
{
	float4 pos : POSITION;
	float4 tex : TEXCOORD;
    float4 normal : NORMAL;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
float4 col : COLOR;
	float4 tex : TEXCOORD;
    float4 normal : NORMAL;
};

PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(input.pos,FUSEE_MVP);
	/*output.col = FUSEE_MV._m00_m01_m02_m03;*/
/*    output.normal = input.normal;
	output.tex = input.tex;*/
 /*   if (FUSEE_MV._m00 == 4.0f)
        output.col = float4(1,0,0,1);
    else
        output.col = float4(0,0,1,1);*/

	return output;
}

float4 PS( PS_IN input ) : SV_Target
{
	return input.col;
	/*return Testfarbe;*/
	/*return  imageFG.Sample(pictureSampler,input.tex);*/
    
    }
";
        string Ps = @"cbuffer Wariables : register(b0){

} 
SamplerState pictureSampler;
Texture2D imageFG;

struct VS_IN
{
	float4 pos : POSITION;
	float4 tex : TEXCOORD;
    float4 normal : NORMAL;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
    float4 col : COLOR;
	float4 tex : TEXCOORD;
    float4 normal : NORMAL;

};

PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;

	output.pos = input.pos;
	
/*    output.normal = input.normal;
	output.tex = input.tex;*/
    /*if (FUSEE_MV._m00 == 4.0f)
        output.col = float4(0,1,0,1);
    else
        output.col = float4(0,0,1,1);*/

	return output;
}

float4 PS( PS_IN input ) : SV_Target
{
	return input.col;
	/*return Testfarbe;*/
	/*return  imageFG.Sample(pictureSampler,input.tex);*/
    
    
}
";
        public override void Init()
        {
            // is called on startup
            RC.ClearColor = new float4(0.5f, 0.5f, 0.5f, 1);
            RC.ClearDepth = 1.0f;

            Geometry geo2 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Dreieck.obj.model"));
            _myMesh = geo2.ToMesh();
            // _myMesh = new Mesh();
            ShaderProgram sp = RC.CreateShader(Vs, Ps);
            RC.SetShader(sp);
            _vTextureParam = sp.GetShaderParam("Testfarbe");




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

            _imgData1 = RC.LoadImage("Assets/world_map.jpg");
            _iTex1 = RC.CreateTexture(_imgData1);

            //_myMesh.Vertices = myVertices;
            // _myMesh.Colors = myColors;

            //_myMesh.UVs = myUvs;
            //_myMesh_.Vertices = myVert;
            //_myMesh.Triangles = triangles;


        }

        public override void RenderAFrame()
        {
            // is called once a frame
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            if (_myMesh != null)
            {
                float4x4 mtxRot = float4x4.CreateRotationY(5) * float4x4.CreateRotationZ(10);
                //float4x4 mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);

                //RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;
                RC.ModelView = mtxRot * (new float4x4(1.0f, 0.0f, 0.0f, 0.0f,
                                            0.0f, 1.0f, 0.0f, 0.0f,
                                            0.0f, 0.0f, 1.0f, 0.0f,
                                            0.0f, 0.0f, 0.0f, 1.0f));


                //RC.SetShaderParamTexture(_vTextureParam, _iTex1);
                RC.SetShaderParam(_vTextureParam, new float4x4(0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
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
            RC.Projection = float4x4.Identity;
            //RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new DreieckDx11();
            app.Run();
        }

    }
}
