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
                    .SetOrigin(new Point(16, 32))
                    .AddEffect("Shaders/SolidColor", false)
                    .AddBehavior(wo => new EnemyBehavior(wo))
                    .AddBehavior(wo => new PlatformerBehavior(wo))
                    .AddBehavior(wo => new TimerBehavior(wo))
                    .AddBehavior(wo => new DamageableBehavior(wo))
                    .AddBehavior(wo => new SpriteSheetBehavior(wo, new[]
                    {
                        new SpriteSheetBehavior.Animation
                        {
                            Name = "Idle",
                            ImageFilePath = "player",
                            SecPerFrame = 0.1f,
                            Loop = true,
                            Frames = new []
                            {
                                new SpriteSheetBehavior.Frame(96, 32, 32, 32),
                                new SpriteSheetBehavior.Frame(0, 64, 32, 32),
                                new SpriteSheetBehavior.Frame(32, 64, 32, 32),
                                new SpriteSheetBehavior.Frame(64, 64, 32, 32),
                                new SpriteSheetBehavior.Frame(96, 64, 32, 32),
                                new SpriteSheetBehavior.Frame(0, 96, 32, 32)
                            }
                        }
                    }));

                return other;
            });
        }
    }
}
