using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Core.Graphics
{
    public static class Renderer
    {
        public static Texture2D BlankTexture => _texture;

        private static SpriteBatch _spriteBatch;
        private static Texture2D _texture;
        private static Effect _effect;
        private static Matrix _transformation;

        internal static void Initialize()
        {
            _spriteBatch = new SpriteBatch(App.Graphics);
            _texture = new Texture2D(App.Graphics, 1, 1);
            _texture.SetData(new[] { Color.White });
        }

        internal static void BeginDraw(Matrix matrix)
        {
            _transformation = matrix;
            _effect = null;

            CreateBlackBars();
            PrepareBatch();
        }

        private static void PrepareBatch()
        {
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullCounterClockwise,
                _effect,
                _transformation
            );
        }

        public static void Draw(Texture2D texture, Vector2 position, Color color)
        {
            HandleEffectChange(null);

            _spriteBatch.Draw(texture, position, color);
        }

        public static void Draw(Texture2D texture, Rectangle source, Rectangle destination, Color color, bool flipped, Effect effect = null)
        {
            HandleEffectChange(effect);

            _spriteBatch.Draw(texture, destination, source, color, 0, Vector2.Zero, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public static void Draw(Texture2D texture, Rectangle source, Vector2 position, Color color, bool flipped, Effect effect = null)
        {
            HandleEffectChange(effect);

            _spriteBatch.Draw(texture, position, source, color, 0, Vector2.Zero, Vector2.One, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public static void DrawRectangle(Rectangle rectangle, Color color)
        {
            HandleEffectChange(null);

            _spriteBatch.Draw(_texture, rectangle, null, color);
        }

        public static void DrawRectangleOutline(Rectangle rectangle, Color color, int lineWidth = 1)
        {
            HandleEffectChange(null);

            _spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            _spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            _spriteBatch.Draw(_texture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            _spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }

        private static void HandleEffectChange(Effect effect)
        {
            if (_effect != effect)
            {
                _effect = effect;

                EndDraw();
                PrepareBatch();
            }
        }

        internal static void EndDraw()
        {
            _spriteBatch.End();
        }

        internal static void Unload()
        {
            _spriteBatch.Dispose();
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
