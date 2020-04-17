using System;

namespace Reaper.Engine
{
    public struct Timer
    {
        public string Name;
        public float Time;
        public Action TimerCallback;
    }
}
