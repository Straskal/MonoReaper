using Microsoft.Xna.Framework;
using Reaper.Engine;
using Reaper.Ogmo;

namespace Reaper.Objects
{
    public static class LevelTransitionDefinition
    {
        public static WorldObjectDefinition Definition()
        {
            return new WorldObjectDefinition()
                .SetSize(16, 16)
                .AddBehavior(wo => new LevelTransitionBehavior(wo));
        }
    }

    public class LevelTransitionBehavior : Behavior
    {
        private WorldObject _player;

        public LevelTransitionBehavior(WorldObject owner) : base(owner)
        {
        }

        public string Level { get; set; }

        public override void OnOwnerCreated()
        {
            _player = Owner.Layout.GetWorldObjectOfType<PlayerBehavior>().Owner;
        }

        public override void Tick(GameTime gameTime)
        {
            if (Owner.Bounds.Intersects(_player.Bounds)) 
            {
                Owner.Layout.Game.LoadOgmoLayout(Level);
            }
        }
    }
}
