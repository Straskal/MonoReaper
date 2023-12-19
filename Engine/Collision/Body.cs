using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine.Collision
{
    /// <summary>
    /// This class is used for moving collidable objects. It contains the logic for collision detection and collision responses.
    /// </summary>
    public sealed class Body : Box
    {
        public Body(float width, float height) : base(width, height)
        {
        }

        public Body(float width, float height, int layerMask) : base(0, 0, width, height, layerMask)
        {
        }

        /// <summary>
        /// Moves the entity using the given velocity.
        /// </summary>
        /// <param name="velocity">The direction and speed to move the object.</param>
        /// <param name="layerMask">A layer mask used for filtering collisions.</param>
        /// <param name="response">A callback that resolves the collision, if there is one.</param>
        public void MoveAndCollide(ref Vector2 velocity, int layerMask, CollisionResponseCallback response)
        {
            if (velocity == Vector2.Zero)
            {
                return;
            }

            var visited = new HashSet<Box>() { this };
            var potentialCollisions = new List<Box>();

            while (true)
            {
                potentialCollisions.Clear();

                var bounds = CalculateBounds();
                var broadphaseRectangle = bounds.Union(velocity);

                foreach (var box in Level.Partition.Query(broadphaseRectangle))
                {
                    if ((box.LayerMask | layerMask) != layerMask)
                    {
                        continue;
                    }

                    if (visited.Contains(box))
                    {
                        continue;
                    }

                    if (!broadphaseRectangle.Intersects(box.CalculateBounds()))
                    {
                        continue;
                    }

                    potentialCollisions.Add(box);
                }

                if (!Sweep.Test(bounds, velocity, potentialCollisions, out var collision))
                {
                    Move(velocity);
                    break;
                }

                visited.Add(collision.Box);
                MoveTo(collision.Position);
                velocity = response.Invoke(collision);
                collision.Box.NotifyCollidedWith(this, collision);
            }
        }
    }
}
