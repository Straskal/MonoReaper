using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Reaper.Engine;
using System;
using System.Collections.Generic;

namespace Reaper.Behaviors.Common
{
    /// <summary>
    /// The tilemap behavior draws the given data.
    /// If this behavior's owner is solid, then it will create an invisible solid for each tile.
    /// 
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
    /// </summary>
    public class TilemapBehavior : Behavior
    {
        public class MapData
        {
            public int CellSize { get; set; }
            public int CellsX { get; set; }
            public int CellsY { get; set; }
            public int[] Tiles { get; set; }
            public string TilesetFilePath { get; set; }
            public Texture2D Texture { get; set; }
        }

        private readonly List<WorldObject> _colliders;

        public TilemapBehavior(WorldObject owner, MapData data) : base(owner)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));

            _colliders = new List<WorldObject>();
        }

        public MapData Data { get; }

        public override void Load(ContentManager contentManager)
        {
            Data.Texture = contentManager.Load<Texture2D>(Data.TilesetFilePath);
        }

        public override void OnLayoutStarted()
        {
            if (!Owner.IsSolid)
                return;

            var tileDefinition = new WorldObjectDefinition();
            tileDefinition.SetSize(Data.CellSize, Data.CellSize);
            tileDefinition.MakeSolid();

            foreach (var tile in GetTileInfo())
            {
                _colliders.Add(Owner.Layout.Spawn(tileDefinition, new Vector2(tile.Position.X, tile.Position.Y)));
            }
        }

        public override void Draw(Renderer renderer)
        {
            foreach (var tile in GetTileInfo())
            {
                renderer.Draw(Data.Texture, tile.Source, tile.Position, Color.White, false);
            }
        }

        private struct TileInfo 
        {
            public Rectangle Source;
            public Vector2 Position;
        }

        private IEnumerable<TileInfo> GetTileInfo()
        {
            int numHorizontalCells = Data.Texture.Width / Data.CellSize;
            float currentX = Owner.Position.X;
            float currentY = Owner.Position.Y;

            for (int j = 0; j < Data.Tiles.Length; j++)
            {
                if (currentX == Data.CellsX)
                {
                    currentX = 0;
                    currentY++;
                }

                float cx = currentX++;

                if (Data.Tiles[j] == -1)
                    continue;

                int row = (int)Math.Floor((double)(Data.Tiles[j] / numHorizontalCells));
                int col = (int)Math.Floor((double)(Data.Tiles[j] % numHorizontalCells));

                yield return new TileInfo
                {
                    Source = new Rectangle(col * Data.CellSize, row * Data.CellSize, Data.CellSize, Data.CellSize),
                    Position = new Vector2(cx * Data.CellSize, currentY * Data.CellSize)
                };
            }
        }
    }
}
