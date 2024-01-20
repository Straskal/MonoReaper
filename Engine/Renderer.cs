using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine
{
    public class Renderer
    {
        private readonly GraphicsDevice graphicsDevice;
        private readonly SpriteBatch spriteBatch;
        private Matrix transformationMatrix;
        private Effect effect;

        public static Texture2D BlankTexture { get; private set; }

        public Renderer(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);

            BlankTexture = new Texture2D(graphicsDevice, 1, 1);
            BlankTexture.SetData(new[] { Color.White });
        }

        public void Dispose()
        {
            spriteBatch.Dispose();
            BlankTexture.Dispose();
        }

        public void SetTarget(RenderTarget2D renderTarget2D)
        {
            graphicsDevice.SetRenderTarget(renderTarget2D);
        }

        public void SetViewport(Viewport viewport)
        {
            graphicsDevice.Viewport = viewport;
        }

        public void Clear()
        {
            graphicsDevice.Clear(Color.Black);
        }

        public void BeginDraw()
        {
            transformationMatrix = Matrix.Identity;
            BeginSpriteBatch();
        }

        public void BeginDraw(Matrix transformationMatrix)
        {
            this.transformationMatrix = transformationMatrix;
            BeginSpriteBatch();
        }

        public void EndDraw()
        {
            transformationMatrix = default;
            effect = null;
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
            spriteBatch.Draw(texture, position, source, color, 0f, Vector2.Zero, Vector2.One, spriteEffects, 0);
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
            spriteBatch.Draw(texture, destination, source, color, 0f, Vector2.Zero, spriteEffects, 0);
        }

        public void DrawString(SpriteFont spriteFont, string text, float x, float y, Color color)
        {
            spriteBatch.DrawString(spriteFont, text, new Vector2(x, y), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(spriteFont, text, position, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public void DrawRectangle(Rectangle rectangle, Color color)
        {
            spriteBatch.Draw(BlankTexture, rectangle, color);
        }

        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            spriteBatch.Draw(BlankTexture, new Rectangle(x, y, width, height), color);
        }

        public void DrawRectangleOutline(Rectangle rectangle, Color color)
        {
            DrawLine(new Vector2(rectangle.Left, rectangle.Top), new Vector2(rectangle.Right, rectangle.Top), color);
            DrawLine(new Vector2(rectangle.Left, rectangle.Bottom), new Vector2(rectangle.Right, rectangle.Bottom), color);
            DrawLine(new Vector2(rectangle.Left, rectangle.Top), new Vector2(rectangle.Left, rectangle.Bottom), color);
            DrawLine(new Vector2(rectangle.Right, rectangle.Top), new Vector2(rectangle.Right, rectangle.Bottom), color);
        }

        public void DrawCircleOutline(Vector2 position, float radius, int resolution, Color color)
        {
            DrawCircleOutline(position.X, position.Y, radius, resolution, color);
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

            spriteBatch.Draw(BlankTexture, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Check if the given effect changes the current effect. If so, flush the current batch and then start a new one with the new effect.
        /// </summary>
        /// <param name="effect"></param>
        private void SwapEffectIfNeeded(Effect effect)
        {
            if (this.effect != effect)
            {
                this.EndSpriteBatch();
                this.effect = effect;
                this.BeginSpriteBatch();
            }
        }

        private void BeginSpriteBatch()
        {
            spriteBatch.Begin(
                 sortMode: SpriteSortMode.Deferred,
                 blendState: BlendState.AlphaBlend,
                 samplerState: SamplerState.PointClamp,
                 depthStencilState: DepthStencilState.Default,
                 rasterizerState: RasterizerState.CullCounterClockwise,
                 effect: effect,
                 transformMatrix: transformationMatrix);
        }

        private void EndSpriteBatch()
        {
            spriteBatch.End();
        }
    }
}
