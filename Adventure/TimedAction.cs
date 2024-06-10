using Microsoft.Xna.Framework;
using System;

namespace Adventure
{
    public sealed class TimedAction
    {
        public TimedAction(float timeMilliseconds, Action action) 
        {
            TimeMilliseconds = timeMilliseconds;
            Action = action;
        }

        public float TimeMilliseconds { get; }
        public Action Action { get; }
        public float AccumulatorMilliseconds { get; private set; }

        public void Update(GameTime gameTime) 
        {
            AccumulatorMilliseconds += gameTime.ElapsedGameTime.Milliseconds;

            if (AccumulatorMilliseconds >= TimeMilliseconds) 
            {
                AccumulatorMilliseconds = 0f;
                Action.Invoke();
            }
        }
    }
}
