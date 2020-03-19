namespace Reaper.Tiled.Models
{
    public class TiledTilesetInfo
    {
        public string Source { get; set; }
    }

    public class TiledMap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public TiledLayer[] Layers { get; set; }
        public TiledTilesetInfo[] Tilesets { get; set; }
    }
}
