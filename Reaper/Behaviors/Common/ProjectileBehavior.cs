using Microsoft.Xna.Framework;
using Reaper.Engine;
using System.Linq;

namespace Reaper.Behaviors.Common
{
    public class ProjectileBehavior : Behavior
    {
        public ProjectileBehavior(WorldObject owner) : base(owner) { }

        public Vector2 Direction { get; set; }
        public float Speed { get; set; }

        public override void Tick(GameTime gameTime)
        {
            foreach (var overlap in Owner.MoveXAndOverlap(Direction.X * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds)) 
            {
                if (overlap.Tags.Contains("enemy") || overlap.IsSolid)
                {
                    overlap.Destroy();
                    Owner.Destroy();
                }
            }

            foreach (var overlap in Owner.MoveYAndOverlap(Direction.Y * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds))
            {
                if (overlap.Tags.Contains("enemy") || overlap.IsSolid)
                {
                    overlap.Destroy();
                    Owner.Destroy();
                }
            }
        }
    }
}
