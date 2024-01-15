using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine
{
    public class Entity
    {
        public World World { get; internal set; }
        public HashSet<string> Tags { get; }
        public Origin Origin { get; set; } = Origin.Center;
        public Vector2 Position { get; set; }
        public Collider Collider { get; protected set; }
        public GraphicsComponent GraphicsComponent { get; protected set; }
        public bool IsDestroyed { get; internal set; }

        public int DrawOrder 
        {
            get 
            {
                if (GraphicsComponent != null) 
                {
                    return GraphicsComponent.DrawOrder;
                }
                return 0;
            }
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

        public bool Overlaps(RectangleF rectangle) 
        {
            if (Collider != null) 
            {
                return Collider.Overlaps(rectangle);
            }
            return false;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void PostUpdate(GameTime gameTime)
        {
            GraphicsComponent?.PostUpdate(gameTime);
        }

        public virtual void Draw(Renderer renderer, GameTime gameTime)
        {
            GraphicsComponent?.Draw(renderer, gameTime);
        }

        public virtual void DebugDraw(Renderer renderer, GameTime gameTime)
        {
            Collider?.Draw(renderer, gameTime);
            GraphicsComponent?.DebugDraw(renderer, gameTime);
        }

        public virtual void OnCollision(Entity other, Collision collision)
        {
        }
    }
}
