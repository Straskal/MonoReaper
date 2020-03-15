using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Reaper.Engine.Builtins
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

        public MapData Data { get; }

        public TilemapBehavior(WorldObject owner, MapData data) : base(owner)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public override void Load(ContentManager contentManager)
        {
            var tileDefinition = new WorldObjectDefinition(32, 32);
            tileDefinition.MakeSolid();

            Data.Texture = contentManager.Load<Texture2D>(Data.TilesetFilePath);

            var tiles = Data.Tiles;

            int cellSize = Data.CellSize;
            int cellsX = Data.CellsX;

            int currentX = 0;
            int currentY = 0;

            for (int j = 0; j < tiles.Length; j++)
            {
                if (currentX == (cellsX))
                {
                    currentX = 0;
                    currentY++;
                }

                int cx = currentX++;

                if (tiles[j] == -1)
                {
                    continue;
                }

                Owner.Layout.Spawn(tileDefinition, new Vector2(cx * cellSize, currentY * cellSize));
            }
        }

        public override void Draw(LayoutView view)
        {
            var texture = Data.Texture;
            var tiles = Data.Tiles;

            int cellSize = Data.CellSize;
            int cellsX = Data.CellsX;

            int currentX = 0;
            int currentY = 0;

            for (int j = 0; j < tiles.Length; j++)
            {
                if (currentX == (cellsX))
                {
                    currentX = 0;
                    currentY++;
                }

                int cx = currentX++;

                if (tiles[j] == -1)
                {
                    continue;
                }

                Rectangle dest = new Rectangle(cx * cellSize, currentY * cellSize, cellSize, cellSize);
                Rectangle source = new Rectangle(0, 0, 32, 32);

                view.Draw(texture, source, dest, Color.White, false);
            }
        }
    }
}
