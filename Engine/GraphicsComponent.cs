using Microsoft.Xna.Framework;

namespace Engine
{
    public abstract class GraphicsComponent
    {
        public GraphicsComponent(Entity owner) 
        {
            Entity = owner;
        }

        public Entity Entity { get; }

        private int drawOrder;
        public int DrawOrder 
        {
            get => drawOrder;
            set 
            {
                drawOrder = value;
            }
        }

        public virtual void PostUpdate(GameTime gameTime)
        {
        }

        public virtual void Draw(Renderer renderer, GameTime gameTime)
        {
        }

        public virtual void DebugDraw(Renderer renderer, GameTime gameTime)
        {
        }
    }
}
