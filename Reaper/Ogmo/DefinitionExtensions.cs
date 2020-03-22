using Reaper.Engine;
using Reaper.Ogmo;
using Reaper.Ogmo.Models;
using System;

namespace Reaper.Ogmo
{
    public static class DefinitionExtensions
    {
        public static void UseOgmoLoader(this WorldObjectDefinition definition, Action<WorldObject, OgmoEntity> loader) 
        {
            Loaders.Register(definition.Guid, loader);
        }
    }
}
