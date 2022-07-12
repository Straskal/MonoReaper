using Core;
using Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Reaper
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

        public DistortionPostProcessingEffect(Effect effect) : base(App.Graphics, App.ViewportWidth, App.ViewportHeight)
        {
            _effect = effect;
        }

        public override void OnTick(GameTime gameTime)
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

                _effect.Parameters["thickness"].SetValue(thickness);
                _effect.Parameters["force"].SetValue(forceNormalized);
                _effect.Parameters["center"].SetValue(_explosion);
                _effect.Parameters["radius"].SetValue(radius);
            }
        }

        public override void OnDraw(Level level)
        {
            _effect.Parameters["resolution"].SetValue(new Vector2(App.ViewportWidth, App.ViewportHeight));
            _effect.Parameters["view"].SetValue(level.Camera.TransformationMatrix);

            if (_explosion == Vector2.Zero) 
            {
                Renderer.Draw(level.RenderTarget, Vector2.Zero, Color.White);
                return;
            }

            Renderer.Draw(level.RenderTarget, Vector2.Zero, Color.White, _effect);
        }
    }
}
