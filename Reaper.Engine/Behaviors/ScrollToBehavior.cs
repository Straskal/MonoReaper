using Microsoft.Xna.Framework;

namespace Reaper.Engine.Behaviors
{
    public class ScrollToBehavior : Behavior
    {
        public ScrollToBehavior(WorldObject owner) : base(owner)
        {
        }

        public float Smoothing { get; set; } = 0.3f;
        public Vector2 Offset { get; set; }

        public override void PostTick(GameTime gameTime)
        {
            Layout.Position = Vector2.SmoothStep(Layout.Position, Owner.Position + Offset, Smoothing);
        }
    }
}
