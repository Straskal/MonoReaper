using Microsoft.Xna.Framework;

namespace Adventure.Content
{
    public sealed class EntityData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public EntityDataFields Fields { get; set; } = new EntityDataFields();
    }
}
