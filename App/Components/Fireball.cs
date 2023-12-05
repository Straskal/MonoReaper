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

            IsUpdateEnabled = true;
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

        public static Entity Create(Vector2 velocity) 
        {
            return new Entity(Origin.Center, new Fireball(velocity));
        }

        public override void OnLoad(ContentManager content)
        {
            _sound = content.Load<SoundEffect>("audio/fireball_shoot");
            Entity.AddComponent(_body = new Body(4, 4, EntityLayers.PlayerProjectile));
            Entity.AddComponent(new Particles(content.Load<Texture2D>("art/player/fire"), new Rectangle(8, 8, 8, 8))
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
        }

        public override void OnUpdate(GameTime gameTime)
        {
            _body.MoveAndCollide(ref _velocity, EntityLayers.Enemy | EntityLayers.Solid, HandleCollision);
        }

        private Vector2 HandleCollision(Hit hit)
        {
            Level.Destroy(Entity);
            var entity = new Entity(Origin.Center);
            entity.AddComponent(new Explosion());
            Level.Spawn(entity, Entity.Position);

            if (hit.Other.Entity.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.Damage(1);
                if (damageable.Flammable)
                {
                    hit.Other.Entity.AddComponent(new OnFire());
                }
            }

            return hit.Ignore();
        }
    }
}
