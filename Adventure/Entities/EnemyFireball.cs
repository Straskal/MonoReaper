﻿using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using static Adventure.Constants;

namespace Adventure.Components
{
    public sealed class EnemyFireball : Entity
    {
        private Vector2 velocity;

        public EnemyFireball(Vector2 velocity)
        {
            this.velocity = velocity;
        }

        public static void Preload(ContentManager content) 
        {
            Explosion.Preload(content);
        }

        public override void Spawn()
        {
            Collider = new CircleCollider(this, Vector2.Zero, 4);
            Collider.Layer = EntityLayers.Projectile;
            Collider.Enable();
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
            var i = SharedContent.Sounds.Shoot.CreateInstance();
            i.Pitch = Math.Clamp(App.Random.NextSingle(), 0.6f, 1f);
            i.Play();
        }

        public override void Update(GameTime gameTime)
        {
            Collide(ref velocity, EntityLayers.Player | EntityLayers.Solid);
        }

        public override void OnCollision(Entity other, Collision collision)
        {
            World.Destroy(this);
            World.Spawn(new Explosion(), Position);

            if (other is IDamageable damageable)
            {
                damageable.Damage(1);

                if (damageable.Flammable)
                {
                    World.Spawn(new Fire(other));
                }
            }
        }
    }
}