using Microsoft.Xna.Framework;
using Engine;
using Engine.Extensions;

namespace Adventure.Entities
{
    public sealed class Fire : Entity
    {
        private readonly Entity target;
        private float timer;
        private int hitCount;

        public Fire(Entity target)
        {
            this.target = target;
        }

        public override void Spawn()
        {
            GraphicsComponent = new Particles(this, Store.Gfx.Fire, new Rectangle(8, 8, 8, 8))
            {
                MaxParticles = 100,
                Velocity = new Vector2(10f, -50f),
                AngularVelocity = 10f,
                MinColor = Color.White,
                MaxColor = Color.White * 0.1f,
                MaxTime = 0.25f
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (target.IsDestroyed)
            {
                World.Destroy(this);
                return;
            }

            if ((timer -= gameTime.GetDeltaTime()) <= 0f)
            {
                if (target is IDamageable damageable)
                {
                    damageable.Damage(1);
                }

                if (++hitCount == 3)
                {
                    World.Destroy(this);
                }
                else
                {
                    timer = 1f;
                }
            }
        }

        public override void PostUpdate(GameTime gameTime)
        {
            Position = target.Position;
            base.PostUpdate(gameTime);
        }
    }
}
