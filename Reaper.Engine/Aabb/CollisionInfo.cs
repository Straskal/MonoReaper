using Microsoft.Xna.Framework;
using Reaper.Engine.Components;
using System;

namespace Reaper.Engine.AABB
{
    public struct CollisionInfo
    {
        public Box Other;
        public Vector2 Velocity;
        public Vector2 Normal;
        public Vector2 Position;
        public float Time;

        public float RemainingTime => 1f - Time;

        public static CollisionInfo NoHit(Vector2 position) 
        {
            return new(null, Vector2.Zero, Vector2.Zero, 1f, position);
        }

        public CollisionInfo(Box other, Vector2 velocity, Vector2 normal, float collisionTime, Vector2 position)
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
            if (!Other.IsSolid)
            {
                return Ignore();
            }

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
            if (!Other.IsSolid) 
            {
                return Ignore();
            }

            var velocity = Velocity;
            var normal = Normal;
            var dot = RemainingTime * (velocity.X * normal.Y + velocity.Y * normal.X);

            return new Vector2(normal.Y, normal.X) * dot; ;
        }
    }
}
