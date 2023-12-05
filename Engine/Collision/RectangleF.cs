using System;
using Microsoft.Xna.Framework;

namespace Core.Collision
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
        public readonly Vector2 BottomRight => new(Right, Bottom);
        public readonly Vector2 Center => new(X + Width / 2f, Y + Height / 2f);
        public readonly Vector2 Position => new(X, Y);
        public readonly Vector2 Size => new(Width, Height);

        public RectangleF(float x, float y)
        {
            X = x;
            Y = y;
            Width = 0f;
            Height = 0f;
        }

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public readonly bool Intersects(RectangleF other)
        {
            return other.Left < Right 
                && Left < other.Right 
                && other.Top < Bottom 
                && Top < other.Bottom;
        }

        public readonly RectangleF Offset(Vector2 direction)
        {
            return new(X + direction.X, Y + direction.Y, Width, Height);
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

        public readonly RectangleF Project(Vector2 direction) 
        {
            return Union(Offset(direction));
        }

        public readonly Rectangle ToXnaRect()
        {
            return new(
                (int)Math.Floor(X),
                (int)Math.Floor(Y),
                (int)Math.Floor(Width),
                (int)Math.Floor(Height));
        }
    }
}
