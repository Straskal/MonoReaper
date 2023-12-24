using Microsoft.Xna.Framework;
using Engine.Graphics;

namespace Adventure.Components
{
    public static class ExplosionAnimations
    {
        public static readonly Animation[] Frames = new[]
            {
                new Animation("default")
                {
                    Frames = new []
                    {
                        new Rectangle(32 * 0, 0, 32, 32),
                        new Rectangle(32 * 1, 0, 32, 32),
                        new Rectangle(32 * 2, 0, 32, 32),
                        new Rectangle(32 * 3, 0, 32, 32),
                        new Rectangle(32 * 4, 0, 32, 32),
                        new Rectangle(32 * 5, 0, 32, 32),
                        new Rectangle(32 * 6, 0, 32, 32),
                    }
                }
            };
    }
}
