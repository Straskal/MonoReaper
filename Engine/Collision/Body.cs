using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Core.Collision
{
    public sealed class Body : Box
    {
        public Body(float width, float height) : base(width, height) 
        {
        }

        public Body(float width, float height, int layerMask) : base(0, 0, width, height, layerMask) 
        {
        }

        public void Move(ref Vector2 velocity, int layerMask, CollisionCallback response)
        {
            if (velocity == Vector2.Zero)
            {
                return;
            }

            var visited = new HashSet<Box>() { this };
            var broadphaseResults = new List<Box>();

            while (true)
            {
                broadphaseResults.Clear();

                var bounds = CalculateBounds();
                var broadphaseRectangle = bounds.Project(velocity);

                foreach (var box in Level.Partition.QueryBounds(broadphaseRectangle))
                {
                    if ((box.LayerMask | layerMask) == layerMask && broadphaseRectangle.Intersects(box.CalculateBounds()) && !visited.Contains(box))
                    {
                        broadphaseResults.Add(box);
                    }
                }

                if (!Sweep.Test(bounds, velocity, broadphaseResults, out var hit))
                {
                    MoveTo(Entity.Position + velocity);
                    break;
                }

                visited.Add(hit.Other);
                MoveTo(Offset.Create(Entity.Origin, hit.Position.X, hit.Position.Y, Width, Height));
                velocity = response.Invoke(hit);
            }
        }
    }
}
