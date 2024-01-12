using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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

        internal void Load(ContentManager content)
        {
            OnLoad(content);
        }

        internal void Spawn()
        {
            OnSpawn();
        }

        internal void Destroy()
        {
            OnDestroy();
            Collider?.Disable();
        }

        internal void Start()
        {
            OnStart();
        }

        internal void End()
        {
            OnEnd();
        }

        internal void Update(GameTime gameTime)
        {
            OnUpdate(gameTime);
        }

        internal void PostUpdate(GameTime gameTime)
        {
            OnPostUpdate(gameTime);
            GraphicsComponent?.OnPostUpdate(gameTime);
            Collider?.ClearContacts();
        }

        internal void Draw(Renderer renderer, GameTime gameTime)
        {
            GraphicsComponent?.OnDraw(renderer, gameTime);
        }

        internal void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            Collider?.Draw(renderer, gameTime);
            GraphicsComponent?.OnDebugDraw(renderer, gameTime);
        }

        internal void Collision(Entity other, Collision collision)
        {
            OnCollision(other, collision);
        }

        protected void Collide(Vector2 velocity, int layerMask)
        {
            Collider?.Collide(velocity, layerMask);
        }

        protected void Collide(ref Vector2 velocity, int layerMask)
        {
            Collider?.Collide(ref velocity, layerMask);
        }

        protected virtual void OnLoad(ContentManager content)
        {
        }

        protected virtual void OnSpawn()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnEnd()
        {
        }

        protected virtual void OnUpdate(GameTime gameTime)
        {
        }

        protected virtual void OnPostUpdate(GameTime gameTime)
        {
        }

        protected virtual void OnCollision(Entity other, Collision collision)
        {
        }
    }
}
