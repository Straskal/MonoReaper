using Microsoft.Xna.Framework;
using System.Linq;

namespace Reaper.Engine.Behaviors
{
    /// <summary>
    /// The LOS is basically a rectangle raycast that is blocked by solids.
    /// 
    /// Can be used for enemy lines of sight.
    /// </summary>
    public class LineOfSightBehavior : Behavior
    {
        public LineOfSightBehavior(WorldObject owner) : base(owner) { }

        public int Distance { get; set; } = 128;

        public bool HasLOS(WorldObject worldObject)
        {
            Rectangle ray = new Rectangle(
                Owner.IsMirrored ? (int)Owner.Position.X - Distance : (int)Owner.Position.X,
                Owner.Bounds.Top,
                Distance,
                Owner.Bounds.Height);

            var hitsByDistance = Layout.Grid.QueryBounds(ray)
                .Where(wo => wo != Owner)
                .OrderBy(wo => Vector2.Distance(Owner.Position, wo.Position));

            foreach (var hit in hitsByDistance)
            {
                if (hit == worldObject)
                    return true;

                if (hit.IsSolid)
                    return false;
            }

            return false;
        }
    }
}
