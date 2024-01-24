using Microsoft.Xna.Framework;

namespace Engine
{
    public sealed class CircleCollider : Collider
    {
        private CircleF shape;

        public CircleCollider(Entity entity, float radius) : base(entity)
        {
            Radius = radius;
        }

        public CircleCollider(Entity entity, float x, float y, float radius) : base(entity)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }

        public override void CalculateBounds()
        {
            Bounds = Entity.TransformOrigin(X, Y, Radius * 2f, Radius * 2f);
            shape = new CircleF(Bounds.Center, Radius);
        }

        public override bool Overlaps(Collider collider)
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

        public override bool Intersects(Collider collider, Segment segment, out Intersection intersection)
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

        public override void Draw(Renderer renderer)
        {
            renderer.DrawCircleOutline(Entity.Position.X - X, Entity.Position.Y + Y, Radius, 10, new Color(Color.White, 0.1f));
        }
    }
}
