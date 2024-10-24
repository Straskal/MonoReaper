using Engine;

namespace Adventure.Core
{
    public abstract class Trigger : Actor
    {
        public abstract void OnTouch(Actor entity);
    }
}
