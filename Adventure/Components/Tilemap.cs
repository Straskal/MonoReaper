using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using static Adventure.Constants;
using Engine;
using Engine.Graphics;
using Engine.Collision;

namespace Adventure.Components
{
    /// <summary>
    /// The tilemap behavior draws the given data.
    /// If this behavior's owner is solid, then it will create an invisible solid for each tile.
    ///// 
    /// POSSIBLE IMPROVEMENTS:
    /// 
    /// The tilemap does not handle movement at the moment. If we do need to support the movement of solid tilemaps, then
    /// well have to move every single collider object as well.
    /// 
    /// If tilemaps need to be destroyed, then all collider objects will have to be destroyed as well.
    /// 
    /// If a tilemap is disabled, it will have to disable all colliders as well.
    /// 
    /// Parallaxing support will have to be added to tilemaps, or just to world objects in general.
    /// 
    /// The invisible solid objects could definitely be improved. We could just create larger solid areas instead of smaller individual ones.
    /// This would involve doing some calculations to find out which tiles are neighbors.
    /// 
    /// It's kind of annoying that all of the tiles are world objects.
    /// 
    /// </summary>
    public class Tilemap : Component
    {
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

            IsDrawEnabled = true;
        }

        public MapData Data { get; }

        public override void OnLoad(ContentManager content)
        {
            Data.Texture = content.Load<Texture2D>(Data.TilesetFilePath);
        }

        public override void OnSpawn()
        {
            if (!Data.IsSolid)
                return;

            foreach (var tile in Data.Tiles)
            {
                Entity.AddComponent(new Box(tile.Position.X, tile.Position.Y, Data.CellSize, Data.CellSize, EntityLayers.Solid));
            }
        }

        public override void OnDraw()
        {
            foreach (var tile in Data.Tiles)
            {
                Renderer.Draw(Data.Texture, tile.Position, tile.Source, Color.White, SpriteEffects.None);
            }
        }

        public struct TileInfo
        {
            public Rectangle Source;
            public Vector2 Position;
        }

        //private IEnumerable<TileInfo> GetTileInfos()
        //{
        //    int numHorizontalCells = Data.Texture.Width / Data.CellSize;
        //    float currentX = Entity.Position.X;
        //    float currentY = Entity.Position.Y;

        //    for (int j = 0; j < Data.Tiles.Length; j++)
        //    {
        //        if (currentX == Data.CellsX)
        //        {
        //            currentX = 0;
        //            currentY++;
        //        }

        //        float cx = currentX++;

        //        if (Data.Tiles[j] == -1)
        //            continue;

        //        int row = (int)Math.Floor((double)(Data.Tiles[j] / numHorizontalCells));
        //        int col = (int)Math.Floor((double)(Data.Tiles[j] % numHorizontalCells));

        //        yield return new TileInfo
        //        {
        //            Source = new Rectangle(col * Data.CellSize, row * Data.CellSize, Data.CellSize, Data.CellSize),
        //            Position = new Vector2(cx * Data.CellSize, currentY * Data.CellSize)
        //        };
        //    }
        //}
    }
}
