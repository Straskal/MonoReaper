using Microsoft.Xna.Framework;

namespace Reaper.Engine
{
    public static class WorldObjectMovement
    {
        /// <summary>
        /// Move along x axis, resolve any collisions.
        /// Move along y axis, resolve any collisions.
        /// 
        /// The last axis overlap is returned.
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

        public static bool MoveAndOverlap(this WorldObject worldObject, Vector2 direction, out Overlap overlap)
        {
            worldObject.Move(direction);
            return worldObject.Layout.Grid.IsOverlapping(worldObject, out overlap);
        }

        public static bool MoveAndOverlap(this WorldObject worldObject, Vector2 direction, string[] ignoreTags, out Overlap overlap)
        {
            worldObject.Move(direction);
            return worldObject.Layout.Grid.IsOverlapping(worldObject, ignoreTags, out overlap);
        }
    }
}
