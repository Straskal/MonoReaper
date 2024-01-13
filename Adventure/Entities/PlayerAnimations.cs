using Microsoft.Xna.Framework;
using Engine;

namespace Adventure.Entities
{
    internal class PlayerAnimations
    {
        public static readonly Animation[] Frames = new[]
            {
                new Animation("idle")
                {
                    Loop = true,
                    Frames = new []
                    {
                        new Rectangle(0, 0, 16, 16),
                    }
},
                new Animation("walk_down")
                {
                    Loop = true,
                    Frames = new[]
                    {
                        new Rectangle(16 * 1, 0, 16, 16),
                        new Rectangle(16 * 2, 0, 16, 16),
                    }
                },
                new Animation("walk_up")
                {
                    Loop = true,
                    Frames = new[]
                    {
                        new Rectangle(16 * 3, 0, 16, 16),
                        new Rectangle(16 * 4, 0, 16, 16),
                    }
                },
                new Animation("walk_left")
                {
                    Loop = true,
                    Frames = new[]
                    {
                        new Rectangle(16 * 5, 0, 16, 16),
                        new Rectangle(16 * 6, 0, 16, 16),
                    }
                },
                new Animation("walk_right")
                {
                    Loop = true,
                    Frames = new[]
                    {
                        new Rectangle(16 * 7, 0, 16, 16),
                        new Rectangle(16 * 8, 0, 16, 16),
                    }
                },
            };
    }
}
