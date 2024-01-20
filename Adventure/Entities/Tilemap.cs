using Adventure.Components;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Adventure.Constants;

namespace Adventure.Entities
{
    public class Tilemap : Entity
    {
        public struct TileInfo
        {
            public Rectangle Source;
            public Vector2 Position;
        }

        public class MapData
        {
            public int CellSize { get; set; }
            public int CellsX { get; set; }
            public int CellsY { get; set; }
            public TileInfo[] Tiles { get; set; }
            public string TilesetFilePath { get; set; }
            public Texture2D Texture { get; set; }
            public bool IsSolid { get; set; }
        }

        public Tilemap(MapData data)
        {
            Data = data;
        }

        public MapData Data { get; }

        public override void Spawn()
        {
            Data.Texture = Adventure.Instance.Content.Load<Texture2D>(Data.TilesetFilePath);
            Collider = new TilemapCollider(this, 0, 0, Data);
            Collider.Layer = EntityLayers.Solid;
            Collider.Enable();
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            foreach (var tile in Data.Tiles)
            {
                renderer.Draw(Data.Texture, tile.Position, tile.Source, Color.White, SpriteEffects.None);
            }
        }
    }
}
