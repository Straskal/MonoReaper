using Microsoft.Xna.Framework;
using System;

namespace Reaper.Engine
{
    /// <summary>
    /// A point on a world object is basically just metadata. Points are useful if you need to spawn objects at a certain position.
    /// </summary>
    public class WorldObjectPoint
    {
        private readonly WorldObject _worldObject;
        private readonly Vector2 _value;

        internal WorldObjectPoint(WorldObject worldObject, float x, float y) 
        {
            _worldObject = worldObject ?? throw new ArgumentNullException(nameof(worldObject));
            _value = new Vector2(x, y);
        }

        public Vector2 Value => new Vector2(_worldObject.Bounds.Left, _worldObject.Bounds.Top) + _value;
    }
}
