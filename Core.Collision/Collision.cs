using Microsoft.Xna.Framework;

namespace Engine
{
    public readonly struct Collision
    {
        public static Collision Empty => new(Vector2.Zero, new Intersection(Vector2.Zero, Vector2.Zero, float.PositiveInfinity));

        public Collision(Vector2 velocity, Intersection intersection)
        {
            Direction = Vector2.Normalize(velocity);
            Length = velocity.Length();
            Intersection = intersection;
        }

        public readonly Intersection Intersection;
        public readonly Vector2 Direction;
        public readonly float Length;

        public readonly float RemainingTime => Length - Intersection.Time;
    }
}
