using ItsGood;
using ItsGood.Builtins;
using Microsoft.Xna.Framework;

namespace Core.Objects
{
    public static class Player
    {
        public static void CreatePlayer(this Layout layout, float x, float y) 
        {
            var player = layout.CreateObject(
                    new Rectangle(0, 0, 32, 32),
                    new Vector2(x, y));

            player.WithBehavior<PlayerBehavior>();
            player.WithBehavior<PlatformerBehavior>();
            player.WithBehavior<AnimatedSpriteBehavior, AnimatedSpriteBehavior.Params>(new AnimatedSpriteBehavior.Params
            {
                Animations = new[]
                {
                        new AnimatedSpriteBehavior.Animation
                        {
                            Name = "Idle",
                            ImageFilePath = "player",
                            SecPerFrame = 0.1f,
                            Loop = true,
                            Frames = new []
                            {
                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(96, 32, 32, 32) },

                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(0, 64, 32, 32) },
                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(32, 64, 32, 32) },
                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(64, 64, 32, 32) },
                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(96, 64, 32, 32) },

                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(0, 96, 32, 32) }
                            }
                        },
                        new AnimatedSpriteBehavior.Animation
                        {
                            Name = "Run",
                            ImageFilePath = "player",
                            SecPerFrame = 0.2f,
                            Loop = true,
                            Frames = new []
                            {
                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(96, 0, 32, 32) },

                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(0, 32, 32, 32) },
                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(32, 32, 32, 32) },
                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(64, 32, 32, 32) },
                            }
                        },
                        new AnimatedSpriteBehavior.Animation
                        {
                            Name = "Jump",
                            ImageFilePath = "player",
                            SecPerFrame = 0.2f,
                            Frames = new []
                            {
                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(32, 0, 32, 32) },
                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(64, 0, 32, 32) },
                            }
                        },
                        new AnimatedSpriteBehavior.Animation
                        {
                            Name = "Attack",
                            ImageFilePath = "player_attack",
                            SecPerFrame = 0.1f,
                            Frames = new []
                            {
                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(0, 0, 64, 32) },
                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(64, 0, 64, 32) },

                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(0, 32, 64, 32) },
                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(64, 32, 64, 32) },

                                new AnimatedSpriteBehavior.AnimationFrame { Source = new Rectangle(0, 64, 64, 32) },
                            }
                        }
                    }
            });
        }
    }
}
