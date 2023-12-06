using Engine;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    public sealed class DistortionPostProcessingEffect : PostProcessingEffect
    {
        private readonly Effect _effect;

        private static Vector2 _explosion;
        public static Vector2 Explosion
        {
            set
            {
                _explosion = value;
                _timer = 0f;
            }
        }

        private static float _timer = 0f;

        public DistortionPostProcessingEffect(Effect effect) : base(App.Graphics, Resolution.RenderTargetWidth, Resolution.RenderTargetHeight)
        {
            _effect = effect;
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

                _effect.Parameters["Thickness"].SetValue(thickness);
                _effect.Parameters["Force"].SetValue(forceNormalized);
                _effect.Parameters["Center"].SetValue(_explosion);
                _effect.Parameters["Radius"].SetValue(radius);
            }
        }

        public override void OnDraw(Texture2D currentTarget, Matrix transformation)
        {
            _effect.Parameters["Resolution"].SetValue(new Vector2(Resolution.RenderTargetWidth, Resolution.RenderTargetHeight));
            _effect.Parameters["View"].SetValue(transformation);

            if (_explosion == Vector2.Zero)
            {
                Renderer.Draw(currentTarget, Vector2.Zero, Color.White);
                return;
            }

            Renderer.Draw(currentTarget, Vector2.Zero, Color.White, _effect);
        }
    }
}
