using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Extensions;

namespace Engine.Graphics
{
    /// <summary>
    /// The virtual screen that handles scaling logic for the target resolution.
    /// </summary>
    public sealed class VirtualScreen : IDisposable
    {
        private readonly GraphicsDevice _graphicsDevice;

        private int _previousBackBufferWidth;
        private int _previousBackBufferHeight;

        public VirtualScreen(GraphicsDevice graphicsDevice, int width, int height, ResolutionScaleMode scaleMode)
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

        /// <summary>
        /// Gets the virtual backbuffer render target
        /// </summary>
        public RenderTarget2D VirtualBackBuffer
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
        /// Gets the screen's scale matrix
        /// </summary>
        public Matrix ScaleMatrix 
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the screen's inverted scale matrix
        /// </summary>
        public Matrix InvertedScaleMatrix 
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
        public Matrix VirtualBackBufferScaleMatrix
        {
            get;
            private set;
        }

        /// <summary>
        /// Updates the virtual screen resolution to match the window size.
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

        /// <summary>
        /// Transforms a virtual screen position to screen position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 ToScreen(Vector2 position)
        {
            position.X += LetterboxViewport.X;
            position.Y += LetterboxViewport.Y;

            return Vector2.Transform(position, ScaleMatrix);
        }

        /// <summary>
        /// Transforms a screen position to a virtual screen position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 ToVirtualScreen(Vector2 position)
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
