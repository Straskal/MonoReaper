namespace Engine
{
    /// <summary>
    /// Resolution scale mode affects when resolution upscaling happens.
    /// </summary>
    public enum ResolutionScaleMode
    {
        /// <summary>
        /// Rendered items are scaled up to the display's resolution when they are drawn.
        /// </summary>
        /// <remarks>
        /// This makes for smoother looking pixels, but floating point imprecisions can cause artifacts.
        /// </remarks>
        Renderer,

        /// <summary>
        /// Rendered items are drawn as their native size to a viewport matching the target width and height. The entire viewport is then scaled to the display resolution.
        /// </summary>
        /// <remarks>
        /// This is a more accurate pixel perfect rendering, but the upscale can result in jitter for low resolutions.
        /// </remarks>
        Viewport
    }
}
