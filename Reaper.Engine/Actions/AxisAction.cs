using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Reaper.Engine
{ 
    public class AxisAction : InputAction
    {
        private readonly List<Tuple<Keys, Keys>> _keys;

        public AxisAction(InputState state) : base(state)
        {
            _keys = new List<Tuple<Keys, Keys>>();
        }

        public void AddKeys(Keys x, Keys y)
        {
            _keys.Add(new Tuple<Keys, Keys>(x, y));
        }

        public float GetAxis()
        {
            float value = 0;

            for (int i = 0; i < _keys.Count; i++)
            {
                var x = _keys[i].Item1;
                var y = _keys[i].Item2;

                if (inputState.KeyState.IsKeyDown(x))
                    value += -1f;

                if (inputState.KeyState.IsKeyDown(y))
                    value += 1f;
            }

            return MathHelper.Clamp(value, -1f, 1f);
        }
    }
}
