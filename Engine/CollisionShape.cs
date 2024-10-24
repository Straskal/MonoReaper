using Microsoft.Xna.Framework;

namespace Engine
{
    public struct CollisionShape
    {
        public enum Shape
        {
            Circle,
            Rectangle
        }

        public CollisionShape(Shape type)
        {
            Type = type;
            Position = Vector2.Zero;
            Size = Vector2.Zero;
        }

        /// <summary>
        /// The type of shape.
        /// </summary>
        public readonly Shape Type;

        /// <summary>
        /// The position of the shape.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The size of the shape.
        /// </summary>
        public Vector2 Size;

        /// <summary>
        /// The center of the shape.
        /// </summary>
        public Vector2 Center
        {
            get
            {
                if (Type == Shape.Circle)
                {
                    return Position;
                }
                else
                {
                    return Rectangle.Center;
                }
            }
        }

        /// <summary>
        /// Rectangle representation of the collision shape.
        /// </summary>
        public readonly RectangleF Rectangle => new(Position.X - Size.X * 0.5f, Position.Y - Size.Y * 0.5f, Size.X, Size.Y);

        /// <summary>
        /// Circle representation of the collision shape.
        /// </summary>
        public readonly CircleF Circle => new(Position.X, Position.Y, Size.X);

        public static CollisionShape CreateCircle(float x, float y, float radius)
        {
            return new CollisionShape(Shape.Circle)
            {
                Position = new Vector2(x, y),
                Size = new Vector2(radius, radius)
            };
        }

        public static CollisionShape CreateBox(float x, float y, float width, float height)
        {
            return new CollisionShape(Shape.Rectangle)
            {
                Position = new Vector2(x, y),
                Size = new Vector2(width, height)
            };
        }

        public readonly bool Overlaps(CollisionShape other)
        {
            if (Type == Shape.Circle)
            {
                return other.Overlaps(Circle);
            }
            else
            {
                return other.Overlaps(Rectangle);
            }
        }

        public readonly bool Overlaps(Vector2 other)
        {
            if (Type == Shape.Circle)
            {
                return OverlapTests.CircleVsPoint(Circle, other);
            }
            else
            {
                return OverlapTests.RectangleVsPoint(Rectangle, other);
            }
        }

        public readonly bool Overlaps(CircleF other)
        {
            if (Type == Shape.Circle)
            {
                return OverlapTests.CircleVsCircle(Circle, other);
            }
            else
            {
                return OverlapTests.RectangleVsCircle(Rectangle, other);
            }
        }

        public readonly bool Overlaps(RectangleF other)
        {
            if (Type == Shape.Circle)
            {
                return OverlapTests.CircleVsRectangle(Circle, other);
            }
            else
            {
                return OverlapTests.RectangleVsRectangle(Rectangle, other);
            }
        }

        public readonly bool Intersects(CollisionShape shape, Segment segment, out Intersection intersection)
        {
            if (Type == Shape.Circle)
            {
                return shape.Intersects(Circle, segment, out intersection);
            }
            else
            {
                return shape.Intersects(Rectangle, segment, out intersection);
            }
        }

        public readonly bool Intersects(Segment segment, out Intersection intersection)
        {
            if (Type == Shape.Circle)
            {
                return IntersectionTests.SegmentVersusCircle(segment, Circle, out intersection);
            }
            else
            {
                return IntersectionTests.SegmentVersusRectangle(segment, Rectangle, out intersection);
            }
        }

        public readonly bool Intersects(CircleF circle, Segment segment, out Intersection intersection)
        {
            if (Type == Shape.Circle)
            {
                return IntersectionTests.CircleSegmentVersusCircle(circle, segment, Circle, out intersection);
            }
            else
            {
                return IntersectionTests.CircleSegmentVersusRectangle(circle, segment, Rectangle, out intersection);
            }
        }

        public readonly bool Intersects(RectangleF rectangle, Segment segment, out Intersection intersection)
        {
            if (Type == Shape.Circle)
            {
                return IntersectionTests.RectangleSegmentVsRectangle(rectangle, segment, Rectangle, out intersection);
            }
            else
            {
                return IntersectionTests.RectangleSegmentVsRectangle(rectangle, segment, Rectangle, out intersection);
            }
        }

        public readonly void DebugDraw(Renderer renderer)
        {
            if (Type == Shape.Circle)
            {
                renderer.DrawCircleOutline(Position, Size.X, 1, Color.Blue);
            }
            else
            {
                renderer.DrawRectangleOutline(Rectangle.ToXnaRect(), Color.Blue);
            }
        }
    }
}
