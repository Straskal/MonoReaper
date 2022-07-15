using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Graphics
{
    public static class Renderer
    {
        public static Texture2D BlankTexture => _blankTexture;

        private static SpriteBatch _spriteBatch;
        private static Texture2D _blankTexture;
        private static RenderTarget2D _currentTarget;
        private static Effect _currentEffect;
        private static Matrix _currentTransformation;

        internal static void Initialize()
        {
            _spriteBatch = new SpriteBatch(App.Graphics);
            _blankTexture = new Texture2D(App.Graphics, 1, 1);
            _blankTexture.SetData(new[] { Color.White });
        }

        internal static void Unload()
        {
            _spriteBatch.Dispose();
        }

        internal static void BeginDraw(Matrix matrix, RenderTarget2D target = null)
        {
            if (_currentTarget != target) 
            {
                _currentTarget = target;

                App.Graphics.SetRenderTarget(target);
                App.Graphics.LetterboxClear(App.ViewportWidth, App.ViewportHeight, Color.Transparent);
            }

            _currentTransformation = matrix;
            _currentEffect = null;

            Prepare();
        }

        internal static void EndDraw()
        {
            _spriteBatch.End();
        }

        public static void Draw(Texture2D texture, Vector2 position, Color color)
        {
            SetEffect(null);

            _spriteBatch.Draw(texture, position, color);
        }

        public static void Draw(Texture2D texture, Vector2 position, Color color, Effect effect)
        {
            SetEffect(effect);

            _spriteBatch.Draw(texture, position, color);
        }

        public static void Draw(Texture2D texture, Rectangle source, Rectangle destination, Color color, bool flipped, Effect effect = null)
        {
            SetEffect(effect);

            _spriteBatch.Draw(texture, destination, source, color, 0, Vector2.Zero, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public static void Draw(Texture2D texture, Rectangle source, Vector2 position, Color color, bool flipped, Effect effect = null)
        {
            SetEffect(effect);

            _spriteBatch.Draw(texture, position, source, color, 0, Vector2.Zero, Vector2.One, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public static void DrawRectangle(Rectangle rectangle, Color color)
        {
            SetEffect(null);

            _spriteBatch.Draw(_blankTexture, rectangle, null, color);
        }

        public static void DrawRectangleOutline(Rectangle rectangle, Color color, int lineWidth = 1)
        {
            SetEffect(null);

            _spriteBatch.Draw(_blankTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            _spriteBatch.Draw(_blankTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            _spriteBatch.Draw(_blankTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            _spriteBatch.Draw(_blankTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }

        private static void Prepare()
        {
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullCounterClockwise,
                _currentEffect,
                _currentTransformation
            );
        }

        private static void SetEffect(Effect effect)
        {
            if (_currentEffect != effect)
            {
                _currentEffect = effect;

                EndDraw();
                Prepare();
            }
        }
    }
}
