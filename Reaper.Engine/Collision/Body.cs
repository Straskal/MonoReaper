using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Core.Collision
{
    public delegate Vector2 CollisionCallback(Hit hit);

    public sealed class Body : Box
    {
        private readonly List<Box> _visited = new();

        public Body(float width, float height, int layerMask) : base(width, height, false, layerMask)
        {
        }

        public Body(float width, float height) : base(width, height, false)
        {
        }

        public void Move(Vector2 velocity, CollisionCallback response = null)
        {
            Move(ref velocity, int.MaxValue, response);
        }

        public void Move(Vector2 velocity, int layerMask, CollisionCallback response = null)
        {
            Move(ref velocity, layerMask, response);
        }

        public void Move(ref Vector2 velocity, CollisionCallback response = null)
        {
            Move(ref velocity, int.MaxValue, response);
        }

        public void Move(ref Vector2 velocity, int layerMask, CollisionCallback response = null)
        {
            _visited.Clear();
            _visited.Add(this);

            while (true)
            {
                var bounds = CalculateBounds();
                var offsetBounds = bounds.Offset(velocity);
                var broadphase = bounds.Union(offsetBounds);

                var others = Level.Partition.QueryBounds(broadphase)
                    .Where(other => broadphase.Intersects(other.CalculateBounds()))
                    .Where(other => (other.LayerMask | layerMask) == layerMask)
                    .Except(_visited);

                var collided = Sweep.TestAABB(bounds, velocity, others, out var hit);
                var newPosition = Offset.Create(Entity.Origin, hit.Position.X, hit.Position.Y, Width, Height);

                MoveTo(newPosition);

                if (!collided) break;

                velocity = response?.Invoke(hit) ?? Vector2.Zero;

                _visited.Add(hit.Other);
            }
        }
    }
}
