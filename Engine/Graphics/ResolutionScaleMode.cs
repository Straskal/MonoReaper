namespace Engine.Graphics
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
}
