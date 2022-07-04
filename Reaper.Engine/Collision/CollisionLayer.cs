using System;

namespace Core.Collision
{
    [Flags]
    public enum CollisionLayer
    {
        Pass    = 1 << 0,
        Overlap = 1 << 1,
        Solid   = Overlap | (1 << 2)
    }
}
