using System;
using System.Collections.Generic;

namespace Reaper.Engine
{
    public sealed class WorldObjectPointList
    {
        private readonly WorldObject _owner;
        private readonly Dictionary<string, WorldObjectPoint> _points;

        public WorldObjectPointList(WorldObject owner) 
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _points = new Dictionary<string, WorldObjectPoint>();
        }

        public void Add(string name, float x, float y)
        {
            _points.Add(name, new WorldObjectPoint(_owner, x, y));
        }

        public WorldObjectPoint Get(string name)
        {
            if (!_points.TryGetValue(name, out var point))
                throw new ArgumentException($"Point {name} does not exist.");

            return point;
        }
    }
}
