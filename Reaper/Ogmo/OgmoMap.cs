namespace Reaper.Ogmo
{
    public class OgmoMapValues
    {
        public int SpatialCellSize { get; set; }
    }

    public class OgmoMap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public OgmoLayer[] Layers { get; set; }
        public OgmoMapValues Values { get; set; }
    }
}