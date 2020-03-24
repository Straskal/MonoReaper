using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Reaper.Engine
{
    /// <summary>
    /// Definitions track the steps necessary to build a certain type of world object.
    /// </summary>
    public sealed class WorldObjectDefinition
    {
        private readonly List<Action<WorldObject>> _buildSteps = new List<Action<WorldObject>>();

        public Guid Guid { get; } = Guid.NewGuid();

        public void SetSize(int width, int height) 
        {
            _buildSteps.Add(worldObject =>
            {
                worldObject.Bounds = new Rectangle(0, 0, width, height);
            });
        }

        public void SetOrigin(Point origin)
        {
            _buildSteps.Add(worldObject => worldObject.Origin = origin);
        }

        public void MakeSolid() 
        {
            _buildSteps.Add(worldObject => worldObject.Type = SpatialType.Solid);
        }

        public void MakeDecal()
        {
            _buildSteps.Add(worldObject => worldObject.Type = SpatialType.Pass);
        }

        public void AddBehavior(Func<WorldObject, Behavior> createFunc)
        {
            _buildSteps.Add(worldObject => worldObject.AddBehavior(createFunc));
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
