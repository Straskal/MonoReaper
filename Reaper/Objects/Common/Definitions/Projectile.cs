using Microsoft.Xna.Framework;
using Reaper.Engine;

namespace Reaper
{
    public static class Projectile
    {
        public static WorldObjectDefinition Definition()
        {
            var def = new WorldObjectDefinition();
            def.SetSize(8, 8);
            def.SetOrigin(4, 4);
            def.AddBehavior(wo => new DestroyOutsideLayoutBehavior(wo));
            def.AddBehavior(wo => new ProjectileBehavior(wo));
            def.AddBehavior(wo => new SpriteSheetBehavior(wo, new[]
            {
                new SpriteSheetBehavior.Animation
                {
                    Name = "idle",
                    ImageFilePath = "art/player/fireball",
                    Origin = new Point(4, 4),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 8, 8),
                    }
                }
            }));
            return def;
        }

        public static WorldObject CreateProjectile(this WorldObjectList worldObjectList, Vector2 position, Vector2 direction, float speed, params string[] ignoreTags) 
        {
            var proj = worldObjectList.Create(Definition(), position);
            var projBehavior = proj.Behaviors.Get<ProjectileBehavior>();
            projBehavior.Direction = direction;
            projBehavior.Speed = speed;
            projBehavior.IgnoreTags = ignoreTags;
            return proj;
        }
    }
}
