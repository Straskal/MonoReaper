using Microsoft.Xna.Framework;
using Reaper.Engine;

namespace Reaper.Behaviors.Enemies
{
    public class BlobBehavior : Behavior
    {
        public BlobBehavior(WorldObject owner) : base(owner) { }

        public float Speed { get; set; } = 50f;
        public Vector2 Direction { get; set; } = new Vector2(0f, 1f);

        public override void Tick(GameTime gameTime)
        {
            if (Owner.MoveAndCollide(Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds, out var overlap))
            {
                Direction *= new Vector2(0, -1f);
            }    
        }
    }
}
