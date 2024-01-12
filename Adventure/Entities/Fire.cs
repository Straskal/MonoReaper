using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Engine;
using Engine.Extensions;

namespace Adventure.Components
{
    public sealed class Fire : Entity
    {
        public Fire(Entity target)
        {
            Target = target;
        }

        public Entity Target 
        {
            get;
        }

        public float Timer 
        {
            get;
            private set;
        }

        public int HitCount 
        {
            get;
            private set;
        }

        public override void Spawn()
        {
            GraphicsComponent = new Particles(this, SharedContent.Graphics.Fire, new Rectangle(8, 8, 8, 8))
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
            Timer -= gameTime.GetDeltaTime();

            if (Timer < 0f)
            {
                if (Target is IDamageable damageable) 
                {
                    damageable.Damage(1);
                }

                HitCount++;

                if (HitCount == 3)
                {
                    Others.Destroy(this);
                }
                else
                {
                    Timer = 1f;
                }
            }

            if (Target.IsDestroyed)
            {
                Others.Destroy(this);
            }
        }

        public override void PostUpdate(GameTime gameTime)
        {
            Position = Target.Position;
            base.PostUpdate(gameTime);
        }
    }
}
