using Microsoft.Xna.Framework;
using System;

namespace Engine.Collision
{
    public static class IntersectionTests
    {
        public static bool MovingCircleVsCircle(CircleF circle0, IntersectionPath path, CircleF circle1, out float time, out Vector2 contact, out Vector2 normal)
        {
            var inflatedCircle = CircleF.Inflate(circle1, circle0);

            if (PathVsCircle(path, inflatedCircle, out time, out contact))
            {
                normal = GetNormal(contact, circle1.Center);
                return true;
            }

            normal = Vector2.Zero;
            return false;
        }

        public static bool MovingCircleVsRectangle(CircleF circle, IntersectionPath path, RectangleF rectangle, out float time, out Vector2 contact, out Vector2 normal)
        {
            var inflatedRectangle = RectangleF.Inflate(rectangle, circle);

            if (!PathVsRectangle(path, inflatedRectangle, out time, out contact))
            {
                normal = Vector2.Zero;
                return false;
            }

            if (!TryGetRectangleCorner(contact, rectangle, out var corner))
            {
                normal = GetNormal(contact, inflatedRectangle);
                return true;
            }

            if (!PathVsCircle(path, new CircleF(corner, circle.Radius), out time, out contact))
            {
                normal = Vector2.Zero;
                return false;
            }

            normal = GetNormal(contact, corner);
            return true;
        }

        public static bool MovingRectangleVsRectangle(RectangleF rectangle0, IntersectionPath path, RectangleF rectangle1, out float time, out Vector2 contact, out Vector2 normal)
        {
            var inflatedRectangle = RectangleF.Inflate(rectangle1, rectangle0);

            if (PathVsRectangle(path, inflatedRectangle, out time, out contact))
            {
                normal = GetNormal(contact, rectangle1);
                return false;
            }

            normal = Vector2.Zero;
            return true;
        }

        public static bool PathVsCircle(IntersectionPath path, CircleF circle, out float time, out Vector2 contact)
        {
            return RayVsCircle(path.Ray, circle.Center, circle.Radius, out time, out contact) && time <= path.Length;
        }

        public static bool PathVsRectangle(IntersectionPath segment, RectangleF rectangle, out float time, out Vector2 contact)
        {
            return RayVsRectangle(segment.Ray, rectangle, out time, out contact) && time <= segment.Length;
        }

        public static bool RayVsCircle(RayF ray, Vector2 center, float radius, out float time, out Vector2 contact)
        {
            // t² + 2(m · d)t + (m · m) - r²    
            // t² + 2bt + c = 0                 (quadratic formula)
            // t = -b ± √(b² - c)               (quadratic solutions)
            contact = Vector2.Zero;
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
            contact = ray.Position + ray.Direction * time;
            return true;
        }

        public static bool RayVsRectangle(RayF ray, RectangleF rectangle, out float time, out Vector2 contact)
        {
            contact = Vector2.Zero;
            time = 0f;

            var tmin = float.MinValue;
            var tmax = float.MaxValue;

            if (!RayVsEdges(ray.Direction.X, ray.InverseDirection.X, ray.Position.X, rectangle.Min.X, rectangle.Max.X, ref tmin, ref tmax)) 
            {
                return false;
            }

            if (!RayVsEdges(ray.Direction.Y, ray.InverseDirection.Y, ray.Position.Y, rectangle.Min.Y, rectangle.Max.Y, ref tmin, ref tmax))
            {
                return false;
            }

            time = tmin;
            contact = ray.Position + ray.Direction * time;

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
            return Vector2.Normalize(point - center);
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
