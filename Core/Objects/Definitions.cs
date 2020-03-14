using ItsGood;
using ItsGood.Builtins;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Core.Objects
{
    public static class Definitions
    {
        private static readonly Dictionary<string, Func<WorldObjectDefinition>> _definitionFactories = new Dictionary<string, Func<WorldObjectDefinition>>();
        private static readonly Dictionary<string, WorldObjectDefinition> _definitions = new Dictionary<string, WorldObjectDefinition>();

        public static WorldObjectDefinition Get(string name) 
        {
            if (!_definitions.ContainsKey(name)) 
            {
                _definitions.Add(name, _definitionFactories[name].Invoke());
            }

            return _definitions[name];
        }

        public static void Register() 
        {
            _definitionFactories.Add("player", Player.Definition);

            _definitionFactories.Add("other", () => 
            {
                var other = new WorldObjectDefinition(32, 32);

                other
                    .AddImage("player", new Rectangle(0, 0, 32, 32))
                    .SetOrigin(new Point(16, 32))
                    .AddEffect("Shaders/SolidColor", false)
                    .AddBehavior<EnemyBehavior>()
                    .AddBehavior<PlatformerBehavior>()
                    .AddBehavior<TimerBehavior>()
                    .AddBehavior<DamageableBehavior>()
                    .AddBehavior<AnimationBehavior, AnimationBehavior.Params>(new AnimationBehavior.Params
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
                                    new AnimationBehavior.Frame { Source = new Rectangle(96, 32, 32, 32) },
                                    new AnimationBehavior.Frame { Source = new Rectangle(0, 64, 32, 32) },
                                    new AnimationBehavior.Frame { Source = new Rectangle(32, 64, 32, 32) },
                                    new AnimationBehavior.Frame { Source = new Rectangle(64, 64, 32, 32) },
                                    new AnimationBehavior.Frame { Source = new Rectangle(96, 64, 32, 32) },
                                    new AnimationBehavior.Frame { Source = new Rectangle(0, 96, 32, 32) }
                                }
                            }
                        }
                    });

                return other;
            });
        }
    }
}
