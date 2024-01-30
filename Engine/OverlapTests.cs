using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Engine
{
    public static class OverlapTests
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CircleVsCircle(CircleF circle0, CircleF circle1)
        {
            return CircleVsPoint(CircleF.Inflate(circle0, circle1), circle1.Center);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CircleVsRectangle(CircleF circle, RectangleF rectangle)
        {
            var closestPoint = new Vector2(
                Math.Clamp(circle.Center.X, rectangle.Left, rectangle.Right),
                Math.Clamp(circle.Center.Y, rectangle.Top, rectangle.Bottom));

            return CircleVsPoint(circle, closestPoint);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CircleVsPoint(CircleF circle, Vector2 point)
        {
            return (circle.Center - point).LengthSquared() < circle.Radius * circle.Radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RectangleVsRectangle(RectangleF rectangle0, RectangleF rectangle1)
        {
            return rectangle1.X < rectangle0.Right
                && rectangle0.X < rectangle1.Right
                && rectangle1.Y < rectangle0.Bottom
                && rectangle0.Y < rectangle1.Bottom;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RectangleVsCircle(RectangleF rectangle, CircleF circle)
        {
            return CircleVsRectangle(circle, rectangle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RectangleVsPoint(RectangleF rectangle0, Vector2 point)
        {
            return point.X > rectangle0.X
                && point.X < rectangle0.Right
                && point.Y > rectangle0.Y
                && point.Y < rectangle0.Bottom;
        }
    }
}
