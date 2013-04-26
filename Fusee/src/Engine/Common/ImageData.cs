﻿namespace Fusee.Engine
{
    /// <summary>
    /// Struct containing Image Data for further processing (e.g. texturing)
    /// </summary>
    public struct ImageData
    {
        /// <summary>
        /// The width  
        /// </summary>
        public int Width;
        /// <summary>
        /// The height
        /// </summary>
        public int Height;
        /// <summary>
        /// Number of bytes in one row 
        /// </summary>
        public int Stride;
        /// <summary>
        /// The pixel data array
        /// </summary>
        public byte[] PixelData;
    }
}