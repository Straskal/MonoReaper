using Core;
using Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Reaper
{
    public class NegativePostProcessEffect : PostProcessingEffect
    {
        private readonly Effect _effect;

        public NegativePostProcessEffect(Effect effect) : base(App.Graphics, App.ViewportWidth, App.ViewportHeight)
        {
            _effect = effect;
        }

        public override void OnDraw(Level level)
        {
            Renderer.Draw(level.RenderTarget, Vector2.Zero, Color.Red, _effect);
        }
    }
}
