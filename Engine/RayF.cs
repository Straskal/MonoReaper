using Microsoft.Xna.Framework;

namespace Engine
{
    public readonly ref struct RayF
    {
        public RayF(Vector2 position, Vector2 direction)
        {
            Position = position;
            Direction = Vector2.Normalize(direction);
            InverseDirection = Vector2.One / Direction;
        }

        public readonly Vector2 Position;
        public readonly Vector2 Direction;
        public readonly Vector2 InverseDirection;
    }
}
