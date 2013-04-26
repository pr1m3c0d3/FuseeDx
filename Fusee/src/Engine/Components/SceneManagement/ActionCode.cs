﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Fusee.Engine;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// ActionCode class grants access to an SceneEntities Components.
    /// Supported: DeltaTime, Input recognition
    /// </summary>
    public class ActionCode : Component
    {
        //TODO: Test without new calls 
        #region Fields

        public SceneEntity SceneEntity;
        public Transformation transform;
        public Renderer renderer;

        #endregion

        #region Public Members
        /// <summary>
        /// Init is called upon creation of an ActionCode Object and utilizes the access to a SceneEntitie's transform and renderer objects.
        /// </summary>
        public void Init(SceneEntity entity)
        {
            SceneEntity = entity;
            transform = SceneEntity.transform;
            renderer = SceneEntity.renderer;
        }

        /// <summary>
        /// Traverse is giving the Update routine access to the DeltaTime and the Input class and afterwards executes the Update routine.
        /// </summary>
        public void TraverseForRendering(SceneVisitorRendering sceneVisitorRendering)
        {
            Update();
        }

        #endregion

        #region Overrideable Functions
        /// <summary>
        /// Start is called only once. At the time of the call other objects inside the scene are already initialized.
        /// </summary>
        virtual public void Start()
        {
            
        }

        /// <summary>
        /// Update is called every frame. Please avoid placing heavy calculations inside this Function.
        /// </summary>
        virtual public void Update()
        {
            
        }

        public override void Accept(SceneVisitor sv)
        {
            sv.Visit((ActionCode)this);
        }
        #endregion
    }   
}

