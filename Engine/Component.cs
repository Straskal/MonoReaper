using Microsoft.Xna.Framework;
using Engine.Graphics;

namespace Engine
{
    /// <summary>
    /// This is the base class for components.
    /// </summary>
    public abstract class Component
    {
        public Entity Entity
        {
            get;
            internal set;
        }

        public Level Level
        {
            get => Entity.Level;
        }

        public int ZOrder
        {
            get;
            set;
        }

        public virtual void OnSpawn()
        {
        }

        public virtual void OnDestroy()
        {
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnEnd()
        {
        }

        public virtual void OnUpdate(GameTime gameTime)
        {
        }

        public virtual void OnPostUpdate(GameTime gameTime)
        {
        }

        public virtual void OnDraw(Renderer renderer, GameTime gameTime)
        {
        }

        public virtual void OnDebugDraw(Renderer renderer, GameTime gameTime)
        {
        }
    }
}
