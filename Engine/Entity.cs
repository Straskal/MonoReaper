using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Engine
{
    public class Entity
    {
        public Level Level
        {
            get;
            internal set;
        }

        public Origin Origin
        {
            get;
            set;
        } = Origin.Center;

        public Vector2 Position
        {
            get;
            set;
        }

        public Collider Collider
        {
            get;
            protected set;
        }

        public GraphicsComponent GraphicsComponent
        {
            get;
            protected set;
        }

        public bool IsDestroyed
        {
            get;
            internal set;
        }

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

        protected void MoveAndCollide(ref Vector2 velocity, int layerMask, CollisionCallback response)
        {
            Collider?.MoveAndCollide(ref velocity, layerMask, response);
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
    }
}
