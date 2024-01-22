using Microsoft.Xna.Framework;

namespace Engine
{
    public class BoxCollider : Collider
    {
        public BoxCollider(Entity entity) : this(entity, 0f, 0f)
        {
        }

        public BoxCollider(Entity entity, float width, float height) : this(entity, 0, 0, width, height)
        {
        }

        public BoxCollider(Entity entity, float x, float y, float width, float height) : base(entity)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public override void CalculateBounds()
        {
            Bounds = Entity.TransformOrigin(X, Y, Width, Height);
        }

        public override bool Overlaps(Collider collider)
        {
            return collider.OverlapRectangle(Bounds);
        }

        public override bool OverlapPoint(Vector2 point)
        {
            return OverlapTests.RectangleVsPoint(Bounds, point);
        }

        public override bool OverlapCircle(CircleF circle)
        {
            return OverlapTests.RectangleVsCircle(Bounds, circle);
        }

        public override bool OverlapRectangle(RectangleF rectangle)
        {
            return OverlapTests.RectangleVsRectangle(Bounds, rectangle);
        }

        public override bool Intersects(Collider collider, Segment segment, out Intersection intersection)
        {
            return collider.IntersectRectangleSegment(Bounds, segment, out intersection);
        }

        public override bool IntersectSegment(Segment segment, out Intersection intersection)
        {
            return IntersectionTests.SegmentVsRectangle(segment, Bounds, out intersection);
        }

        public override bool IntersectCircleSegment(CircleF circle, Segment segment, out Intersection intersection)
        {
            return IntersectionTests.CircleSegmentVsRectangle(circle, segment, Bounds, out intersection);
        }

        public override bool IntersectRectangleSegment(RectangleF rectangle, Segment segment, out Intersection intersection)
        {
            return IntersectionTests.RectangleSegmentVsRectangle(rectangle, segment, Bounds, out intersection);
        }

        public override void Draw(Renderer renderer)
        {
            renderer.DrawRectangleOutline(Bounds.ToXnaRect(), Color.White);
        }
    }
}
