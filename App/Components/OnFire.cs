using Core;
using Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Reaper.Components
{
    public sealed class OnFire : Component
    {
        private IDamageable _damageable;
        private float _timer;
        private int _hits;

        public OnFire() 
        {
            IsUpdateEnabled = true;
        }

        public override void OnLoad(ContentManager content)
        {
            var texture = content.Load<Texture2D>("art/player/fire");

            Entity.AddComponent(new Particles(texture, new Rectangle(8, 8, 8, 8))
            {
                MaxParticles = 100,
                MaxVelocity = new Vector2(10f, -50f),
                MaxAngularVelocity = 10f,
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
