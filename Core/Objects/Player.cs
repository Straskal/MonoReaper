using ItsGood;
using ItsGood.Builtins;
using Microsoft.Xna.Framework;

namespace Core.Objects
{
    public static class Player
    {
        public static WorldObjectDefinition GetTemplate() 
        {
            var playerDefinition = new WorldObjectDefinition(32, 32);

            playerDefinition.AddImage("player", new Rectangle(0, 0, 32, 32));
            playerDefinition.SetOrigin(new Point(16, 32));
            playerDefinition.AddBehavior<PlayerBehavior>();
            playerDefinition.AddBehavior<PlatformerBehavior>();
            playerDefinition.AddBehavior<AnimationBehavior, AnimationBehavior.Params>(new AnimationBehavior.Params
            {
                Animations = new[]
                {
                        new AnimationBehavior.Animation
                        {
                            Name = "Idle",
                            ImageFilePath = "player",
                            SecPerFrame = 0.1f,
                            Loop = true,
                            Frames = new []
                            {
                                new AnimationBehavior.Frame(96, 32, 32, 32),
                                new AnimationBehavior.Frame(0, 64, 32, 32),
                                new AnimationBehavior.Frame(32, 64, 32, 32),
                                new AnimationBehavior.Frame(64, 64, 32, 32),
                                new AnimationBehavior.Frame(96, 64, 32, 32),
                                new AnimationBehavior.Frame(0, 96, 32, 32)
                            }
                        },
                        new AnimationBehavior.Animation
                        {
                            Name = "Run",
                            ImageFilePath = "player",
                            SecPerFrame = 0.2f,
                            Loop = true,
                            Frames = new []
                            {
                                new AnimationBehavior.Frame(96, 0, 32, 32),
                                new AnimationBehavior.Frame(0, 32, 32, 32),
                                new AnimationBehavior.Frame(32, 32, 32, 32),
                                new AnimationBehavior.Frame(64, 32, 32, 32) ,
                            }
                        },
                        new AnimationBehavior.Animation
                        {
                            Name = "Jump",
                            ImageFilePath = "player",
                            SecPerFrame = 0.2f,
                            Frames = new []
                            {
                                new AnimationBehavior.Frame(32, 0, 32, 32),
                                new AnimationBehavior.Frame(64, 0, 32, 32),
                            }
                        },
                        new AnimationBehavior.Animation
                        {
                            Name = "Attack",
                            ImageFilePath = "player_attack",
                            SecPerFrame = 0.05f,
                            Frames = new []
                            {
                                new AnimationBehavior.Frame(0, 0, 64, 32),
                                new AnimationBehavior.Frame(64, 0, 64, 32),
                                new AnimationBehavior.Frame(0, 32, 64, 32),
                                new AnimationBehavior.Frame(64, 32, 64, 32),
                                new AnimationBehavior.Frame(0, 64, 64, 32),
                            }
                        }
                    }
            });

            return playerDefinition;
        }
    }
}
