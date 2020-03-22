using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Reaper.Engine.Singletons
{
    public sealed class Input : Singleton
    {
        public class InputState
        {
            public KeyboardState KeyState { get; set; }
            public KeyboardState PreviousKeyState { get; set; }
            public GamePadState GamePadState { get; set; }
            public GamePadState PreviousGamePadState { get; set; }
        }

        public class Action
        {
            protected readonly InputState inputState;

            public Action(InputState state)
            {
                inputState = state;
            }
        }

        public class PressedAction : Action
        {
            private readonly List<Keys> _keys;

            public PressedAction(string name, InputState state) : base(state)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));

                _keys = new List<Keys>();
            }

            public string Name { get; }

            public void AddKey(Keys key)
            {
                _keys.Add(key);
            }

            public bool IsDown()
            {
                for (int i = 0; i < _keys.Count; i++)
                {
                    if (inputState.KeyState.IsKeyDown(_keys[i]))
                        return true;
                }

                return false;
            }

            public bool WasPressed()
            {
                for (int i = 0; i < _keys.Count; i++)
                {
                    if (inputState.KeyState.IsKeyDown(_keys[i]) && inputState.PreviousKeyState.IsKeyUp(_keys[i]))
                        return true;
                }

                return false;
            }
        }

        public class AxisAction : Action
        {
            public enum ThumbSticks
            {
                None, Left, Right
            }

            private readonly List<Tuple<Keys, Keys>> _keys;

            public AxisAction(string name, InputState state) : base(state)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));

                _keys = new List<Tuple<Keys, Keys>>();
            }

            public string Name { get; }

            public void AddKeys(Keys left, Keys right) 
            {
                _keys.Add(new Tuple<Keys, Keys>(left, right));
            }

            public float GetAxis()
            {
                float value = 0;

                for (int i = 0; i < _keys.Count; i++)
                {
                    var left = _keys[i].Item1;
                    var right = _keys[i].Item2;

                    if (inputState.KeyState.IsKeyDown(left))
                        value += -1f;

                    if (inputState.KeyState.IsKeyDown(right))
                        value += 1f;
                }

                return value;
            }
        }

        private readonly InputState _state = new InputState();
        private readonly Dictionary<string, Action> _actions = new Dictionary<string, Action>();

        public override void Tick(GameTime gameTime)
        {
            _state.PreviousKeyState = _state.KeyState;
            _state.PreviousGamePadState = _state.GamePadState;

            _state.KeyState = Keyboard.GetState();
            _state.GamePadState = GamePad.GetState(0);
        }

        public PressedAction NewPressedAction(string name)
        {
            var action = new PressedAction(name, _state);

            _actions.Add(name, action);

            return action;
        }

        public AxisAction NewAxisAction(string name)
        {
            var action = new AxisAction(name, _state);

            _actions.Add(name, action);

            return action;
        }

        public T GetAction<T>(string name) where T : Action
        {
            return _actions[name] as T;
        }
    }
}
