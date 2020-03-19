using Reaper.Engine;
using System;
using System.Collections.Generic;

namespace Reaper.Objects
{
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

        public static void Register() 
        {
            _definitionFactories.Add("player", Player.Definition);
            _definitionFactories.Add("thug", Thug.Definition);
            _definitionFactories.Add("transition", LevelTransitionDefinition.Definition);
        }
    }
}
