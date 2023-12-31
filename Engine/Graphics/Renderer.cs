﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Graphics
{
    /// <summary>
    /// This class contains all draw functions as well as graphics device functions.
    /// </summary>
    public class Renderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly BackBuffer _backBuffer;
        private readonly SpriteBatch _spriteBatch;

        private Matrix _transformationMatrix;
        private Effect _effect;

        /// <summary>
        /// A blank 1px x 1px white texture. Used for drawing rectangles and missing textures.
        /// </summary>
        public static Texture2D BlankTexture
        {
            get;
            private set;
        }

        public Renderer(GraphicsDevice graphicsDevice, BackBuffer backBuffer) 
        {
            _graphicsDevice = graphicsDevice;
            _backBuffer = backBuffer;
            _spriteBatch = new SpriteBatch(graphicsDevice);

            BlankTexture = new Texture2D(graphicsDevice, 1, 1);
            BlankTexture.SetData(new[] { Color.White });
        }

        internal void Dispose()
        {
            _spriteBatch.Dispose();
            BlankTexture.Dispose();
        }

        /// <summary>
        /// Sets the current render target on the graphics device.
        /// </summary>
        /// <param name="renderTarget2D"></param>
        public void SetTarget(RenderTarget2D renderTarget2D)
        {
            _graphicsDevice.SetRenderTarget(renderTarget2D);
        }

        /// <summary>
        /// Sets the graphics device viewport
        /// </summary>
        /// <param name="viewport"></param>
        public void SetViewport(Viewport viewport) 
        {
            _graphicsDevice.Viewport = viewport;
        }

        /// <summary>
        /// Clears the current render target
        /// </summary>
        public void Clear() 
        {
            _graphicsDevice.Clear(Color.Black);
        }

        public void BeginDraw()
        {
            _transformationMatrix = _backBuffer.RendererScaleMatrix;
            BeginSpriteBatch();
        }

        public void BeginDraw(Matrix transformationMatrix)
        {
            _transformationMatrix = transformationMatrix;
            BeginSpriteBatch();
        }

        public void EndDraw()
        {
            _transformationMatrix = default;
            _effect = null;
            EndSpriteBatch();
        }

        public void Draw(Texture2D texture, Vector2 position)
        {
            Draw(texture, position, null, Color.White, SpriteEffects.None, null);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? source)
        {
            Draw(texture, position, source, Color.White, SpriteEffects.None, null);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color color)
        {
            Draw(texture, position, source, color, SpriteEffects.None, null);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color color, SpriteEffects spriteEffects)
        {
            Draw(texture, position, source, color, spriteEffects, null);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color color, SpriteEffects spriteEffects, Effect effect)
        {
            SwapEffectIfNeeded(effect);
            _spriteBatch.Draw(texture, position, source, color, 0f, Vector2.Zero, Vector2.One, spriteEffects, 0);
        }

        public void Draw(Texture2D texture, Rectangle destination, Rectangle source)
        {
            Draw(texture, destination, source, Color.White, SpriteEffects.None, null);
        }

        public void Draw(Texture2D texture, Rectangle destination, Rectangle source, Color color)
        {
            Draw(texture, destination, source, color, SpriteEffects.None, null);
        }

        public void Draw(Texture2D texture, Rectangle destination, Rectangle source, Color color, SpriteEffects spriteEffects)
        {
            Draw(texture, destination, source, color, spriteEffects, null);
        }

        public void Draw(Texture2D texture, Rectangle destination, Rectangle source, Color color, SpriteEffects spriteEffects, Effect effect)
        {
            SwapEffectIfNeeded(effect);
            _spriteBatch.Draw(texture, destination, source, color, 0f, Vector2.Zero, spriteEffects, 0);
        }

        public void DrawString(SpriteFont spriteFont, string text, float x, float y, Color color)
        {
            _spriteBatch.DrawString(spriteFont, text, new Vector2(x, y), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            _spriteBatch.DrawString(spriteFont, text, position, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public void DrawRectangle(Rectangle rectangle, Color color)
        {
            _spriteBatch.Draw(BlankTexture, rectangle, color);
        }

        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            _spriteBatch.Draw(BlankTexture, new Rectangle(x, y, width, height), color);
        }

        public void DrawLine(Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(point1, distance, angle, color, thickness);
        }

        public void DrawLine(Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);

            _spriteBatch.Draw(BlankTexture, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public void DrawRectangleOutline(Rectangle rectangle, Color color)
        {
            // Top
            _spriteBatch.Draw(BlankTexture, new Rectangle(rectangle.X, rectangle.Y, 1, rectangle.Height), color);
            // Left
            _spriteBatch.Draw(BlankTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, 1), color);
            // Right
            _spriteBatch.Draw(BlankTexture, new Rectangle(rectangle.X + rectangle.Width - 1, rectangle.Y, 1, rectangle.Height), color);
            // Bottom
            _spriteBatch.Draw(BlankTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - 1, rectangle.Width, 1), color);
        }

        public void DrawCircleOutline(float x, float y, float radius, int resolution, Color color)
        {
            for (var i = 0; i < 360; i += resolution) 
            {
                var pos = new Vector2(x, y);
                var p1 = new Vector2(MathF.Cos(MathHelper.ToRadians(i)), MathF.Sin(MathHelper.ToRadians(i))) * radius;
                var p2 = new Vector2(MathF.Cos(MathHelper.ToRadians(i + resolution)), MathF.Sin(MathHelper.ToRadians(i + resolution))) * radius;

                DrawLine(pos + p1, pos + p2, color);
            }
        }

        /// <summary>
        /// Check if the given effect changes the current effect. If so, flush the current batch and then start a new one with the new effect.
        /// </summary>
        /// <param name="effect"></param>
        private void SwapEffectIfNeeded(Effect effect) 
        {
            if (_effect != effect)
            {
                EndSpriteBatch();
                _effect = effect;
                BeginSpriteBatch();
            }
        }

        private void BeginSpriteBatch() 
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

        private void EndSpriteBatch()
        {
            _spriteBatch.End();
        }
    }
}
