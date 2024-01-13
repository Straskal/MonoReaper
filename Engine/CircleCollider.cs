using Microsoft.Xna.Framework;

namespace Engine
{
    public class CircleCollider : Collider
    {
        public CircleCollider(Entity entity, Vector2 position, float radius)
            : this(entity, position, radius, 0)
        {
        }

        public CircleCollider(Entity entity, Vector2 position, float radius, int layerMask)
            : base(entity)
        {
            Position = position;
            Radius = radius;
            Layer = layerMask;
        }

        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public CircleF Circle { get => new(Bounds.Center, Radius); }

        public override RectangleF Bounds => Entity.Origin.Tranform(Entity.Position.X + Position.X, Entity.Position.Y + Position.Y, Radius * 2f, Radius * 2f);

        public override void SetPosition(Vector2 position)
        {
            Entity.Position = position - Position;
            UpdateBounds();
        }

        public override bool Overlaps(CircleF circle)
        {
            return OverlapTests.CircleVsCircle(Circle, circle);
        }

        public override bool Overlaps(RectangleF rectangle)
        {
            return OverlapTests.CircleVsRectangle(Circle, rectangle);
        }

        public override bool Overlaps(Collider collider)
        {
            return collider.IsOverlapped(this);
        }

        public override bool IsOverlapped(BoxCollider collider)
        {
            return OverlapTests.CircleVsRectangle(Circle, collider.Bounds);
        }

        public override bool IsOverlapped(CircleCollider collider)
        {
            return OverlapTests.CircleVsCircle(Circle, collider.Circle);
        }

        public override bool Intersects(Collider collider, Segment segment, out Intersection intersection)
        {
            return collider.IsIntersected(this, segment, out intersection);
        }

        public override bool IsIntersected(BoxCollider collider, Segment segment, out Intersection intersection)
        {
            // TODO: Make moving rectangle vs circle intersections work correctly. Right now they are treated as rectangle vs rectangle.
            return IntersectionTests.MovingRectangleVsRectangle(collider.Bounds, segment, Bounds, out intersection);
        }

        public override bool IsIntersected(CircleCollider collider, Segment segment, out Intersection intersection)
        {
            return IntersectionTests.MovingCircleVsCircle(collider.Circle, segment, Circle, out intersection);
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            renderer.DrawCircleOutline(Entity.Position.X - Position.X, Entity.Position.Y + Position.Y, Radius, 10, new Color(Color.White, 0.1f));
        }
    }
}
