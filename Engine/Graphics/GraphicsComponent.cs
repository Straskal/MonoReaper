using Microsoft.Xna.Framework;

namespace Engine.Graphics
{
    public abstract class GraphicsComponent
    {
        private sealed class EmptyGraphicsComponent : GraphicsComponent
        {
            public override void OnDebugDraw(Renderer renderer, GameTime gameTime)
            {
            }

            public override void OnDraw(Renderer renderer, GameTime gameTime)
            {
            }
        }

        public static GraphicsComponent Empty { get; } = new EmptyGraphicsComponent();

        public int DrawOrder 
        {
            get;
            set;
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
