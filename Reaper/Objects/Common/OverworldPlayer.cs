﻿using Microsoft.Xna.Framework;
using Reaper.Behaviors.Common;
using Reaper.Engine;

namespace Reaper.Objects.Common
{
    [Definition]
    public static class OverworldPlayer
    {
        static OverworldPlayer() 
        {
            Definitions.Register(typeof(OverworldPlayer), Definition);
        }

        public static WorldObjectDefinition Definition() 
        {
            var playerDefinition = new WorldObjectDefinition();
            playerDefinition.SetTags("player");
            playerDefinition.SetSize(32, 32);
            playerDefinition.SetOrigin(16, 16);
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
                    SecPerFrame = 0.1f,
                    Loop = true,
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 32, 32),
                    }
                },
                new SpriteSheetBehavior.Animation
                {
                    Name = "walk",
                    ImageFilePath = "art/player/peasant",
                    SecPerFrame = 0.5f,
                    Loop = true,
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(32, 0, 32, 32),
                        new SpriteSheetBehavior.Frame(64, 0, 32, 32),
                    }
                },
            };
        }
    }
}