using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public sealed class BackBuffer : IDisposable
    {
        private readonly GameWindow window;
        private readonly GraphicsDevice graphicsDevice;

        public BackBuffer(GameWindow window, GraphicsDevice graphicsDevice, int width, int height)
        {
            this.window = window ?? throw new ArgumentNullException(nameof(window));
            this.graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

            Width = width;
            Height = height;
            AspectRatio = width / (float)height;
            RenderTarget = new RenderTarget2D(graphicsDevice, width, height);    
            
            window.ClientSizeChanged += OnWindowClientSizeChanged;
            Update();
        }

        public int Width { get; }
        public int Height { get; }
        public float AspectRatio { get; }
        public RenderTarget2D RenderTarget { get; }
        public Viewport RenderTargetViewport { get; private set; }
        public Viewport LetterboxViewport { get; private set; }
        public Matrix ScaleMatrix { get; private set; }
        public Matrix InvertedScaleMatrix { get; private set; }

        public void Update()
        {
            var backBufferWidth = graphicsDevice.PresentationParameters.BackBufferWidth;
            var backBufferHeight = graphicsDevice.PresentationParameters.BackBufferHeight;
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

            RenderTargetViewport = new Viewport(0, 0, Width, Height);
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

        public void Dispose()
        {
            window.ClientSizeChanged -= OnWindowClientSizeChanged;
            RenderTarget.Dispose();
        }

        private void OnWindowClientSizeChanged(object sender, EventArgs eventArgs)
        {
            Update();
        }
    }
}
