using Microsoft.Xna.Framework;

namespace Reaper.Engine.Behaviors
{
    public class ScrollToBehavior : Behavior
    {
        public ScrollToBehavior(WorldObject owner) : base(owner) { }

        public float Smoothing { get; set; } = 0.3f;

        public override void OnLayoutStarted()
        {
            Layout.View.Position = Owner.Position;
        }

        public override void PostTick(GameTime gameTime)
        {
            // Subpixel rendering seems to be causing weird artifacts.
            Layout.View.Position = Vector2.SmoothStep(Layout.View.Position, Owner.Position, Smoothing);
            //Layout.View.Position = new Vector2((int)Layout.View.Position.X, (int)Layout.View.Position.Y);
        }
    }
}
