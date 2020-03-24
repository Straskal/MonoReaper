using Microsoft.Xna.Framework;
using Reaper.Engine;
using Reaper.Objects.Player;
using Reaper.Ogmo;
using System;

namespace Reaper.Objects.Common
{
    public static class LevelTransitionDefinition
    {
        public static WorldObjectDefinition Method()
        {
            var def = new WorldObjectDefinition();

            def.AddBehavior(wo => new LevelTransitionBehavior(wo));
            def.LoadFromOgmo((wo, oe) =>
            {
                if (string.IsNullOrWhiteSpace(oe.Values.Level))
                    throw new ArgumentException("Level transitions must be provided with a layout to load");

                wo.Width = oe.Width;
                wo.Height = oe.Height;

                wo.GetBehavior<LevelTransitionBehavior>().Level = oe.Values.Level;
                wo.GetBehavior<LevelTransitionBehavior>().SpawnPoint = oe.Values.SpawnPoint;
            });

            return def;
        }
    }

    public class LevelTransitionBehavior : Behavior
    {
        private WorldObject _player;

        public LevelTransitionBehavior(WorldObject owner) : base(owner) { }

        public string Level { get; set; }
        public string SpawnPoint { get; set; }

        public override void OnLayoutStarted()
        {
            _player = Owner.Layout.GetWorldObjectAsBehavior<PlayerBehavior>().Owner;
        }

        public override void Tick(GameTime gameTime)
        {
            if (Owner.Bounds.Intersects(_player.Bounds))
            {
                Owner.Layout.Game.LoadOgmoLayout(Level, SpawnPoint);
            }
        }
    }
}
