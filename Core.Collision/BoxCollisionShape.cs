using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace Engine
{
    public sealed class BoxCollisionShape : CollisionShape
    {
        public BoxCollisionShape(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public BoxCollisionShape(float x, float y, float width, float height)
        {
            Position = new Vector2(x, y);
            Width = width;
            Height = height;
        }

        public float Width { get; set; }
        public float Height { get; set; }

        public override void CalculateBounds()
        {
            Bounds = new RectangleF(Position.X - Width * 0.5f, Position.Y - Height * 0.5f, Width, Height); ;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Overlaps(CollisionShape collider)
        {
            return collider.OverlapRectangle(Bounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool OverlapPoint(Vector2 point)
        {
            return OverlapTests.RectangleVsPoint(Bounds, point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool OverlapCircle(CircleF circle)
        {
            return OverlapTests.RectangleVsCircle(Bounds, circle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool OverlapRectangle(RectangleF rectangle)
        {
            return OverlapTests.RectangleVsRectangle(Bounds, rectangle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Intersects(CollisionShape collider, Segment segment, out Intersection intersection)
        {
            return collider.IntersectRectangleSegment(Bounds, segment, out intersection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool IntersectSegment(Segment segment, out Intersection intersection)
        {
            return IntersectionTests.SegmentVsRectangle(segment, Bounds, out intersection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool IntersectCircleSegment(CircleF circle, Segment segment, out Intersection intersection)
        {
            return IntersectionTests.CircleSegmentVsRectangle(circle, segment, Bounds, out intersection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool IntersectRectangleSegment(RectangleF rectangle, Segment segment, out Intersection intersection)
        {
            return IntersectionTests.RectangleSegmentVsRectangle(rectangle, segment, Bounds, out intersection);
        }
    }
}
