using Microsoft.Xna.Framework;

namespace Engine
{
    public sealed class CircleCollisionShape : CollisionShape
    {
        private CircleF shape;

        public CircleCollisionShape(float radius)
        {
            Radius = radius;
        }

        public CircleCollisionShape(float x, float y, float radius)
        {
            Position = new Vector2(x, y);
            Radius = radius;
        }

        public float Radius { get; set; }

        public override void CalculateBounds()
        {
            Bounds = new RectangleF(Position.X, Position.Y, Radius * 2f, Radius * 2f);
            shape = new CircleF(Position, Radius);
        }

        public override bool Overlaps(CollisionShape collider)
        {
            return collider.OverlapCircle(shape);
        }

        public override bool OverlapPoint(Vector2 point)
        {
            return OverlapTests.CircleVsPoint(shape, point);
        }

        public override bool OverlapCircle(CircleF circle)
        {
            return OverlapTests.CircleVsCircle(shape, circle);
        }

        public override bool OverlapRectangle(RectangleF rectangle)
        {
            return OverlapTests.CircleVsRectangle(shape, rectangle);
        }

        public override bool Intersects(CollisionShape collider, Segment segment, out Intersection intersection)
        {
            return collider.IntersectCircleSegment(shape, segment, out intersection);
        }

        public override bool IntersectSegment(Segment segment, out Intersection intersection)
        {
            return IntersectionTests.SegmentVsCircle(segment, shape, out intersection);
        }

        public override bool IntersectCircleSegment(CircleF circle, Segment segment, out Intersection intersection)
        {
            return IntersectionTests.CircleSegmentVsCircle(circle, segment, shape, out intersection);
        }

        public override bool IntersectRectangleSegment(RectangleF rectangle, Segment segment, out Intersection intersection)
        {
            return IntersectionTests.RectangleSegmentVsRectangle(rectangle, segment, Bounds, out intersection);
        }
    }
}
