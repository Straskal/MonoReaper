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

        public sealed class TileBoxes
        {
            private readonly Partition _partition;

            public TileBoxes(Partition partition)
            {
                _partition = partition;
            }

            public void Add(Box box)
            {
                _partition.Add(box);
            }

            public void Remove(Box box)
            {
                _partition.Remove(box);
            }
        }

        public Tilemap(MapData data)
        {
            Data = data;
        }

        private TileBoxes Boxes 
        {
            get;
            set;
        }

        public MapData Data 
        { 
            get; 
        }

        protected override void OnLoad(ContentManager content)
        {
            Data.Texture = content.Load<Texture2D>(Data.TilesetFilePath);
            GraphicsComponent = new TilemapRenderer(Data);
            Boxes = new TileBoxes(Level.Partition);
        }

        protected override void OnSpawn()
        {
            if (!Data.IsSolid)
            {
                return;
            }

            foreach (var tile in Data.Tiles)
            {
                Boxes.Add(new Box(this, tile.Position.X, tile.Position.Y, Data.CellSize, Data.CellSize, EntityLayers.Solid));
            }
        }
    }
}
