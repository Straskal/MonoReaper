using Core;
using Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Reaper
{
    public sealed class DistortionPostProcessingEffect : PostProcessEffect
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

        public DistortionPostProcessingEffect(Effect effect)
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

                var thickness = 0.1f;

                // Increase size from 0 -> 1
                var radius = _timer;

                // Decrease the force over time
                var multiplier = 1.5f;
                var force = 5f;
                var forceNormalized = MathHelper.SmoothStep(force, 0f, _timer * (multiplier * 2f));

                _effect.Parameters["thickness"].SetValue(thickness);
                _effect.Parameters["force"].SetValue(forceNormalized);
                _effect.Parameters["center"].SetValue(_explosion);
                _effect.Parameters["radius"].SetValue(radius * multiplier);


                //_effect.Parameters["thickness"].SetValue(0.01f);
                //_effect.Parameters["force"].SetValue(15f);
                //_effect.Parameters["center"].SetValue(new Vector2(0.2f, 0.2f));
                //_effect.Parameters["radius"].SetValue(0.2f);
            }
        }

        public override void Draw(Level level)
        {
            if (_explosion == Vector2.Zero) 
            {
                Renderer.Draw(level.RenderTexture, Vector2.Zero, Color.White);
                return;
            }

            Renderer.Draw(level.RenderTexture, Vector2.Zero, Color.White, _effect);
        }
    }
}
