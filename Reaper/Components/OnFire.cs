using Microsoft.Xna.Framework;
using Reaper.Engine;
using Reaper.Engine.Components;

namespace Reaper.Components
{
    public sealed class OnFire : Component
    {
        private float duration;

        public Box Target { get; }

        public OnFire(Box target, float duration = 10f) 
        {
            Target = target;
            this.duration = duration;
        }

        public override void OnPostTick(GameTime gameTime)
        {
            Entity.Position = new Vector2(
                Target.Entity.Position.X,
                Target.Bounds.Top);

            duration -= gameTime.GetDeltaTime();

            if (duration <= 0f) 
            {
                Entity.Destroy();
            }
        }
    }
}
