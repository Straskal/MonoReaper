using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Core
{
    public class PressedAction : InputAction
    {
        private readonly List<Keys> _keys;

        public PressedAction(InputState state) : base(state)
        {
            _keys = new List<Keys>();
        }

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
}
