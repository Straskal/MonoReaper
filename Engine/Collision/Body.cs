using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Core.Collision
{
    public sealed class Body : Box
    {
        public delegate Vector2 CollisionHandler(Hit hit);

        private readonly List<Box> _collidables = new();
        private readonly HashSet<Box> _visited = new();

        public Body(float width, float height) : base(0, 0, width, height) { }
        public Body(float width, float height, int layerMask) : base(0, 0, width, height, layerMask) { }

        public void Move(ref Vector2 velocity, CollisionHandler response = null)
        {
            Move(ref velocity, int.MaxValue, response);
        }

        public void Move(ref Vector2 velocity, int layerMask, CollisionHandler response)
        {
            if (velocity == Vector2.Zero)
            {
                return;
            }

            ClearVisitedColliders();

            while (true)
            {
                _collidables.Clear();

                var bounds = CalculateBounds();
                var broadphase = bounds.Project(velocity);

                foreach (var box in Level.Partition.QueryBounds(broadphase))
                {
                    if ((box.LayerMask | layerMask) == layerMask && broadphase.Intersects(box.CalculateBounds()) && !_visited.Contains(box))
                    {
                        _collidables.Add(box);
                    }
                }

                if (!SweptAlgorithm.Test(bounds, velocity, _collidables, out var hit))
                {
                    MoveTo(Entity.Position + velocity);
                    break;
                }

                AddVisitedCollider(hit.Other);
                MoveTo(Offset.Create(Entity.Origin, hit.Position.X, hit.Position.Y, Width, Height));
                velocity = response.Invoke(hit);
            }
        }

        private void ClearVisitedColliders()
        {
            _visited.Clear();
            _visited.Add(this);
        }

        private void AddVisitedCollider(Box box)
        {
            _visited.Add(box);
        }
    }
}
