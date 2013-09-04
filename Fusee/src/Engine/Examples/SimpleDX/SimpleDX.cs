using System;
using System.Windows.Forms;
using Fusee.Engine;
using Fusee.Math;
using SharpDX.DirectInput;
using MouseButtons = Fusee.Engine.MouseButtons;

namespace Examples.SimpleDX
{
    public class SimpleDX : RenderCanvas
    {
        //Pixel and Vertex Shader    ((float3x3)fMatrix) = transpose((float3x3)fMatrix);
        public string Vs = @"cbuffer Variables : register(b0){
float4 Testfarbe;
float4x4 FUSEE_MVP;
float4x4 FUSEE_ITMV;
} 
struct VS_IN
{
	float4 pos : POSITION;
	float2 tex : TEXCOORD;
    float3 normal : NORMAL;
};
struct PS_IN
{
	float4 pos : SV_POSITION;
    float4 col : COLOR;
	float2 tex : TEXCOORD;
    float3 normal : NORMAL;
};
PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;
    input.pos.w = 1.0f;
	output.pos = mul(input.pos,FUSEE_MVP);
    float3x3 fMatrix = {FUSEE_ITMV[0].xyz,FUSEE_ITMV[1].xyz,FUSEE_ITMV[2].xyz};

    output.normal = normalize(mul(input.normal,(float3x3)fMatrix));
    output.col = Testfarbe;
    output.tex = input.tex;
	return output;
}
";

        string Ps = @"

struct PS_IN
{
	float4 pos : SV_POSITION;
    float4 col : COLOR;
	float2 tex : TEXCOORD;
    float3 normal : NORMAL;
};



float4 PS( PS_IN input ) : SV_Target
{
float3 vec3 = {0,0,1};
float res =   dot(input.normal, vec3);
	return mul(input.col,res);
	/*return  imageFG.Sample(pictureSampler,input.tex);*/
    }
";




        private const string VsTexture = @"cbuffer Variablenn : register(b0){

float4x4 FUSEE_MVP;
float4x4 FUSEE_ITMV;
} 
struct VS_IN
{
	float4 pos : POSITION;
	float2 tex : TEXCOORD;
    float3 normal : NORMAL;
};
struct PS_IN
{
	float4 pos : SV_POSITION;

	float2 tex : TEXCOORD;
    float3 normal : NORMAL;
};
PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;
    input.pos.w = 1.0f;
	output.pos = mul(input.pos,FUSEE_MVP);
    float3x3 fMatrix = {FUSEE_ITMV[0].xyz,FUSEE_ITMV[1].xyz,FUSEE_ITMV[2].xyz};
  
    output.normal = normalize(mul(input.normal,(float3x3)fMatrix));
    output.tex = input.tex;
	return output;
}
";


        private const string PsTexture = @"
SamplerState pictureSampler;
Texture2D texture1;
struct PS_IN
{
	float4 pos : SV_POSITION;
    float4 col : COLOR;
	float2 tex : TEXCOORD;
    float3 normal : NORMAL;
};



float4 PS( PS_IN input ) : SV_Target
{
float3 vec3 = {0,0,1};
float res =   dot(input.normal, vec3);
	/*return mul(input.col,res);*/
	return texture1.Sample(pictureSampler,input.tex);
    }";


        //angle variable
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;
        //modell variable
        private Mesh Mesh, _meshFace;
        //variable for color
        private IShaderParam _colorParam;
        private IShaderParam _textureParam;
        private ImageData _imgData1;

        private ITexture _iTex1;
  
        private ShaderProgram sp, _spTexture;

        public override void Init()
        {
            RC.ClearColor = new float4(0.5f, 0.5f, 0.5f, 1);
            RC.ClearDepth = 1.0f;
            //initialize the variable
            Mesh = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");

            _meshFace = MeshReader.LoadMesh(@"Assets/Face.obj.model");

            sp = RC.CreateShader(Vs, Ps);
            _spTexture = RC.CreateShader(VsTexture, PsTexture);
            _colorParam = sp.GetShaderParam("Testfarbe");
            
            _textureParam = _spTexture.GetShaderParam("texture1");

            // load texture
            _imgData1 = RC.LoadImage("Assets/cube_tex.jpg");

            _iTex1 = RC.CreateTexture(_imgData1);

        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);


            // move per mouse
            if (Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY);
            }
            else
            {
                var curDamp = (float)Math.Exp(-Damping * Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;


            if (Input.Instance.IsKeyDown(KeyCodes.Left))
                _angleHorz -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Right))
                _angleHorz += RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Up))
                _angleVert -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Down))
                _angleVert += RotationSpeed * (float)Time.Instance.DeltaTime;


            //if (Input.Instance.IsButtonDown(MouseButtons.Left))
            //{
            //    MessageBox.Show(Input.Instance.GetAxis(InputAxis.MouseX).ToString() + Input.Instance.GetAxis(InputAxis.MouseX).ToString());
            //}

            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            var mtxCam = float4x4.LookAt(0, 200, 500, 0, 0, 0, 0, 1, 0);


            
            // first mesh

            RC.ModelView = float4x4.CreateTranslation(0, -50, 0) * mtxRot * float4x4.CreateTranslation(-150, 0, 0) * mtxCam;

            RC.SetShader(sp);


            //mapping
            RC.SetShaderParam(_colorParam, new float4(0.0f, 1.0f, 0.0f, 1.0f));
            RC.Render(_meshFace);
            
            //second mesh
            RC.ModelView = mtxRot * float4x4.CreateTranslation(150, 0, 0) * mtxCam;
            //RC.ModelView = float4x4.CreateTranslation(0, -50, 0) * mtxRot * float4x4.CreateTranslation(-150, 0, 0) * mtxCam;


            RC.SetShader(_spTexture);
            RC.SetShaderParamTexture(_textureParam, _iTex1);

            RC.Render(Mesh);

           Present();
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            float aspectRatio = Width / (float)Height;
            //RC.Projection = float4x4.Identity;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {

            SimpleDX app = new SimpleDX();
            app.Run();

        }

    }
}