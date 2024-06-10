using System;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class Animation
    {
        public Animation(string name)
        {
            Name = name;
            HashCode = GetNameHashCode(name);
        }

        public string Name { get; }
        public int HashCode { get; }
        public Rectangle[] Frames { get; set; } = Array.Empty<Rectangle>();
        public bool Loop { get; set; }

        public static float FpsToSpeed(int framesPerSecond)
        {
            return 1f / framesPerSecond;
        }

        public static int GetNameHashCode(string animationName)
        {
            return System.HashCode.Combine(animationName);
        }
    }
}
