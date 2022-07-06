using Microsoft.Xna.Framework;
using Core;
using Core.Collision;
using Core.Graphics;

using static Reaper.Constants;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Reaper.Components
{
    public sealed class Fireball : Component
    {
        private Body _body;

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
        }

        public override void OnLoad(ContentManager content)
        {
            var texture = content.Load<Texture2D>("art/player/fire");

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

        public override void OnTick(GameTime gameTime)
        {
            _body.Move(_velocity, EntityLayers.Enemy | EntityLayers.Wall, hit =>
            {
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
