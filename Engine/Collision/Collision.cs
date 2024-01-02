using Microsoft.Xna.Framework;
using System;

namespace Engine.Collision
{
    public readonly ref struct Collision
    {
        public static Collision Empty => new(null, Vector2.Zero, Vector2.Zero, float.PositiveInfinity, Vector2.Zero);

        public Collision(Collider other, Vector2 velocity, Vector2 normal, float collisionTime, Vector2 position)
        {
            Collider = other;
            Velocity = velocity;
            Normal = normal;
            Time = collisionTime;
            Position = position;
            Direction = Vector2.Normalize(Velocity);
            Length = Velocity.Length();
            RemainingTime = Length - Time;
        }

        public readonly Collider Collider;
        public readonly Vector2 Velocity;
        public readonly Vector2 Direction;
        public readonly Vector2 Normal;
        public readonly Vector2 Position;
        public readonly float Time;
        public readonly float Length;
        public readonly float RemainingTime;

        public readonly bool IsEmpty 
        {
            get => Time == float.PositiveInfinity;
        }

        public Vector2 Ignore()
        {
            return Direction * RemainingTime;
        }

        public Vector2 Bounce()
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

        public Vector2 Slide()
        {
            return Direction * RemainingTime - Vector2.Dot(Direction * RemainingTime, Normal) * Normal;
        }
    }
}
