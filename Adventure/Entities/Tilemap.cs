using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
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

        public MapData Data { get; set; }
        public override EntityType Type => EntityType.Tilemap;

        private List<Collider> colliders = new();

        public Tilemap() 
        {
            IsNetEntity = false;
        }

        public override void Spawn()
        {
            Data.Texture = Adventure.Instance.Content.Load<Texture2D>(Data.TilesetFilePath);

            foreach (var tile in Data.Tiles)
            {
                colliders.Add(new Collider(this, new BoxCollisionShape(Data.CellSize, Data.CellSize))
                {
                    Layer = EntityLayers.Solid,
                    Offset = new Vector2(tile.Position.X + Data.CellSize * 0.5f, tile.Position.Y + Data.CellSize * 0.5f)
                });
            }

            foreach (var collider in colliders)
            {
                collider.Enable();
            }
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            foreach (var tile in Data.Tiles)
            {
                renderer.Draw(Data.Texture, tile.Position, tile.Source, Color.White);
            }
        }
    }
}
