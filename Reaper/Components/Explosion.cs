using Core;
using Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Reaper.Components
{
    public sealed class Explosion : Component
    {
        private SpriteSheet _spriteSheet;
        private SoundEffect _sound;

        public override void OnLoad(ContentManager content)
        {
            var playerTexture = content.Load<Texture2D>("art/common/explosion-1");

            _sound = content.Load<SoundEffect>("audio/explosion4");

            Entity.AddComponent(new Sprite(playerTexture) { ZOrder = 10 });
            Entity.AddComponent(_spriteSheet = new SpriteSheet(new[]
            {
                new SpriteSheet.Animation
                {
                    Name = "idle",
                    Loop = false,
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
            { Speed = 0.1f });
        }

        public override void OnSpawn()
        {
            _sound.Play();
        }

        public override void OnTick(GameTime gameTime)
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
