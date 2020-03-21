using Reaper.Engine;
using Reaper.Ogmo;
using Reaper.Ogmo.Models;
using System;

namespace Reaper.Objects
{
    public static class DefinitionExtensions
    {
        public static void UseLoader(this WorldObjectDefinition definition, Action<WorldObject, OgmoEntity> loader) 
        {
            Loaders.Register(definition.Guid, loader);
        }
    }
}
