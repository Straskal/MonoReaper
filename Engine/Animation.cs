using System;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class Animation
    {
        public Animation(string name)
        {
            Name = name;
            NameHashCode = GetNameHashCode(name);
        }

        public string Name { get; }
        public Rectangle[] Frames { get; set; } = Array.Empty<Rectangle>();
        public bool Loop { get; set; }
        public int NameHashCode { get; }

        public static float FpsToSpeed(int framesPerSecond)
        {
            return 1f / framesPerSecond;
        }

        public static int GetNameHashCode(string animationName)
        {
            return HashCode.Combine(animationName);
        }
    }
}
