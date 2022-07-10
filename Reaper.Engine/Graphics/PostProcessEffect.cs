using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Graphics
{
    public class PostProcessEffect
    {
        public RenderTarget2D Target { get; }

        public PostProcessEffect() 
        {
            Target = new RenderTarget2D(App.Graphics, App.ViewportWidth, App.ViewportWidth);
        }

        public virtual void OnTick(GameTime gameTime) 
        {
        }

        public virtual void Draw(Level level) 
        {
            Renderer.Draw(level.RenderTexture, Vector2.Zero, Color.Red);
        }
    }
}
