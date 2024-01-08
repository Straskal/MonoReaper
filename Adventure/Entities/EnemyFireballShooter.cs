using Adventure.Components;
using Engine;
using Engine.Extensions;
using Microsoft.Xna.Framework;

namespace Adventure.Entities
{
    public class EnemyFireballShooter : Entity
    {
        private float timer = 0f;

        protected override void OnUpdate(GameTime gameTime)
        {
            timer -= gameTime.GetDeltaTime();

            if (timer <= 0f) 
            {
                Level.Spawn(new EnemyFireball(Vector2.UnitY), Position);
                timer = 0.5f;
            }

            base.OnUpdate(gameTime);
        }
    }
}
