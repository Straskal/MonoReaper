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

        public Box Box
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
            Box = new Box(this);
            OnLoad(content);
        }

        internal void Spawn()
        {
            OnSpawn();
            Level.Partition.Add(Box);
        }

        internal void Destroy()
        {
            OnDestroy();
            Level.Partition.Remove(Box);
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
            Box.OnDebugDraw(renderer, gameTime);
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
            Level.Partition.Update(Box);
        }

        public void Move(Vector2 direction)
        {
            Position += direction;
            UpdateBBox();
        }

        public void MoveTo(Vector2 position)
        {
            Position = Origin.Invert(position.X, position.Y, Box.Width, Box.Height).Position;
            UpdateBBox();
        }

        protected void MoveAndCollide(ref Vector2 velocity, int layerMask, CollisionResponseCallback response)
        {
            if (velocity == Vector2.Zero)
            {
                return;
            }

            var visited = new HashSet<Box>() { Box };
            var potentialCollisions = new List<Box>();

            while (true)
            {
                potentialCollisions.Clear();

                var bounds = Box.CalculateBounds();
                var broadphaseRectangle = bounds.Union(velocity);

                foreach (var box in Level.Partition.Query(broadphaseRectangle))
                {
                    if ((box.LayerMask | layerMask) != layerMask)
                    {
                        continue;
                    }

                    if (visited.Contains(box))
                    {
                        continue;
                    }

                    if (!broadphaseRectangle.Intersects(box.CalculateBounds()))
                    {
                        continue;
                    }

                    potentialCollisions.Add(box);
                }

                if (!Sweep.Test(bounds, velocity, potentialCollisions, out var collision))
                {
                    Move(velocity);
                    break;
                }

                visited.Add(collision.Box);
                MoveTo(collision.Position);
                velocity = response.Invoke(collision);
                collision.Box.NotifyCollidedWith(Box, collision);
            }
        }
    }
}
