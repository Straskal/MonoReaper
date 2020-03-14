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
                ViewportWidth = 320,
                ViewportHeight = 180,
                IsFullscreen = false,
                IsResizable = true,
                IsBordered = true,
                IsVsyncEnabled = true
            };

            using (var game = MainGameFactory.Create(settings))
            {
                game.RunningLayout.Spawn(Player.GetTemplate(), new Vector2(32, 32));

                var other = new WorldObjectDefinition(32, 32);

                other
                    .AddImage("player", new Rectangle(0, 0, 32, 32))
                    .SetOrigin(new Point(16, 32))
                    .AddEffect("Shaders/SolidColor", false)
                    .AddBehavior<EnemyBehavior>()
                    .AddBehavior<PlatformerBehavior>()
                    .AddBehavior<TimerBehavior>()
                    .AddBehavior<DamageableBehavior>()
                    .AddBehavior<AnimationBehavior, AnimationBehavior.Params>(new AnimationBehavior.Params
                    {
                        Animations = new[]
                        {
                            new AnimationBehavior.Animation
                            {
                                Name = "Idle",
                                ImageFilePath = "player",
                                SecPerFrame = 0.1f,
                                Loop = true,
                                Frames = new []
                                {
                                    new AnimationBehavior.Frame { Source = new Rectangle(96, 32, 32, 32) },
                                    new AnimationBehavior.Frame { Source = new Rectangle(0, 64, 32, 32) },
                                    new AnimationBehavior.Frame { Source = new Rectangle(32, 64, 32, 32) },
                                    new AnimationBehavior.Frame { Source = new Rectangle(64, 64, 32, 32) },
                                    new AnimationBehavior.Frame { Source = new Rectangle(96, 64, 32, 32) },
                                    new AnimationBehavior.Frame { Source = new Rectangle(0, 96, 32, 32) }
                                }
                            }
                        }
                    });

                game.RunningLayout.Spawn(other, new Vector2(64, 32));

                var tile = new WorldObjectDefinition(32, 32);
                tile.AddImage("tiles", new Rectangle(0, 0, 32, 32));
                tile.MakeSolid();

                int num = 320 / 32;

                for (int i = 0; i < num; i++) 
                {
                    game.RunningLayout.Spawn(tile, new Vector2(i * 32, 160));
                }

                game.Run();
            }
        }
    }
}
