using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine;
using Engine.Extensions;
using Engine.Graphics;

namespace Adventure.Components
{
    public sealed class OnFire : Component
    {
        private IDamageable _damageable;
        private float _timer;
        private int _hits;
        private Particles _particles;

        public override void OnLoad(ContentManager content)
        {
            Entity.AddComponent(_particles = new Particles(content.Load<Texture2D>("art/player/fire"), new Rectangle(8, 8, 8, 8))
            {
                MaxParticles = 100,
                Velocity = new Vector2(10f, -50f),
                AngularVelocity = 10f,
                MinColor = Color.White,
                MaxColor = Color.White * 0.1f,
                MaxTime = 0.25f
            });

        }

        public override void OnStart()
        {
            _damageable = Entity.GetComponent<IDamageable>();
        }

        public override void OnUpdate(GameTime gameTime)
        {
            _timer -= gameTime.GetDeltaTime();

            if (_timer < 0f)
            {
                _damageable?.Damage(1);

                _hits++;

                if (_hits == 3)
                {
                    Entity.RemoveComponent(_particles);
                    Entity.RemoveComponent(this);
                }
                else
                {
                    _timer = 1f;
                }
            }
        }
    }
}
