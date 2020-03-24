using Microsoft.Xna.Framework.Input;
using Reaper.Engine.Singletons;

namespace Reaper
{
    public static class InputBindings
    {
        public static void Initialize(Input input) 
        {
            var move = input.NewAxisAction("move");
            move.AddKeys(Keys.A, Keys.D);

            var jump = input.NewPressedAction("jump");
            jump.AddKey(Keys.Space);

            var attack = input.NewPressedAction("attack");
            attack.AddKey(Keys.Left);

            var toggleFullscreen = input.NewPressedAction("toggleFullscreen");
            toggleFullscreen.AddKey(Keys.F);

            var exitGameAction = input.NewPressedAction("exitGame");
            exitGameAction.AddKey(Keys.Escape);
        }
    }
}
