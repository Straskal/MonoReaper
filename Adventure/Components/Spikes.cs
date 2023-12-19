using Engine;
using Engine.Collision;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static Adventure.Constants;

namespace Adventure.Components
{
    public sealed class Spikes : Component
    {
        private Box _box;

        private AnimatedSprite _spriteSheet;

        private float _animationTimer;

        public override void OnLoad(ContentManager content)
        {
            Entity.AddComponent(_spriteSheet = new AnimatedSprite(content.Load<Texture2D>("art/common/spikes"), SpikeAnimations.Frames));
            Entity.AddComponent(_box = new Box(0, 0, 16, 16, EntityLayers.Enemy | EntityLayers.Enemy));
        }

        public override void OnStart()
        {
            _box.CollidedWith += OnCollidedWith;
        }

        public override void OnEnd()
        {
            _box.CollidedWith -= OnCollidedWith;
        }

        public override void OnUpdate(GameTime gameTime)
        {
            Animate(gameTime);
        }

        private void Animate(GameTime gameTime)
        {
            _animationTimer += gameTime.GetDeltaTime();
            var animationSpeed = 0.35f;

            if (_animationTimer < 1f * animationSpeed)
            {
                _spriteSheet.Play("retracted");
            }
            else if (_animationTimer >= 1f * animationSpeed && _animationTimer < 2f * animationSpeed)
            {
                _spriteSheet.Play("moving");
            }
            else if (_animationTimer >= 2f * animationSpeed && _animationTimer < 3f * animationSpeed)
            {
                _spriteSheet.Play("extended");
            }
            else if (_animationTimer >= 3f * animationSpeed)
            {
                _animationTimer = 0f;
            }
        }

        private void OnCollidedWith(Body body, Collision collision)
        {
            if (body.LayerMask == EntityLayers.Player)
            {

            }
            if (body.Entity.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.Damage(1);
            }
        }
    }
}
