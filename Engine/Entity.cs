using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Engine
{
    public class Entity
    {
        public Level Level { get; internal set; }
        public Origin Origin { get; set; } = Origin.Center;
        public Vector2 Position { get; set; }
        public Collider Collider { get; protected set; }
        public GraphicsComponent GraphicsComponent { get; protected set; }
        public bool IsDestroyed { get; internal set; }

        internal void Load(ContentManager content)
        {
            OnLoad(content);
        }

        internal void Spawn()
        {
            OnSpawn();
            Collider?.Register();
        }

        internal void Destroy()
        {
            OnDestroy();
            Collider?.Unregister();
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
            Collider?.DebugDraw(renderer, gameTime);
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

        protected void Collide(Vector2 velocity, int layerMask, int ignoreMask)
        {
            Collider?.Collide(velocity, layerMask, ignoreMask);
        }

        protected void Collide(ref Vector2 velocity, int layerMask, int ignoreMask)
        {
            Collider?.Collide(ref velocity, layerMask, ignoreMask);
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
