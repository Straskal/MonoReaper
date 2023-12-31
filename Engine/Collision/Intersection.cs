using Microsoft.Xna.Framework;
using System;

namespace Engine.Collision
{
    public static class Intersection
    {
        public static bool RayVsCircle(RayF ray, CircleF circle, out float time)
        {
            return RayVsCircle(ray, circle.Center, circle.Radius, out time);
        }

        public static bool RayVsCircle(RayF ray, Vector2 center, float radius, out float time)
        {
            // t² + 2(m · d)t + (m · m) - r²    
            // t² + 2bt + c = 0                 (quadratic formula)
            // t = -b ± √(b² - c)               (quadratic solutions)
            time = 0f;

            var m = ray.Position - center;
            var b = Vector2.Dot(m, ray.Direction);
            var c = Vector2.Dot(m, m) - radius * radius;

            if (c > 0f && b > 0f)
            {
                return false;
            }

            var disc = b * b - c;

            if (disc < 0f)
            {
                return false;
            }

            time = MathF.Max(-b - MathF.Sqrt(disc), 0f);
            return true;
        }

        public static bool RayVsRectangle(RayF ray, RectangleF rectangle, out float time)
        {
            time = 0f;

            var tmin = float.MinValue;
            var tmax = float.MaxValue;

            if (MathF.Abs(ray.Direction.X) < float.Epsilon)
            {
                if (ray.Position.X < rectangle.Min.X || ray.Position.X > rectangle.Max.X)
                {
                    return false;
                }
            }
            else
            {
                var t1 = (rectangle.Min.X - ray.Position.X) * ray.InverseDirection.X;
                var t2 = (rectangle.Max.X - ray.Position.X) * ray.InverseDirection.X;

                if (t1 > t2)
                {
                    (t1, t2) = (t2, t1);
                }

                tmin = MathF.Max(tmin, t1);
                tmax = MathF.Min(tmax, t2);

                if (tmin > tmax)
                {
                    return false;
                }
            }

            if (MathF.Abs(ray.Direction.Y) < float.Epsilon)
            {
                if (ray.Position.Y < rectangle.Min.Y || ray.Position.Y > rectangle.Max.Y)
                {
                    return false;
                }
            }
            else
            {
                var t1 = (rectangle.Min.Y - ray.Position.Y) * ray.InverseDirection.Y;
                var t2 = (rectangle.Max.Y - ray.Position.Y) * ray.InverseDirection.Y;

                if (t1 > t2) 
                {
                    (t1, t2) = (t2, t1);
                }

                tmin = MathF.Max(tmin, t1);
                tmax = MathF.Min(tmax, t2);

                if (tmin > tmax)
                {
                    return false;
                }
            }

            time = tmin;
            return true;
        }

        public static bool SegmentVsCircle(IntersectionPath path, CircleF circle, out float time)
        {
            return SegmentVsCircle(path, circle.Center, circle.Radius, out time) && time < path.Length;
        }

        public static bool SegmentVsCircle(IntersectionPath path, Vector2 center, float radius, out float time)
        {
            return RayVsCircle(path.Ray, center, radius, out time) && time < path.Length;
        }

        public static bool SegmentVsRectangle(IntersectionPath segment, RectangleF rectangle, out float time)
        {
            return RayVsRectangle(segment.Ray, rectangle, out time) && time < segment.Length;
        }

        public static bool MovingCircleVsCircle(CircleF c0, IntersectionPath path, CircleF c1, out float time, out Vector2 contact, out Vector2 normal)
        {
            c1.Radius += c0.Radius;

            if (!SegmentVsCircle(path, c1, out time)) 
            {
                contact = normal = Vector2.Zero;
                return false;
            }

            contact = path.Position + path.NormalizedDirection * time;
            normal = GetNormal(contact, c1.Center);

            return true;
        }

        public static bool MovingCircleVsRectangle(CircleF circle, IntersectionPath path, RectangleF rectangle, out float time, out Vector2 contact, out Vector2 normal)
        {
            var boundingRectangle = RectangleF.Inflate(rectangle, circle);

            if (!SegmentVsRectangle(path, boundingRectangle, out time))
            {
                contact = normal = Vector2.Zero;
                return false;
            }

            contact = path.Position + path.NormalizedDirection * time;

            if (!IsCorner(contact, rectangle, out var corner))
            {
                normal = GetNormal(contact, boundingRectangle);
                return true;
            }

            if (!SegmentVsCircle(path, corner, circle.Radius, out time))
            {
                normal = Vector2.Zero;
                return false;
            }

            contact = path.Position + path.NormalizedDirection * time;
            normal = GetNormal(contact, corner);
            return true;
        }

        public static bool MovingRectangleVsRectangle(RectangleF rectangle1, IntersectionPath path, RectangleF rectangle2, out float time, out Vector2 contact, out Vector2 normal)
        {
            rectangle2.X -= rectangle1.HalfSize.X;
            rectangle2.Y -= rectangle1.HalfSize.Y;
            rectangle2.Width += rectangle1.Size.X;
            rectangle2.Height += rectangle1.Size.Y;

            if (!SegmentVsRectangle(path, rectangle2, out time))
            {
                contact = normal = Vector2.Zero;
                return false;
            }

            contact = path.Position + path.NormalizedDirection * time;
            normal = GetNormal(contact, rectangle2);

            return true;
        }

        private static bool IsCorner(Vector2 point, RectangleF rectangle, out Vector2 corner)
        {
            int u = 0;
            if (point.X < rectangle.Left) u |= 1;
            if (point.X > rectangle.Right) u |= 2;
            if (point.Y < rectangle.Top) u |= 4;
            if (point.Y > rectangle.Bottom) u |= 8;
            if ((u & (u - 1)) == 0)
            {
                corner = Vector2.Zero;
                return false;
            }
            corner = GetCorner(rectangle, u);
            return true;
        }

        private static Vector2 GetCorner(RectangleF rectangle, int u)
        {
            var corner = Vector2.Zero;
            switch (u)
            {
                case 1 | 4:
                    corner = rectangle.TopLeft;
                    break;
                case 2 | 4:
                    corner = rectangle.TopRight;
                    break;
                case 1 | 8:
                    corner = rectangle.BottomLeft;
                    break;
                case 2 | 8:
                    corner = rectangle.BottomRight;
                    break;
            }
            return corner;
        }

        public static Vector2 GetNormal(Vector2 point, Vector2 center)
        {
            return Vector2.Normalize(point - center);
        }

        public static Vector2 GetNormal(Vector2 point, RectangleF rectangle)
        {
            point = ClosestPointComputations.Rectangle(point, rectangle);
            var result = Vector2.Zero;
            if (point.X == rectangle.Left) result.X = -1f;
            else if (point.X == rectangle.Right) result.X = 1f;
            else if (point.Y == rectangle.Top) result.Y = -1f;
            else if (point.Y == rectangle.Bottom) result.Y = 1f;
            return result;
        }
    }
}
