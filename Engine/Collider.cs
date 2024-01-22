using Microsoft.Xna.Framework;

namespace Engine
{
    public abstract class Collider
    {
        public Collider(Entity entity)
        {
            Entity = entity;
        }

        public Entity Entity { get; }
        public RectangleF Bounds { get; protected set; }
        public uint Layer { get; set; }

        public abstract void CalculateBounds();
        public abstract bool Overlaps(Collider collider);
        public abstract bool OverlapPoint(Vector2 point);
        public abstract bool OverlapCircle(CircleF circle);
        public abstract bool OverlapRectangle(RectangleF rectangle);
        public abstract bool Intersects(Collider collider, Segment segment, out Intersection intersection);
        public abstract bool IntersectSegment(Segment segment, out Intersection intersection);
        public abstract bool IntersectCircleSegment(CircleF circle, Segment segment, out Intersection intersection);
        public abstract bool IntersectRectangleSegment(RectangleF rectangle, Segment segment, out Intersection intersection);
        public abstract void Draw(Renderer renderer);

        public bool CheckMask(uint mask)
        {
            return (mask & Layer) != 0;
        }

        public virtual void Enable()
        {
            CalculateBounds();
            Entity.World.EnableCollider(this);
        }

        public virtual void Disable()
        {
            CalculateBounds();
            Entity.World.DisableCollider(this);
        }

        public virtual void Update()
        {
            CalculateBounds();
        }
    }
}
