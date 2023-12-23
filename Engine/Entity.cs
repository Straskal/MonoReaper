using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Engine
{
    public class Entity
    {
        internal List<Component> Components
        {
            get;
        } = new List<Component>();

        public Level Level
        {
            get;
            internal set;
        }

        public Origin Origin
        {
            get;
            init;
        } = Origin.Center;

        public Vector2 Position
        {
            get;
            set;
        }

        public bool IsDestroyed
        {
            get;
            internal set;
        }

        public void AddComponent(Component component)
        {
            Components.Add(component);
            Level?.AddComponent(this, component);
        }

        public void RemoveComponent(Component component)
        {
            Components.Remove(component);
            Level?.RemoveComponent(component);
        }

        public T GetComponent<T>() where T : class
        {
            var result = default(T);

            foreach (var component in Components)
            {
                if (component is T t)
                {
                    result = t;
                    break;
                }
            }

            return result;
        }

        public void Spawn(Entity entity, Vector2 position)
        {
            Level.Spawn(entity, position);
        }

        public void Destroy(Entity entity)
        {
            Level.Destroy(entity);
        }

        public void DestroySelf()
        {
            Level.Destroy(this);
        }

        public virtual void OnLoad(ContentManager content)
        {
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
    }
}
