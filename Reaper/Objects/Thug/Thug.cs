using Microsoft.Xna.Framework;
using Reaper.Engine;
using Reaper.Engine.Behaviors;

namespace Reaper.Objects
{
    public static class Thug
    {
        public static WorldObjectDefinition Definition(OgmoEntity entity)
        {
            var thugDef = new WorldObjectDefinition(32, 32);

            thugDef.SetOrigin(new Point(entity.OriginX, entity.OriginY));
            thugDef.AddBehavior(wo => new EnemyBehavior(wo));
            thugDef.AddBehavior(wo => new PlatformerBehavior(wo));
            thugDef.AddBehavior(wo => new TimerBehavior(wo));
            thugDef.AddBehavior(wo => new DamageableBehavior(wo));
            thugDef.AddBehavior(wo => new SpriteSheetBehavior(wo, new[]
            {
                new SpriteSheetBehavior.Animation
                {
                    Name = "Idle",
                    ImageFilePath = "thug",
                    SecPerFrame = 0.1f,
                    Loop = true,
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(64, 32, 32, 32),
                        new SpriteSheetBehavior.Frame(96, 32, 32, 32),
                        new SpriteSheetBehavior.Frame(0, 64, 32, 32),
                        new SpriteSheetBehavior.Frame(32, 64, 32, 32),
                    }
                }
            }));

            return thugDef;
        }
    }
}
