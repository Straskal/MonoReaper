using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine;
using static Adventure.Constants;

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

        public override void Spawn()
        {
            _sound = Adventure.Instance.Content.Load<SoundEffect>("audio/explosion4");
            _texture = Adventure.Instance.Content.Load<Texture2D>("art/common/explosion-1");

            GraphicsComponent = _animatedSprite = new AnimatedSprite(this, _texture, ExplosionAnimations.Frames)
            {
                Speed = 0.1f,
                DrawOrder = 10
            };

            _sound.Play();
        }

        public override void Start()
        {
            foreach (var entity in World.GetOverlappingEntities(new CircleF(Position, 20), EntityLayers.Enemy))
            {
                if (entity is IDamageable damageable)
                {
                    damageable.Damage(1);

                    if (damageable.Flammable)
                    {
                        World.Spawn(new Fire(entity));
                    }
                }
            }
            base.Start();
        }

        public override void Update(GameTime gameTime)
        {
            if (_animatedSprite.CurrentAnimationFinished)
            {
                World.Destroy(this);
            }
        }
    }
}
