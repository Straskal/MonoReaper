using Engine;
using Engine.Collision;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static Adventure.Constants;

namespace Adventure.Components
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

            Entity.GetComponent<AnimatedSprite>().Play("open");

            isMoving = true;
        }

        public override void OnUpdate(GameTime gameTime)
        {
            if (isMoving && Entity.GetComponent<AnimatedSprite>().IsFinished)
            {
                isMoving = false;
                isOpen = true;

                Entity.RemoveComponent(Entity.GetComponent<Box>());
            }
        }

        public override void OnLoad(ContentManager content)
        {
            Entity.AddComponent(new Box(0, 0, 16, 16, EntityLayers.Enemy | EntityLayers.Solid));
            Entity.AddComponent(new AnimatedSprite(content.Load<Texture2D>("art/common/door_"), new Animation[]
            {
                new Animation("idle")
                {
                    Frames = new []
                    {
                        new Rectangle(16 * 0, 0, 16, 16)
                    }
                },
                new Animation("open")
                {
                    Frames = new []
                    {
                        new Rectangle(16 * 0, 0, 16, 16),
                        new Rectangle(16 * 1, 0, 16, 16),
                        new Rectangle(16 * 2, 0, 16, 16),
                        new Rectangle(16 * 3, 0, 16, 16),
                        new Rectangle(16 * 4, 0, 16, 16),
                    }
                },
                new Animation("close")
                {
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
