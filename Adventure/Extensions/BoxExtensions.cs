using Engine;
using static Adventure.Constants;

namespace Adventure.Extensions
{
    public static class BoxExtensions
    {
        public static bool IsSolid(this CollisionComponent box)
        {
            return (box.Layer & EntityLayers.Solid) == EntityLayers.Solid;
        }
    }
}
