using Microsoft.Xna.Framework;
using Reaper.Engine;
using Reaper.Ogmo;

namespace Reaper.Behaviors.Common
{
    public class LevelTransitionBehavior : Behavior
    {
        private WorldObject _player;

        public LevelTransitionBehavior(WorldObject owner) : base(owner) { }

        public string Level { get; set; }
        public string SpawnPoint { get; set; }

        public override void OnLayoutStarted()
        {
            _player = Owner.Layout.Objects.GetFirstBehaviorOfType<PlayerBehavior>().Owner;
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
