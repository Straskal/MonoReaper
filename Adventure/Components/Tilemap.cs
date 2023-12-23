using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine;
using Engine.Collision;

using static Adventure.Constants;

namespace Adventure.Components
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
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public MapData Data { get; }

        public override void OnLoad(ContentManager content)
        {
            Data.Texture = content.Load<Texture2D>(Data.TilesetFilePath);
            AddComponent(new TilemapRenderer(Data));
        }

        public override void OnSpawn()
        {
            if (!Data.IsSolid)
            {
                return;
            }

            foreach (var tile in Data.Tiles)
            {
                AddComponent(new Box(tile.Position.X, tile.Position.Y, Data.CellSize, Data.CellSize, EntityLayers.Solid));
            }
        }
    }
}
