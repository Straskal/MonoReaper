using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Reaper.Engine.Components;

namespace Reaper.Engine.AABB
{
    public static class Collision
    {
        public const float CorrectionBuffer = 0.005f;
        public const float BroadphasePadding = 10f;

        public static bool TestAABB(Box box, Vector2 velocity, IEnumerable<Box> others, out CollisionInfo info)
        {
            info = CollisionInfo.NoHit(position: box.Entity.Position + velocity);

            var broadphase = GetBroadphaseRectangle(box.Bounds.Position, box.Bounds.Size, velocity);
            var bounds = box.Bounds;

            foreach (var other in others)
            {
                var otherBounds = other.Bounds;

                if (broadphase.Intersects(otherBounds)) 
                {
                    var collisionTime = Allocate_Sweep(bounds.Position, bounds.Size, otherBounds.Position, otherBounds.Size, velocity, out var normal);

                    if (collisionTime < info.Time)
                    {
                        info = new CollisionInfo(
                            other: other,
                            velocity: velocity,
                            normal: normal,
                            collisionTime: collisionTime,
                            position: box.Entity.Position + velocity * collisionTime + (CorrectionBuffer * normal)); // Give buffer so we completely separate the two shapes.
                    }
                }
            }

            return info.Time < 1f;
        }

        public static bool TestRay(Vector2 position, Vector2 direction, IEnumerable<Box> others, out CollisionInfo info)
        {
            info = CollisionInfo.NoHit(position: position + direction);

            var broadphase = GetBroadphaseRectangle(position, Vector2.Zero, direction);

            foreach (var other in others)
            {
                var otherBounds = other.Bounds;

                if (broadphase.Intersects(otherBounds)) 
                {
                    var collisionTime = Allocate_Sweep(position, Vector2.Zero, otherBounds.Position, otherBounds.Size, direction, out var normal);

                    if (collisionTime < info.Time)
                    {
                        info = new CollisionInfo(
                            other: other,
                            velocity: direction,
                            normal: normal,
                            collisionTime: collisionTime,
                            position: position + direction * collisionTime + (CorrectionBuffer * normal)); // Give buffer so we completely separate the two shapes.
                    }
                }
            }

            return info.Time < 1f;
        }

        private static RectangleF GetBroadphaseRectangle(Vector2 position, Vector2 padding, Vector2 length)
        {
            var offset = position + length;

            RectangleF broadphase;

            broadphase.X        = Math.Min(position.X, offset.X);
            broadphase.Y        = Math.Min(position.Y, offset.Y);
            broadphase.Width    = Math.Abs(length.X) + padding.X;
            broadphase.Height   = Math.Abs(length.Y) + padding.Y;

            broadphase.X        -= BroadphasePadding;
            broadphase.Y        -= BroadphasePadding;
            broadphase.Width    += BroadphasePadding;
            broadphase.Height   += BroadphasePadding;

            return broadphase;
        }

        private static float Allocate_Sweep(Vector2 a, Vector2 aPadding, Vector2 b, Vector2 bPadding, Vector2 direction, out Vector2 normal)
        {
            Span<float> amin    = stackalloc float[2];
            Span<float> amax    = stackalloc float[2];
            Span<float> bmin    = stackalloc float[2];
            Span<float> bmax    = stackalloc float[2];
            Span<float> v       = stackalloc float[2];
            Span<float> dn      = stackalloc float[2];
            Span<float> df      = stackalloc float[2];
            Span<float> tn      = stackalloc float[2];
            Span<float> tf      = stackalloc float[2];

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

        private static float Allocate_RayAABBSweep(Vector2 position, Vector2 direction, RectangleF b, out Vector2 normal) 
        {
            Span<float> amin    = stackalloc float[2];
            Span<float> amax    = stackalloc float[2];
            Span<float> bmin    = stackalloc float[2];
            Span<float> bmax    = stackalloc float[2];
            Span<float> v       = stackalloc float[2];
            Span<float> dn      = stackalloc float[2];
            Span<float> df      = stackalloc float[2];
            Span<float> tn      = stackalloc float[2];
            Span<float> tf      = stackalloc float[2];

            amin[0] = position.X;
            amin[1] = position.Y;
            amax[0] = position.X;
            amax[1] = position.Y;

            bmin[0] = b.Left;
            bmin[1] = b.Top;
            bmax[0] = b.Right;
            bmax[1] = b.Bottom;

            v[0] = direction.X;
            v[1] = direction.Y;

            return Sweep(amin, amax, bmin, bmax, v, dn, df, tn, tf, out normal);
        }

        private static float Allocate_AABBAABBSweep(RectangleF a, RectangleF b, Vector2 velocity, out Vector2 normal)
        {
            Span<float> amin    = stackalloc float[2];
            Span<float> amax    = stackalloc float[2];
            Span<float> bmin    = stackalloc float[2];
            Span<float> bmax    = stackalloc float[2];
            Span<float> v       = stackalloc float[2];
            Span<float> dn      = stackalloc float[2];
            Span<float> df      = stackalloc float[2];
            Span<float> tn      = stackalloc float[2];
            Span<float> tf      = stackalloc float[2];

            amin[0] = a.Left;
            amin[1] = a.Top;
            amax[0] = a.Right;
            amax[1] = a.Bottom;

            bmin[0] = b.Left;
            bmin[1] = b.Top;
            bmax[0] = b.Right;
            bmax[1] = b.Bottom;

            v[0] = velocity.X;
            v[1] = velocity.Y;

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

            for (int i = 0; i < 2; i++)
            {
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

                if (Math.Abs(v[i]) < 0.0001f)
                {
                    tn[i] = float.MinValue;
                    tf[i] = float.MaxValue;
                }
                else
                {
                    tn[i] = dn[i] / v[i];
                    tf[i] = df[i] / v[i];
                }
            }

            float near = Math.Max(tn[0], tn[1]);
            float far = Math.Min(tf[0], tf[1]);

            if (near > far)
            {
                return 1f;
            }

            for (int i = 0; i < 2; i++)
            {
                if (tn[i] > 1f)
                {
                    tn[i] = float.MinValue;
                }
            }

            if (tn[0] < 0f && tn[1] < 0f)
            {
                return 1f;
            }

            for (int i = 0; i < 2; i++)
            {
                if (tn[i] < 0f && (amax[i] < bmin[i] || amin[i] > bmax[i]))
                {
                    return 1f;
                }
            }

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

            return near;
        }
    }
}
