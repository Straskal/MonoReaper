using Microsoft.Xna.Framework;

namespace Engine
{
    public abstract class CollisionShape
    {
        public Vector2 Position { get; set; }
        public RectangleF Bounds { get; protected set; }

        public abstract void CalculateBounds();
        public abstract bool Overlaps(CollisionShape collider);
        public abstract bool OverlapPoint(Vector2 point);
        public abstract bool OverlapCircle(CircleF circle);
        public abstract bool OverlapRectangle(RectangleF rectangle);
        public abstract bool Intersects(CollisionShape collider, Segment segment, out Intersection intersection);
        public abstract bool IntersectSegment(Segment segment, out Intersection intersection);
        public abstract bool IntersectCircleSegment(CircleF circle, Segment segment, out Intersection intersection);
        public abstract bool IntersectRectangleSegment(RectangleF rectangle, Segment segment, out Intersection intersection);
    }
}
