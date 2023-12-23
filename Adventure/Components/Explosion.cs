using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine;
using Engine.Graphics;

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

        public override void OnLoad(ContentManager content)
        {
            _sound = content.Load<SoundEffect>("audio/explosion4");
            _texture = content.Load<Texture2D>("art/common/explosion-1");
        }

        public override void OnSpawn()
        {
            AddComponent(_animatedSprite = new AnimatedSprite(_texture, ExplosionAnimations.Frames)
            {
                Speed = 0.1f,
                ZOrder = 10
            });

            _sound.Play();
        }

        public override void OnUpdate(GameTime gameTime)
        {
            if (_animatedSprite.CurrentAnimationFinished)
            {
                DestroySelf();
            }
        }
    }
}
