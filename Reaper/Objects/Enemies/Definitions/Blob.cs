using Microsoft.Xna.Framework;
using Reaper.Engine;

namespace Reaper
{
    public static class Blob
    {
        [Definition]
        public static WorldObjectDefinition Definition()
        {
            var def = new WorldObjectDefinition();
            def.SetTags("enemy");
            def.SetSize(32, 32);
            def.SetOrigin(16, 16);
            def.AddBehavior(wo => new BlobBehavior(wo));
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
                    Origin = new Point(16, 16),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 32, 32),
                        new SpriteSheetBehavior.Frame(32, 0, 32, 32),
                        new SpriteSheetBehavior.Frame(64, 0, 32, 32),
                    }
                },
            };
        }
    }
}
