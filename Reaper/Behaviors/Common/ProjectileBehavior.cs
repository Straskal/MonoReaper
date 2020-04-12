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
        }

        private void CheckOverlap(WorldObject overlap) 
        {
            if (overlap.Tags.Contains("enemy"))
            {
                overlap.Destroy();
                Owner.Destroy();
            }
            else if (overlap.IsSolid)
                Owner.Destroy();
        }
    }
}
