using Microsoft.Xna.Framework;
using Reaper;
using Reaper.Engine;

namespace Reaper
{
    [Definition]
    public static class Key
    {
        static Key()
        {
            DefinitionList.Register(typeof(Key), Definition);
        }

        public static WorldObjectDefinition Definition() 
        {
            var def = new WorldObjectDefinition();
            def.SetSize(32, 32);
            def.SetOrigin(16, 16);
            def.SetZOrder(5);
            def.MakeDecal();
            def.AddBehavior(wo => new KeyBehavior(wo));
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
                    ImageFilePath = "art/common/key",
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
