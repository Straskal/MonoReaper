using Microsoft.Xna.Framework;
using Core;
using Core.Collision;
using Core.Graphics;

using static Reaper.Constants;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Reaper.Components
{
    public sealed class Fireball : Component
    {
        private Body _body;
        private SoundEffect _sound;

        public Fireball(Vector2 velocity)
        {
            _velocity = velocity;
        }

        private Vector2 _velocity;
        public Vector2 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public static void Preload(ContentManager content) 
        {
            content.Load<Texture2D>("art/player/fire");
            content.Load<SoundEffect>("audio/fireball_shoot");
        }

        public override void OnLoad(ContentManager content)
        {
            var texture = content.Load<Texture2D>("art/player/fire");

            _sound = content.Load<SoundEffect>("audio/fireball_shoot");

            Entity.AddComponent(_body = new Body(4, 4, EntityLayers.PlayerProjectile));
            Entity.AddComponent(new Particles(texture, new Rectangle(8, 8, 8, 8))
            {
                MaxParticles = 100,
                MaxVelocity = new Vector2(25f),
                MaxAngularVelocity = 10f,
                MinColor = Color.White,
                MaxColor = Color.White * 0.1f,
                MaxTime = 0.25f,
                ZOrder = 5
            });
        }

        public override void OnSpawn()
        {
            _sound.Play();
            //DistortionPostProcessingEffect.explostionPoint = Entity.Position / new Vector2(App.ResolutionWidth, App.ResolutionHeight);
        }

        public override void OnTick(GameTime gameTime)
        {
            _body.Move(_velocity, EntityLayers.Enemy | EntityLayers.Wall, hit =>
            {
                //DistortionPostProcessingEffect.explostionPoint = Entity.Position / new Vector2(App.ResolutionWidth, App.ResolutionHeight);

                var e = new Entity(Origin.Center);
                e.AddComponent(new Explosion());
                Level.Spawn(e, Entity.Position);

                Level.Destroy(Entity);

                if (hit.Other.Entity.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.Damage(1);

                    if (damageable.Flammable) 
                    {
                        hit.Other.Entity.AddComponent(new OnFire());
                    }                 
                }

                return Vector2.Zero;
            });

            Entity.Position += Velocity * gameTime.GetDeltaTime();
        }
    }
}
