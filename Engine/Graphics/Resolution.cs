using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;

namespace Reaper.Engine.Graphics
{
    public enum ResolutionScaleMode 
    {
        PreScale,
        PostScale
    }

    public static class Resolution
    {
        public static int ScreenWidth { get; set; }
        public static int ScreenHeight { get; set; }
        public static int VirtualWidth { get; set; }
        public static int VirtualHeight { get; set; }
        public static Viewport Viewport { get; set; }
        public static float RatioX { get; set; }
        public static float RatioY { get; set; }

        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static int RenderTargetWidth { get; private set; }
        public static int RenderTargetHeight { get; private set; }
        public static ResolutionScaleMode ScaleMode { get; private set; }
        public static Matrix TransformationMatrix { get; private set; }
        public static Matrix PreScaleTransform { get; private set; }
        public static Matrix PostScaleTransform { get; private set; }

        internal static void Initialize(int width, int height, ResolutionScaleMode scaleMode) 
        {
            Width = width;
            Height = height;
            ScaleMode = scaleMode;

            if (scaleMode == ResolutionScaleMode.PreScale)
            {
                RenderTargetWidth = App.Graphics.DisplayMode.Width;
                RenderTargetHeight = App.Graphics.DisplayMode.Height;
            }
            else 
            {
                RenderTargetWidth = Width;
                RenderTargetHeight = Height;
            }
        }

        internal static void Calculate() 
        {
            var scale = (float)App.Graphics.PresentationParameters.BackBufferWidth / Width;

            TransformationMatrix = Matrix.CreateScale(scale, scale, 1f);

            if (ScaleMode == ResolutionScaleMode.PreScale)
            {
                PreScaleTransform = TransformationMatrix;
                PostScaleTransform = Matrix.Identity;
            }
            else
            {
                PreScaleTransform = Matrix.Identity;
                PostScaleTransform = TransformationMatrix;
            }
        }
    }
}
