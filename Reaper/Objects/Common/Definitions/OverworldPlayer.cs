using Microsoft.Xna.Framework;
using Reaper.Engine;

namespace Reaper
{
    public static class OverworldPlayer
    {
        [Definition]
        public static WorldObjectDefinition Definition()
        {
            var playerDefinition = new WorldObjectDefinition();
            playerDefinition.SetTags("player");
            playerDefinition.SetSize(24, 32);
            playerDefinition.SetOrigin(12, 32);
            playerDefinition.AddPoint("projectileSpawn", 12, 16);
            playerDefinition.SetZOrder(10);
            playerDefinition.AddBehavior(wo => new PlayerBehavior(wo));
            playerDefinition.AddBehavior(wo => new SpriteSheetBehavior(wo, GetPlayerAnimations()));
            return playerDefinition;
        }

        private static SpriteSheetBehavior.Animation[] GetPlayerAnimations()
        {
            return new[]
            {
                new SpriteSheetBehavior.Animation
                {
                    Name = "idle",
                    ImageFilePath = "art/player/peasant",
                    Loop = true,
                    Origin = new Point(16, 32),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 32, 32),
                    }
                },
                new SpriteSheetBehavior.Animation
                {
                    Name = "walk",
                    ImageFilePath = "art/player/peasant",
                    SecPerFrame = 0.2f,
                    Loop = true,
                    Origin = new Point(16, 32),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 32, 32),
                        new SpriteSheetBehavior.Frame(32, 0, 32, 32),
                        new SpriteSheetBehavior.Frame(64, 0, 32, 32),
                        new SpriteSheetBehavior.Frame(96, 0, 32, 32),
                    }
                },
            };
        }
    }
}
