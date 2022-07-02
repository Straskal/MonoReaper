using System;
using Microsoft.Xna.Framework;

namespace Reaper.Engine.AABB
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
        public Vector2 Center 
        {
            get 
            {
                return new Vector2(X + Width / 2f, Y + Height / 2f);
            }
        }

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
            if (other.Left < Right && Left < other.Right && other.Top < Bottom)
            {
                return Top < other.Bottom;
            }

            return false;
        }

        public Rectangle AsRectangle => new(
            (int)Math.Floor(X),
            (int)Math.Floor(Y),
            (int)Math.Floor(Width),
            (int)Math.Floor(Height));
    }
}
