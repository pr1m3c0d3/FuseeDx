using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.Simple
{
    public class Simple : RenderCanvas
    {
        // angle variables
       

        public string Vs = @"
            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
                    
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vUV = fuUV;
            }";

        public string Ps = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform vec4 vColor;
            varying vec3 vNormal;

            void main()
            {
                gl_FragColor = vColor * dot(vNormal, vec3(0, 0, 1));
            }";

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
            _vTextureParam = sp.GetShaderParam("vColor");


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
            //var mtxRot = float4x4.CreateRotationY(0) * float4x4.CreateRotationZ(0);


            //// first mesh
            ////RC.ModelView = float4x4.CreateTranslation(0, 0, 0) * mtxRot * float4x4.CreateTranslation(-0, 0, 0) * mtxCam;
            //RC.ModelView = mtxRot * (new float4x4(1.0f, 0.0f, 0.0f, 0.0f,
            //                                     0.0f, 1.0f, 0.0f, 0.0f,
            //                                     0.0f, 0.0f, 1.0f, 0.0f,
            //                                     0.0f, 0.0f, 0.0f, 1.0f));
            RC.SetShader(sp);

            //mapping
            RC.SetShaderParam(_vTextureParam, new float4(0.0f, 1.0f, 0.0f, 1.0f));
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
            var app = new Simple();
            app.Run();
        }
    }
}