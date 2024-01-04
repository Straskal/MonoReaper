﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine;

namespace Adventure.Components
{
    public sealed class Explosion : Entity
    {
        private Texture2D _texture;
        private AnimatedSprite _animatedSprite;
        private SoundEffect _sound;

        public static void Preload(ContentManager content) 
        {
            content.Load<SoundEffect>("audio/explosion4");
            content.Load<Texture2D>("art/common/explosion-1");
        }

        protected override void OnLoad(ContentManager content)
        {
            _sound = content.Load<SoundEffect>("audio/explosion4");
            _texture = content.Load<Texture2D>("art/common/explosion-1");
        }

        protected override void OnSpawn()
        {
            GraphicsComponent = _animatedSprite = new AnimatedSprite(this, _texture, ExplosionAnimations.Frames)
            {
                Speed = 0.1f,
                DrawOrder = 10
            };

            _sound.Play();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (_animatedSprite.CurrentAnimationFinished)
            {
                Level.Destroy(this);
            }
        }
    }
}
