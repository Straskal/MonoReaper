using Engine;

namespace Adventure.Entities
{
    public abstract class Trigger : Entity
    {
        public abstract void OnTouch(Entity entity);
    }
}
