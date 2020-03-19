namespace Reaper.Tiled.Models
{
    public class TiledObject
    {
        public string Name { get; set; }
        public string Template { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public TiledProperty[] Properties { get; set; }
    }
}
