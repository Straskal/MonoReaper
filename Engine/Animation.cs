using System;
using Microsoft.Xna.Framework;

namespace Engine
{
    /// <summary>
    /// This class represents an animation containing it's name and animation frames.
    /// </summary>
    public class Animation
    {
        public Animation(string name)
        {
            Name = name;
            NameHashCode = GetNameHashCode(name);
        }

        /// <summary>
        /// Gets the animation's name
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets the animation's frames
        /// </summary>
        public Rectangle[] Frames
        {
            get;
            init;
        } = Array.Empty<Rectangle>();

        /// <summary>
        /// Returns true if the animation is set to loop
        /// </summary>
        public bool Loop
        {
            get;
            init;
        }

        /// <summary>
        /// Gets this animation's name hashcode
        /// </summary>
        public int NameHashCode
        {
            get;
        }

        /// <summary>
        /// Converts frames per second to animation speed
        /// </summary>
        /// <param name="framesPerSecond"></param>
        /// <returns></returns>
        public static float FpsToSpeed(int framesPerSecond)
        {
            return 1f / framesPerSecond;
        }

        /// <summary>
        /// Gets the hashcode for animation name
        /// </summary>
        /// <param name="animationName"></param>
        /// <returns></returns>
        /// <remarks>
        /// This will return a new hashcode every time the program is run.
        /// </remarks>
        public static int GetNameHashCode(string animationName)
        {
            return HashCode.Combine(animationName);
        }
    }
}
