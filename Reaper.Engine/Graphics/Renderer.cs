using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Reaper.Engine.Graphics
{
    public static class Renderer
    {
        private static SpriteBatch batcher;
        private static Texture2D texture;
        private static Effect effect;
        private static Matrix transformation;

        internal static void Initialize()
        {
            batcher = new SpriteBatch(App.Graphics);
            texture = new Texture2D(App.Graphics, 1, 1);
            texture.SetData(new[] { Color.White });
        }

        internal static void BeginDraw(Matrix matrix)
        {
            transformation = matrix;
            effect = null;

            CreateBlackBars();
            PrepareBatch();
        }

        private static void PrepareBatch()
        {
            batcher.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullCounterClockwise,
                effect,
                transformation
            );
        }

        public static void Draw(Texture2D texture, Vector2 position, Color color)
        {
            HandleEffectChange(null);
            batcher.Draw(texture, position, color);
        }

        public static void Draw(Texture2D texture, Rectangle source, Rectangle destination, Color color, bool flipped, Effect effect = null)
        {
            HandleEffectChange(effect);
            batcher.Draw(texture, destination, source, color, 0, Vector2.Zero, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public static void Draw(Texture2D texture, Rectangle source, Vector2 position, Color color, bool flipped, Effect effect = null)
        {
            HandleEffectChange(effect);
            batcher.Draw(texture, position, source, color, 0, Vector2.Zero, Vector2.One, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public static void DrawRectangle(Rectangle rectangle, Color color)
        {
            HandleEffectChange(null);
            batcher.Draw(texture, rectangle, null, color);
        }

        private static void HandleEffectChange(Effect effect)
        {
            if (Renderer.effect != effect)
            {
                Renderer.effect = effect;

                EndDraw();
                PrepareBatch();
            }
        }

        internal static void EndDraw()
        {
            batcher.End();
        }

        internal static void Unload()
        {
            batcher.Dispose();
        }

        private static void CreateBlackBars()
        {
            App.Graphics.Viewport = GetFullViewport();
            App.Graphics.Clear(Color.Black);
            App.Graphics.Viewport = GetLargestVirtualViewport();
        }

        private static Viewport GetFullViewport()
        {
            return new Viewport
            {
                X = 0,
                Y = 0,
                Width = App.Graphics.PresentationParameters.BackBufferWidth,
                Height = App.Graphics.PresentationParameters.BackBufferHeight
            };
        }

        private static Viewport GetLargestVirtualViewport()
        {
            // Start off assuming letterbox.
            float targetAspectRatio = App.ViewportWidth / (float)App.ViewportHeight;
            int screenWidth = App.Graphics.PresentationParameters.BackBufferWidth;
            int screenHeight = App.Graphics.PresentationParameters.BackBufferHeight;
            int width = screenWidth;
            int height = (int)(screenWidth / targetAspectRatio + 0.5f);

            // Check if we need pillarbox instead.
            if (height > screenHeight)
            {
                height = screenHeight;
                width = (int)(height * targetAspectRatio + .5f);
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
