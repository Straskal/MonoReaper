using System;

namespace Reaper.Engine.Collision
{
    [Flags]
    public enum CollisionLayer
    {
        Pass    = 1 << 0,
        Overlap = 1 << 1,
        Solid   = Overlap | (1 << 2)
    }
}
