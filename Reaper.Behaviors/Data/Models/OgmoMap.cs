namespace Reaper
{
    public class OgmoMapValues
    {
        public int SpatialCellSize { get; set; }
        public string EntrySpawnPoint { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
    }

    public class OgmoMap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public OgmoLayer[] Layers { get; set; }
        public OgmoMapValues Values { get; set; }
    }
}