using Reaper.Engine;
using Reaper.Ogmo.Models;
using System;
using System.Collections.Generic;

namespace Reaper.Ogmo
{
    public static class Loaders
    {
        private static readonly Dictionary<string, Action<WorldObject, OgmoEntity>> _loaders = new Dictionary<string, Action<WorldObject, OgmoEntity>>();

        public static void Register(string name, Action<WorldObject, OgmoEntity> loader)
        {
            _loaders.Add(name, loader);
        }

        public static void Load(string name, WorldObject worldObject, OgmoEntity ogmoEntity) 
        {
            if (_loaders.TryGetValue(name, out var loader)) 
            {
                loader.Invoke(worldObject, ogmoEntity);
            }
        }
    }
}
