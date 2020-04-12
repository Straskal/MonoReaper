using Microsoft.Xna.Framework;
using Reaper.Engine;

namespace Reaper.Behaviors
{
    public static class MovementStrategies
    {
        public static bool MoveAndCollide(this WorldObject worldObject, Vector2 direction, out Overlap overlap)
        {
            overlap = new Overlap();
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
    }
}
