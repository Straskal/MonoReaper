namespace Adventure
{
    public class Timestep
    {
        /// <summary>
        /// Game runs at a fixed 60 frames per second.
        /// </summary>
        public const float FixedDelta = 1f / 60f;

        /// <summary>
        /// Wrap a tick count at 1024.
        /// </summary>
        public const int MaxTicks = 1024;
        public const int HalfMaxTicks = MaxTicks / 2;

        /// <summary>
        /// Get the difference between two ticks.
        /// </summary>
        /// <param name="tickA"></param>
        /// <param name="tickB"></param>
        /// <returns></returns>
        public static int TickDiff(int tickA, int tickB)
        {
            return (tickA - tickB + HalfMaxTicks * 3) % (HalfMaxTicks * 2) - HalfMaxTicks;
        }

        public static bool IsLatestTick(int current, int previous)
        {
            return TickDiff(current, previous) > 0;
        }

        /// <summary>
        /// Get the elapsed time between two ticks.
        /// </summary>
        /// <param name="tickA"></param>
        /// <param name="tickB"></param>
        /// <returns></returns>
        public static float GetElapsedTime(int tickA, int tickB)
        {
            return TickDiff(tickA, tickB) * FixedDelta;
        }

        /// <summary>
        /// Increment a tick count and wrap when exceeds MaxTicks.
        /// </summary>
        /// <param name="tick"></param>
        public static void IncrementTick(ref int tick)
        {
            tick = (tick + 1) % MaxTicks;
        }

        /// <summary>
        /// Increment a tick count and wrap when exceeds MaxTicks.
        /// </summary>
        /// <param name="tick"></param>
        public static int IncrementTick(int tick)
        {
            return (tick + 1) % MaxTicks;
        }
    }
}
