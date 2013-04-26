﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// A derivate of SceneVisitor, used to Visit Renderer, Transformation and Mesh components and submit to renderqueue.
    /// </summary>
    public class SceneVisitorRendering : SceneVisitor
    {
        /// <summary>
        /// The _has transform
        /// </summary>
        private Stack<bool> _hasTransform;
        /// <summary>
        /// The _has renderer
        /// </summary>
        private Stack<bool> _hasRenderer;
        /// <summary>
        /// The _has mesh
        /// </summary>
        private Stack<bool> _hasMesh;
        /// <summary>
        /// The _queue
        /// </summary>
        private SceneManager _queue;
        /// <summary>
        /// The _MTX model view stack
        /// </summary>
        private Stack<float4x4> _mtxModelViewStack;
        /// <summary>
        /// The _mesh stack
        /// </summary>
        private Stack<Mesh> _meshStack;
        /// <summary>
        /// The _ renderer stack
        /// </summary>
        private Stack<Renderer> _RendererStack;
        /// <summary>
        /// The _delta time
        /// </summary>
        private double _deltaTime;
        /// <summary>
        /// The _input
        /// </summary>
        private Input _input;



        /// <summary>
        /// Sets the delta time.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public void SetDeltaTime(double delta)
        {
            _deltaTime = delta;
        }




        /// <summary>
        /// Gets or sets the input.
        /// </summary>
        /// <value>
        /// The input.
        /// </value>
        public Input Input
        {
            set { _input = value; }
            get
            {
                if (_input != null)
                {
                    return _input;
                }else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the delta time.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        public void GetDeltaTime(out double deltaTime)
        {
            deltaTime = _deltaTime;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SceneVisitorRendering"/> class.
        /// </summary>
        /// <param name="queue">The SceneManager reference.</param>
        public SceneVisitorRendering(SceneManager queue)
        {
            _queue = queue;
            
            _mtxModelViewStack = new Stack<float4x4>();
            _meshStack = new Stack<Mesh>();
            _RendererStack = new Stack<Renderer>();
            _hasTransform = new Stack<bool>();
            _hasRenderer = new Stack<bool>();
            _hasMesh = new Stack<bool>();
            _mtxModelViewStack.Push(float4x4.Identity);
            //PrepareDoubleDispatch();
            /*
            
            _meshStack.Push(null);
            _RendererStack.Push(null);
            */
        }
        // TODO: Store Mesh as local variable instead of stacks as it is not used in further traversals.
        /// <summary>
        /// Stores the mesh in internal stack.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        private void StoreMesh(Mesh mesh)
        {
            
            _hasMesh.Pop();
            _meshStack.Push(mesh);
            _hasMesh.Push(true);
            
            if (HasRenderingTriple())
            {
                AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _RendererStack.Peek());
            }

        }

        /// <summary>
        /// Stores the renderer in internal stack.
        /// </summary>
        /// <param name="Renderer">The renderer.</param>
        private void StoreRenderer(Renderer Renderer)
        {

                _hasRenderer.Pop();

            _RendererStack.Push(Renderer);
            _hasRenderer.Push(true);
            if (HasRenderingTriple())
            {
                AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _RendererStack.Peek());
            }

        }



        /// <summary>
        /// Adds the transform to the internal stack.
        /// </summary>
        /// <param name="mtx">The MTX.</param>
        private void AddTransform(float4x4 mtx)
        {
            _hasTransform.Pop();
                _mtxModelViewStack.Push(mtx * _mtxModelViewStack.Pop());

            
            _hasTransform.Push(true);
            if (HasRenderingTriple())
            {
                AddRenderJob(_mtxModelViewStack.Peek(), _meshStack.Peek(), _RendererStack.Peek());
            }
        }

        /// <summary>
        /// Determines whether [has rendering triple].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has rendering triple]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasRenderingTriple()
        {
            return (_hasMesh.Peek() && _hasRenderer.Peek() && _hasTransform.Peek());
        }



        /// <summary>
        /// Pushes this instance.
        /// </summary>
        private void Push()
        {

                _mtxModelViewStack.Push(_mtxModelViewStack.Peek());

            
            _hasMesh.Push(false);
            _hasRenderer.Push(false);
            _hasTransform.Push(false);
        }

        /// <summary>
        /// Pops this instance.
        /// </summary>
        private void Pop()
        {
            _mtxModelViewStack.Pop();
            _hasTransform.Pop();
            if(_meshStack.Count > 0)
            {
                _meshStack.Pop();
                _hasMesh.Pop();  
            }
            
            if(_RendererStack.Count > 0)
            {
                _RendererStack.Pop();
                _hasRenderer.Pop();
            }
            
            
        }

        /// <summary>
        /// Adds the directional lightto the rendering queue.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="color">The color.</param>
        /// <param name="type">The type.</param>
        /// <param name="channel">The channel.</param>
        public void AddLightDirectional(float3 direction, float4 color, Light.LightType type, int channel) 
        {
            RenderDirectionalLight light = new RenderDirectionalLight(direction, color, type, channel );
            _queue.AddLightJob(light);
        }

        /// <summary>
        /// Adds the point light to the rendering queue.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="color">The color.</param>
        /// <param name="type">The type.</param>
        /// <param name="channel">The channel.</param>
        public void AddLightPoint(float3 position, float4 color, Light.LightType type, int channel) 
        {
            RenderPointLight light = new RenderPointLight(position, color, type, channel );
            _queue.AddLightJob(light);
        }

        /// <summary>
        /// Adds the spot light to the rendering queue.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="color">The color.</param>
        /// <param name="type">The type.</param>
        /// <param name="channel">The channel.</param>
        public void AddLightSpot(float3 position, float3 direction, float4 color, Light.LightType type, int channel) 
        {
            RenderSpotLight light = new RenderSpotLight(position, direction, color, type, channel );
            _queue.AddLightJob(light);
        }

        /// <summary>
        /// Adds the render job that consists of Matrix, Renderer and Mesh to the rendering queue.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="mesh">The mesh.</param>
        /// <param name="renderer">The renderer.</param>
        public void AddRenderJob(float4x4 matrix, Mesh mesh, Renderer renderer)
        {
            //Console.WriteLine("_meshstack"+_meshStack.Count+"_viewstack"+_mtxModelViewStack.Count+"_renderstack"+_RendererStack.Count);
            //Console.WriteLine("_hasTransform+"+_hasTransform.Count+"_hasMesh+"+_hasMesh.Count+"_hasRenderer"+_hasRenderer.Count);
            RenderMatrix renderMatrix = new RenderMatrix(matrix);
            _queue.AddRenderJob(renderMatrix);
            RenderMesh renderMesh = new RenderMesh(mesh);
            _queue.AddRenderJob(renderMesh);
            RenderRenderer renderRenderer = new RenderRenderer(renderer);
            _queue.AddRenderJob(renderRenderer);
        }


        // Polymorphic Visitcomponent Methods
        /// <summary>
        /// Visits the specified cEntity to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="cEntity">The cEntity.</param>
        public override void Visit(SceneEntity cEntity)
        {
            Push();
            cEntity.TraverseForRendering(this);
            Pop();
        }
        /*
        private void VisitComponent(ActionCode actionCode)
        {
            actionCode.TraverseForRendering(this);
        }
        private void VisitComponent(Renderer renderer)
        {
            StoreMesh(renderer.mesh);
            StoreRenderer(renderer);
        }
        private void VisitComponent(Transformation transform)
        {
            AddTransform(transform.Matrix);
        }
        private void VisitComponent(DirectionalLight directionalLight)
        {
            directionalLight.TraverseForRendering(this);
        }
        private void VisitComponent(PointLight pointLight)
        {
            pointLight.TraverseForRendering(this);
        }
        private void VisitComponent(SpotLight spotLight)
        {
            spotLight.TraverseForRendering(this);
        }
        private void VisitComponent(Camera camera)
        {
            
            if (_mtxModelViewStack.Peek() != null)
            {
                camera.ViewMatrix = _mtxModelViewStack.Peek();
                _queue.AddCamera(camera.SubmitWork());
            }
        }*/

        /// <summary>
        /// Visits the specified action code to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="actionCode">The action code.</param>
        override public void Visit(ActionCode actionCode)
        {
            actionCode.TraverseForRendering(this);
        }
        /// <summary>
        /// Visits the specified renderer to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        override public void Visit(Renderer renderer)
        {
            StoreMesh(renderer.mesh);
            StoreRenderer(renderer);
        }
        /// <summary>
        /// Visits the specified transform.
        /// </summary>
        /// <param name="transform">The transform.</param>
        override public void Visit(Transformation transform)
        {
            AddTransform(transform.Matrix);
        }
        /// <summary>
        /// Visits the specified directional light to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="directionalLight">The directional light.</param>
        override public void Visit(DirectionalLight directionalLight)
        {
            directionalLight.TraverseForRendering(this);
        }
        /// <summary>
        /// Visits the specified point light to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="pointLight">The point light.</param>
        override public void Visit(PointLight pointLight)
        {
            pointLight.TraverseForRendering(this);
        }
        /// <summary>
        /// Visits the specified spot light to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="spotLight">The spot light.</param>
        override public void Visit(SpotLight spotLight)
        {
            spotLight.TraverseForRendering(this);
        }
        /// <summary>
        /// Visits the specified camera to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="camera">The camera.</param>
        override public void Visit(Camera camera)
        {

            if (_mtxModelViewStack.Peek() != null)
            {
                camera.ViewMatrix = _mtxModelViewStack.Peek();
                _queue.AddCamera(camera.SubmitWork());
            }
        }
        
    }
}
