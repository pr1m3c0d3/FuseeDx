﻿using Fusee.Engine;
using Fusee.Math;

namespace Examples.CubeAndTiles
{
    public class Field
    {
        private readonly Level _curLevel;
        private readonly Mesh _fieldMesh;
        private readonly int _fieldId;

        internal int[] CoordXY { get; private set; }

        private float _posZ;
        private float _veloZ;
        private float _curBright;

        private readonly float _randomRotZ;

        internal FieldTypes Type { get; private set; }
        internal FieldStates State { get; private set; }

        // enums
        public enum FieldTypes
        {
            FtNull = 0,
            FtStart = 1,
            FtEnd = 3,
            FtNormal = 2
        }

        public enum FieldStates
        {
            FsLoading,
            FsAlive,
            FsDead
        }

        // constructor
        public Field(Level curLevel, int id, int x, int y, FieldTypes type)
        {
            _curLevel = curLevel;
            _fieldMesh = _curLevel.GlobalFieldMesh;
            _fieldId = id;

            CoordXY = new[] {x, y};

            _posZ = 0.0f;
            _veloZ = 0.0f;
            _curBright = 1.0f;
            
            _randomRotZ = curLevel.ObjRandom.Next(0, 4);

            Type = type;
            State = FieldStates.FsLoading;
        }

        // methods
        public void ResetField()
        {
            State = FieldStates.FsLoading;

            _posZ = -_fieldId/2.0f;
            _veloZ = 0.1f;

            // default brightness: z coord divided by maximum dist
            _curBright = 1 - (_posZ/(-_curLevel.FieldCount/2.0f));
        }

        public void DeadField()
        {
            if (State != FieldStates.FsDead)
            {
                State = FieldStates.FsDead;

                _posZ = 0;
                _veloZ = (Type == FieldTypes.FtEnd) ? -0.4f : -0.1f;
            }
        }

        private void LoadAnimation()
        {
            if (State != FieldStates.FsLoading) return;

            _veloZ = System.Math.Max(-0.01f, -_posZ/10.0f);
            _posZ += _veloZ;

            _curBright = 1 - (_posZ)/(-_curLevel.FieldCount/2.0f);

            if (_posZ > -0.01f)
            {
                _posZ = 0;
                _veloZ = 0;
                _curBright = 1.0f;

                State = FieldStates.FsAlive;
            }                
        }

        private void DeadAnimation()
        {
            if (State != FieldStates.FsDead) return;

            if (_curBright > 0.0f)
            {
                _posZ += _veloZ;
                _curBright -= .02f;
            }
            else
                _curBright = 0;
        }

        public void Render(float4x4 mtxObjRot, bool onlyRender = false)
        {
            // do not render dead fields with brightness <= 0
            if ((_curBright <= 0) && (State == FieldStates.FsDead))
                return;

            if (!onlyRender)
            {
                LoadAnimation();
                DeadAnimation();
            }

            // color fields
            float3 vColor;
            var val = 1.0f;

            switch (Type)
            {
                case FieldTypes.FtStart:
                    val = 0.8f;
                    vColor = new float3(0.0f, 1.0f, 0.0f);
                    break;

                case FieldTypes.FtEnd:
                    val = 1.0f;
                    vColor = new float3(1.0f, 0.1f, 0.1f);
                    break;

                case FieldTypes.FtNormal:
                    vColor = new float3(0.8f, 0.8f, 0.8f);
                    break;

                default:
                    vColor = new float3(0.0f, 0.0f, 0.0f);
                    break;
            }

            // translate fields
            var mtxFieldRot = float4x4.CreateRotationZ((float) (_randomRotZ*System.Math.PI/2));

            var mtxObjPos = float4x4.CreateTranslation(CoordXY[0]*200, CoordXY[1]*200,
                                                       _posZ*100 - (RollingCube.CubeSize/2.0f + 15));

            // set translation and color, then render
            _curLevel.RContext.ModelView = _curLevel.AddCameraTrans(mtxObjRot*mtxFieldRot*mtxObjPos);

            _curLevel.RContext.SetShaderParam(_curLevel.VColorObj, new float4(vColor, _curBright * val));
            _curLevel.RContext.SetShaderParamTexture(_curLevel.VTextureObj, _curLevel.TextureField);

            _curLevel.RContext.Render(_fieldMesh);
        }
    }
}
