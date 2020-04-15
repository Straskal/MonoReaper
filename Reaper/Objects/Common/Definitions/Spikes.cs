using Microsoft.Xna.Framework;
using Reaper.Engine;

namespace Reaper
{
    public static class Spikes
    {
        [Definition]
        public static WorldObjectDefinition Definition() 
        {
            var def = new WorldObjectDefinition();
            def.SetSize(32, 32);
            def.SetOrigin(16, 16);
            def.AddBehavior(wo => new SpriteSheetBehavior(wo, GetAnimations()));
            def.AddBehavior(wo => new SpikeBehavior(wo));
            return def;
        }

        private static SpriteSheetBehavior.Animation[] GetAnimations()
        {
            return new[]
            {
                new SpriteSheetBehavior.Animation
                {
                    Name = "go_up",
                    ImageFilePath = "art/common/spikes",
                    Origin = new Point(16, 16),
                    SecPerFrame = 0.2f,
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 32, 32),
                        new SpriteSheetBehavior.Frame(32, 0, 32, 32),
                        new SpriteSheetBehavior.Frame(64, 0, 32, 32),
                    }
                },

                 new SpriteSheetBehavior.Animation
                {
                    Name = "go_down",
                    ImageFilePath = "art/common/spikes",
                    SecPerFrame = 0.5f,
                    Origin = new Point(16, 16),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(32, 0, 32, 32),
                        new SpriteSheetBehavior.Frame(0, 0, 32, 32),
                    }
                },
            };
        }
    }
}
