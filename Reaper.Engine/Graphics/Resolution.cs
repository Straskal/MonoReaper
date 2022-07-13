using Core;
using Microsoft.Xna.Framework;

namespace Reaper.Engine.Graphics
{
    public enum ResolutionScaleMode 
    {
        PreScale,
        PostScale
    }

    public static class Resolution
    {
        private static Vector3 _resolutionScale;
        private static Matrix _resolutionScaleMatrix;

        public static ResolutionScaleMode ScaleMode { get; set; } = ResolutionScaleMode.PreScale;
        public static int Width { get; set; }
        public static int Height { get; set; }

        public static (int width, int height) RenderTargetResolution 
        {
            get 
            {
                return ScaleMode == ResolutionScaleMode.PostScale
                    ? (Width, Height)
                    : (App.Graphics.PresentationParameters.BackBufferWidth, App.Graphics.PresentationParameters.BackBufferHeight);
            }
        }

        public static Matrix TransformationMatrix
        {
            get
            {
                var scale = (float)App.Graphics.PresentationParameters.BackBufferWidth / Width;

                _resolutionScale.X = scale;
                _resolutionScale.Y = scale;
                _resolutionScale.Z = 1f;

                Matrix.CreateScale(ref _resolutionScale, out _resolutionScaleMatrix);

                return _resolutionScaleMatrix;
            }
        }

        public static Matrix PreScaleTransform => ScaleMode == ResolutionScaleMode.PreScale ? TransformationMatrix : Matrix.Identity;
        public static Matrix PostScaleTransform => ScaleMode == ResolutionScaleMode.PostScale ? TransformationMatrix : Matrix.Identity;
    }
}
