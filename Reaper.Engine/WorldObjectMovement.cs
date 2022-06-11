using Microsoft.Xna.Framework;
using System;

namespace Reaper.Engine
{
    // TODO: All movement has great potential for optimization.
    // We perform two spatial queries per movement when combining X and Y movement.
    // This is a quick and easy solution for now.
    // But if we can start taking the objects direction (Vector2) into the mix, we can calculate the "slide" direction.
    public static class WorldObjectMovement
    {
        private const float OVERLAP_BUFFER = 0.005f;

        /// <summary>
        /// Moves the world object and stops when it hits a solid.
        /// Returns true if there was a collision and outputs the overlap.
        /// </summary>
        /// <param name="worldObject"></param>
        /// <param name="direction"></param>
        /// <param name="overlap"></param>
        /// <returns></returns>
        public static bool MoveAndCollideX(this WorldObject worldObject, float x) 
        {
            if (worldObject.Layout.Grid.IsCollidingAtOffset(worldObject, x, 0f, out var overlap))
            {
                var depthX = overlap.Depth.X;
                var depthSign = Math.Sign(depthX);
                var correction = depthX + OVERLAP_BUFFER * depthSign;

                worldObject.Move(x + correction, 0f);
                return true;
            }

            worldObject.Move(x, 0f);
            return false;
        }


        /// <summary>
        /// Moves the world object and stops when it hits a solid.
        /// Returns true if there was a collision and outputs the overlap.
        /// </summary>
        /// <param name="worldObject"></param>
        /// <param name="direction"></param>
        /// <param name="overlap"></param>
        /// <returns></returns>
        public static bool MoveAndCollideY(this WorldObject worldObject, float y)
        {
            if (worldObject.Layout.Grid.IsCollidingAtOffset(worldObject, 0f, y, out var overlap))
            {
                var depthY = overlap.Depth.Y;
                var depthSign = Math.Sign(depthY);
                var correction = depthY + OVERLAP_BUFFER * depthSign;

                worldObject.Move(0f, y + correction);
                return true;
            }

            worldObject.Move(0f, y);
            return false;
        }

        /// <summary>
        /// Moves the world object and stops when it hits a solid.
        /// Returns true if there was a collision and outputs the overlap.
        /// </summary>
        /// <param name="worldObject"></param>
        /// <param name="direction"></param>
        /// <param name="overlap"></param>
        /// <returns></returns>
        public static bool MoveAndCollide(this WorldObject worldObject, Vector2 direction, out Overlap overlap)
        {
            overlap = new Overlap();
            
            if (direction == Vector2.Zero)
                return false;

            bool result = false;

            if (worldObject.Layout.Grid.IsCollidingAtOffset(worldObject, direction.X, 0f, out var overlapX))
            {
                var depthX = overlapX.Depth.X;
                var depthSign = Math.Sign(depthX);
                var correction = depthX + OVERLAP_BUFFER * depthSign;

                direction.X += correction;
                overlap = overlapX;
                result = true;
            }

            if (worldObject.Layout.Grid.IsCollidingAtOffset(worldObject, 0f, direction.Y, out var overlapY))
            {
                var depthY = overlapY.Depth.Y;
                var depthSign = Math.Sign(depthY);
                var correction = depthY + OVERLAP_BUFFER * depthSign;

                direction.Y += correction;

                if (!result)
                    overlap = overlapY;

                result = true;
            }

            worldObject.Move(direction.X, direction.Y);
            return result;
        }

        /// <summary>
        /// Moves the world object while looking for overlaps.
        /// Returns true if there is an overlap and outputs the overlap.
        /// </summary>
        /// <param name="worldObject"></param>
        /// <param name="direction"></param>
        /// <param name="overlap"></param>
        /// <returns></returns>
        public static bool MoveAndOverlap(this WorldObject worldObject, Vector2 direction, out Overlap overlap)
        {
            worldObject.Move(direction);

            return worldObject.Layout.Grid.IsOverlapping(worldObject, out overlap);
        }

        /// <summary>
        /// Moves the world object while looking for overlaps, ignoring any overlaps with the given tags.
        /// Returns true if there is an overlap and outputs the overlap.
        /// </summary>
        /// <param name="worldObject"></param>
        /// <param name="direction"></param>
        /// <param name="overlap"></param>
        /// <returns></returns>
        public static bool MoveAndOverlap(this WorldObject worldObject, Vector2 direction, string[] ignoreTags, out Overlap overlap)
        {
            worldObject.Move(direction);

            return worldObject.Layout.Grid.IsOverlapping(worldObject, ignoreTags, out overlap);
        }
    }
}
