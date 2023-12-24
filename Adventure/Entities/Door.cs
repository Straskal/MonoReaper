//using Engine;
//using Engine.Collision;
//using Engine.Graphics;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using static Adventure.Constants;

//namespace Adventure.Components
//{
//    public sealed class Door : Entity, IDamageable
//    {
//        private bool isOpen;
//        private bool isMoving;

//        public bool Flammable => false;

//        public void Damage(int amount)
//        {
//            if (isOpen)
//            {
//                return;
//            }

//            GetComponent<AnimatedSprite>().Play("open");

//            isMoving = true;
//        }

//        public override void OnUpdate(GameTime gameTime)
//        {
//            if (isMoving && GetComponent<AnimatedSprite>().CurrentAnimationFinished)
//            {
//                isMoving = false;
//                isOpen = true;

//                RemoveComponent(GetComponent<Box>());
//            }
//        }

//        public override void OnLoad(ContentManager content)
//        {
//            AddComponent(new Box(0, 0, 16, 16, EntityLayers.Enemy | EntityLayers.Solid));
//            AddComponent(new AnimatedSprite(content.Load<Texture2D>("art/common/door_"), new Animation[]
//            {
//                new Animation("idle")
//                {
//                    Frames = new []
//                    {
//                        new Rectangle(16 * 0, 0, 16, 16)
//                    }
//                },
//                new Animation("open")
//                {
//                    Frames = new []
//                    {
//                        new Rectangle(16 * 0, 0, 16, 16),
//                        new Rectangle(16 * 1, 0, 16, 16),
//                        new Rectangle(16 * 2, 0, 16, 16),
//                        new Rectangle(16 * 3, 0, 16, 16),
//                        new Rectangle(16 * 4, 0, 16, 16),
//                    }
//                },
//                new Animation("close")
//                {
//                    Frames = new []
//                    {
//                        new Rectangle(16 * 4, 0, 16, 16),
//                        new Rectangle(16 * 3, 0, 16, 16),
//                        new Rectangle(16 * 2, 0, 16, 16),
//                        new Rectangle(16 * 1, 0, 16, 16),
//                        new Rectangle(16 * 0, 0, 16, 16),
//                    }
//                },
//            }));
//        }
//    }
//}
