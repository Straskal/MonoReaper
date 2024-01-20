namespace Adventure.Content
{
    public sealed class LayerData
    {
        public string TilesetPath { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public bool IsSolid { get; set; }
        public TileData[] Tiles { get; set; }
    }
}
