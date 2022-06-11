using Microsoft.Xna.Framework;
using System;

namespace Reaper.Engine
{
    /// <summary>
    /// The bounding box of a world object.
    /// </summary>
    public struct WorldObjectBounds
    {
        public WorldObjectBounds(float x, float y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float X;
        public float Y;
        public int Width;
        public int Height;

        public float Left => X;
        public float Right => X + Width;
        public float Top => Y;
        public float Bottom => Y + Height;

        public static WorldObjectBounds Empty => new WorldObjectBounds();

        public Rectangle ToRectangle()
        {
            return new Rectangle(
                (int)Math.Round(X),
                (int)Math.Round(Y),
                Width,
                Height);
        }

        public void Offset(float x, float y)
        {
            X += x;
            Y += y;
        }

        public bool Intersects(WorldObjectBounds aabb2)
        {
            return !(Right <= aabb2.Left || Left >= aabb2.Right)
                && !(Bottom <= aabb2.Top || Top >= aabb2.Bottom);
        }

        public Vector2 GetIntersectionDepth(WorldObjectBounds aabb2)
        {
            // Calculate half sizes.
            float halfWidthA = Width / 2f;
            float halfHeightA = Height / 2f;
            float halfWidthB = aabb2.Width / 2f;
            float halfHeightB = aabb2.Height / 2f;

            // Calculate centers.
            Vector2 centerA = new Vector2(Left + halfWidthA, Top + halfHeightA);
            Vector2 centerB = new Vector2(aabb2.Left + halfWidthB, aabb2.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }
    }
}
