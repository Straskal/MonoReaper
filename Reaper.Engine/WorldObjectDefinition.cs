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

        public void WithTags(params string[] tags) 
        {
            _buildSteps.Add(worldObject => worldObject.Tags = tags);
        }

        public void SetSize(int width, int height) 
        {
            _buildSteps.Add(worldObject => worldObject.Bounds = new Rectangle(0, 0, width, height));
        }

        public void SetOrigin(int x, int y)
        {
            _buildSteps.Add(worldObject => worldObject.Origin = new Point(x, y));
        }

        public void MakeSolid() 
        {
            _buildSteps.Add(worldObject => worldObject.SpatialType = SpatialType.Solid);
        }

        public void MakeDecal()
        {
            _buildSteps.Add(worldObject => worldObject.SpatialType = SpatialType.Pass);
        }

        public void AddBehavior(Func<WorldObject, Behavior> createFunc)
        {
            _buildSteps.Add(worldObject => worldObject.AddBehavior(createFunc));
        }

        internal void Apply(WorldObject worldObject) 
        {
            foreach (var step in _buildSteps)
                step.Invoke(worldObject);

            worldObject.UpdateBBox();
        }
    }
}
