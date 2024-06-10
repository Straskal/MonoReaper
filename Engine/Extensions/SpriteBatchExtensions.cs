using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public static class SpriteBatchExtensions
    {
        public static void BeginWithDefaultValues(this SpriteBatch spriteBatch, Effect effect = null, Matrix? transformMatrix = null)
        {
            spriteBatch.Begin(
                 sortMode: SpriteSortMode.Deferred,
                 blendState: BlendState.AlphaBlend,
                 samplerState: SamplerState.PointClamp,
                 depthStencilState: DepthStencilState.Default,
                 rasterizerState: RasterizerState.CullCounterClockwise,
                 effect: effect,
                 transformMatrix: transformMatrix);
        }
    }
}
