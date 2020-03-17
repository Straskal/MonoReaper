using Reaper.Engine;
using System;
using System.Collections.Generic;
using Reaper.Ogmo;

namespace Reaper.Objects
{
    public static class Definitions
    {
        private static readonly Dictionary<string, Func<WorldObjectDefinition>> _definitionFactories = new Dictionary<string, Func<WorldObjectDefinition>>();
        private static readonly Dictionary<string, WorldObjectDefinition> _definitions = new Dictionary<string, WorldObjectDefinition>();

        public static WorldObjectDefinition Get(OgmoEntity ogmoEntity) 
        {
            if (!_definitions.ContainsKey(ogmoEntity.Name)) 
            {
                _definitions.Add(ogmoEntity.Name, _definitionFactories[ogmoEntity.Name].Invoke());
            }

            return _definitions[ogmoEntity.Name];
        }

        public static WorldObjectDefinition Get(string name)
        {
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
