using Adventure.Components;
using Engine;
using Engine.Extensions;
using Microsoft.Xna.Framework;

namespace Adventure.Entities
{
    public class EnemyFireballShooter : Entity
    {
        private float timer = 0f;

        public override void Update(GameTime gameTime)
        {
            timer -= gameTime.GetDeltaTime();

            if (timer <= 0f) 
            {
                World.Spawn(new EnemyFireball(Vector2.UnitY), Position);
                timer = 0.5f;
            }

            base.Update(gameTime);
        }
    }
}
