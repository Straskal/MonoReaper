using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine
{
    public class Renderer : SpriteBatch
    {
        public Renderer(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            BlankTexture = new Texture2D(graphicsDevice, 1, 1);
            BlankTexture.SetData(new[] { Color.White });
        }

        public static Texture2D BlankTexture { get; private set; }

        public void Begin(Matrix transformMatrix)
        {
            Begin(
                 sortMode: SpriteSortMode.Deferred,
                 blendState: BlendState.AlphaBlend,
                 samplerState: SamplerState.PointClamp,
                 depthStencilState: DepthStencilState.Default,
                 rasterizerState: RasterizerState.CullCounterClockwise,
                 effect: null,
                 transformMatrix: transformMatrix
            );
        }

        public void Draw(Sprite sprite, Vector2 position) 
        {
            Draw(sprite.Texture, position, sprite.SourceRectangle, sprite.Color, 0f, Vector2.Zero, 1f, sprite.SpriteEffects, 0f);
        }

        public void DrawRectangle(Rectangle rectangle, Color color)
        {
            Draw(BlankTexture, rectangle, color);
        }

        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            Draw(BlankTexture, new Rectangle(x, y, width, height), color);
        }

        public void DrawRectangleOutline(Rectangle rectangle, Color color, int lineWidth = 1)
        {
            Draw(BlankTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            Draw(BlankTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            Draw(BlankTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            Draw(BlankTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
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

            Draw(BlankTexture, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                BlankTexture.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
