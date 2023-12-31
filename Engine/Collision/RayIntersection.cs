using Microsoft.Xna.Framework;
using System;

namespace Engine.Collision
{
    public static class RayIntersection
    {
        public static bool VsRectangle(RayF ray, RectangleF rectangle, out float time)
        {
            var t1 = (rectangle.Min.X - ray.Position.X) * ray.InverseDirection.X;
            var t2 = (rectangle.Max.X - ray.Position.X) * ray.InverseDirection.X;
            var tmin = MathF.Min(t1, t2);
            var tmax = MathF.Max(t1, t2);
            t1 = (rectangle.Min.Y - ray.Position.Y) * ray.InverseDirection.Y;
            t2 = (rectangle.Max.Y - ray.Position.Y) * ray.InverseDirection.Y;
            tmin = MathF.Max(tmin, MathF.Min(MathF.Min(t1, t2), tmax));
            tmax = MathF.Min(tmax, MathF.Max(MathF.Max(t1, t2), tmin));
            time = tmin;
            return tmax > MathF.Max(tmin, 0f);
        }

        public static bool VsCircle(RayF ray, CircleF circle, out float time)
        {
            time = 0f;
            var u0 = circle.Center - ray.Position;
            var u1 = Vector2.Dot(u0, ray.Direction) * ray.Direction;
            var u2 = u0 - u1;
            var ds = Vector2.Dot(u2, u2);
            var rs = circle.Radius * circle.Radius;
            if (ds >= rs) return false;
            var m = MathF.Sqrt(rs - ds);
            time = (u1 - m * ray.Direction).Length();
            return true;
        }
    }
}
