﻿using System;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.SolarSystem
{
    internal class RotationScript : ActionCode
    {
        private readonly String[] _planets =
            {
                "Earth", "Moon", "Mercury", "Venus", "Mars", "Jupiter", "Saturn", "Uranus", "Neptun", "Sun"
            };

        private int _currentTargetId;
        private SceneEntity _target;

        public override void Start()
        {
            _target = SceneEntity.FindSceneEntity(_planets[_currentTargetId]);
        }

        public override void Update()
        {
            if (Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                var mousemoveX = Input.Instance.GetAxis(InputAxis.MouseX);
                var mousemoveY = Input.Instance.GetAxis(InputAxis.MouseY);

                transform.LocalEulerAngles += new float3(0, -mousemoveX*100, 0);
                transform.LocalEulerAngles += new float3(-mousemoveY*100, 0, 0);
            }

            if (Input.Instance.IsKeyDown(KeyCodes.Up))
            {
                var timeAdd = Time.Instance.TimeFlow;

                if (Time.Instance.TimeFlow > 2)
                    timeAdd += 0.1f;
                else
                    timeAdd += 0.0025f;

                Time.Instance.TimeFlow = timeAdd;
            }

            if (Input.Instance.IsKeyDown(KeyCodes.Down))
            {
                if (Time.Instance.TimeFlow >= 0)
                {
                    var timeDec = Time.Instance.TimeFlow;

                    if (Time.Instance.TimeFlow > 2)
                        timeDec -= 0.1f;
                    else
                        timeDec -= 0.0025f;

                    Time.Instance.TimeFlow = timeDec;
                }
                else
                    Time.Instance.TimeFlow = 0;
            }

            if (Input.Instance.OnKeyDown(KeyCodes.Right))
            {
                _currentTargetId++;

                if (_currentTargetId >= _planets.Length)
                    _currentTargetId = 0;

                _target = SceneEntity.FindSceneEntity(_planets[_currentTargetId]);
            }

            if (Input.Instance.OnKeyDown(KeyCodes.Left))
            {
                _currentTargetId--;

                if (_currentTargetId <= 0)
                    _currentTargetId = _planets.Length - 1;

                _target = SceneEntity.FindSceneEntity(_planets[_currentTargetId]);
            }

            transform.GlobalPosition = _target.transform.GlobalPosition;
        }
    }
}