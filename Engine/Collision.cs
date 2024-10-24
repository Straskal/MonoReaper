using Microsoft.Xna.Framework;

namespace Engine
{
    public readonly struct Collision(Vector2 velocity, Intersection intersection)
    {
        public static Collision Empty => new(Vector2.Zero, new Intersection(Vector2.Zero, Vector2.Zero, float.PositiveInfinity));

        public readonly Intersection Intersection = intersection;
        public readonly Vector2 Direction = Vector2.Normalize(velocity);
        public readonly float Length = velocity.Length();

        public readonly float RemainingTime => Length - Intersection.Time;
    }
}
