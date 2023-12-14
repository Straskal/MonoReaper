using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Extensions;
using Engine.Graphics;

namespace Adventure
{
    public sealed class DistortionPostProcessingEffect : PostProcessingEffect
    {
        private static Vector2 _explosion;
        public static Vector2 Explosion
        {
            get => _explosion;
            set
            {
                _explosion = value;
                _timer = 0f;
            }
        }

        private static float _timer = 0f;

        public DistortionPostProcessingEffect(Effect effect) : base(effect)
        {
        }

        public override void OnUpdate(GameTime gameTime)
        {
            if (_explosion != Vector2.Zero)
            {
                _timer += gameTime.GetDeltaTime() * 2f;

                if (_timer > 1f)
                {
                    _timer = 0;
                    _explosion = Vector2.Zero;
                }

                var thickness = 0.001f;

                // Increase size from 0 -> 1
                var radius = _timer * 0.15f;

                // Decrease the force over time
                var force = 25f;
                var forceNormalized = MathHelper.SmoothStep(force, 0f, _timer);

                //Parameters["Thickness"].SetValue(thickness);
                //Parameters["Force"].SetValue(forceNormalized);
                //Parameters["Center"].SetValue(_explosion);
                //Parameters["Radius"].SetValue(radius); 
                //Parameters["Resolution"].SetValue(new Vector2(Resolution.RenderTargetWidth, Resolution.RenderTargetHeight));
                //Parameters["View"].SetValue(transformation);
            }
        }
    }
}
