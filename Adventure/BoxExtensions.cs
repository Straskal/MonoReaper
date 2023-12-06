using Engine.Collision;
using static Adventure.Constants;

namespace Adventure
{
    public static class BoxExtensions
    {
        public static bool IsSolid(this Box box) 
        {
            return (box.LayerMask & EntityLayers.Solid) == EntityLayers.Solid;
        }
    }
}
