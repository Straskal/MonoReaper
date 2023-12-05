using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Core.Collision
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

                foreach (var box in Level.Partition.QueryBounds(broadphaseRectangle))
                {
                    // Don't check for collisions if the object is filtered out with a layer mask.
                    if ((box.LayerMask | layerMask) != layerMask) 
                    {
                        continue;
                    }

                    // Don't collide with objects that we've already collided with this frame.
                    if (visited.Contains(box))
                    {
                        continue;
                    }

                    // Do a broadphase collision check on objects so we are only running the sweep logic on potential collisions.
                    // This essentially rules out any other boxes that this body could not possibly collide with.
                    if (!broadphaseRectangle.Intersects(box.CalculateBounds())) 
                    {
                        continue;
                    }

                    potentialCollisions.Add(box);
                }

                // Run the narrow collision sweep test on all potential collisions.
                if (!Sweep.Test(bounds, velocity, potentialCollisions, out var hit))
                {
                    // If no collisions are detected, just move the object using the given velocity.
                    Move(velocity);
                    break;
                }

                // If a collision is detected, mark the collided object as visited and set the object's position to the hit position.
                visited.Add(hit.Other);
                MoveTo(hit.Position);

                // Run the collision response to get the new velocity.
                velocity = response.Invoke(hit);
            }
        }
    }
}
