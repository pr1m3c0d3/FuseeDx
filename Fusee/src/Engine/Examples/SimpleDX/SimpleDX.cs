using System;
using Fusee.Engine;
using Fusee.Math;


namespace Examples.SimpleDX
{
    public class SimpleDX : RenderCanvas
    {
        //Pixel and Vertex Shader
        public string Vs = @"cbuffer Variables : register(b0){
float4 Testfarbe;
float4x4 FUSEE_TMVP;
float4x4 FUSEE_IMV;
} 
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
    input.pos.w = 1.0f;
	output.pos = mul(input.pos,FUSEE_TMVP);
    output.normal = input.normal;
    output.col = Testfarbe;
	/*output.col = FUSEE_MV._m00_m01_m02_m03;*/
/*    output.normal = input.normal;
	output.tex = input.tex;*/
/* if (FUSEE_MVP._m44 == 1.0f)
        output.col = float4(1,0,0,1);
    else
        output.col = float4(0,0,1,1);*/

	return output;
}
";

        string Ps = @"
SamplerState pictureSampler;
Texture2D imageFG;
struct PS_IN
{
	float4 pos : SV_POSITION;
    float4 col : COLOR;
	float4 tex : TEXCOORD;
    float4 normal : NORMAL;
};



float4 PS( PS_IN input ) : SV_Target
{
	return input.col;
	/*return  imageFG.Sample(pictureSampler,input.tex);*/
    }
";
        //angle variable
        private static float _angleHorz = 0.0f, _angleVert = 0.0f, _angleVelHorz = 0, _angleVelVert = 0, RotationSpeed = 1.0f, Damping = 0.92f;
        //modell variable
        private Mesh Mesh, MeshFace;
        //variable for color
        private IShaderParam VColorParam;
        private IShaderParam _vTextureParam;
        private ImageData _imgData1;
        private ImageData _imgData2;
        private ITexture _iTex1;
        private ITexture _iTex2;
        private ShaderProgram sp;

        public override void Init()
        {
            RC.ClearColor = new float4(0.5f, 0.5f, 0.5f, 1);
            RC.ClearDepth = 1.0f;
            //initialize the variable
             Mesh = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            


            //ShaderProgram sp = RC.CreateShader(Vs, Ps);
            sp = RC.CreateShader(Vs, Ps);
            _vTextureParam = sp.GetShaderParam("Testfarbe");
            

            _imgData1 = RC.LoadImage("Assets/world_map.jpg");
            

            _iTex1 = RC.CreateTexture(_imgData1);

        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);


            //float4x4 mtxRot = float4x4.CreateRotationY(0) * float4x4.CreateRotationZ(0);
            //float4x4 mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);

            //float4x4 mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);

            //RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;


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

            // move per keyboard
            if (Input.Instance.IsKeyDown(KeyCodes.Left))
                _angleHorz -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Right))
                _angleHorz += RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Up))
                _angleVert -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Down))
                _angleVert += RotationSpeed * (float)Time.Instance.DeltaTime;

            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            var mtxCam = float4x4.LookAt(0, 200, 500, 0, 0, 0, 0, 1, 0);

            // first mesh
          
            RC.ModelView = float4x4.CreateTranslation(0, -50, 0) * mtxRot * float4x4.CreateTranslation(-150, 0, 0) * mtxCam;
            
            RC.SetShader(sp);
        

            //mapping
            RC.SetShaderParam(_vTextureParam, new float4(0.0f,1.0f, 0.0f, 1.0f));
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