using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Reaper.Engine
{
    public class WorldObjectDefinition
    {
        private readonly List<Action<WorldObject>> _buildSteps = new List<Action<WorldObject>>();

        public WorldObjectDefinition(int width, int height) 
        {
            _buildSteps.Add(worldObject => 
            {
                worldObject.Bounds = new Rectangle(0, 0, width, height);
            });
        }

        public WorldObjectDefinition SetOrigin(Point origin)
        {
            _buildSteps.Add(worldObject => worldObject.Origin = origin);
            return this;
        }

        public WorldObjectDefinition MakeSolid() 
        {
            _buildSteps.Add(worldObject => worldObject.IsSolid = true);
            return this;
        }

        public WorldObjectDefinition AddBehavior(Func<WorldObject, Behavior> createFunc)
        {
            _buildSteps.Add(worldObject => worldObject.AddBehavior(createFunc));
            return this;
        }

        public WorldObjectDefinition AddEffect(string name, bool isEnabled) 
        {
            return this;
        }

        internal void Build(WorldObject worldObject) 
        {
            foreach (var step in _buildSteps)
            {
                step.Invoke(worldObject);
            }
        }
    }
}
