using Core;
using Core.Collision;
using Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static Reaper.Constants;

namespace Reaper.Components
{
    public sealed class Door : Component, IDamageable
    {
        private bool isOpen;
        private bool isMoving;

        public bool Flammable => false;

        public void Damage(int amount)
        {
            if (isOpen) 
            {
                return;
            }

            Entity.GetComponent<SpriteSheet>().Play("open");

            isMoving = true;
        }

        public override void OnUpdate(GameTime gameTime)
        {
            if (isMoving && Entity.GetComponent<SpriteSheet>().IsFinished) 
            {
                isMoving = false;
                isOpen = true;

                Entity.RemoveComponent(Entity.GetComponent<Box>());
            }
        }

        public override void OnLoad(ContentManager content)
        {
            var texture = content.Load<Texture2D>("art/common/door_");

            Entity.AddComponent(new Box(0, 0, 16, 16, EntityLayers.Enemy | EntityLayers.Wall));
            Entity.AddComponent(new Sprite(texture, new Rectangle(0, 0, 16, 16)));
            Entity.AddComponent(new SpriteSheet(new SpriteSheet.Animation[] 
            {
                new SpriteSheet.Animation
                {
                    Name = "idle",
                    Loop = false,
                    Frames = new []
                    {
                        new Rectangle(16 * 0, 0, 16, 16)
                    }
                },
                new SpriteSheet.Animation
                {
                    Name = "open",
                    Loop = false,
                    Frames = new []
                    {
                        new Rectangle(16 * 0, 0, 16, 16),
                        new Rectangle(16 * 1, 0, 16, 16),
                        new Rectangle(16 * 2, 0, 16, 16),
                        new Rectangle(16 * 3, 0, 16, 16),
                        new Rectangle(16 * 4, 0, 16, 16),
                    }
                },
                new SpriteSheet.Animation
                {
                    Name = "close",
                    Loop = false,
                    Frames = new []
                    {
                        new Rectangle(16 * 4, 0, 16, 16),
                        new Rectangle(16 * 3, 0, 16, 16),
                        new Rectangle(16 * 2, 0, 16, 16),
                        new Rectangle(16 * 1, 0, 16, 16),
                        new Rectangle(16 * 0, 0, 16, 16),
                    }
                },
            }));
        }
    }
}
