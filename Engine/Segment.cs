using Microsoft.Xna.Framework;

namespace Engine
{
    public readonly ref struct Segment
    {
        public Segment(Vector2 start, Vector2 velocity)
        {
            Ray = new Ray(start, velocity);
            Velocity = velocity;
            Length = Velocity.Length();
        }

        public readonly Ray Ray;
        public readonly Vector2 Velocity;
        public readonly float Length;
    }
}
