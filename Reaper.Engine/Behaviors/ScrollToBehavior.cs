using Microsoft.Xna.Framework;

namespace Reaper.Engine.Behaviors
{
    public class ScrollToBehavior : Behavior
    {
        public ScrollToBehavior(WorldObject owner) : base(owner) { }

        public float Smoothing { get; set; } = 0.3f;

        public override void OnLayoutStarted()
        {
            Layout.Position = Owner.Position;
        }

        public override void PostTick(GameTime gameTime)
        {
            Layout.Position = Vector2.SmoothStep(Layout.Position, Owner.Position, Smoothing);
        }
    }
}
