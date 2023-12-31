using Microsoft.Xna.Framework;

namespace Engine.Collision
{
    /// <summary>
    /// A function that handles a collision and returns a new velocity to resolve the collision using the hit helper methods.
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    public delegate Vector2 CollisionResponseCallback(Collision hit);

    /// <summary>
    /// A function that is invoked when a box is collided with.
    /// </summary>
    /// <param name="body"></param>
    /// <param name="collision"></param>
    public delegate void CollidedWithCallback(Collider body, Collision collision);
}
