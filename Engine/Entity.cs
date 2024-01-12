using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine
{
    public class Entity
    {
        private readonly HashSet<string> tags = new();

        public EntityManager Others { get; internal set; }
        public Origin Origin { get; set; } = Origin.Center;
        public Vector2 Position { get; set; }
        public Collider Collider { get; protected set; }
        public GraphicsComponent GraphicsComponent { get; protected set; }
        public bool IsDestroyed { get; internal set; }

        public void AddTag(string tag)
        {
            tags.Add(tag);
        }

        public void RemoveTag(string tag)
        {
            tags.Remove(tag);
        }

        public void HasTag(string tag)
        {
            tags.Contains(tag);
        }

        protected void Collide(Vector2 velocity, int layerMask)
        {
            Collider?.Collide(velocity, layerMask);
        }

        protected void Collide(ref Vector2 velocity, int layerMask)
        {
            Collider?.Collide(ref velocity, layerMask);
        }

        public virtual void Spawn()
        {
        }

        public virtual void Destroy()
        {
            Collider?.Disable();
        }

        public virtual void Start()
        {
        }

        public virtual void End()
        {
            Collider?.Disable();
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void PostUpdate(GameTime gameTime)
        {
            GraphicsComponent?.OnPostUpdate(gameTime);
            Collider?.ClearContacts();
        }

        public virtual void Draw(Renderer renderer, GameTime gameTime)
        {
            GraphicsComponent?.OnDraw(renderer, gameTime);
        }

        public virtual void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            Collider?.Draw(renderer, gameTime);
            GraphicsComponent?.OnDebugDraw(renderer, gameTime);
        }

        public virtual void OnCollision(Entity other, Collision collision)
        {
        }
    }
}
