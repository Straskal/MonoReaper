using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ItsGood
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

        public WorldObjectDefinition AddImage(string path, Rectangle source) 
        {
            _buildSteps.Add(worldObject =>
            {
                worldObject.ImageFilePath = path;
                worldObject.Source = source;
                worldObject.Color = Color.White;
            });
            return this;
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

        public WorldObjectDefinition AddBehavior<T>() where T : Behavior, new()
        {
            _buildSteps.Add(worldObject => worldObject.AddBehavior<T>());
            return this;
        }

        public WorldObjectDefinition AddBehavior<T, U>(U state) where T : Behavior<U>, new()
        {
            _buildSteps.Add(worldObject => worldObject.AddBehavior<T, U>(state));
            return this;
        }

        public WorldObjectDefinition AddEffect(string name, bool isEnabled) 
        {
            _buildSteps.Add(worldObject => 
            {
                worldObject.EffectFilePath = name;
                worldObject.IsEffectEnabled = isEnabled;
            });
            
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
