using Microsoft.Xna.Framework;

namespace Engine
{
    public readonly struct Intersection(Vector2 point, Vector2 normal, float time)
    {
        public static Intersection Empty => new(Vector2.Zero, Vector2.Zero, 0f);

        public readonly Vector2 Point = point;
        public readonly Vector2 Normal = normal;
        public readonly float Time = time;
    }
}
