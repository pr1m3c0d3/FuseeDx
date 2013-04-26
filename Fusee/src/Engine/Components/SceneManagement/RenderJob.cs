﻿using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    /// <summary>
    /// Provides the virtual SubmitWork method.
    /// </summary>
    public class RenderJob
    {
        /// <summary>
        /// The SubmitWork method will be overwritten by a visited Component that "want's" to be rendered.
        /// Therefore a RenderContext is needed.
        /// </summary>
        virtual public void SubmitWork(RenderContext renderContext)
        { 
        
        }

    }
}
