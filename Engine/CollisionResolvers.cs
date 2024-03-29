﻿using Microsoft.Xna.Framework;
using System;

namespace Engine
{
    public delegate Vector2 CollisionResolver(Collision collision);

    public static class CollisionResolvers
    {
        public static Vector2 Bounce(Collision collision)
        {
            var result = collision.Direction * collision.RemainingTime;

            if (Math.Abs(collision.Intersection.Normal.X) > float.Epsilon)
            {
                result.X *= -1;
            }
            if (Math.Abs(collision.Intersection.Normal.Y) > float.Epsilon)
            {
                result.Y *= -1;
            }

            return result;
        }

        public static Vector2 Slide(Collision collision)
        {
            var v = collision.Direction * collision.RemainingTime;
            return v - Vector2.Dot(v, collision.Intersection.Normal) * collision.Intersection.Normal;
        }
    }
}
