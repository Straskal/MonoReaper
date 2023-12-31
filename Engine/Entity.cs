using Engine.Collision;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

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
        } = GraphicsComponent.Empty;

        public bool IsDestroyed
        {
            get;
            internal set;
        }

        internal void Load(ContentManager content)
        {
            Collider = new Box(this);
            OnLoad(content);
        }

        internal void Spawn()
        {
            OnSpawn();
            Level.Partition.Add(Collider);
        }

        internal void Destroy()
        {
            OnDestroy();
            Level.Partition.Remove(Collider);
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
            GraphicsComponent.OnPostUpdate(gameTime);
        }

        internal void Draw(Renderer renderer, GameTime gameTime)
        {
            GraphicsComponent.OnDraw(renderer, gameTime);
        }

        internal void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            Collider.OnDebugDraw(renderer, gameTime);
            GraphicsComponent.OnDebugDraw(renderer, gameTime);
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

        public void UpdateBBox()
        {
            Level.Partition.Update(Collider);
        }

        public void Move(Vector2 direction)
        {
            Position += direction;
            UpdateBBox();
        }

        public void MoveTo(Vector2 position)
        {
            Position = position; //Origin.Invert(position.X, position.Y, Collider.Bounds.Width, Collider.Bounds.Height).Position;
            UpdateBBox();
        }

        protected void MoveAndCollide(ref Vector2 velocity, int layerMask, CollisionResponseCallback response)
        {
            if (velocity == Vector2.Zero) return;
            while (true)
            {
                var path = new IntersectionPath(Position, velocity);
                var broadphaseRectangle = Collider.Bounds.Union(velocity);
                var collision = Collision.Collision.Empty;
                foreach (var collider in Level.Partition.Query(broadphaseRectangle))
                {
                    if (!((collider.LayerMask | layerMask) == layerMask)) continue;
                    if (!broadphaseRectangle.Intersects(collider.Bounds)) continue;
                    if (!Collider.Intersect(collider, path, out var time, out var contact, out var normal)) continue;
                    if (!(time < collision.Time)) continue;
                    collision = new Collision.Collision(collider, velocity, normal, time, contact);
                }
                if (collision.Time == 1f) 
                {
                    Move(velocity);
                    break;
                }
                MoveTo(collision.Position + Sweep.Correction * collision.Normal);
                velocity = response.Invoke(collision);
                collision.Collider.NotifyCollidedWith(Collider, collision);
                if (IsDestroyed) break;
            }
        }
    }
}
