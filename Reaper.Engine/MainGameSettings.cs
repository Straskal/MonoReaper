namespace Reaper.Engine
{
    public class GameSettings
    {
        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }
        public bool IsFullscreen { get; set; }
        public bool IsResizable { get; set; }
        public bool IsBordered { get; set; }
        public bool IsVsyncEnabled { get; set; }
    }
}
