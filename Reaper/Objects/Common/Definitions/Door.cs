using Microsoft.Xna.Framework;
using Reaper.Engine;
namespace Reaper.Objects.Common.Definitions
{
    [Definition]
    public static class Door
    {
        static Door()
        {
            DefinitionList.Register(typeof(Door), Definition);
        }

        public static WorldObjectDefinition Definition()
        {
            var def = new WorldObjectDefinition();
            def.SetTags("door");
            def.SetSize(32, 32);
            def.SetOrigin(16, 16);
            def.SetZOrder(5);
            def.MakeSolid();
            def.AddBehavior(wo => new SpriteSheetBehavior(wo, GetAnimations()));
            return def;
        }

        private static SpriteSheetBehavior.Animation[] GetAnimations()
        {
            return new[]
            {
                new SpriteSheetBehavior.Animation
                {
                    Name = "idle",
                    ImageFilePath = "art/common/door",
                    Origin = new Point(16, 16),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 32, 32),
                    }
                },
            };
        }
    }
}
