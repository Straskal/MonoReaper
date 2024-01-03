using Microsoft.Xna.Framework;
using System;

namespace Engine.Collision
{
    public static class IntersectionTests
    {
        public static bool MovingCircleVsCircle(CircleF circle0, IntersectionPath path, CircleF circle1, out float time, out Vector2 contact, out Vector2 normal)
        {
            return PathVsCircle(path, CircleF.Inflate(circle1, circle0), out time, out contact, out normal);
        }

        public static bool MovingCircleVsRectangle(CircleF circle, IntersectionPath path, RectangleF rectangle, out float time, out Vector2 contact, out Vector2 normal)
        {
            if (!PathVsRectangle(path, RectangleF.Inflate(rectangle, circle), out time, out contact, out normal))
            {
                return false;
            }

            if (TryGetRectangleCorner(contact, rectangle, out var corner) && !PathVsCircle(path, new CircleF(corner, circle.Radius), out time, out contact, out normal))
            {
                return false;
            }

            return true;
        }

        public static bool MovingRectangleVsRectangle(RectangleF rectangle0, IntersectionPath path, RectangleF rectangle1, out float time, out Vector2 contact, out Vector2 normal)
        {
            return PathVsRectangle(path, RectangleF.Inflate(rectangle1, rectangle0), out time, out contact, out normal);
        }

        public static bool PathVsCircle(IntersectionPath path, CircleF circle, out float time, out Vector2 contact, out Vector2 normal)
        {
            return RayVsCircle(path.Ray, circle.Center, circle.Radius, out time, out contact, out normal) && time <= path.Length;
        }

        public static bool PathVsRectangle(IntersectionPath segment, RectangleF rectangle, out float time, out Vector2 contact, out Vector2 normal)
        {
            return RayVsRectangle(segment.Ray, rectangle, out time, out contact, out normal) && time <= segment.Length;
        }

        public static bool RayVsCircle(RayF ray, Vector2 center, float radius, out float time, out Vector2 contact, out Vector2 normal)
        {
            time = 0f;
            contact = Vector2.Zero;
            normal = Vector2.Zero;

            var m = ray.Position - center;
            var b = Vector2.Dot(m, ray.Direction);

            if (b > 0f)
            {
                return false;
            }

            var c = Vector2.Dot(m, m) - radius * radius;
            var d = b * b - c;

            if (d < 0f)
            {
                return false;
            }

            time = MathF.Max(-b - MathF.Sqrt(d), 0f);
            contact = ray.Position + ray.Direction * time;
            normal = GetNormal(contact, center);
            return true;
        }

        public static bool RayVsRectangle(RayF ray, RectangleF rectangle, out float time, out Vector2 contact, out Vector2 normal)
        {
            time = 0f;
            contact = Vector2.Zero;
            normal = Vector2.Zero;

            var tmin = float.MinValue;
            var tmax = float.MaxValue;

            if (!RayVsEdges(ray.Direction.X, ray.InverseDirection.X, ray.Position.X, rectangle.TopLeft.X, rectangle.BottomRight.X, ref tmin, ref tmax)) 
            {
                return false;
            }

            if (!RayVsEdges(ray.Direction.Y, ray.InverseDirection.Y, ray.Position.Y, rectangle.TopLeft.Y, rectangle.BottomRight.Y, ref tmin, ref tmax))
            {
                return false;
            }

            time = MathF.Max(tmin, 0f);
            contact = ray.Position + ray.Direction * time;
            normal = GetNormal(contact, rectangle);
            return true;
        }

        private static bool RayVsEdges(float direction, float inverseDirection, float position, float min, float max, ref float tmin, ref float tmax)
        {
            if (MathF.Abs(direction) < float.Epsilon)
            {
                if (position < min || position > max)
                {
                    return false;
                }
            }
            else
            {
                var t1 = (min - position) * inverseDirection;
                var t2 = (max - position) * inverseDirection;

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

            return true;
        }

        private static bool TryGetRectangleCorner(Vector2 point, RectangleF rectangle, out Vector2 corner)
        {
            var mask = 0;

            if (point.X < rectangle.Left)
            {
                mask |= 1;
            }
            else if (point.X > rectangle.Right)
            {
                mask |= 2;
            }

            if (point.Y < rectangle.Top)
            {
                mask |= 4;
            }
            else if (point.Y > rectangle.Bottom)
            {
                mask |= 8;
            }

            switch (mask)
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
                default:
                    corner = Vector2.Zero;
                    return false;
            }

            return true;
        }

        public static Vector2 GetNormal(Vector2 point, Vector2 center)
        {
            var m = point - center;
            return Vector2.Normalize(m) + m * 0.0001f; // Offset the normal
        }

        public static Vector2 GetNormal(Vector2 point, RectangleF rectangle)
        {
            var result = Vector2.Zero;
            var closestPoint = ClosestPointComputations.Rectangle(point, rectangle);

            if (closestPoint.X == rectangle.Left)
            {
                result.X = -1f;
            }
            else if (closestPoint.X == rectangle.Right)
            {
                result.X = 1f;
            }
            else if (closestPoint.Y == rectangle.Top)
            {
                result.Y = -1f;
            }
            else if (closestPoint.Y == rectangle.Bottom)
            {
                result.Y = 1f;
            }

            return result;
        }
    }
}
