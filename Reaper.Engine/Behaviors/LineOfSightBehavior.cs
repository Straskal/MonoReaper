using Microsoft.Xna.Framework;
using System.Linq;

namespace Reaper.Engine.Behaviors
{
    public class LineOfSightBehavior : Behavior
    {
        public LineOfSightBehavior(WorldObject owner) : base(owner)
        {
        }

        public int Distance { get; set; } = 128;

        public bool HasLOS(WorldObject worldObject) 
        {
            bool mirrored = Owner.IsMirrored;
            Rectangle bounds = Owner.Bounds;

            Rectangle ray = new Rectangle(
                mirrored ? (int)Owner.Position.X - Distance : (int)Owner.Position.X,
                bounds.Top,
                Distance,
                bounds.Height);

            var hitsByDistance = Owner.Layout.QueryBounds(ray)
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
