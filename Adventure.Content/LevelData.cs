using Microsoft.Xna.Framework;
namespace Adventure.Content
{
    public class LevelData
    {
        public string Name { get; init; }
        public Rectangle Bounds { get; init; }
        public EntityData[] Entities { get; init; }
        public LayerData[] Layers { get; init; }
    }
}