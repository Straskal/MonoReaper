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

        public float Left => X;
        public float Right => X + Width;
        public float Top => Y;
        public float Bottom => Y + Height;

        public Vector2 Center => new(X + Width / 2f, Y + Height / 2f);
        public Vector2 Position => new(X, Y);
        public Vector2 Size => new(Width, Height);

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Intersects(RectangleF other)
        {
            return other.Left < Right && Left < other.Right && other.Top < Bottom && Top < other.Bottom;
        }

        public RectangleF Offset(Vector2 offset)
        {
            return new(X + offset.X, Y + offset.Y, Width, Height);
        }

        public RectangleF Union(RectangleF other)
        {
            RectangleF result;

            result.X = Math.Min(X, other.X);
            result.Y = Math.Min(Y, other.Y);
            result.Width = Math.Max(Right, other.Right) - result.X;
            result.Height = Math.Max(Bottom, other.Bottom) - result.Y;

            return result;
        }

        public Rectangle ToXnaRect()
        {
            return new(
                (int)Math.Floor(X),
                (int)Math.Floor(Y),
                (int)Math.Floor(Width),
                (int)Math.Floor(Height));
        }
    }
}
