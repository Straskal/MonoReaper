﻿using System;
using Engine.Collision;
using Microsoft.Xna.Framework;

namespace Engine
{
    /// <summary>
    /// A rectangle containing floating point position and size.
    /// </summary>
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
        public readonly Vector2 Center => new(X + Width / 2f, Y + Height / 2f);
        public readonly Vector2 Position => new(X, Y);
        public readonly Vector2 Size => new(Width, Height);
        public readonly Vector2 HalfSize => new(Width / 2f, Height / 2f);
        public readonly Vector2 Min => TopLeft;
        public readonly Vector2 Max => BottomRight;

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

        public readonly bool Contains(Vector2 position) 
        {
            return position.X >= Left && position.X <= Right && position.Y >= Top && position.Y <= Bottom;
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
    }
}
