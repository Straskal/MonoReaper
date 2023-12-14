using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Extensions;

namespace Engine.Graphics
{
    /// <summary>
    /// The virtual resolution that handles scaling and viewport logic
    /// </summary>
    public sealed class VirtualResolution : IDisposable
    {
        private readonly GraphicsDevice _graphicsDevice;

        private int _previousBackBufferWidth;
        private int _previousBackBufferHeight;

        public VirtualResolution(GraphicsDevice graphicsDevice, int targetWidth, int targetHeight, ResolutionScaleMode resolutionScaleMode)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

            Width = targetWidth;
            Height = targetHeight;
            ScaleMode = resolutionScaleMode;

            int renderTargetWidth;
            int renderTargetHeight;

            switch (resolutionScaleMode)
            {
                case ResolutionScaleMode.Renderer:
                    renderTargetWidth = graphicsDevice.DisplayMode.Width;
                    renderTargetHeight = graphicsDevice.DisplayMode.Height;
                    break;
                case ResolutionScaleMode.Viewport:
                default:
                    renderTargetWidth = targetWidth;
                    renderTargetHeight = targetHeight;
                    break;
            }

            RenderTarget = new RenderTarget2D(graphicsDevice, renderTargetWidth, renderTargetHeight);
        }

        /// <summary>
        /// Gets target resolution width
        /// </summary>
        public int Width
        {
            get;
        }

        /// <summary>
        /// Gets the target resolution height
        /// </summary>
        public int Height
        {
            get;
        }

        /// <summary>
        /// Gets the resolution scale mode
        /// </summary>
        public ResolutionScaleMode ScaleMode
        {
            get;
        }

        public RenderTarget2D RenderTarget
        {
            get;
        }

        /// <summary>
        /// Gets the full display viewport
        /// </summary>
        public Viewport FullViewport
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the scaled, letterbox or pillarbox viewport
        /// </summary>
        public Viewport LetterboxViewport
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the renderer scale matrix
        /// </summary>
        public Matrix RendererScaleMatrix
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the viewport scale matrix
        /// </summary>
        public Matrix ViewportScaleMatrix
        {
            get;
            private set;
        }

        /// <summary>
        /// Updates the resolution to match any window changes. TODO: This could probably just be wired up to the window size changed event.
        /// </summary>
        public void Update()
        {
            var backBufferWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;
            var backBufferHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;

            if (!(backBufferWidth == _previousBackBufferWidth && backBufferHeight == _previousBackBufferHeight))
            {
                _previousBackBufferWidth = backBufferWidth;
                _previousBackBufferHeight = backBufferHeight;

                var scale = Math.Min((float)backBufferWidth / Width, (float)backBufferHeight / Height);
                var transformationMatrix = Matrix.CreateScale(scale, scale, 1f);

                FullViewport = _graphicsDevice.GetFullViewport();
                LetterboxViewport = _graphicsDevice.GetLetterboxViewport(Width, Height);

                switch (ScaleMode)
                {
                    case ResolutionScaleMode.Renderer:
                        RendererScaleMatrix = transformationMatrix;
                        ViewportScaleMatrix = Matrix.Identity;
                        break;
                    case ResolutionScaleMode.Viewport:
                        RendererScaleMatrix = Matrix.Identity;
                        ViewportScaleMatrix = transformationMatrix;
                        break;
                }
            }
        }

        /// <summary>
        /// Transforms a world position to screen position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 ToScreen(Vector2 position)
        {
            position.X += LetterboxViewport.X;
            position.Y += LetterboxViewport.Y;

            return Vector2.Transform(position, ViewportScaleMatrix);
        }

        /// <summary>
        /// Transforms a screen position to world position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 ToVirtualScreen(Vector2 position)
        {
            position.X -= LetterboxViewport.X;
            position.Y -= LetterboxViewport.Y;

            return Vector2.Transform(position, Matrix.Invert(ViewportScaleMatrix));
        }

        public void Dispose()
        {
            RenderTarget.Dispose();
        }
    }
}
