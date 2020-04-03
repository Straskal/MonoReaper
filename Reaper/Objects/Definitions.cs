using Reaper.Engine;
using System;
using System.Collections.Generic;
using Reaper.Objects.Common;

using ObjectDefinitions = Reaper.Objects.Constants;

namespace Reaper.Objects
{
    /// <summary>
    /// Contains all of the game's world object definitions.
    /// 
    /// The factory methods and instances are stored seperately so we can free up the lists on every layout change.
    /// </summary>
    public static class Definitions
    {
        private static readonly Dictionary<string, Func<WorldObjectType>> _definitionFactories = new Dictionary<string, Func<WorldObjectType>>();
        private static readonly Dictionary<string, WorldObjectType> _definitions = new Dictionary<string, WorldObjectType>();

        public static WorldObjectType Get(string name)
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
            _definitionFactories.Add(ObjectDefinitions.Player, PlayerSpawnPoint.Method);
            _definitionFactories.Add(ObjectDefinitions.PlayerInstance, Player.Player.Method);
            _definitionFactories.Add(ObjectDefinitions.LevelTransition, LevelTransitionDefinition.Method);
        }
    }
}
