using Microsoft.Xna.Framework;
using Reaper.Engine;

namespace Reaper.Singletons
{
    public class GlobalInputHandler : Singleton
    {
        public GlobalInputHandler(MainGame game) : base(game) { }

        public override void Tick(GameTime gameTime)
        {
            var inputManager = Game.Singletons.Get<InputManager>();
            var toggleFullscreen = inputManager.GetAction<InputManager.PressedAction>("toggleFullscreen");
            var toggleDebug = inputManager.GetAction<InputManager.PressedAction>("toggleDebug");
            var exitGame = inputManager.GetAction<InputManager.PressedAction>("exitGame");
            if (toggleFullscreen.WasPressed()) Game.ToggleFullscreen();
            if (toggleDebug.WasPressed()) Game.ToggleDebug();
            if (exitGame.WasPressed()) Game.Exit();
        }
    }
}
