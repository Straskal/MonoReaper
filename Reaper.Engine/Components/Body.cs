using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Reaper.Engine.AABB;

namespace Reaper.Engine.Components
{
    public delegate Vector2 CollisionCallback(Hit hit);

    public sealed class Body : Box
    {
        private readonly List<Box> visited = new();

        public Body(float width, float height) : base(CollisionLayer.Overlap, width, height)
        {
        }

        public void Move(ref Vector2 velocity, CollisionCallback response = null)
        {
            visited.Clear();
            visited.Add(this);

            var collided = true;

            while (collided)
            {
                var previousPosition = Entity.Position;
                var broadphase = Collision.GetBroadphaseRectangle(Bounds.Position, Bounds.Size, velocity);
                var others = Level.Partition.QueryBounds(broadphase).Except(visited);

                collided = Collision.TestAABB(Bounds, velocity, others, out var info);

                Entity.Position = OriginHelpers.Offset(Entity.Origin, info.Position.X, info.Position.Y, Width, Height);

                UpdateBBox(previousPosition);

                if (!collided) break;

                velocity = response?.Invoke(info) ?? Vector2.Zero;

                visited.Add(info.Other);
            }
        }
    }
}
