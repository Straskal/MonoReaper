using Microsoft.Xna.Framework;
using Core;
using Core.Collision;

namespace Reaper.Components
{
    public sealed class Projectile : Component
    {
        private Body _body;

        public Projectile(Vector2 velocity) 
        {
            _velocity = velocity;
        }

        private Vector2 _velocity;
        public Vector2 Velocity 
        {
            get => _velocity;
            set => _velocity = value;
        }

        public override void OnSpawn()
        {
            Entity.AddComponent(_body = new Body(16, 16));
        }

        public override void OnTick(GameTime gameTime)
        {
            _body.Move(ref _velocity, hit =>
            {
                if (hit.Other.IsSolid) 
                {
                    Level.Destroy(Entity);
                }

                return hit.Ignore();
            });

            Entity.Position += Velocity * gameTime.GetDeltaTime();
        }
    }
}
