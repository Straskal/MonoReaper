using Reaper.Engine;
using Reaper.Engine.Behaviors;
using Reaper.Ogmo;

namespace Reaper.Objects
{
    public static class Player
    {
        public static WorldObjectDefinition Definition(OgmoEntity entity)
        {
            var playerDefinition = new WorldObjectDefinition(32, 32);

            playerDefinition.SetOgmoDefaults(entity);
            playerDefinition.AddBehavior(wo =>
            {
                return new PlayerBehavior(wo)
                {
                    Health = entity.Values.Health
                };
            });

            playerDefinition.AddBehavior(wo => new PlatformerBehavior(wo));
            playerDefinition.AddBehavior(wo => new TimerBehavior(wo));
            playerDefinition.AddBehavior(wo => new DamageableBehavior(wo));

            playerDefinition.AddBehavior(wo => new SpriteSheetBehavior(wo, new[]
            {
                new SpriteSheetBehavior.Animation
                {
                    Name = "Idle",
                    ImageFilePath = "player",
                    SecPerFrame = 0.1f,
                    Loop = true,
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(96, 32, 32, 32),
                        new SpriteSheetBehavior.Frame(0, 64, 32, 32),
                        new SpriteSheetBehavior.Frame(32, 64, 32, 32),
                        new SpriteSheetBehavior.Frame(64, 64, 32, 32),
                        new SpriteSheetBehavior.Frame(96, 64, 32, 32),
                        new SpriteSheetBehavior.Frame(0, 96, 32, 32)
                    }
                },
                new SpriteSheetBehavior.Animation
                {
                    Name = "Run",
                    ImageFilePath = "player",
                    SecPerFrame = 0.15f,
                    Loop = true,
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(96, 0, 32, 32),
                        new SpriteSheetBehavior.Frame(0, 32, 32, 32),
                        new SpriteSheetBehavior.Frame(32, 32, 32, 32),
                        new SpriteSheetBehavior.Frame(64, 32, 32, 32) ,
                    }
                },
                new SpriteSheetBehavior.Animation
                {
                    Name = "Jump",
                    ImageFilePath = "player",
                    SecPerFrame = 0.2f,
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(32, 0, 32, 32),
                    }
                },
                 new SpriteSheetBehavior.Animation
                {
                    Name = "Fall",
                    ImageFilePath = "player",
                    SecPerFrame = 0.2f,
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(64, 0, 32, 32),
                    }
                },
                new SpriteSheetBehavior.Animation
                {
                    Name = "Attack",
                    ImageFilePath = "player_attack",
                    SecPerFrame = 0.05f,
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 64, 32),
                        new SpriteSheetBehavior.Frame(64, 0, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 32, 64, 32),
                        new SpriteSheetBehavior.Frame(64, 32, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 64, 64, 32),
                    }
                }
            }));

            return playerDefinition;
        }
    }
}
