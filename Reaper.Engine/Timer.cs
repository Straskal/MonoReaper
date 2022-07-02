using System;
using Microsoft.Xna.Framework;

namespace Reaper.Engine
{
    public class Timer
    {
        public bool Started { get; private set; }
        public float CurrentTime { get; private set; }
        public float Time { get; private set; }
        public Action Callback { get; set; }

        public Timer(float time, Action callback) 
        {
            Time = time;
            Callback = callback;
        }

        public void Start() 
        {
            Started = true;
        }

        public void Tick(GameTime gameTime) 
        {
            if (Started) 
            {
                CurrentTime += gameTime.GetDeltaTime();

                if (CurrentTime >= Time)
                {
                    Callback?.Invoke();
                    Started = false;
                }
            }  
        }
    }
}
