using Core;
using Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Reaper
{
    public class NegativePostProcessEffect : PostProcessEffect
    {
        private readonly Effect _effect;

        public NegativePostProcessEffect(Effect effect) 
        {
            _effect = effect;
        }

        public override void Draw(Level level)
        {
            Renderer.Draw(level.RenderTexture, Vector2.Zero, Color.Red, _effect);
        }
    }
}
