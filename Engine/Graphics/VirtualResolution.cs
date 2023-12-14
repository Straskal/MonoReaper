using Engine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Graphics
{
    public sealed class VirtualResolution
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

            switch (resolutionScaleMode) 
            {
                case ResolutionScaleMode.Camera:
                    RenderTargetWidth = graphicsDevice.DisplayMode.Width;
                    RenderTargetHeight = graphicsDevice.DisplayMode.Height;
                    break;
                case ResolutionScaleMode.RenderTarget:
                    RenderTargetWidth = targetWidth;
                    RenderTargetHeight = targetHeight;
                    break;
            }
        }

        public int Width
        {
            get;
        }

        public int Height
        {
            get;
        }

        public int RenderTargetWidth
        {
            get;
        }

        public int RenderTargetHeight
        {
            get;
        }

        public ResolutionScaleMode ScaleMode
        {
            get;
        }

        public Viewport FullViewport 
        {
            get;
            private set;
        }

        public Viewport LetterboxViewport
        {
            get;
            private set;
        }

        public Viewport CameraViewport
        {
            get;
            private set;
        }

        public Viewport RenderTargetViewport
        {
            get;
            private set;
        }

        public Matrix ScaleTransformationMatrix
        {
            get;
            private set;
        }

        public Matrix RenderTargetScaleMatrix
        {
            get;
            private set;
        }

        public RenderTarget2D CreateRenderTarget() 
        {
            return new RenderTarget2D(_graphicsDevice, RenderTargetWidth, RenderTargetHeight);
        }

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

                if (ScaleMode == ResolutionScaleMode.Camera)
                {
                    ScaleTransformationMatrix = transformationMatrix;
                    RenderTargetScaleMatrix = Matrix.Identity;
                    CameraViewport = LetterboxViewport;
                    RenderTargetViewport = FullViewport;
                }
                else
                {
                    ScaleTransformationMatrix = Matrix.Identity;
                    RenderTargetScaleMatrix = transformationMatrix;
                    CameraViewport = FullViewport;
                    RenderTargetViewport = LetterboxViewport;
                }
            }
        }
    }
}
