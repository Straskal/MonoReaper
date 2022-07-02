using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Reaper.Engine.AABB;

namespace Reaper.Engine.Components
{
    public delegate Vector2 CollisionCallback(CollisionInfo hit);

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

            while (true)
            {
                var previousPosition = Entity.Position;
                var others = Level.Partition.QueryBounds(Bounds).Except(visited);
                var collided = Collision.TestAABB(this, velocity, others, out var info);

                Entity.Position = OriginHelpers.Offset(Entity.Origin, info.Position.X, info.Position.Y, Width, Height);

                UpdateBBox(previousPosition);

                if (!collided) break;

                velocity = response?.Invoke(info) ?? Vector2.Zero;

                visited.Add(info.Other);
            }
        }
    }
}
