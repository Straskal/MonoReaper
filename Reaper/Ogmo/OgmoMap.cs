using Newtonsoft.Json.Linq;

namespace Reaper.Ogmo
{
    public class OgmoMap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public OgmoLayer[] Layers { get; set; }
    }

    public class OgmoLayer
    {
        public int GridCellWidth { get; set; }
        public int GridCellHeight { get; set; }
        public int GridCellsX { get; set; }
        public int GridCellsY { get; set; }
        public string Tileset { get; set; }
        public int[] Data { get; set; }
        public OgmoEntity[] Entities { get; set; }
    }

    public class OgmoEntity
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int OriginX { get; set; }
        public int OriginY { get; set; }
        public bool FlippedX { get; set; }
        public dynamic Values { get; set; }
    }
}