using Microsoft.Xna.Framework;
using Reaper.Engine;

namespace Reaper
{
    public class GlobalInputHandler : Singleton
    {
        public GlobalInputHandler(MainGame game) : base(game) { }

        public override void HandleInput(GameTime gameTime)
        {
            var inputManager = Game.Singletons.Get<InputManager>();
            var toggleFullscreen = inputManager.GetAction<PressedAction>("toggleFullscreen");
            var togglePaused = inputManager.GetAction<PressedAction>("togglePaused");
            var toggleDebug = inputManager.GetAction<PressedAction>("toggleDebug");
            var exitGame = inputManager.GetAction<PressedAction>("exitGame");

            if (toggleFullscreen.WasPressed()) Game.ToggleFullscreen();
            if (togglePaused.WasPressed()) Game.TogglePaused();
            if (toggleDebug.WasPressed()) Game.ToggleDebug();
            if (exitGame.WasPressed()) Game.Exit();
        }
    }
}
