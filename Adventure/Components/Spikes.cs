using Engine;
using Engine.Collision;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using static Adventure.Constants;

namespace Adventure.Components
{
    public sealed class Spikes : Component
    {
        private readonly Body _body;

        private AnimatedSprite _spriteSheet;

        private float _animationTimer;

        public override void OnLoad(ContentManager content)
        {
            Entity.AddComponent(_spriteSheet = new AnimatedSprite(content.Load<Texture2D>("art/common/spikes"), SpikeAnimations.Frames));
            Entity.AddComponent(new Box(0, 0, 16, 16, EntityLayers.Enemy | EntityLayers.Solid));
        }

        public override void OnDestroy() => throw new NotImplementedException();

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
    }
}
