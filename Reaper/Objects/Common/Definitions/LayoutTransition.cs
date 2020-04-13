using Reaper;
using Reaper.Engine;
using Reaper;
using System;

namespace Reaper
{
    [Definition]
    public static class LayoutTransition
    {
        static LayoutTransition()
        {
            DefinitionList.Register(typeof(LayoutTransition), Definition);
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
