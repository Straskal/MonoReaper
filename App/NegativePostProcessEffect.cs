using Engine;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure
{
    public class NegativePostProcessEffect : PostProcessingEffect
    {
        private readonly Effect _effect;

        public NegativePostProcessEffect(Effect effect) : base(App.Graphics, Resolution.RenderTargetWidth, Resolution.RenderTargetHeight)
        {
            _effect = effect;
        }

        public override void OnDraw(Texture2D currentTarget, Matrix transformation)
        {
            Renderer.Draw(currentTarget, Vector2.Zero, Color.Red, _effect);
        }
    }
}
