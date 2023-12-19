using System;

namespace Adventure.Content
{
    public sealed class LevelData
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public EntityData[] Entities { get; set; } = Array.Empty<EntityData>();
        public TileLayerData[] TileLayers { get; set; } = Array.Empty<TileLayerData>();
    }
}