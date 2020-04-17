using System;
using System.Collections.Generic;

namespace Reaper.Engine
{
    public sealed class TimerList
    {
        private readonly MainGame _game;
        private readonly List<Timer> _timers;

        public TimerList(MainGame game) 
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _timers = new List<Timer>();
        }

        public void Start(string name, float time, Action timerCallback)
        {
            _timers.Add(new Timer { Name = name, Time = _game.TotalTime + time, TimerCallback = timerCallback });
        }

        internal void Tick()
        {
            _timers.RemoveAll(timer =>
            {
                if (_game.TotalTime > timer.Time)
                {
                    timer.TimerCallback?.Invoke();
                    return true;
                }
                return false;
            });
        }
    }
}
