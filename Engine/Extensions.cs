using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core
{
    public static class GameTimeExtensions
    {
        public static float GetDeltaTime(this GameTime gameTime) 
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }

    public static class GraphicsDeviceExtensions 
    {
        public static void FullViewportClear(this GraphicsDevice graphics, Color color) 
        {
            var screenWidth = graphics.PresentationParameters.BackBufferWidth;
            var screenHeight = graphics.PresentationParameters.BackBufferHeight;

            graphics.Viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = screenWidth,
                Height = screenHeight
            };

            graphics.Clear(color);
        }

        public static void LetterboxClear(this GraphicsDevice graphics, int targetWidth, int targetHeight, Color color, bool flag = true)
        {
            var screenWidth = graphics.PresentationParameters.BackBufferWidth;
            var screenHeight = graphics.PresentationParameters.BackBufferHeight;

            graphics.Viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = screenWidth,
                Height = screenHeight
            };

            graphics.Clear(color);

            if (!flag)
                return;

            var targetRatio = targetWidth / (float)targetHeight;
            var width = screenWidth;
            var height = (int)(screenWidth / targetRatio + 0.5f);

            if (height > screenHeight)
            {
                height = screenHeight;
                width = (int)(height * targetRatio + 0.5f);
            }

            graphics.Viewport = new Viewport
            {
                X = screenWidth / 2 - width / 2,
                Y = screenHeight / 2 - height / 2,
                Width = width,
                Height = height
            };
        }
    }
}
