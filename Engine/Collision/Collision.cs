﻿using System;
using Microsoft.Xna.Framework;

namespace Engine.Collision
{
    /// <summary>
    /// This class represents a collision hit and contains some helper methods to resolve the collision.
    /// </summary>
    public readonly ref struct Collision
    {
        /// <summary>
        /// The other object involved in the collision.
        /// </summary>
        public readonly Box Box;

        /// <summary>
        /// The velocity that the moving object is traveling at.
        /// </summary>
        public readonly Vector2 Velocity;

        /// <summary>
        /// The collision normal.
        /// </summary>
        /// <remarks>
        /// Collision normals give you information about the direction of the collision. Normals are used mostly in resolving the collision.
        /// </remarks>
        public readonly Vector2 Normal;

        /// <summary>
        /// The result position of the moving object after the collision.
        /// </summary>
        public readonly Vector2 Position;

        /// <summary>
        /// The time at which the collision occured.
        /// </summary>
        public readonly float Time;

        /// <summary>
        /// The amount of time remaining since the collision occured.
        /// </summary>
        public float RemainingTime => 1f - Time;

        /// <summary>
        /// An empty hit.
        /// </summary>
        public static Collision Empty => new(null, Vector2.Zero, Vector2.Zero, 1f, Vector2.Zero);

        public Collision(Box other, Vector2 velocity, Vector2 normal, float collisionTime, Vector2 position)
        {
            Box = other;
            Velocity = velocity;
            Normal = normal;
            Time = collisionTime;
            Position = position;
        }

        /// <summary>
        /// Ignores the collision and continues moving the object in the direction that it was travelling.
        /// </summary>
        /// <returns>
        /// The unchanged velocity as if the collision didn't occur.
        /// </returns>
        /// <remarks>
        /// This method is intended to be used as a response to a collision.
        /// </remarks>
        public Vector2 Ignore()
        {
            return Velocity * RemainingTime;
        }

        /// <summary>
        /// Bounces the moving object off of the collision surface.
        /// </summary>
        /// <returns>
        /// The object's new velocity.
        /// </returns>
        /// <remarks>
        /// This method is intended to be used as a response to a collision.
        /// </remarks>
        public Vector2 Bounce()
        {
            var result = Velocity * RemainingTime;

            if (Math.Abs(Normal.X) > 0.0001f)
            {
                result.X *= -1;
            }
            if (Math.Abs(Normal.Y) > 0.0001f)
            {
                result.Y *= -1;
            }

            return result;
        }

        /// <summary>
        /// Slides an object along the surface that it collided with.
        /// </summary>
        /// <returns>
        /// The object's new velocity.
        /// </returns>
        /// <remarks>
        /// This method is intended to be used as a response to a collision.
        /// </remarks>
        public Vector2 Slide()
        {
            var inverseNormal = new Vector2(Normal.Y, Normal.X);

            return inverseNormal * Velocity * inverseNormal * RemainingTime;
        }
    }
}