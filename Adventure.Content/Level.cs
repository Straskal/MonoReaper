using System;

namespace Adventure.Content
{
    public sealed class Level
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Entity[] Entities { get; set; } = Array.Empty<Entity>();
        public TileLayer[] TileLayers { get; set; } = Array.Empty<TileLayer>();
    }
}