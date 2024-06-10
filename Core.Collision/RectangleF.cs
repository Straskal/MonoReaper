using Microsoft.Xna.Framework;
using System;

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

        public readonly bool Contains(Vector2 point)
        {
            return OverlapTests.RectangleVsPoint(this, point);
        }

        public readonly bool Overlaps(RectangleF other)
        {
            return OverlapTests.RectangleVsRectangle(this, other);
        }

        public readonly RectangleF Union(Vector2 direction)
        {
            var minx = MathF.Min(X, X + direction.X);
            var miny = MathF.Min(Y, Y + direction.Y);
            var maxx = MathF.Max(Width, Width + direction.X);
            var maxy = MathF.Max(Height, Height + direction.Y);

            return new RectangleF(minx, miny, maxx, maxy);
        }

        public readonly RectangleF Union(RectangleF other)
        {
            var minx = MathF.Min(X, other.X);
            var miny = MathF.Min(Y, other.Y);
            var maxx = MathF.Max(Right, other.Right);
            var maxy = MathF.Max(Bottom, other.Bottom);

            return new RectangleF(minx, miny, maxx, maxy);
        }

        public readonly Rectangle ToXnaRect()
        {
            var minx = (int)X;
            var miny = (int)Y;
            var maxx = (int)Width;
            var maxy = (int)Height;

            return new Rectangle(minx, miny, maxx, maxy);
        }

        public static RectangleF Inflate(RectangleF rectangle, CircleF circle)
        {
            var minx = rectangle.X - circle.Radius;
            var miny = rectangle.Y - circle.Radius;
            var maxx = rectangle.Width + circle.Radius * 2f;
            var maxy = rectangle.Height + circle.Radius * 2f;

            return new RectangleF(minx, miny, maxx, maxy);
        }

        public static RectangleF Inflate(RectangleF rectangle0, RectangleF rectangle1)
        {
            var minx = rectangle0.X - rectangle1.Width * 0.5f;
            var miny = rectangle0.Y - rectangle1.Height * 0.5f;
            var maxx = rectangle0.Width + rectangle1.Width;
            var maxy = rectangle0.Height + rectangle1.Height;

            return new RectangleF(minx, miny, maxx, maxy);
        }
    }
}