using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Reaper.Engine;
using System.Collections.Generic;

namespace Reaper
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

    public sealed class InputManager : Singleton
    {
        private readonly InputState _state;
        private readonly Dictionary<string, InputAction> _actions;

        public InputManager(MainGame game) : base(game) 
        {
            _state = new InputState();
            _actions = new Dictionary<string, InputAction>();
        }

        public override void HandleInput(GameTime gameTime)
        {
            _state.PreviousKeyState = _state.KeyState;
            _state.PreviousGamePadState = _state.GamePadState;
            _state.KeyState = Keyboard.GetState();
            _state.GamePadState = GamePad.GetState(0);
        }

        public PressedAction NewPressedAction(string name)
        {
            var action = new PressedAction(_state);
            _actions.Add(name, action);
            return action;
        }

        public AxisAction NewAxisAction(string name)
        {
            var action = new AxisAction(_state);
            _actions.Add(name, action);
            return action;
        }

        public T GetAction<T>(string name) where T : InputAction
        {
            return _actions[name] as T;
        }
    }
}
