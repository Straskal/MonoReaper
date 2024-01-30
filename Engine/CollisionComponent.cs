using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace Engine
{
    public sealed class CollisionComponent
    {
        public enum CollisionComponentType
        {
            Circle,
            Box
        }

        // Width used as circle radius.
        private float width;
        private float height;

        public CollisionComponent(Entity entity)
        {
            Entity = entity;
        }

        public Entity Entity { get; }
        public CollisionComponentType Type { get; private set; }
        public Vector2 Position { get; set; }
        public RectangleF Bounds { get; private set; }
        public uint Layer { get; set; }

        public static CollisionComponent CreateBox(Entity owner, float x, float y, float width, float height)
        {
            return new CollisionComponent(owner)
            {
                Type = CollisionComponentType.Box,
                Position = new Vector2(x, y),
                width = width,
                height = height
            };
        }

        public static CollisionComponent CreateCircle(Entity owner, float x, float y, float radius)
        {
            return new CollisionComponent(owner)
            {
                Type = CollisionComponentType.Circle,
                Position = new Vector2(x, y),
                width = radius
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckLayer(uint mask)
        {
            return (mask & Layer) != 0;
        }

        public void Enable()
        {
            CalculateBounds();
            Entity.World.EnableCollider(this);
        }

        public void Disable()
        {
            CalculateBounds();
            Entity.World.DisableCollider(this);
        }

        public void Update()
        {
            CalculateBounds();
        }

        public void CalculateBounds()
        {
            switch (Type)
            {
                case CollisionComponentType.Box:
                    Bounds = Entity.TransformOrigin(Position.X, Position.Y, width, height);
                    break;
                case CollisionComponentType.Circle:
                    Bounds = Entity.TransformOrigin(Position.X, Position.Y, width * 2f, width * 2f);
                    break;
            }
        }

        public bool Overlaps(CollisionComponent collider)
        {
            var overlaps = false;

            switch (Type)
            {
                case CollisionComponentType.Box:
                    overlaps = collider.OverlapRectangle(Bounds);
                    break;
                case CollisionComponentType.Circle:
                    overlaps = collider.OverlapCircle(new CircleF(Position, width));
                    break;
            }

            return overlaps;
        }

        public bool OverlapPoint(Vector2 point)
        {
            var overlaps = false;

            switch (Type)
            {
                case CollisionComponentType.Box:
                    overlaps = OverlapTests.RectangleVsPoint(Bounds, point);
                    break;
                case CollisionComponentType.Circle:
                    overlaps = OverlapTests.CircleVsPoint(new CircleF(Position, width), point);
                    break;
            }

            return overlaps;
        }

        public bool OverlapCircle(CircleF circle)
        {
            var overlaps = false;

            switch (Type)
            {
                case CollisionComponentType.Box:
                    overlaps = OverlapTests.RectangleVsCircle(Bounds, circle);
                    break;
                case CollisionComponentType.Circle:
                    overlaps = OverlapTests.CircleVsCircle(new CircleF(Position, width), circle);
                    break;
            }

            return overlaps;
        }

        public bool OverlapRectangle(RectangleF rectangle)
        {
            var overlaps = false;

            switch (Type)
            {
                case CollisionComponentType.Box:
                    overlaps = OverlapTests.RectangleVsRectangle(Bounds, rectangle);
                    break;
                case CollisionComponentType.Circle:
                    overlaps = OverlapTests.CircleVsRectangle(new CircleF(Position, width), rectangle);
                    break;
            }

            return overlaps;
        }

        public bool Intersects(CollisionComponent collider, Segment segment, out Intersection intersection)
        {
            intersection = Intersection.Empty;
            var intersects = false;

            switch (Type)
            {
                case CollisionComponentType.Box:
                    intersects = collider.IntersectRectangleSegment(Bounds, segment, out intersection);
                    break;
                case CollisionComponentType.Circle:
                    intersects = collider.IntersectCircleSegment(new CircleF(Position, width), segment, out intersection);
                    break;
            }

            return intersects;
        }

        public bool IntersectSegment(Segment segment, out Intersection intersection)
        {
            intersection = Intersection.Empty;
            var intersects = false;

            switch (Type)
            {
                case CollisionComponentType.Box:
                    intersects = IntersectionTests.SegmentVsRectangle(segment, Bounds, out intersection);
                    break;
                case CollisionComponentType.Circle:
                    intersects = IntersectionTests.SegmentVsCircle(segment, new CircleF(Position, width), out intersection);
                    break;
            }

            return intersects;
        }

        public bool IntersectCircleSegment(CircleF circle, Segment segment, out Intersection intersection)
        {
            intersection = Intersection.Empty;
            var intersects = false;

            switch (Type)
            {
                case CollisionComponentType.Box:
                    intersects = IntersectionTests.CircleSegmentVsRectangle(circle, segment, Bounds, out intersection);
                    break;
                case CollisionComponentType.Circle:
                    intersects = IntersectionTests.CircleSegmentVsCircle(circle, segment, new CircleF(Position, width), out intersection);
                    break;
            }

            return intersects;
        }

        public bool IntersectRectangleSegment(RectangleF rectangle, Segment segment, out Intersection intersection)
        {
            intersection = Intersection.Empty;
            var intersects = false;

            switch (Type)
            {
                case CollisionComponentType.Box:
                    intersects = IntersectionTests.RectangleSegmentVsRectangle(rectangle, segment, Bounds, out intersection);
                    break;
                case CollisionComponentType.Circle:
                    intersects = IntersectionTests.RectangleSegmentVsRectangle(rectangle, segment, Bounds, out intersection);
                    break;
            }

            return intersects;
        }

        public void Draw(Renderer renderer)
        {
            switch (Type)
            {
                case CollisionComponentType.Box:
                    renderer.DrawRectangleOutline(Bounds.ToXnaRect(), Color.White);
                    break;
                case CollisionComponentType.Circle:
                    renderer.DrawCircleOutline(Entity.Position.X - Position.X, Entity.Position.Y + Position.Y, width, 10, new Color(Color.White, 0.1f));
                    break;
            }
        }
    }
}
