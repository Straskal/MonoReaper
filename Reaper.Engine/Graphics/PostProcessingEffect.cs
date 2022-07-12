using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Core.Graphics
{
    public class PostProcessingEffect : IDisposable
    {
        private bool disposed;

        public RenderTarget2D Target { get; }

        public PostProcessingEffect(GraphicsDevice graphicsDevice, int width, int height) 
        {
            Target = new RenderTarget2D(graphicsDevice, width, height);
        }

        public virtual void OnTick(GameTime gameTime) 
        {
        }

        public virtual void OnDraw(Level level) 
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Target.Dispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
