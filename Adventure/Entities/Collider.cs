using Engine;
using Microsoft.Xna.Framework;
using static Adventure.Constants;

namespace Adventure
{
    public sealed class Collider
    {
        public Collider(Entity entity, CollisionShape shape)
        {
            Entity = entity;
            Shape = shape;
        }

        public Entity Entity { get; }
        public CollisionShape Shape { get; }
        public Vector2 Offset { get; set; }
        public uint Layer { get; set; }

        public bool CheckMask(uint mask)
        {
            return (mask & Layer) != 0;
        }

        public bool IsSolid()
        {
            return (Layer & EntityLayers.Solid) == EntityLayers.Solid;
        }

        public void Update()
        {
            Shape.Position = Entity.Position + Offset;
            Shape.CalculateBounds();
        }

        public void Enable() 
        {
            Entity.World.EnableCollider(this);
        }

        public void Disable()
        {
            Entity.World.DisableCollider(this);
        }
    }
}
