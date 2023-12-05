using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Core;

namespace Reaper
{
    internal class GameplayLevel : Level
    {
        private readonly PressedAction toggleDebug;
        private readonly PressedAction toggleFullscreen;
        private readonly PressedAction quit;

        public GameplayLevel(int cellSize, int width, int height) : base(cellSize, width, height)
        {
            toggleDebug = Input.NewPressedAction(Keys.OemTilde);
            toggleFullscreen = Input.NewPressedAction(Keys.F);
            quit = Input.NewPressedAction(Keys.Escape);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (toggleDebug.WasPressed()) 
            {
                App.Current.ToggleDebug();
            }

            if (toggleFullscreen.WasPressed())
            {
                App.ToggleFullscreen();
            }

            if (quit.WasPressed()) 
            {
                App.Current.Exit();
            }
        }
    }
}
