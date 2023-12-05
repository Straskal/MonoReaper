using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Core.Collision
{
    public static class Sweep
    {
        public const float Correction = 0.005f;

        public static bool Test(Vector2 position, Vector2 direction, IEnumerable<Box> others, out Hit hit)
        {
            return Test(new RectangleF(position.X, position.Y, 0f, 0f), direction, others, out hit);
        }

        public static bool Test(RectangleF rect, Vector2 velocity, IEnumerable<Box> others, out Hit hit)
        {
            hit = Hit.Empty;

            foreach (var other in others)
            {
                var time = Perform(rect, other.CalculateBounds(), velocity, out var normal);

                if (time < hit.Time)
                {
                    hit = new Hit(
                        other: other,
                        velocity: velocity,
                        normal: normal,
                        collisionTime: time,
                        // Give buffer so we completely separate the two shapes.
                        position: rect.Position + velocity * time + (Correction * normal)
                    );
                }
            }

            return hit.Time < 1f;
        }

        private static float Perform(RectangleF bounds, RectangleF otherBounds, Vector2 velocity, out Vector2 normal)
        {
            normal = Vector2.Zero;

            // Do not handle two objects that are already colliding.
            if (bounds.Intersects(otherBounds)) 
            {
                return 1f;
            }

            float dnx, dfx;
            float dny, dfy;
            float tnx, tfx;
            float tny, tfy;

            // Calculate near and far distance
            if (velocity.X > 0f)
            {
                dnx = otherBounds.Left - bounds.Right;
                dfx = otherBounds.Right - bounds.Left;
            }
            else
            {
                dnx = otherBounds.Right - bounds.Left;
                dfx = otherBounds.Left - bounds.Right;
            }

            if (velocity.Y > 0f)
            {
                dny = otherBounds.Top - bounds.Bottom;
                dfy = otherBounds.Bottom - bounds.Top;
            }
            else
            {
                dny = otherBounds.Bottom - bounds.Top;
                dfy = otherBounds.Top - bounds.Bottom;
            }

            // Calculate near and far time
            if (Math.Abs(velocity.X) > 0.001f)
            {
                tnx = dnx / velocity.X;
                tfx = dfx / velocity.X;
            }
            else
            {
                tnx = float.MinValue;
                tfx = float.MaxValue;
            }

            if (Math.Abs(velocity.Y) > 0.001f)
            {
                tny = dny / velocity.Y;
                tfy = dfy / velocity.Y;
            }
            else
            {
                tny = float.MinValue;
                tfy = float.MaxValue;
            }

            var n = Math.Max(Math.Max(tnx, tny), 0f);
            var f = Math.Min(Math.Max(tfx, tfy), 1f);

            // No collision
            if (n > f)
            {
                return 1f;
            }

            // Calculate normal
            if (tnx > tny)
            {
                if (dnx < 0f)
                {
                    normal.X = 1f;
                    normal.Y = 0f;
                }
                else
                {
                    normal.X = -1f;
                    normal.Y = 0f;
                }
            }
            else
            {
                if (dny < 0f)
                {
                    normal.X = 0f;
                    normal.Y = 1f;
                }
                else
                {
                    normal.X = 0f;
                    normal.Y = -1f;
                }
            }

            return n;
        }
    }
}

