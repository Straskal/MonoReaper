using Engine;
using Microsoft.Xna.Framework;
using static Adventure.Constants;

namespace Adventure.Entities
{
    public sealed class EnemyFireball : KinematicEntity
    {
        private Vector2 velocity;

        public EnemyFireball(Vector2 velocity)
        {
            this.velocity = velocity;
        }

        public override void Spawn()
        {
            Collider = new CircleCollider(this, Vector2.Zero, 4);
            Collider.Layer = EntityLayers.Projectile;
            Collider.Enable();
            GraphicsComponent = new Particles(this, Store.Gfx.Fire, new Rectangle(8, 8, 8, 8))
            {
                MaxParticles = 100,
                Velocity = new Vector2(25f),
                AngularVelocity = 10f,
                MinColor = Color.White,
                MaxColor = Color.White * 0.1f,
                MaxTime = 0.25f,
                DrawOrder = 5
            };
            Store.Sfx.Shoot.Play();
        }

        public override void Update(GameTime gameTime)
        {
            Collide(velocity);
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
