using Core.Objects;
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
                game.RunningLayout.CreatePlayer(32, 64);

                var other = game.RunningLayout.CreateObject(
                    new Rectangle(0, 0, 32, 32),
                    new Vector2(96, 128));

                other.WithBehavior<AnimatedSpriteBehavior, AnimatedSpriteBehavior.Params>(new AnimatedSpriteBehavior.Params
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
                        }
                    }
                });

                int num = 320 / 32;

                for (int i = 0; i < num; i++) 
                {
                    game.RunningLayout.CreateObject(
                        "tiles",
                        new Rectangle(0, 0, 32, 32),
                        new Vector2(i * 32, 160))
                        .MakeSolid();
                }

                game.Run();
            }
        }
    }
}
