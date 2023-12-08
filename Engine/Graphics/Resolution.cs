using System;
using Microsoft.Xna.Framework;

namespace Engine.Graphics
{
    public static class Resolution
    {
        /// <summary>
        /// Resolution scale mode affects when resolution upscaling happens.
        /// </summary>
        public enum ResolutionScaleMode 
        {
            /// <summary>
            /// Renderable items are upscaled and drawn to a display resolution render target.
            /// </summary>
            /// <remarks>
            /// This makes for smoother looking pixels, but floating point imprecisions can cause artifacts.
            /// </remarks>
            Camera,

            /// <summary>
            /// Renderable items are drawn to a target resolution render target, which is upscaled when drawn to the screen.
            /// </summary>
            /// <remarks>
            /// This is a more accurate pixel perfect rendering, but the upscale can result in jitter for low resolutions.
            /// </remarks>
            RenderTarget
        }

        /// <summary>
        /// Gets the resolution scale mode.
        /// </summary>
        public static ResolutionScaleMode ScaleMode { get; private set; } = ResolutionScaleMode.Camera;

        /// <summary>
        /// Gets the target resolution width
        /// </summary>
        public static int Width { get; private set; }

        /// <summary>
        /// Gets the target resolution height
        /// </summary>
        public static int Height { get; private set; }

        /// <summary>
        /// Gets the intended width for render targets
        /// </summary>
        public static int RenderTargetWidth { get; private set; }

        /// <summary>
        /// Gets the intended height for render targets
        /// </summary>
        public static int RenderTargetHeight { get; private set; }

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
        internal static void Initialize(int width, int height)
        {
            Width = width;
            Height = height;

            if (ScaleMode == ResolutionScaleMode.Camera)
            {
                RenderTargetWidth = App.Graphics.DisplayMode.Width;
                RenderTargetHeight = App.Graphics.DisplayMode.Height;
            }
            else 
            {
                RenderTargetWidth = width;
                RenderTargetHeight = height;
            }
        }

        /// <summary>
        /// Updates the current resolution upscaling matrix if the backbuffer size changes.
        /// </summary>
        internal static void Update()
        {
            var backBufferWidth = App.Graphics.PresentationParameters.BackBufferWidth;
            var backBufferHeight = App.Graphics.PresentationParameters.BackBufferHeight;

            if (!(backBufferWidth == _previousBackBufferWidth && backBufferHeight == _previousBackBufferHeight)) 
            {
                _previousBackBufferWidth = backBufferWidth;
                _previousBackBufferHeight = backBufferHeight;

                var scale = Math.Min((float)backBufferWidth / Width, (float)backBufferHeight / Height);
                var transformationMatrix = Matrix.CreateScale(scale, scale, 1f);

                if (ScaleMode == ResolutionScaleMode.Camera)
                {
                    CameraUpscalingMatrix = transformationMatrix;
                    RenderTargetUpscalingMatrix = Matrix.Identity;
                }
                else
                {
                    CameraUpscalingMatrix = Matrix.Identity;
                    RenderTargetUpscalingMatrix = transformationMatrix;
                }
            }
        }
    }
}
