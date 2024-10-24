//using Microsoft.Xna.Framework;
//using System;

//namespace Engine
//{
//    public static class IntersectionTests
//    {
//        public static bool CircleSegmentVsCircle(CircleF a, Segment segment, CircleF b, out Intersection intersection)
//        {
//            // Test a line segment against B by A.
//            return SegmentVsCircle(segment, CircleF.Grow(b, a), out intersection);
//        }

//        public static bool CircleSegmentVsRectangle(CircleF a, Segment segment, RectangleF b, out Intersection intersection)
//        {
//            // Test a line segment against B by A.
//            if (!SegmentVsRectangle(segment, RectangleF.Grow(b, a), out intersection))
//            {
//                return false;
//            }

//            // Test to see if the intersection happened at a corner.
//            if (TryGetRectangleCorner(intersection.Point, b, out var corner))
//            {
//                // Test the line segment against the corner circle.
//                if (!SegmentVsCircle(segment, new CircleF(corner, a.Radius), out intersection)) 
//                {
//                    return false;
//                }
//            }

//            return true;
//        }

//        public static bool RectangleSegmentVsRectangle(RectangleF a, Segment segment, RectangleF b, out Intersection intersection)
//        {            
//            // Test a line segment against B by A.
//            return SegmentVsRectangle(segment, RectangleF.Grow(b, a), out intersection);
//        }

//        public static bool SegmentVsCircle(Segment segment, CircleF circle, out Intersection intersection)
//        {
//            // Test if ray intersects with circle and that the time of intersection happened within the line segment.
//            return RayVsCircle(segment.Ray, circle.Center, circle.Radius, out intersection) && intersection.Time <= segment.Length;
//        }

//        public static bool SegmentVsRectangle(Segment segment, RectangleF rectangle, out Intersection intersection)
//        {
//            // Test if ray intersects with rectangle and that the time of intersection happened within the line segment.
//            return RayVsRectangle(segment.Ray, rectangle, out intersection) && intersection.Time <= segment.Length;
//        }

//        public static bool RayVsCircle(Ray ray, Vector2 center, float radius, out Intersection intersection)
//        {
//            intersection = Intersection.Empty;

//            // Calclate direction from ray to circle center.
//            var m = ray.Position - center;

//            // Calculate the dot product of ray direction to circle and direction of ray. 
//            var b = Vector2.Dot(m, ray.Direction);

//            // If dot product is greater than 0, then the ray is moving away from the circle and there is no intersection.
//            if (b > 0f)
//            {
//                return false;
//            }

//            // Below we work with squared values to avoid expensive square root calculations.
//            // Calculate the square length of the direction to circle and subtract the radius squared.
//            var c = Vector2.Dot(m, m) - radius * radius;

//            // Calculate the discriminant of quadratic equation.
//            var d = b * b - c;

//            // If the discriminant is negative, then we do not have any solutions and there is no intersection.
//            if (d < 0f)
//            {
//                return false;
//            }

//            // Take the nearest solution of quadratic equation because we want to know the time of the ray entering the circle, not leaving it.
//            var time = MathF.Max(-b - MathF.Sqrt(d), 0f);

//            // Calculate the point at which the intersection occurs.
//            var point = ray.Position + ray.Direction * time;

//            // Calculate the normal by normalizing the direction of the intersection point to the center of the circle.
//            var normal = Vector2.Normalize(point - center);

//            intersection = new Intersection(point, normal, time);

//            return true;
//        }

//        public static bool RayVsRectangle(Ray ray, RectangleF rectangle, out Intersection intersection)
//        {
//            intersection = Intersection.Empty;

//            var tmin = 0f;
//            var tmax = float.MaxValue;

//            if (!RayVsEdges(ray.Position.X, ray.Direction.X, ray.InverseDirection.X, rectangle.TopLeft.X, rectangle.BottomRight.X, ref tmin, ref tmax))
//            {
//                return false;
//            }

//            if (!RayVsEdges(ray.Position.Y, ray.Direction.Y, ray.InverseDirection.Y, rectangle.TopLeft.Y, rectangle.BottomRight.Y, ref tmin, ref tmax))
//            {
//                return false;
//            }

//            var time = MathF.Max(tmin, 0f);
//            var point = ray.Position + ray.Direction * time;
//            var normal = GetNormal(point, rectangle);

//            intersection = new Intersection(point, normal, time);

//            return true;
//        }

//        private static bool RayVsEdges(float position, float direction, float inverseDirection, float min, float max, ref float tmin, ref float tmax)
//        {
//            if (MathF.Abs(direction) < float.Epsilon && (position < min || position > max))
//            {
//                return false;
//            }

//            var t1 = (min - position) * inverseDirection;
//            var t2 = (max - position) * inverseDirection;

//            if (t1 > t2)
//            {
//                (t1, t2) = (t2, t1);
//            }

//            tmin = MathF.Max(tmin, t1);
//            tmax = MathF.Min(tmax, t2);

//            return tmin < tmax;
//        }

//        private static bool TryGetRectangleCorner(Vector2 point, RectangleF rectangle, out Vector2 corner)
//        {
//            var mask = 0;

//            if (point.X < rectangle.Left)
//            {
//                mask |= 1;
//            }
//            else if (point.X > rectangle.Right)
//            {
//                mask |= 2;
//            }

//            if (point.Y < rectangle.Top)
//            {
//                mask |= 4;
//            }
//            else if (point.Y > rectangle.Bottom)
//            {
//                mask |= 8;
//            }

//            return TryGetRectangleCorner(rectangle, mask, out corner);
//        }

//        private static bool TryGetRectangleCorner(RectangleF rectangle, int mask, out Vector2 corner)
//        {
//            switch (mask)
//            {
//                case 1 | 4:
//                    corner = rectangle.TopLeft;
//                    break;
//                case 2 | 4:
//                    corner = rectangle.TopRight;
//                    break;
//                case 1 | 8:
//                    corner = rectangle.BottomLeft;
//                    break;
//                case 2 | 8:
//                    corner = rectangle.BottomRight;
//                    break;
//                default:
//                    corner = Vector2.Zero;
//                    return false;
//            }

//            return true;
//        }

//        private static Vector2 GetNormal(Vector2 point, RectangleF rectangle)
//        {
//            var normal = Vector2.Zero;

//            if (point.X == rectangle.Left)
//            {
//                normal.X = -1f;
//            }
//            else if (point.X == rectangle.Right)
//            {
//                normal.X = 1f;
//            }
//            else if (point.Y == rectangle.Top)
//            {
//                normal.Y = -1f;
//            }
//            else if (point.Y == rectangle.Bottom)
//            {
//                normal.Y = 1f;
//            }

//            return normal;
//        }
//    }
//}
