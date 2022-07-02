using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Reaper.Engine
{
    public abstract class InputAction
    {
        protected readonly InputState inputState;

        public InputAction(InputState state)
        {
            inputState = state;
        }
    }

    public class InputState
    {
        public KeyboardState KeyState { get; set; }
        public KeyboardState PreviousKeyState { get; set; }
        public GamePadState GamePadState { get; set; }
        public GamePadState PreviousGamePadState { get; set; }
    }

    public static class Input
    {
        private static readonly InputState state = new();

        internal static void Poll()
        {
            state.PreviousKeyState = state.KeyState;
            state.PreviousGamePadState = state.GamePadState;
            state.KeyState = Keyboard.GetState();
            state.GamePadState = GamePad.GetState(0);
        }

        public static PressedAction NewPressedAction()
        {
            var action = new PressedAction(state);
            return action;
        }

        public static PressedAction NewPressedAction(Keys key)
        {
            var action = new PressedAction(state);
            action.AddKey(key);
            return action;
        }

        public static AxisAction NewAxisAction()
        {
            var action = new AxisAction(state);
            return action;
        }

        public static AxisAction NewAxisAction(Keys x, Keys y)
        {
            var action = new AxisAction(state);
            action.AddKeys(x, y);
            return action;
        }
    }
}
