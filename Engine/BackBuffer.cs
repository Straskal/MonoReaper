using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public sealed class BackBuffer : IDisposable
    {
        private readonly GameWindow window;
        private readonly GraphicsDevice graphicsDevice;

        public BackBuffer(GameWindow window, GraphicsDevice graphicsDevice, int width, int height, bool isPixelPerfect = true)
        {
            this.window = window ?? throw new ArgumentNullException(nameof(window));
            this.graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            Width = width;
            Height = height;
            IsPixelPerfect = isPixelPerfect;
            RenderTarget = new RenderTarget2D(graphicsDevice, isPixelPerfect ? width : graphicsDevice.DisplayMode.Width, isPixelPerfect ? height : graphicsDevice.DisplayMode.Height);
            window.ClientSizeChanged += OnWindowClientSizeChanged;
            Update();
        }

        public int Width { get; }
        public int Height { get; }
        public bool IsPixelPerfect { get; }
        public RenderTarget2D RenderTarget { get; }
        public Viewport LetterboxViewport { get; private set; }
        public Viewport CameraViewport { get; private set; }
        public Viewport RenderTargetViewport { get; private set; }
        public Matrix ScaleMatrix { get; private set; }
        public Matrix InvertedScaleMatrix { get; private set; }
        public Matrix CameraScaleMatrix { get; private set; }
        public Matrix RenderTargetScaleMatrix { get; private set; }

        public void Update()
        {
            var backBufferWidth = graphicsDevice.PresentationParameters.BackBufferWidth;
            var backBufferHeight = graphicsDevice.PresentationParameters.BackBufferHeight;
            var scale = MathF.Min((float)backBufferWidth / Width, (float)backBufferHeight / Height);

            ScaleMatrix = Matrix.CreateScale(scale, scale, 1f);
            InvertedScaleMatrix = Matrix.Invert(ScaleMatrix);

            var targetRatio = Width / (float)Height;
            var width = backBufferWidth;
            var height = (int)(backBufferWidth / targetRatio + 0.5f);

            if (height > backBufferHeight)
            {
                height = backBufferHeight;
                width = (int)(height * targetRatio + 0.5f);
            }

            LetterboxViewport = new Viewport(backBufferWidth / 2 - width / 2, backBufferHeight / 2 - height / 2, width, height);

            if (IsPixelPerfect)
            {
                CameraScaleMatrix = Matrix.Identity;
                RenderTargetScaleMatrix = ScaleMatrix;
                CameraViewport = new Viewport(0, 0, Width, Height);
                RenderTargetViewport = LetterboxViewport;
            }
            else
            {
                CameraScaleMatrix = ScaleMatrix;
                RenderTargetScaleMatrix = Matrix.Identity;
                CameraViewport = new Viewport(0, 0, backBufferWidth, backBufferHeight);
                RenderTargetViewport = LetterboxViewport;
            }
        }

        public Vector2 Project(Vector2 position)
        {
            position.X += RenderTargetViewport.X;
            position.Y += RenderTargetViewport.Y;
            return Vector2.Transform(position, RenderTargetScaleMatrix);
        }

        public Vector2 Unproject(Vector2 position)
        {
            position.X -= RenderTargetViewport.X;
            position.Y -= RenderTargetViewport.Y;
            return Vector2.Transform(position, Matrix.Invert(RenderTargetScaleMatrix));
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
