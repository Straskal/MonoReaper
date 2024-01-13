using System;
using Microsoft.Xna.Framework;

namespace Engine
{
    public struct RectangleF
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public readonly float Left => X;
        public readonly float Right => X + Width;
        public readonly float Top => Y;
        public readonly float Bottom => Y + Height;
        public readonly Vector2 TopLeft => new(Left, Top);
        public readonly Vector2 TopRight => new(Right, Top);
        public readonly Vector2 BottomLeft => new(Left, Bottom);
        public readonly Vector2 BottomRight => new(Right, Bottom);
        public readonly Vector2 Center => new(X + Width * 0.5f, Y + Height * 0.5f);
        public readonly Vector2 Position => new(X, Y);
        public readonly Vector2 Size => new(Width, Height);
        public readonly Vector2 HalfSize => Size * 0.5f;

        public RectangleF(Rectangle rectangle)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
        {
        }

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public readonly bool Overlaps(RectangleF other)
        {
            return OverlapTests.RectangleVsRectangle(this, other);
        }

        public readonly RectangleF Union(Vector2 direction)
        {
            return Union(new RectangleF(X + direction.X, Y + direction.Y, Width, Height));
        }

        public readonly RectangleF Union(RectangleF other)
        {
            RectangleF result;
            result.X = Math.Min(X, other.X);
            result.Y = Math.Min(Y, other.Y);
            result.Width = Math.Max(Right, other.Right) - result.X;
            result.Height = Math.Max(Bottom, other.Bottom) - result.Y;
            return result;
        }

        public readonly Rectangle ToXnaRect()
        {
            return new(
                (int)Math.Floor(X),
                (int)Math.Floor(Y),
                (int)Math.Floor(Width),
                (int)Math.Floor(Height));
        }

        public static RectangleF Inflate(RectangleF rectangle, CircleF circle) 
        {
            var result = rectangle;
            result.X -= circle.Radius;
            result.Y -= circle.Radius;
            result.Width += circle.Radius * 2f;
            result.Height += circle.Radius * 2f;
            return result;
        }

        public static RectangleF Inflate(RectangleF rectangle0, RectangleF rectangle1)
        {
            var result = rectangle0;
            result.X -= rectangle1.HalfSize.X;
            result.Y -= rectangle1.HalfSize.Y;
            result.Width += rectangle1.Width;
            result.Height += rectangle1.Height;
            return result;
        }
    }
}
