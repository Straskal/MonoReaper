using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Extensions;

namespace Engine.Graphics
{
    /// <summary>
    /// This class contains all draw functions as well as graphics device functions.
    /// </summary>
    public static class Renderer
    {
        private static GraphicsDevice _graphicsDevice;
        private static SpriteBatch _spriteBatch;
        private static Matrix _transformationMatrix;
        private static Effect _effect;

        /// <summary>
        /// A blank 1px x 1px white texture. Used for drawing rectangles and missing textures.
        /// </summary>
        public static Texture2D BlankTexture
        {
            get;
            private set;
        }

        internal static void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
            BlankTexture = new Texture2D(graphicsDevice, 1, 1);
            BlankTexture.SetData(new[] { Color.White });
        }

        internal static void Deinitialize()
        {
            _spriteBatch.Dispose();
            BlankTexture.Dispose();
        }

        /// <summary>
        /// Sets the current render target on the graphics device.
        /// </summary>
        /// <param name="renderTarget2D"></param>
        public static void SetRenderTarget(RenderTarget2D renderTarget2D)
        {
            _graphicsDevice.SetRenderTarget(renderTarget2D);
        }

        /// <summary>
        /// Clears the entire viewport.
        /// </summary>
        public static void FullViewportClear() 
        {
            _graphicsDevice.FullViewportClear(Color.Black);
        }

        /// <summary>
        /// Clears the entire viewport and sets the viewport to user letterbox or pillarbox.
        /// </summary>
        public static void LetterboxClear()
        {
            _graphicsDevice.LetterboxClear(Resolution.Width, Resolution.Height, Color.Black);
        }

        public static void BeginDraw()
        {
            _transformationMatrix = Matrix.Identity;
            BeginSpriteBatch();
        }

        public static void BeginDraw(Matrix transformationMatrix)
        {
            _transformationMatrix = transformationMatrix;
            BeginSpriteBatch();
        }

        public static void EndDraw()
        {
            _transformationMatrix = default;
            _effect = null;
            EndSpriteBatch();
        }

        public static void Draw(Texture2D texture, Vector2 position)
        {
            Draw(texture, position, null, Color.White, SpriteEffects.None, null);
        }

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source)
        {
            Draw(texture, position, source, Color.White, SpriteEffects.None, null);
        }

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color color)
        {
            Draw(texture, position, source, color, SpriteEffects.None, null);
        }

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color color, SpriteEffects spriteEffects)
        {
            Draw(texture, position, source, color, spriteEffects, null);
        }

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color color, SpriteEffects spriteEffects, Effect effect)
        {
            SwapEffectIfNeeded(effect);
            _spriteBatch.Draw(texture, position, source, color, 0f, Vector2.Zero, Vector2.One, spriteEffects, 0);
        }

        public static void Draw(Texture2D texture, Rectangle destination, Rectangle source)
        {
            Draw(texture, destination, source, Color.White, SpriteEffects.None, null);
        }

        public static void Draw(Texture2D texture, Rectangle destination, Rectangle source, Color color)
        {
            Draw(texture, destination, source, color, SpriteEffects.None, null);
        }

        public static void Draw(Texture2D texture, Rectangle destination, Rectangle source, Color color, SpriteEffects spriteEffects)
        {
            Draw(texture, destination, source, color, spriteEffects, null);
        }

        public static void Draw(Texture2D texture, Rectangle destination, Rectangle source, Color color, SpriteEffects spriteEffects, Effect effect)
        {
            SwapEffectIfNeeded(effect);
            _spriteBatch.Draw(texture, destination, source, color, 0f, Vector2.Zero, spriteEffects, 0);
        }

        public static void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            _spriteBatch.DrawString(spriteFont, text, position, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public static void DrawRectangle(Rectangle rectangle, Color color)
        {
            _spriteBatch.Draw(BlankTexture, rectangle, color);
        }

        public static void DrawRectangleOutline(Rectangle rectangle, Color color, int lineWidth = 1)
        {
            // Top
            _spriteBatch.Draw(BlankTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            // Left
            _spriteBatch.Draw(BlankTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, lineWidth), color);
            // Right
            _spriteBatch.Draw(BlankTexture, new Rectangle(rectangle.X + rectangle.Width - 1, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            // Bottom
            _spriteBatch.Draw(BlankTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth - 1, lineWidth), color);
        }

        /// <summary>
        /// Check if the given effect changes the current effect. If so, flush the current batch and then start a new one with the new effect.
        /// </summary>
        /// <param name="effect"></param>
        private static void SwapEffectIfNeeded(Effect effect) 
        {
            if (_effect != effect)
            {
                EndSpriteBatch();
                _effect = effect;
                BeginSpriteBatch();
            }
        }

        private static void BeginSpriteBatch() 
        {
            _spriteBatch.Begin(
                 sortMode: SpriteSortMode.Deferred,
                 blendState: BlendState.AlphaBlend,
                 samplerState: SamplerState.PointClamp,
                 depthStencilState: DepthStencilState.Default,
                 rasterizerState: RasterizerState.CullCounterClockwise,
                 effect: _effect,
                 transformMatrix: _transformationMatrix);
        }

        private static void EndSpriteBatch()
        {
            _spriteBatch.End();
        }
    }
}
