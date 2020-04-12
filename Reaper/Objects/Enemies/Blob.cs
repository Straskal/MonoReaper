using Reaper.Behaviors.Common;
using Reaper.Engine;

namespace Reaper.Objects.Enemies
{
    [Definition]
    public static class Blob
    {
        static Blob()
        {
            Definitions.Register(typeof(Blob), Definition);
        }

        public static WorldObjectDefinition Definition()
        {
            var def = new WorldObjectDefinition();
            def.SetTags("enemy");
            def.SetSize(16, 16);
            def.SetOrigin(8, 8);
            def.AddBehavior(wo => new SpriteSheetBehavior(wo, GetBlobAnimations()));
            return def;
        }

        private static SpriteSheetBehavior.Animation[] GetBlobAnimations()
        {
            return new[]
            {
                new SpriteSheetBehavior.Animation
                {
                    Name = "blob",
                    ImageFilePath = "art/enemies/blob",
                    SecPerFrame = 0.5f,
                    Loop = true,
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 16, 16),
                        new SpriteSheetBehavior.Frame(16, 0, 16, 16),
                        new SpriteSheetBehavior.Frame(32, 0, 16, 16),
                    }
                },
            };
        }
    }
}
