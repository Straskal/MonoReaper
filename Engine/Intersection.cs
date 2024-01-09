using Microsoft.Xna.Framework;

namespace Engine
{
    public readonly struct Intersection
    {
        public static Intersection Empty => new(Vector2.Zero, Vector2.Zero, 0f);

        public Intersection(Vector2 point, Vector2 normal, float time) 
        {
            Point = point;
            Normal = normal;
            Time = time;
        }

        public readonly Vector2 Point;
        public readonly Vector2 Normal;
        public readonly float Time;
    }
}
