using Reaper.Engine;
using System;
using System.Collections.Generic;

namespace Reaper.Objects
{
    /// <summary>
    /// Contains all of the game's world object definitions.
    /// 
    /// The factory methods and instances are stored seperately so we can free up the lists on every layout change.
    /// </summary>
    public static class Definitions
    {
        private static readonly Dictionary<string, Func<WorldObjectDefinition>> _definitionFactories = new Dictionary<string, Func<WorldObjectDefinition>>();
        private static readonly Dictionary<string, WorldObjectDefinition> _definitions = new Dictionary<string, WorldObjectDefinition>();

        public static WorldObjectDefinition Get(string name)
        {
            if (!_definitions.ContainsKey(name))
            {
                _definitions.Add(name, _definitionFactories[name].Invoke());
            }

            return _definitions[name];
        }

        public static void Register(string name, Func<WorldObjectDefinition> factory)
        {
            _definitionFactories.Add(name, factory);
        }
    }
}
