﻿using Microsoft.Xna.Framework;
using System;

namespace Core.Collision
{
    public readonly ref struct Hit
    {
        public readonly Box Other;
        public readonly Vector2 Velocity;
        public readonly Vector2 Normal;
        public readonly Vector2 Position;
        public readonly float Time;

        public float RemainingTime => 1f - Time;

        public static Hit Empty => new(null, Vector2.Zero, Vector2.Zero, 1f, Vector2.Zero);

        public Hit(Box other, Vector2 velocity, Vector2 normal, float collisionTime, Vector2 position)
        {
            Other = other;
            Velocity = velocity;
            Normal = normal;
            Time = collisionTime;
            Position = position;
        }

        public Vector2 Ignore()
        {
            return Velocity * RemainingTime;
        }

        public Vector2 Bounce(ref Vector2 newVelocity)
        {
            newVelocity = Velocity * RemainingTime;
            if (Math.Abs(Normal.X) > 0.0001f)
            {
                newVelocity.X *= -1;
            }
            if (Math.Abs(Normal.Y) > 0.0001f)
            {
                newVelocity.Y *= -1;
            }
            return newVelocity;
        }

        public Vector2 Slide()
        {
            var dot = RemainingTime * (Velocity.X * Normal.Y + Velocity.Y * Normal.X);
            return new Vector2(Normal.Y, Normal.X) * dot;
        }
    }
}
