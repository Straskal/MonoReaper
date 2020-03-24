using Microsoft.Xna.Framework;
using Reaper.Engine;
using Reaper.Engine.Behaviors;
using Reaper.Objects.Common;

namespace Reaper.Objects.Thug
{
    public static class Thug
    {
        public static WorldObjectDefinition Method()
        {
            var thug = new WorldObjectDefinition();

            thug.SetSize(32, 32);
            thug.SetOrigin(new Point(16, 32));
            thug.AddBehavior(wo => new ThugBehavior(wo));
            thug.AddBehavior(wo => new PlatformerBehavior(wo) { MaxSpeed = 50f });
            thug.AddBehavior(wo => new TimerBehavior(wo));
            thug.AddBehavior(wo => new DamageableBehavior(wo));
            thug.AddBehavior(wo => new LineOfSightBehavior(wo));
            thug.AddBehavior(wo => new SpriteSheetBehavior(wo, new[]
            {
                new SpriteSheetBehavior.Animation
                {
                    Name = "idle",
                    ImageFilePath = "art/enemies/thug",
                    SecPerFrame = 0.1f,
                    Loop = true,
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(64, 32, 32, 32),
                        new SpriteSheetBehavior.Frame(96, 32, 32, 32),
                        new SpriteSheetBehavior.Frame(0, 64, 32, 32),
                        new SpriteSheetBehavior.Frame(32, 64, 32, 32),
                    }
                },
                new SpriteSheetBehavior.Animation
                {
                    Name = "walk",
                    ImageFilePath = "art/enemies/thug",
                    SecPerFrame = 0.2f,
                    Loop = true,
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(64, 0, 32, 32),
                        new SpriteSheetBehavior.Frame(96, 0, 32, 32),
                        new SpriteSheetBehavior.Frame(0, 32, 32, 32),
                        new SpriteSheetBehavior.Frame(32, 32, 32, 32),
                    }
                },
                new SpriteSheetBehavior.Animation
                {
                    Name = "stab",
                    ImageFilePath = "art/enemies/thug_attack",
                    SecPerFrame = 0.15f,
                    Loop = false,
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 64, 32),
                        new SpriteSheetBehavior.Frame(64, 0, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 32, 64, 32),
                        new SpriteSheetBehavior.Frame(64, 32, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 64, 64, 32),
                        new SpriteSheetBehavior.Frame(64, 64, 64, 32)
                    }
                }
            }));

            return thug;
        }
    }
}
