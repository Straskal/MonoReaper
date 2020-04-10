using Microsoft.Xna.Framework.Input;
using Reaper.Singletons;

namespace Reaper
{
    public static class InputBindings
    {
        public static void Initialize(InputManager input) 
        {
            var horizontal = input.NewAxisAction("horizontal");
            horizontal.AddKeys(Keys.A, Keys.D);

            var vertical = input.NewAxisAction("vertical");
            vertical.AddKeys(Keys.W, Keys.S);

            var attack = input.NewPressedAction("attack");
            attack.AddKey(Keys.Left);

            var toggleFullscreen = input.NewPressedAction("toggleFullscreen");
            toggleFullscreen.AddKey(Keys.F);

            var toggleDebug = input.NewPressedAction("toggleDebug");
            toggleDebug.AddKey(Keys.OemTilde);

            var exitGameAction = input.NewPressedAction("exitGame");
            exitGameAction.AddKey(Keys.Escape);
        }
    }
}
