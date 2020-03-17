﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Reaper.Engine.Behaviors
{
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

            if (!Owner.IsSolid)
                return;

            var tileDefinition = new WorldObjectDefinition()
                .SetSize(Data.CellSize, Data.CellSize)
                .MakeSolid();

            foreach (var tile in GetTileDestinations()) 
            {
                _colliders.Add(Owner.Layout.Spawn(tileDefinition, new Vector2(tile.Destination.X, tile.Destination.Y)));
            }
        }

        public override void Draw(LayoutView view)
        {
            foreach (var tile in GetTileDestinations())
            {
                view.Draw(Data.Texture, tile.Source, tile.Destination, Color.White, false);
            }
        }

        private struct TileInfo 
        {
            public Rectangle Source;
            public Rectangle Destination;
        }

        private IEnumerable<TileInfo> GetTileDestinations()
        {
            int currentX = 0;
            int currentY = 0;

            for (int j = 0; j < Data.Tiles.Length; j++)
            {
                if (currentX == Data.CellsX)
                {
                    currentX = 0;
                    currentY++;
                }

                int cx = currentX++;

                if (Data.Tiles[j] == -1)
                    continue;

                int numHorizontalCells = Data.Texture.Width / Data.CellSize;
                int row = (int)Math.Floor((double)(Data.Tiles[j] / numHorizontalCells));
                int col = (int)Math.Floor((double)(Data.Tiles[j] % numHorizontalCells));

                yield return new TileInfo
                {
                    Source = new Rectangle(col * Data.CellSize, row * Data.CellSize, Data.CellSize, Data.CellSize),
                    Destination = new Rectangle(cx * Data.CellSize, currentY * Data.CellSize, Data.CellSize, Data.CellSize)
                };
            }
        }
    }
}