using Microsoft.Xna.Framework;

namespace Engine.Collision
{
    public struct CircleF
    {
        public CircleF(float x, float y, float radius)
            : this(new Vector2(x, y), radius)
        {
        }

        public CircleF(Vector2 position, float radius) 
        {
            Center = position;
            Radius = radius;
        }

        public Vector2 Center;
        public float Radius;
    }
}
