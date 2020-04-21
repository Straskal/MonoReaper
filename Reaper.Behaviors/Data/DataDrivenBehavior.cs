using Reaper.Engine;

namespace Reaper
{
    /// <summary>
    /// Custom generic behavior that specifies it's data type in order to load from definition files.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataDrivenBehavior<T> : Behavior
    {
        public DataDrivenBehavior(WorldObject owner, T data) : base(owner)
        {
            Data = data;
        }

        protected T Data { get; }
    }
}
