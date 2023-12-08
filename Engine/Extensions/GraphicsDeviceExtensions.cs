using Microsoft.Xna.Framework.Graphics;

namespace Engine.Extensions
{
    public static class GraphicsDeviceExtensions
    {
        /// <summary>
        /// Returns the displays full viewport
        /// </summary>
        /// <param name="graphics"></param>
        /// <returns></returns>
        public static Viewport GetFullViewport(this GraphicsDevice graphics)
        {
            return new Viewport
            {
                X = 0,
                Y = 0,
                Width = graphics.PresentationParameters.BackBufferWidth,
                Height = graphics.PresentationParameters.BackBufferHeight
            };
        }

        /// <summary>
        /// Returns the displays letterbox viewport
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <returns></returns>
        public static Viewport GetLetterboxViewport(this GraphicsDevice graphics, int targetWidth, int targetHeight) 
        {
            var screenWidth = graphics.PresentationParameters.BackBufferWidth;
            var screenHeight = graphics.PresentationParameters.BackBufferHeight;
            var targetRatio = targetWidth / (float)targetHeight;
            var width = screenWidth;
            var height = (int)(screenWidth / targetRatio + 0.5f);

            if (height > screenHeight)
            {
                height = screenHeight;
                width = (int)(height * targetRatio + 0.5f);
            }

            return new Viewport
            {
                X = screenWidth / 2 - width / 2,
                Y = screenHeight / 2 - height / 2,
                Width = width,
                Height = height
            };
        }
    }
}
