using Microsoft.Xna.Framework;
using Reaper.Engine;

namespace Reaper
{
    public class ProjectileBehavior : Behavior
    {
        public ProjectileBehavior(WorldObject owner) : base(owner) { }

        public Vector2 Direction { get; set; }
        public float Speed { get; set; }

        public override void Tick(GameTime gameTime)
        {
            if (Direction.Length() > 1f)
                Direction.Normalize();

            if (Owner.MoveAndOverlap(Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds, out var overlap)) 
                CheckOverlap(overlap);
        }

        private void CheckOverlap(Overlap overlap) 
        {
            if (overlap.Other.TryGetBehavior<IDamageable>(out var behavior))
            {
                behavior.Damage(1);
                Owner.Destroy();
            }
            else if (overlap.Other.IsSolid)
                Owner.Destroy();
        }
    }
}
