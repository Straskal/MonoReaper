﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Graphics
{
    public class PostProcessingEffect : IDisposable
    {
        private bool disposed;

        public RenderTarget2D Target { get; }

        public PostProcessingEffect(GraphicsDevice graphicsDevice, int width, int height)
        {
            Target = new RenderTarget2D(graphicsDevice, width, height);
        }

        public virtual void OnUpdate(GameTime gameTime) { }
        public virtual void OnDraw(Texture2D currentTarget, Matrix transformation) { }

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
