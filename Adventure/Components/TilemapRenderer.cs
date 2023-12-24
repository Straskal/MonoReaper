using Engine;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Adventure.Components.Tilemap;

namespace Adventure.Components
{
    public sealed class TilemapRenderer : GraphicsComponent
    {
        public TilemapRenderer(MapData data) 
        {
            Data = data;
        }

        public MapData Data { get; }

        public override void OnDraw(Renderer renderer, GameTime gameTime)
        {
            foreach (var tile in Data.Tiles)
            {
                renderer.Draw(Data.Texture, tile.Position, tile.Source, Color.White, SpriteEffects.None);
            }
        }
    }
}
