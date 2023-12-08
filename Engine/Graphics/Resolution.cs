using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Extensions;

namespace Engine.Graphics
{
    public static class Resolution
    {
        /// <summary>
        /// Gets the resolution scale mode.
        /// </summary>
        public static ResolutionScaleMode ScaleMode { get; internal set; } = ResolutionScaleMode.RenderTarget;

        /// <summary>
        /// Gets the target resolution width
        /// </summary>
        public static int Width { get; internal set; }

        /// <summary>
        /// Gets the target resolution height
        /// </summary>
        public static int Height { get; internal set; }

        /// <summary>
        /// Gets the intended width for render targets
        /// </summary>
        public static int RenderTargetWidth { get; private set; }

        /// <summary>
        /// Gets the intended height for render targets
        /// </summary>
        public static int RenderTargetHeight { get; private set; }

        /// <summary>
        /// Gets the resolution full viewport
        /// </summary>
        public static Viewport FullViewport { get; private set; }

        /// <summary>
        /// Gets the resolution letterbox viewport
        /// </summary>
        public static Viewport LetterboxViewport { get; private set; }

        /// <summary>
        /// Gets the camera viewport
        /// </summary>
        public static Viewport CameraViewport { get; private set; }

        /// <summary>
        /// Gets the render target viewport
        /// </summary>
        public static Viewport RenderTargetViewport { get; private set; }

        /// <summary>
        /// Transformation intended to be applied to entities at the time of drawing them.
        /// </summary>
        public static Matrix CameraUpscalingMatrix { get; private set; }

        /// <summary>
        /// Transformation intended to be applied to an entire render target when sending it to the display.
        /// </summary>
        public static Matrix RenderTargetUpscalingMatrix { get; private set; }

        private static int _previousBackBufferWidth;
        private static int _previousBackBufferHeight;

        /// <summary>
        /// Initializes the resolution.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal static void Initialize()
        {
            if (ScaleMode == ResolutionScaleMode.Camera)
            {
                RenderTargetWidth = App.Instance.Graphics.DisplayMode.Width;
                RenderTargetHeight = App.Instance.Graphics.DisplayMode.Height;
            }
            else 
            {
                RenderTargetWidth = Width;
                RenderTargetHeight = Height;
            }
        }

        /// <summary>
        /// Updates the current resolution upscaling matrix if the backbuffer size changes.
        /// </summary>
        internal static void Update()
        {
            var backBufferWidth = App.Instance.Graphics.PresentationParameters.BackBufferWidth;
            var backBufferHeight = App.Instance.Graphics.PresentationParameters.BackBufferHeight;

            if (!(backBufferWidth == _previousBackBufferWidth && backBufferHeight == _previousBackBufferHeight)) 
            {
                _previousBackBufferWidth = backBufferWidth;
                _previousBackBufferHeight = backBufferHeight;

                var scale = Math.Min((float)backBufferWidth / Width, (float)backBufferHeight / Height);
                var transformationMatrix = Matrix.CreateScale(scale, scale, 1f);

                FullViewport = App.Instance.Graphics.GetFullViewport();
                LetterboxViewport = App.Instance.Graphics.GetLetterboxViewport(Width, Height);

                if (ScaleMode == ResolutionScaleMode.Camera)
                {
                    CameraUpscalingMatrix = transformationMatrix;
                    RenderTargetUpscalingMatrix = Matrix.Identity;
                    CameraViewport = LetterboxViewport;
                    RenderTargetViewport = FullViewport;
                }
                else
                {
                    CameraUpscalingMatrix = Matrix.Identity;
                    RenderTargetUpscalingMatrix = transformationMatrix;
                    CameraViewport = FullViewport;
                    RenderTargetViewport = LetterboxViewport;
                }
            }
        }
    }
}
