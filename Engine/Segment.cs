using Microsoft.Xna.Framework;

namespace Engine
{
    public readonly struct Segment
    {
        public Segment(Vector2 position, Vector2 direction)
        {
            Ray = new Ray(position, direction);
            Length = direction.Length();
        }

        public readonly Ray Ray;
        public readonly float Length;
    }
}
