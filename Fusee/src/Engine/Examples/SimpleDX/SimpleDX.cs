using System;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.SimpleDX
{
    public class SimpleDX : RenderCanvas
    {
        //Pixel and Vertex Shader
        public string Vs = @"cbuffer Variables : register(b0){

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
}";

        public string Ps = @"cbuffer Variables : register(b0){

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
        //angle variable
        private static float _angleHorz = 0.0f, _angleVert = 0.0f, _angleVelHorz = 0, _angleVelVert = 0, RotationSpeed = 10.0f, Damping = 0.95f;
        //modell variable
        private Mesh Mesh, MeshFace;
        //variable for color
        protected IShaderParam VColorParam;
        protected IShaderParam _vTextureParam;
        protected ImageData _imgData1;
        protected ImageData _imgData2;
        protected ITexture _iTex1;
        protected ITexture _iTex2;

        public override void Init()
        {
            RC.ClearColor = new float4(0.5f, 0.5f, 0.5f, 1);
            RC.ClearDepth = 1.0f;
            //initialize the variable
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Teapot.obj.model"));
            Mesh = geo.ToMesh();


            ShaderProgram sp = RC.CreateShader(Vs, Ps);
            RC.SetShader(sp);
            VColorParam = sp.GetShaderParam("");
            

            _imgData1 = RC.LoadImage("Assets/world_map.jpg");
            

            _iTex1 = RC.CreateTexture(_imgData1);

        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);


            float4x4 mtxRot = float4x4.CreateRotationY(5) * float4x4.CreateRotationX(10);
            float4x4 mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);

            RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;
            

            //mapping
            RC.SetShaderParamTexture(_vTextureParam, _iTex1);
            RC.Render(Mesh);

            
            Present();
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            float aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {

            SimpleDX app = new SimpleDX();
            app.Run();
           
        }

    }
}