using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Reaper.Engine.Components;

namespace Reaper.Engine.AABB
{
    public static class Collision
    {
        public const float CorrectionBuffer = 0.005f;

        public static bool TestAABB(RectangleF bounds, Vector2 velocity, IEnumerable<Box> others, out Hit hit)
        {
            return Test(bounds.Position, bounds.Size, velocity, others, out hit);
        }

        public static bool TestRay(Vector2 position, Vector2 direction, IEnumerable<Box> others, out Hit hit)
        {
            return Test(position, Vector2.Zero, direction, others, out hit);
        }

        private static bool Test(Vector2 position, Vector2 padding, Vector2 velocity, IEnumerable<Box> others, out Hit hit)
        {
            hit = Hit.NoHit(position: position + velocity);

            foreach (var other in others)
            {
                var otherBounds = other.Bounds;

                var collisionTime = Allocate_Sweep(position, padding, otherBounds.Position, otherBounds.Size, velocity, out var normal);

                if (collisionTime < hit.Time)
                {
                    hit = new Hit(
                        other: other,
                        velocity: velocity,
                        normal: normal,
                        collisionTime: collisionTime,
                        position: position + velocity * collisionTime + (CorrectionBuffer * normal)); // Give buffer so we completely separate the two shapes.
                }
            }

            return hit.Time < 1f;
        }

        public static RectangleF GetBroadphaseRectangle(Vector2 position, Vector2 padding, Vector2 length)
        {
            var offset = position + length;

            RectangleF broadphase;

            broadphase.X = Math.Min(position.X, offset.X);
            broadphase.Y = Math.Min(position.Y, offset.Y);
            broadphase.Width = Math.Abs(length.X) + padding.X;
            broadphase.Height = Math.Abs(length.Y) + padding.Y;

            return broadphase;
        }

        private static float Allocate_Sweep(Vector2 a, Vector2 aPadding, Vector2 b, Vector2 bPadding, Vector2 direction, out Vector2 normal)
        {
            Span<float> amin = stackalloc float[2];
            Span<float> amax = stackalloc float[2];
            Span<float> bmin = stackalloc float[2];
            Span<float> bmax = stackalloc float[2];
            Span<float> v = stackalloc float[2];
            Span<float> dn = stackalloc float[2];
            Span<float> df = stackalloc float[2];
            Span<float> tn = stackalloc float[2];
            Span<float> tf = stackalloc float[2];

            amin[0] = a.X;
            amin[1] = a.Y;
            amax[0] = a.X + aPadding.X;
            amax[1] = a.Y + aPadding.Y;

            bmin[0] = b.X;
            bmin[1] = b.Y;
            bmax[0] = b.X + bPadding.X;
            bmax[1] = b.Y + bPadding.Y;

            v[0] = direction.X;
            v[1] = direction.Y;

            return Sweep(amin, amax, bmin, bmax, v, dn, df, tn, tf, out normal);
        }

        private static float Sweep(
            Span<float> amin,
            Span<float> amax,
            Span<float> bmin,
            Span<float> bmax,
            Span<float> v,
            Span<float> dn,
            Span<float> df,
            Span<float> tn,
            Span<float> tf,
            out Vector2 normal
        )
        {
            normal.X = 0;
            normal.Y = 0;

            var n = 0f;
            var f = 1f;

            for (int i = 0; i < 2; i++)
            {
                // Calculate distance
                if (v[i] > 0f)
                {
                    dn[i] = bmin[i] - amax[i];
                    df[i] = bmax[i] - amin[i];
                }
                else
                {
                    dn[i] = bmax[i] - amin[i];
                    df[i] = bmin[i] - amax[i];
                }

                // Calculate time
                if (Math.Abs(v[i]) > 0.0001f)
                {
                    tn[i] = dn[i] / v[i];
                    tf[i] = df[i] / v[i];
                }
                else
                {
                    tn[i] = float.MinValue;
                    tf[i] = float.MaxValue;
                }

                // Merge axis time
                n = Math.Max(tn[i], n);
                f = Math.Min(tf[i], f);

                // No collision
                if (n > f)
                {
                    return 1f;
                }

                // No collision
                if (tn[i] < 0f && amin[i] > bmax[i])
                {
                    return 1f;
                }

                // No collision
                if (tn[i] > 1f && amax[i] < bmin[i])
                {
                    return 1f;
                }
            }

            // No collision
            if (tn[0] > 1f && tn[1] > 1f)
            {
                return 1f;
            }

            // Calculate normal
            if (tn[0] > tn[1])
            {
                if (dn[0] < 0f || Math.Abs(dn[0]) < 0.0001f && df[0] < 0f)
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
                if (dn[1] < 0f || Math.Abs(dn[1]) < 0.0001f && df[1] < 0f)
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

