using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ItsGood.Builtins
{
    public class TimerBehavior : Behavior
    {
        private struct Instance 
        {
            public float Time;
            public Action Callback;
        }

        private readonly List<Instance> _timers = new List<Instance>();

        private float _currentTime;

        public void StartTimer(float seconds, Action onTimer) 
        {
            _timers.Add(new Instance { Time = _currentTime + seconds, Callback = onTimer });
        }

        public override void Tick(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _currentTime += elapsedTime;

            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                var timer = _timers[i];

                if (_currentTime > timer.Time) 
                {
                    timer.Callback?.Invoke();
                    _timers.RemoveAt(i);
                }
            }
        }
    }
}
