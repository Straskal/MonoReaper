using Microsoft.Xna.Framework;
using Engine;
using Engine.Collision;
using Engine.Graphics;

using static Adventure.Constants;
using Microsoft.Xna.Framework.Content;

namespace Adventure.Components
{
    public sealed class Fireball : Entity
    {
        private Body _body;
        private Vector2 _velocity;

        public Fireball(Vector2 velocity)
        {
            _velocity = velocity;
        }

        public static void Preload(ContentManager content) 
        {
            Explosion.Preload(content);
        }

        public override void OnSpawn()
        {
            AddComponent(_body = new Body(4, 4, EntityLayers.PlayerProjectile));
            AddComponent(new Particles(SharedContent.Graphics.Fire, new Rectangle(8, 8, 8, 8))
            {
                MaxParticles = 100,
                Velocity = new Vector2(25f),
                AngularVelocity = 10f,
                MinColor = Color.White,
                MaxColor = Color.White * 0.1f,
                MaxTime = 0.25f,
                ZOrder = 5
            });

            SharedContent.Sounds.Shoot.Play();
        }

        public override void OnUpdate(GameTime gameTime)
        {
            _body.MoveAndCollide(ref _velocity, EntityLayers.Enemy | EntityLayers.Solid, HandleCollision);
        }

        private Vector2 HandleCollision(Collision collision)
        {
            DestroySelf();
            Spawn(new Explosion(), Position);

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
