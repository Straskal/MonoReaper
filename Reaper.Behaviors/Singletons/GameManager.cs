using Microsoft.Xna.Framework;
using Reaper.Engine;

namespace Reaper
{
    public class GameManager : Singleton
    {
        private WorldObject _player;

        public GameManager(MainGame game) : base(game) { }

        public override void OnLayoutStarted()
        {
            _player = CurrentLayout.Objects.FindFirstWithTag("player");
        }

        public override void Tick(GameTime gameTime)
        {
            if (_player != null && _player.IsDestroyed) 
            {
                _player = null;
                Game.LoadOgmoLayout("dungeontest.json");
            }
        }
    }
}
