using Microsoft.Xna.Framework;

namespace Reaper.Engine
{
    public static class WorldObjectMovement
    {
        public static bool MoveAndCollideX(this WorldObject worldObject, float x) 
        {
            if (worldObject.Layout.Grid.IsCollidingAtOffset(worldObject, x, 0f, out var overlap))
            {
                worldObject.Move(x + overlap.Depth.X, 0f);
                return true;
            }

            worldObject.Move(x, 0f);
            return false;
        }

        public static bool MoveAndCollideY(this WorldObject worldObject, float y)
        {
            if (worldObject.Layout.Grid.IsCollidingAtOffset(worldObject, 0f, y, out var overlap))
            {
                worldObject.Move(0f, y + overlap.Depth.Y);
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
                direction.X += overlapX.Depth.X;
                overlap = overlapX;
                result = true;
            }

            if (worldObject.Layout.Grid.IsCollidingAtOffset(worldObject, 0f, direction.Y, out var overlapY))
            {
                direction.Y += overlapY.Depth.Y;

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
