﻿using Engine;
using Engine.Collision;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using static Adventure.Constants;

namespace Adventure.Components
{
    public sealed class Fireball : Entity
    {
        private Vector2 _velocity;

        public Fireball(Vector2 velocity)
        {
            _velocity = velocity;
        }

        public static void Preload(ContentManager content) 
        {
            Explosion.Preload(content);
        }

        protected override void OnSpawn()
        {
            Box.Width = 4;
            Box.Height = 4;
            Box.LayerMask = EntityLayers.PlayerProjectile;
            GraphicsComponent = new Particles(this, SharedContent.Graphics.Fire, new Rectangle(8, 8, 8, 8))
            {
                MaxParticles = 100,
                Velocity = new Vector2(25f),
                AngularVelocity = 10f,
                MinColor = Color.White,
                MaxColor = Color.White * 0.1f,
                MaxTime = 0.25f,
                DrawOrder = 5
            };
            SharedContent.Sounds.Shoot.Play();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            MoveAndCollide(ref _velocity, EntityLayers.Enemy | EntityLayers.Solid, HandleCollision);
        }

        private Vector2 HandleCollision(Collision collision)
        {
            Level.Destroy(this);
            Level.Spawn(new Explosion(), Position);

            if (collision.Box.Entity is IDamageable damageable) 
            {
                damageable.Damage(1);

                if (damageable.Flammable) 
                {
                    Level.Spawn(new Fire(collision.Box.Entity));
                }
            }

            return collision.Ignore();
        }
    }
}
