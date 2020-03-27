using Microsoft.Xna.Framework;
using Reaper.Engine;
using Reaper.Engine.Behaviors;
using Reaper.Objects.Common;

namespace Reaper.Objects.Player
{
    public static class Player
    {
        public static WorldObjectDefinition Method()
        {
            var playerDefinition = new WorldObjectDefinition();

            playerDefinition.SetSize(16, 16);
            playerDefinition.SetOrigin(new Point(8, 16));
            playerDefinition.AddBehavior(wo => new PlayerBehavior(wo));
            playerDefinition.AddBehavior(wo => new PlatformerBehavior(wo) { MaxSpeed = 350f, GroundBufferInPixels = 2 });
            playerDefinition.AddBehavior(wo => new TimerBehavior(wo));
            playerDefinition.AddBehavior(wo => new DamageableBehavior(wo));
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
                    Name = "idle",
                    ImageFilePath = "art/player/smallplayer",
                    SecPerFrame = 0.1f,
                    Loop = true,
                    Origin = new Vector2(11, 24),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(24, 0, 24, 24),
                    }
                },
                new SpriteSheetBehavior.Animation
                {
                    Name = "Run",
                    ImageFilePath = "art/player/smallplayer",
                    SecPerFrame = 0.15f,
                    Loop = true,
                    Origin = new Vector2(11, 24),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 24, 24),
                        new SpriteSheetBehavior.Frame(24, 0, 24, 24),
                        new SpriteSheetBehavior.Frame(48, 0, 24, 24),
                        new SpriteSheetBehavior.Frame(72, 0, 24, 24) ,
                    }
                },
                new SpriteSheetBehavior.Animation
                {
                    Name = "Jump",
                    ImageFilePath = "art/player/smallplayer",
                    SecPerFrame = 0.2f,
                    Origin = new Vector2(11, 24),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 24, 24),
                    }
                },
                 new SpriteSheetBehavior.Animation
                {
                    Name = "Fall",
                    ImageFilePath = "art/player/smallplayer",
                    SecPerFrame = 0.2f,
                    Origin = new Vector2(11, 24),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 24, 24),
                    }
                },
                new SpriteSheetBehavior.Animation
                {
                    Name = "attack_0",
                    ImageFilePath = "art/player/player_attack",
                    SecPerFrame = 0.05f,
                    Origin = new Vector2(11, 24),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 64, 32),
                        new SpriteSheetBehavior.Frame(64, 0, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 32, 64, 32),
                        new SpriteSheetBehavior.Frame(64, 32, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 64, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 64, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 64, 64, 32),
                    }
                },
                new SpriteSheetBehavior.Animation
                {
                    Name = "attack_1",
                    ImageFilePath = "art/player/player_attack",
                    SecPerFrame = 0.05f,
                    Origin = new Vector2(11, 24),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(64, 64, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 96, 64, 32),
                        new SpriteSheetBehavior.Frame(64, 96, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 128, 64, 32),
                        new SpriteSheetBehavior.Frame(64, 128, 64, 32),
                        new SpriteSheetBehavior.Frame(64, 128, 64, 32),
                    }
                },
                new SpriteSheetBehavior.Animation
                {
                    Name = "attack_2",
                    ImageFilePath = "art/player/player_attack",
                    SecPerFrame = 0.05f,
                    Origin = new Vector2(11, 24),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 64, 32),
                        new SpriteSheetBehavior.Frame(64, 0, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 32, 64, 32),
                        new SpriteSheetBehavior.Frame(64, 32, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 64, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 64, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 64, 64, 32),
                        new SpriteSheetBehavior.Frame(0, 64, 64, 32),
                    }
                },
            };
        }
    }
}
