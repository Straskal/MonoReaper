using Microsoft.Xna.Framework;

namespace Engine
{
    public class BoxCollider : Collider
    {
        public BoxCollider(Entity entity)
            : this(entity, 0f, 0f)
        {
        }

        public BoxCollider(Entity entity, float width, float height)
            : this(entity, width, height, 0)
        {
        }

        public BoxCollider(Entity entity, float width, float height, int layerMask)
            : this(entity, 0f, 0f, width, height, layerMask)
        {
        }

        public BoxCollider(Entity entity, float x, float y, float width, float height)
            : this(entity, x, y, width, height, 0)
        {
        }

        public BoxCollider(Entity entity, float x, float y, float width, float height, int layerMask)
            : base(entity)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Layer = layerMask;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public override RectangleF Bounds => Entity.Origin.Tranform(Entity.Position.X + X, Entity.Position.Y + Y, Width, Height);

        public override bool Overlaps(CircleF circle)
        {
            return OverlapTests.RectangleVsCircle(Bounds, circle);
        }

        public override bool Overlaps(RectangleF rectangle)
        {
            return OverlapTests.RectangleVsRectangle(Bounds, rectangle);
        }

        public override bool Overlaps(Collider collider)
        {
            return collider.IsOverlapped(this);
        }

        public override bool IsOverlapped(BoxCollider collider)
        {
            return OverlapTests.RectangleVsRectangle(Bounds, collider.Bounds);
        }

        public override bool IsOverlapped(CircleCollider collider)
        {
            return OverlapTests.CircleVsRectangle(collider.Circle, Bounds);
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

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            renderer.DrawRectangleOutline(Bounds.ToXnaRect(), Color.White);
        }
    }
}
