using Microsoft.Xna.Framework;
using Engine.Graphics;

namespace Adventure.Components
{
    internal class PlayerAnimations
    {
        public static readonly SpriteSheet.Animation[] Frames = new[]
            {
                new SpriteSheet.Animation
                {
                    Name = "idle",
                    Loop = true,
                    Frames = new []
                    {
                        new Rectangle(0, 0, 16, 16),
                    }
},
                new SpriteSheet.Animation
                {
                    Name = "walk_down",
                    Loop = true,
                    Frames = new[]
                    {
                        new Rectangle(16 * 1, 0, 16, 16),
                        new Rectangle(16 * 2, 0, 16, 16),
                    }
                },
                new SpriteSheet.Animation
                {
                    Name = "walk_up",
                    Loop = true,
                    Frames = new[]
                    {
                        new Rectangle(16 * 3, 0, 16, 16),
                        new Rectangle(16 * 4, 0, 16, 16),
                    }
                },
                new SpriteSheet.Animation
                {
                    Name = "walk_left",
                    Loop = true,
                    Frames = new[]
                    {
                        new Rectangle(16 * 5, 0, 16, 16),
                        new Rectangle(16 * 6, 0, 16, 16),
                    }
                },
                new SpriteSheet.Animation
                {
                    Name = "walk_right",
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
