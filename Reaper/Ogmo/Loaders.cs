using Reaper.Engine;
using Reaper.Ogmo.Models;
using System;
using System.Collections.Generic;

namespace Reaper.Ogmo
{
    public static class Loaders
    {
        private static readonly Dictionary<Guid, Action<WorldObject, OgmoEntity>> _loaders = new Dictionary<Guid, Action<WorldObject, OgmoEntity>>();

        public static void Register(Guid guid, Action<WorldObject, OgmoEntity> loader)
        {
            _loaders.Add(guid, loader);
        }

        public static void Load(Guid guid, WorldObject worldObject, OgmoEntity ogmoEntity) 
        {
            if (_loaders.TryGetValue(guid, out var loader)) 
            {
                loader.Invoke(worldObject, ogmoEntity);
            }
        }
    }
}
