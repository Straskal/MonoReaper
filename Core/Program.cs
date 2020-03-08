using ItsGood;
using ItsGood.Builtins;
using Microsoft.Xna.Framework;
using System;

namespace Core
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var settings = new GameSettings 
            {
                ViewportWidth = 360,
                ViewportHeight = 180,
                IsFullscreen = false,
                IsResizable = true,
                IsBordered = true,
                IsVsyncEnabled = true
            };

            using (var game = MainGameFactory.Create(settings))
            {        
                var player = game.RunningLayout.CreateObject(
                    "player",
                    new Rectangle(0, 0, 32, 32),
                    new Vector2(32, 148));

                player.WithBehavior<PlayerBehavior>();
                player.WithBehavior<PlatformerBehavior>();
                player.WithBehavior<AnimatedBehavior, AnimationParams>(new AnimationParams 
                {
                    Animations = new []
                    {
                        new AnimatedBehavior.AnimationInfo
                        {
                            Name = "Idle",
                            SecPerFrame = 0.1f,
                            Frames = new []
                            {
                                new AnimatedBehavior.AnimationFrameInfo { Source = new Rectangle(96, 32, 32, 32) },

                                new AnimatedBehavior.AnimationFrameInfo { Source = new Rectangle(0, 64, 32, 32) },
                                new AnimatedBehavior.AnimationFrameInfo { Source = new Rectangle(32, 64, 32, 32) },
                                new AnimatedBehavior.AnimationFrameInfo { Source = new Rectangle(64, 64, 32, 32) },
                                new AnimatedBehavior.AnimationFrameInfo { Source = new Rectangle(96, 64, 32, 32) },

                                new AnimatedBehavior.AnimationFrameInfo { Source = new Rectangle(0, 96, 32, 32) }
                            }
                        },
                        new AnimatedBehavior.AnimationInfo
                        {
                            Name = "Run",
                            SecPerFrame = 0.2f,
                            Frames = new []
                            {
                                new AnimatedBehavior.AnimationFrameInfo { Source = new Rectangle(96, 0, 32, 32) },

                                new AnimatedBehavior.AnimationFrameInfo { Source = new Rectangle(0, 32, 32, 32) },
                                new AnimatedBehavior.AnimationFrameInfo { Source = new Rectangle(32, 32, 32, 32) },
                                new AnimatedBehavior.AnimationFrameInfo { Source = new Rectangle(64, 32, 32, 32) },
                            }
                        },
                        new AnimatedBehavior.AnimationInfo
                        {
                            Name = "Jump",
                            SecPerFrame = 0.2f,
                            Frames = new []
                            {
                                new AnimatedBehavior.AnimationFrameInfo { Source = new Rectangle(32, 0, 32, 32) },
                                new AnimatedBehavior.AnimationFrameInfo { Source = new Rectangle(64, 0, 32, 32) },
                            }
                        }
                    }
                });

                var other = game.RunningLayout.CreateObject(
                    "player",
                    new Rectangle(0, 0, 32, 32),
                    new Vector2(96, 148));

                other.MakeSolid();

                game.Run();
            }
        }
    }
}
