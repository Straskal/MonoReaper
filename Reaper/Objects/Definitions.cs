using Reaper.Engine;
using System;
using System.Collections.Generic;
using Reaper.Objects.Player;
using Reaper.Objects.Common;
using Reaper.Objects.Thug;

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

        /// <summary>
        /// The main method for adding a new definition to the game.
        /// </summary>
        public static void Register() 
        {
            _definitionFactories.Add("player", PlayerSpawnPoint.Method);
            _definitionFactories.Add("playerInstance", PlayerDefinition.Method);
            _definitionFactories.Add("thug", ThugDefinition.Method);
            _definitionFactories.Add("transition", value: LevelTransitionDefinition.Method);
        }
    }
}
