using Engine.Graphics;
using Microsoft.Xna.Framework;

namespace Adventure.Components
{
    internal class SpikeAnimations
    {
        public static readonly Animation[] Frames = new[]
            {
                new Animation("retracted")
                {
                    Loop = true,
                    Frames = new []
                    {
                        new Rectangle(0, 0, 16, 16),
                    }
},
                new Animation("moving")
                {
                    Loop = true,
                    Frames = new[]
                    {
                        new Rectangle(16 * 1, 0, 16, 16),
                    }
                },
                new Animation("extended")
                {
                    Loop = true,
                    Frames = new[]
                    {
                        new Rectangle(16 * 2, 0, 16, 16),
                    }
                }
            };
    }
}
