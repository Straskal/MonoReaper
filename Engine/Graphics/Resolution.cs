using Microsoft.Xna.Framework;

namespace Engine.Graphics
{
    public static class Resolution
    {
        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static int RenderTargetWidth { get; private set; }
        public static int RenderTargetHeight { get; private set; }
        public static Matrix TransformationMatrix { get; private set; }

        internal static void Initialize(int width, int height)
        {
            Width = width;
            Height = height;
            RenderTargetWidth = App.Graphics.DisplayMode.Width;
            RenderTargetHeight = App.Graphics.DisplayMode.Height;
        }

        internal static void Update()
        {
            var scaleX = (float)App.Graphics.PresentationParameters.BackBufferWidth / Width;
            var scaleY = (float)App.Graphics.PresentationParameters.BackBufferHeight / Height;

            if (scaleY < scaleX)
            {
                TransformationMatrix = Matrix.CreateScale(scaleY, scaleY, 1f);
            }
            else
            {
                TransformationMatrix = Matrix.CreateScale(scaleX, scaleX, 1f);
            }
        }
    }
}
