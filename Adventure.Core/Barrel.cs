using Engine;
using Microsoft.Xna.Framework;

namespace Adventure.Core
{
    internal sealed class Barrel : Actor
    {
        public Sprite Sprite { get; private set; }

        public override void Spawn()
        {
            Sprite = new Sprite(ContentCache.Gfx.Barrel);
            Sprite.SourceRectangle = new Rectangle(0, 0, 16, 16);
            ShapeLocal = CollisionShape.CreateBox(0, 0, 16, 16);
            LayerMask = CollisionLayers.Solid;
            Game.EnableCollision(this);
        }

        public override void Draw(float deltaTime)
        {
            Game.Renderer.Draw(Sprite, Position);
        }
    }
}
