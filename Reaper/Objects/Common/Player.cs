using Microsoft.Xna.Framework;
using Reaper.Behaviors.Common;
using Reaper.Engine;

namespace Reaper.Objects.Common
{
    public static class Player
    {
        public static WorldObjectDefinition Method()
        {
            var playerDefinition = new WorldObjectDefinition();
            playerDefinition.WithTags("player");
            playerDefinition.SetSize(32, 32);
            playerDefinition.SetOrigin(16, 32);
            playerDefinition.AddBehavior(wo => new OverworldPlayerBehavior(wo));
            playerDefinition.AddBehavior(wo => new ScrollToBehavior(wo));
            playerDefinition.AddBehavior(wo => new SpriteSheetBehavior(wo, GetPlayerAnimations()));
            return playerDefinition;
        }

        private static SpriteSheetBehavior.Animation[] GetPlayerAnimations() 
        {
            return new[]
            {
                new SpriteSheetBehavior.Animation
                {
                    Name = "walk",
                    ImageFilePath = "art/tilesets/peasant",
                    SecPerFrame = 0.5f,
                    Loop = true,
                    Origin = new Vector2(16, 32),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(32, 0, 32, 32),
                        new SpriteSheetBehavior.Frame(64, 0, 32, 32),
                    }
                },
                new SpriteSheetBehavior.Animation
                {
                    Name = "idle",
                    ImageFilePath = "art/tilesets/peasant",
                    SecPerFrame = 0.1f,
                    Loop = true,
                    Origin = new Vector2(16, 32),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 32, 32),
                    }
                },
            };
        }
    }
}
