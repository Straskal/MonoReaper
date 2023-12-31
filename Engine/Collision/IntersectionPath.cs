using Microsoft.Xna.Framework;

namespace Engine.Collision
{
    public readonly ref struct IntersectionPath
    {
        public IntersectionPath(Vector2 start, Vector2 velocity)
        {
            Ray = new RayF(start, velocity);
            Velocity = velocity;
            Length = Velocity.Length();
        }

        public readonly RayF Ray;
        public readonly Vector2 Velocity;
        public readonly float Length;

        public readonly Vector2 Position 
        {
            get => Ray.Position;
        }
        public readonly Vector2 NormalizedDirection 
        {
            get => Ray.Direction;
        }
    }
}
