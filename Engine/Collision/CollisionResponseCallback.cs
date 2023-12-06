using Microsoft.Xna.Framework;

namespace Engine.Collision
{
    /// <summary>
    /// A function that handles a collision and returns a new velocity to resolve the collision using the hit helper methods.
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    public delegate Vector2 CollisionResponseCallback(Hit hit);
}
