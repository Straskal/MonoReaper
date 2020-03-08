using Microsoft.Xna.Framework;

namespace ItsGood
{
    public abstract class Behavior
    {
        protected Behavior() { }

        public WorldObject Owner { get; internal set; }

        public virtual void Initialize() { }
        public virtual void OnOwnerCreated() { }
        public virtual void Tick(GameTime gameTime) { }
        public virtual void OnOwnerDestroyed() { }
    }

    public abstract class Behavior<T> : Behavior
    {
        public T State { get; internal set; }
    }
}
