namespace Adventure.Content
{
    public sealed class TileLayer
    {
        public bool IsSolid { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public Tile[] Tiles { get; set; }
        public string TileSetRelativePath { get; set; }
    }
}
