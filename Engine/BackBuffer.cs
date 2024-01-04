using System;
using Engine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public sealed class BackBuffer : IDisposable
    {
        private readonly GraphicsDevice _graphicsDevice;

        private int _previousBackBufferWidth;
        private int _previousBackBufferHeight;

        public BackBuffer(GraphicsDevice graphicsDevice, int width, int height, ResolutionScaleMode scaleMode)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

            Width = width;
            Height = height;
            ScaleMode = scaleMode;

            int virtualBufferWidth;
            int virtualBufferHeight;

            switch (scaleMode)
            {
                case ResolutionScaleMode.Renderer:
                    virtualBufferWidth = graphicsDevice.DisplayMode.Width;
                    virtualBufferHeight = graphicsDevice.DisplayMode.Height;
                    break;
                case ResolutionScaleMode.Viewport:
                default:
                    virtualBufferWidth = width;
                    virtualBufferHeight = height;
                    break;
            }

            VirtualBackBuffer = new RenderTarget2D(graphicsDevice, virtualBufferWidth, virtualBufferHeight);
        }

        public int Width { get; }
        public int Height { get; }
        public ResolutionScaleMode ScaleMode { get; }
        public RenderTarget2D VirtualBackBuffer { get; }
        public Viewport FullViewport { get; private set; }
        public Viewport LetterboxViewport { get; private set; }
        public Matrix ScaleMatrix { get; private set; }
        public Matrix InvertedScaleMatrix { get; private set; }
        public Matrix RendererScaleMatrix { get; private set; }
        public Matrix VirtualBackBufferScaleMatrix { get; private set; }

        public void Update()
        {
            var backBufferWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;
            var backBufferHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;

            if (!(backBufferWidth == _previousBackBufferWidth && backBufferHeight == _previousBackBufferHeight))
            {
                _previousBackBufferWidth = backBufferWidth;
                _previousBackBufferHeight = backBufferHeight;

                var scale = Math.Min((float)backBufferWidth / Width, (float)backBufferHeight / Height);

                ScaleMatrix = Matrix.CreateScale(scale, scale, 1f);
                InvertedScaleMatrix = Matrix.Invert(ScaleMatrix);
                FullViewport = _graphicsDevice.GetFullViewport();
                LetterboxViewport = _graphicsDevice.GetLetterboxViewport(Width, Height);

                switch (ScaleMode)
                {
                    case ResolutionScaleMode.Renderer:
                        RendererScaleMatrix = ScaleMatrix;
                        VirtualBackBufferScaleMatrix = Matrix.Identity;
                        break;
                    case ResolutionScaleMode.Viewport:
                    default:
                        RendererScaleMatrix = Matrix.Identity;
                        VirtualBackBufferScaleMatrix = ScaleMatrix;
                        break;
                }
            }
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

        public void Dispose()
        {
            VirtualBackBuffer.Dispose();
        }
    }
}
