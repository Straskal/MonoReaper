namespace Adventure.Content
{
    public sealed class TileLayerData
    {
        public bool IsSolid { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public TileData[] Tiles { get; set; }
        public string TileSetRelativePath { get; set; }
    }
}
