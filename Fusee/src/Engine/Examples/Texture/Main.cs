﻿using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.Texture
{
    public class Texture : RenderCanvas
    {
        protected string _vs = @"
            #ifndef GL_ES
               #version 120
            #endif

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
                // vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
                // vec4 norm4 = FUSEE_MVP * vec4(fuNormal, 0.0);
                // vNormal = norm4.xyz;
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
                vUV = fuUV;
            }";

        protected string _ps = @"
           #ifndef GL_ES
              #version 120
           #endif

            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

         
            uniform sampler2D texture1;
            uniform sampler2D texture2;
            uniform vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;

            void main()
            {     
                vec4 tex1 = texture2D(texture1,vUV);
                vec4 tex2 = texture2D(texture2,vUV);
                //gl_FragColor = texture2D(texture1, vUV);        
                gl_FragColor = mix(tex1, tex2, 0.6);  /* *dot(vNormal, vec3(0, 0, 1))*/;
                //gl_FragColor = vColor;
            }";

        private static float _angleHorz = 0.0f, _angleVert = 0.0f, _angleVelHorz = 0, _angleVelVert = 0, _rotationSpeed = 10.0f, _damping = 0.95f;
        protected Mesh _meshSphere, _meshCube;
        protected IShaderParam _vColorParam;
        protected IShaderParam _texture1Param;
        protected IShaderParam _texture2Param;
        protected ImageData _imgData1;
        protected ImageData _imgData2;
        protected ImageData _imgData3;
        protected ITexture _iTex1;
        protected ITexture _iTex2;
        protected ITexture _iTex3;
        

        public override void Init()
        {
                       
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Sphere.obj.model"));
            _meshSphere = geo.ToMesh();

            Geometry geo2 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Cube.obj.model"));
            _meshCube = geo2.ToMesh();


            _angleHorz = 0;
            _rotationSpeed = 10.0f;
            ShaderProgram sp = RC.CreateShader(_vs, _ps);
            RC.SetShader(sp);
            _vColorParam = sp.GetShaderParam("vColor");
            _texture1Param = sp.GetShaderParam("texture1");
            _texture2Param = sp.GetShaderParam("texture2");

            _imgData1 = RC.LoadImage("Assets/world_map.jpg");
            _iTex1 = RC.CreateTexture(_imgData1);
            _imgData2 = RC.LoadImage("Assets/cube_tex.jpg");
            //_imgData2 = RC.CreateImage(500, 500, "Green");
            _imgData2 = RC.TextOnImage(_imgData2, "Verdana", 80f, "FUSEE rocks!", "Black", 0, 30);
            _iTex2 = RC.CreateTexture(_imgData2);
            

        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);


            if (Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = _rotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX) * (float)Time.Instance.DeltaTime;
                _angleVelVert = _rotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY) * (float)Time.Instance.DeltaTime;
            }
            else
            {
                _angleVelHorz *= _damping;
                _angleVelVert *= _damping;
            }
            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            if (Input.Instance.IsKeyDown(KeyCodes.Left))
            {
                _angleHorz -= _rotationSpeed * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Right))
            {
                _angleHorz += _rotationSpeed * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Up))
            {
                _angleVert -= _rotationSpeed * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Down))
            {
                _angleVert += _rotationSpeed * (float)Time.Instance.DeltaTime;
            }

            float4x4 mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            float4x4 mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);


            RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;
            //RC.SetShaderParam(_vColorParam, new float4(0.5f, 0.8f, 0, 1));
            RC.SetShaderParamTexture(_texture1Param, _iTex1);
            RC.Render(_meshSphere);

            RC.ModelView = mtxRot * float4x4.CreateTranslation(100, 0, 0) * mtxCam;
            //RC.SetShaderParam(_vColorParam, new float4(0.8f, 0.5f, 0, 1));
            RC.SetShaderParamTexture(_texture1Param, _iTex2);
            RC.Render(_meshCube);

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
            Texture app = new Texture();
            app.Run();
        }

    }
}