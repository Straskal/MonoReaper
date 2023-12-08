using Engine;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure.Components
{
    public sealed class Explosion : Component
    {
        private AnimatedSprite _spriteSheet;
        private SoundEffect _sound;

        public Explosion() 
        {
            IsUpdateEnabled = true;
        }

        public override void OnLoad(ContentManager content)
        {
            _sound = content.Load<SoundEffect>("audio/explosion4");

            Entity.AddComponent(_spriteSheet = new AnimatedSprite(content.Load<Texture2D>("art/common/explosion-1"), new[]
            {
                new Animation("idle")
                {
                    Frames = new []
                    {
                        new Rectangle(32 * 0, 0, 32, 32),
                        new Rectangle(32 * 1, 0, 32, 32),
                        new Rectangle(32 * 2, 0, 32, 32),
                        new Rectangle(32 * 3, 0, 32, 32),
                        new Rectangle(32 * 4, 0, 32, 32),
                        new Rectangle(32 * 5, 0, 32, 32),
                        new Rectangle(32 * 6, 0, 32, 32),
                    }
                }
            })
            { 
                Speed = 0.1f, 
                ZOrder = 10 
            });
        }

        public override void OnSpawn()
        {
            _sound.Play();
        }

        public override void OnUpdate(GameTime gameTime)
        {
            if (_spriteSheet.CurrentFrame == 1)
            {
                DistortionPostProcessingEffect.Explosion = Entity.Position;
            }

            if (_spriteSheet.IsFinished)
            {
                Level.Destroy(Entity);
            }
        }
    }
}
