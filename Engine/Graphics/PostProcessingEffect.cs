using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{
    public class PostProcessingEffect : Effect
    {
        protected PostProcessingEffect(Effect cloneSource) : base(cloneSource)
        {
        }

        public virtual void OnUpdate(GameTime gameTime)
        {
        }
    }
}
