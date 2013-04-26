using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.DreieckDx11
{
    public class DreieckDx11 : RenderCanvas
    {
        public override void Init()
        {
            // is called on startup
        }

        public override void RenderAFrame()
        {
            // is called once a frame
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new DreieckDx11();
            app.Run();
        }

    }
}
