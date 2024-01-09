using Microsoft.Xna.Framework;

namespace Engine
{
    public readonly ref struct Collision
    {
        public static Collision Empty => new(Vector2.Zero, new Intersection(Vector2.Zero, Vector2.Zero, float.PositiveInfinity));

        public Collision(Vector2 velocity, Intersection intersection)
        {
            Velocity = velocity;
            Point = intersection.Point;
            Normal = intersection.Normal;
            Time = intersection.Time;
            Direction = Vector2.Normalize(Velocity);
            Length = Velocity.Length();
        }

        public readonly Vector2 Velocity;
        public readonly Vector2 Direction;
        public readonly Vector2 Normal;
        public readonly Vector2 Point;
        public readonly float Time;
        public readonly float Length;

        public readonly float RemainingTime => Length - Time;
    }
}
