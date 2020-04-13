using Microsoft.Xna.Framework.Input;
using Reaper;

namespace Reaper
{
    public static class InputBindings
    {
        public static void Initialize(InputManager input) 
        {
            input.NewAxisAction("horizontal").AddKeys(Keys.A, Keys.D);
            input.NewAxisAction("vertical").AddKeys(Keys.W, Keys.S);
            input.NewAxisAction("attackHorizontal").AddKeys(Keys.Left, Keys.Right);
            input.NewAxisAction("attackVertical").AddKeys(Keys.Up, Keys.Down);
            input.NewPressedAction("toggleFullscreen").AddKey(Keys.F);
            input.NewPressedAction("togglePaused").AddKey(Keys.P);
            input.NewPressedAction("toggleDebug").AddKey(Keys.OemTilde);
            input.NewPressedAction("exitGame").AddKey(Keys.Escape);
        }
    }
}
