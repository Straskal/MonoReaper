using Microsoft.Xna.Framework;
using System;

namespace Engine.Collision
{
    public static class ClosestPointComputations
    {
        public static Vector2 Segment(Vector2 point, Vector2 a, Vector2 b)
        {
            var ab = b - a;
            var ap = point - a;
            var time = Math.Clamp(Vector2.Dot(ap, ab) / Vector2.Dot(ab, ab), 0f, 1f);
            point = a + ab * time;
            return point;
        }

        public static Vector2 Rectangle(Vector2 point, RectangleF rectangle)
        {
            point.X = Math.Clamp(point.X, rectangle.Left, rectangle.Right);
            point.Y = Math.Clamp(point.Y, rectangle.Top, rectangle.Bottom);
            return point;
        }

        public static Vector2 Circle(Vector2 point, CircleF circle)
        {
            var ab = point - circle.Center;
            point = circle.Center + ab / ab.Length() * circle.Radius;
            return point;
        }
    }
}
