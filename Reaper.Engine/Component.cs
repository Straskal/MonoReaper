using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Core
{
    public abstract class Component
    {
        public Entity Entity { get; internal set; }
        public Level Level => Entity.Level;

        public int ZOrder { get; set; }
        public bool IsTickEnabled { get; set; } = false;
        public bool IsDrawEnabled { get; set; } = false;

        public virtual void OnLoad(ContentManager content) { }
        public virtual void OnSpawn() { }
        public virtual void OnDestroy() { }
        public virtual void OnStart() { }
        public virtual void OnEnd() { }
        public virtual void OnTick(GameTime gameTime) { }
        public virtual void OnPostTick(GameTime gameTime) { }
        public virtual void OnDraw() { }
        public virtual void OnDebugDraw() { }
    }
}
