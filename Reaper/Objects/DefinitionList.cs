using Reaper.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Reaper
{
    /// <summary>
    /// Contains all of the game's world object definitions.
    /// 
    /// The factory methods and instances are stored seperately so we can free up the lists on every layout change.
    /// </summary>
    public class DefinitionList
    {
        private readonly Dictionary<string, WorldObjectDefinition> _definitions = new Dictionary<string, WorldObjectDefinition>();

        public static DefinitionList GetDefinitions() 
        {
            var definitionList = new DefinitionList();

            var definitionMethods = typeof(DefinitionList).Assembly.GetTypes()
                .SelectMany(type => type.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(RequiredByLayoutLoadAttribute), false).Length > 0);

            foreach (var method in definitionMethods) 
            {
                definitionList._definitions.Add(method.DeclaringType.Name, (WorldObjectDefinition)method.Invoke(null, null));
            }

            return definitionList;
        }

        public WorldObjectDefinition Get(string name) 
        {
            return _definitions[name];
        }
    }
}
