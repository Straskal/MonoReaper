using Reaper.Engine;
using System;

namespace Reaper
{
    public static class LayoutTransition
    {
        [RequiredByLayoutLoad]
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

                wo.Behaviors.Get<LevelTransitionBehavior>().Level = oe.Values.Level;
                wo.Behaviors.Get<LevelTransitionBehavior>().SpawnPoint = oe.Values.SpawnPoint;
            });
            return def;
        }
    }
}
