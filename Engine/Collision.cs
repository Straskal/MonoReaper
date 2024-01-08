﻿using Microsoft.Xna.Framework;
using System;

namespace Engine
{
    public readonly ref struct Collision
    {
        public static Collision Empty => new(Vector2.Zero, Vector2.Zero, float.PositiveInfinity, Vector2.Zero);

        public Collision(Vector2 velocity, Vector2 normal, float collisionTime, Vector2 position)
        {
            Velocity = velocity;
            Normal = normal;
            Time = collisionTime;
            Position = position;
            Direction = Vector2.Normalize(Velocity);
            Length = Velocity.Length();
        }

        public readonly Vector2 Velocity;
        public readonly Vector2 Direction;
        public readonly Vector2 Normal;
        public readonly Vector2 Position;
        public readonly float Time;
        public readonly float Length;

        public readonly float RemainingTime => Length - Time;
        public readonly bool IsEmpty => Time == float.PositiveInfinity;

        public readonly Vector2 Ignore()
        {
            return Direction * RemainingTime;
        }

        public readonly Vector2 Bounce()
        {
            var result = Direction * RemainingTime;

            if (Math.Abs(Normal.X) > float.Epsilon)
            {
                result.X *= -1;
            }
            if (Math.Abs(Normal.Y) > float.Epsilon)
            {
                result.Y *= -1;
            }

            return result;
        }

        public readonly Vector2 Slide()
        {
            var v = Direction * RemainingTime;
            return v - Vector2.Dot(v, Normal) * Normal;
        }
    }
}
