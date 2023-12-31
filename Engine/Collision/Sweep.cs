using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine.Collision
{
    /// <summary>
    /// This class contains the functions for the AABB Swept collision checking.
    /// </summary>
    internal static class Sweep
    {
        public const float Correction = 0.005f;

        /// <summary>
        /// Test a vector against the given collidable objects. This is a raycast.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="others"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        public static bool Test(Vector2 position, Vector2 direction, IEnumerable<Box> others, out Collision hit)
        {
            return Test(new RectangleF(position.X, position.Y, 0f, 0f), direction, others, out hit);
        }

        /// <summary>
        /// Test a rectangle against the given collidable objects.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="velocity"></param>
        /// <param name="others">The other objects to test against.</param>
        /// <param name="collision">If there is a collision, hit will be output with all of the collision info.</param>
        /// <returns>Returns true if there a collision was detected.</returns>
        public static bool Test(RectangleF rect, Vector2 velocity, IEnumerable<Box> others, out Collision collision)
        {
            collision = Collision.Empty;

            foreach (var other in others)
            {
                var time = SweptAabbAabb(rect, other.Bounds, velocity, out var normal);

                if (time < collision.Time)
                {
                    collision = new Collision(
                        other: other,
                        velocity: velocity,
                        normal: normal,
                        collisionTime: time,
                        // If we don't add this correction, boxes can get stuck on corners.
                        // If we do add this correction, then that can create innacurate collision responses.
                        position: rect.Position + velocity * time + (Correction * normal)
                    );
                }
            }

            return collision.Time < 1f;
        }

        private static float SweptAabbAabb(RectangleF bounds, RectangleF otherBounds, Vector2 velocity, out Vector2 normal)
        {
            normal = Vector2.Zero;

            // Do not handle two objects that are already colliding.
            if (bounds.Intersects(otherBounds))
            {
                return 1f;
            }

            var dn = Vector2.Zero;
            var df = Vector2.Zero;
            var tn = Vector2.Zero;
            var tf = Vector2.Zero;

            // Calculate near and far distance
            if (velocity.X > 0f)
            {
                dn.X = otherBounds.Left - bounds.Right;
                df.X = otherBounds.Right - bounds.Left;
            }
            else
            {
                dn.X = otherBounds.Right - bounds.Left;
                df.X = otherBounds.Left - bounds.Right;
            }

            if (velocity.Y > 0f)
            {
                dn.Y = otherBounds.Top - bounds.Bottom;
                df.Y = otherBounds.Bottom - bounds.Top;
            }
            else
            {
                dn.Y = otherBounds.Bottom - bounds.Top;
                df.Y = otherBounds.Top - bounds.Bottom;
            }

            // Calculate near and far time
            if (Math.Abs(velocity.X) > 0.001f)
            {
                tn.X = dn.X / velocity.X;
                tf.X = df.X / velocity.X;
            }
            else
            {
                tn.X = float.MinValue;
                tf.X = float.MaxValue;
            }

            if (Math.Abs(velocity.Y) > 0.001f)
            {
                tn.Y = dn.Y / velocity.Y;
                tf.Y = df.Y / velocity.Y;
            }
            else
            {
                tn.Y = float.MinValue;
                tf.Y = float.MaxValue;
            }

            var n = Math.Max(Math.Max(tn.X, tn.Y), 0f);
            var f = Math.Min(Math.Min(tf.X, tf.Y), 1f);

            // No collision
            if (n > f)
            {
                return 1f;
            }

            // Calculate normal
            if (tn.X > tn.Y)
            {
                if (dn.X < 0f || Math.Abs(dn.X) < 0.0001f && df.X < 0f)
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
                if (dn.Y < 0f || Math.Abs(dn.Y) < 0.0001f && df.Y < 0f)
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

