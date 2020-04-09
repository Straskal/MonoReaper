using Reaper.Behaviors.Common;
using Reaper.Engine;
using Reaper.Ogmo;
using System;

namespace Reaper.Objects.Common
{
    [Definition]
    public static class LevelTransitionDefinition
    {
        static LevelTransitionDefinition()
        {
            Definitions.Register("transition", Definition);
        }

        public static WorldObjectDefinition Definition()
        {
            var def = new WorldObjectDefinition();
            def.AddBehavior(wo => new LevelTransitionBehavior(wo));
            def.LoadFromOgmo((wo, oe) =>
            {
                if (string.IsNullOrWhiteSpace(oe.Values.Level))
                    throw new ArgumentException("Level transitions must be provided with a layout to load");

                wo.Width = oe.Width;
                wo.Height = oe.Height;

                wo.GetBehavior<LevelTransitionBehavior>().Level = oe.Values.Level;
                wo.GetBehavior<LevelTransitionBehavior>().SpawnPoint = oe.Values.SpawnPoint;
            });
            return def;
        }
    }
}
