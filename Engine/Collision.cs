using Microsoft.Xna.Framework;

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
            Contact = position;
            Direction = Vector2.Normalize(Velocity);
            Length = Velocity.Length();
        }

        public readonly Vector2 Velocity;
        public readonly Vector2 Direction;
        public readonly Vector2 Normal;
        public readonly Vector2 Contact;
        public readonly float Time;
        public readonly float Length;

        public readonly float RemainingTime => Length - Time;
        public readonly bool IsEmpty => Time == float.PositiveInfinity;
    }
}
