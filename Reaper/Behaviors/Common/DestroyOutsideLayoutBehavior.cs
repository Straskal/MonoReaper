using Microsoft.Xna.Framework;
using Reaper.Engine;

namespace Reaper.Behaviors.Common
{
    public class DestroyOutsideLayoutBehavior : Behavior
    {
        public DestroyOutsideLayoutBehavior(WorldObject owner) : base(owner)
        {
        }

        public override void Tick(GameTime gameTime)
        {
            if (Owner.Position.X < 0 || Owner.Position.X > Layout.Width || Owner.Position.Y < 0 || Owner.Position.Y > Layout.Height)
                Owner.Destroy();
        }
    }
}
