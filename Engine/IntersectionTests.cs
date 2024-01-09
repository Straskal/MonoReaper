using Microsoft.Xna.Framework;
using System;

namespace Engine
{
    public static class IntersectionTests
    {
        public static bool MovingCircleVsCircle(CircleF circle0, Segment segment, CircleF circle1, out Intersection intersection)
        {
            return SegmentVsCircle(segment, CircleF.Inflate(circle1, circle0), out intersection);
        }

        public static bool MovingCircleVsRectangle(CircleF circle, Segment segment, RectangleF rectangle, out Intersection intersection)
        {
            if (!SegmentVsRectangle(segment, RectangleF.Inflate(rectangle, circle), out intersection))
            {
                return false;
            }

            if (TryGetRectangleCorner(intersection.Point, rectangle, out var corner) && !SegmentVsCircle(segment, new CircleF(corner, circle.Radius), out intersection))
            {
                return false;
            }

            return true;
        }

        public static bool MovingRectangleVsRectangle(RectangleF rectangle0, Segment path, RectangleF rectangle1, out Intersection intersection)
        {
            return SegmentVsRectangle(path, RectangleF.Inflate(rectangle1, rectangle0), out intersection);
        }

        public static bool SegmentVsCircle(Segment segment, CircleF circle, out Intersection intersection)
        {
            return RayVsCircle(segment.Ray, circle.Center, circle.Radius, out intersection) && intersection.Time <= segment.Length;
        }

        public static bool SegmentVsRectangle(Segment segment, RectangleF rectangle, out Intersection intersection)
        {
            return RayVsRectangle(segment.Ray, rectangle, out intersection) && intersection.Time <= segment.Length;
        }

        public static bool RayVsCircle(Ray ray, Vector2 center, float radius, out Intersection intersection)
        {
            intersection = Intersection.Empty;

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

            var time = MathF.Max(-b - MathF.Sqrt(d), 0f);
            var point = ray.Position + ray.Direction * time;
            var normal = GetNormal(point, center);
            intersection = new Intersection(point, normal, time);
            return true;
        }

        public static bool RayVsRectangle(Ray ray, RectangleF rectangle, out Intersection intersection)
        {
            intersection = Intersection.Empty;

            var tmin = float.MinValue;
            var tmax = float.MaxValue;

            if (!RayVsEdges(ray.Position.X, ray.Direction.X, ray.InverseDirection.X, rectangle.TopLeft.X, rectangle.BottomRight.X, ref tmin, ref tmax))
            {
                return false;
            }

            if (!RayVsEdges(ray.Position.Y, ray.Direction.Y, ray.InverseDirection.Y, rectangle.TopLeft.Y, rectangle.BottomRight.Y, ref tmin, ref tmax))
            {
                return false;
            }

            var time = MathF.Max(tmin, 0f);
            var point = ray.Position + ray.Direction * time;
            var normal = GetNormal(point, rectangle);
            intersection = new Intersection(point, normal, time);
            return true;
        }

        private static bool RayVsEdges(float position, float direction, float inverseDirection, float min, float max, ref float tmin, ref float tmax)
        {
            if (MathF.Abs(direction) < float.Epsilon && (position < min || position > max))
            {
                return false;
            }

            var t1 = (min - position) * inverseDirection;
            var t2 = (max - position) * inverseDirection;

            if (t1 > t2)
            {
                (t1, t2) = (t2, t1);
            }

            tmin = MathF.Max(tmin, t1);
            tmax = MathF.Min(tmax, t2);

            return tmin <= tmax;
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

            return TryGetRectangleCorner(rectangle, mask, out corner);
        }

        private static bool TryGetRectangleCorner(RectangleF rectangle, int mask, out Vector2 corner)
        {
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

        private static Vector2 GetNormal(Vector2 point, Vector2 center)
        {
            return Vector2.Normalize(point - center);
        }

        private static Vector2 GetNormal(Vector2 point, RectangleF rectangle)
        {
            var normal = Vector2.Zero;

            if (point.X == rectangle.Left)
            {
                normal.X = -1f;
            }
            else if (point.X == rectangle.Right)
            {
                normal.X = 1f;
            }
            else if (point.Y == rectangle.Top)
            {
                normal.Y = -1f;
            }
            else if (point.Y == rectangle.Bottom)
            {
                normal.Y = 1f;
            }

            return normal;
        }
    }
}
