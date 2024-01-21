using Microsoft.Xna.Framework;

namespace Engine
{
    public class CircleCollider : Collider
    {
        public CircleCollider(Entity entity, Vector2 position, float radius) : base(entity)
        {
            Position = position;
            Radius = radius;
        }

        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public CircleF Circle => new(Bounds.Center, Radius);

        public override RectangleF CalculateBounds()
        {
            var size = Radius * 2f;
            return Entity.TransformOrigin(Position.X, Position.Y, size, size);
        }

        public override bool Overlaps(Collider collider)
        {
            return collider.OverlapCircle(Circle);
        }

        public override bool OverlapPoint(Vector2 point)
        {
            return OverlapTests.CircleVsPoint(Circle, point);
        }

        public override bool OverlapCircle(CircleF circle)
        {
            return OverlapTests.CircleVsCircle(Circle, circle);
        }

        public override bool OverlapRectangle(RectangleF rectangle)
        {
            return OverlapTests.CircleVsRectangle(Circle, rectangle);
        }

        public override bool Intersects(Collider collider, Segment segment, out Intersection intersection)
        {
            return collider.IntersectCircleSegment(Circle, segment, out intersection);
        }

        public override bool IntersectSegment(Segment segment, out Intersection intersection)
        {
            return IntersectionTests.SegmentVsCircle(segment, Circle, out intersection);
        }

        public override bool IntersectCircleSegment(CircleF circle, Segment segment, out Intersection intersection)
        {
            return IntersectionTests.CircleSegmentVsCircle(circle, segment, Circle, out intersection);
        }

        public override bool IntersectRectangleSegment(RectangleF rectangle, Segment segment, out Intersection intersection)
        {
            return IntersectionTests.RectangleSegmentVsRectangle(rectangle, segment, Bounds, out intersection);
        }

        public override void Draw(Renderer renderer)
        {
            renderer.DrawCircleOutline(Entity.Position.X - Position.X, Entity.Position.Y + Position.Y, Radius, 10, new Color(Color.White, 0.1f));
        }
    }
}
