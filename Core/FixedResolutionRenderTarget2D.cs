using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public sealed class FixedResolutionRenderTarget2D : RenderTarget2D
    {
        public FixedResolutionRenderTarget2D(GraphicsDevice graphicsDevice, int width, int height) : base(graphicsDevice, width, height)
        {
            AspectRatio = width / (float)height;

            SyncWithBackBuffer();
        }

        public float AspectRatio { get; }
        public Viewport Viewport { get; private set; }
        public Viewport LetterboxViewport { get; private set; }
        public Matrix ScaleMatrix { get; private set; }
        public Matrix InvertedScaleMatrix { get; private set; }

        public void SyncWithBackBuffer()
        {
            var backBufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            var backBufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            var scale = MathF.Min((float)backBufferWidth / Width, (float)backBufferHeight / Height);

            ScaleMatrix = Matrix.CreateScale(scale, scale, 1f);
            InvertedScaleMatrix = Matrix.Invert(ScaleMatrix);

            var width = backBufferWidth;
            var height = (int)(backBufferWidth / AspectRatio + 0.5f);

            if (height > backBufferHeight)
            {
                height = backBufferHeight;
                width = (int)(height * AspectRatio + 0.5f);
            }

            Viewport = new Viewport(0, 0, Width, Height);
            LetterboxViewport = new Viewport(backBufferWidth / 2 - width / 2, backBufferHeight / 2 - height / 2, width, height);
        }

        public Vector2 Project(Vector2 position)
        {
            position.X += LetterboxViewport.X;
            position.Y += LetterboxViewport.Y;

            return Vector2.Transform(position, ScaleMatrix);
        }

        public Vector2 Unproject(Vector2 position)
        {
            position.X -= LetterboxViewport.X;
            position.Y -= LetterboxViewport.Y;

            return Vector2.Transform(position, InvertedScaleMatrix);
        }
    }
}
