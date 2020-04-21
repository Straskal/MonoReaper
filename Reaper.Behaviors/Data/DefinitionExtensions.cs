using Reaper.Engine;
using Reaper;
using System;

namespace Reaper
{
    public static class DefinitionExtensions
    {
        public static void LoadFromOgmo(this WorldObjectDefinition definition, Action<WorldObject, OgmoEntity> loader) 
        {
            Loaders.Register(definition.Guid, loader);
        }
    }
}
