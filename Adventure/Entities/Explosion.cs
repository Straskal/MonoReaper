using Engine;
using Microsoft.Xna.Framework;
using System;
using static Adventure.Constants;

namespace Adventure.Entities
{
    public sealed class Explosion : Entity
    {
        private AnimatedSprite sprite;
        private bool causedDamage;

        public override void Spawn()
        {
            GraphicsComponent = sprite = new AnimatedSprite(this, Store.Gfx.Explosion, ExplosionAnimations.Frames)
            {
                Speed = 0.1f,
                DrawOrder = 10
            };

            PlayExplosionSound();
            ScreenShake.Shake(0.5f);
        }

        public override void Update(GameTime gameTime)
        {
            DoRadialDamage();
            DestroyAfterAnimation();
        }

        private void DoRadialDamage() 
        {
            if (!causedDamage && sprite.CurrentFrame == 3)
            {
                foreach (var collider in World.OverlapCircle(new CircleF(Position, 20), EntityLayers.Enemy))
                {
                    if (collider is IDamageable damageable)
                    {
                        damageable.Damage(1);

                        if (damageable.Flammable)
                        {
                            World.Spawn(new Fire(collider.Entity));
                        }
                    }
                }

                causedDamage = true;
            }
        }

        private void DestroyAfterAnimation() 
        {
            if (sprite.CurrentAnimationFinished)
            {
                World.Destroy(this);
            }
        }

        private static void PlayExplosionSound()
        {
            var sound = Store.Sfx.Explosion.CreateInstance();
            sound.Pitch = 1f / Random.Shared.Next(8, 10);
            sound.Play();
        }
    }
}
