using Microsoft.Xna.Framework;

namespace Adventure.Entities
{
    public class Tile : Entity
    {
        public Tile(float x, float y, float width, float height)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
            IsNetEntity = false;
        }

        public Vector2 Size { get; set; }

        public override EntityType Type => EntityType.Tile;
    }
}
