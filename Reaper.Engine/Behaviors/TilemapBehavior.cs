using Microsoft.Xna.Framework;
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

            var tileDefinition = new WorldObjectDefinition(Data.CellSize, Data.CellSize).MakeSolid();

            foreach (var dest in GetTileDestinations()) 
            {
                _colliders.Add(Owner.Layout.Spawn(tileDefinition, new Vector2(dest.X, dest.Y)));
            }
        }

        public override void Draw(LayoutView view)
        {
            foreach (var dest in GetTileDestinations())
            {
                Rectangle source = new Rectangle(0, 0, Data.CellSize, Data.CellSize);

                view.Draw(Data.Texture, source, dest, Color.White, false);
            }
        }

        private IEnumerable<Rectangle> GetTileDestinations()
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

                yield return new Rectangle(cx * Data.CellSize, currentY * Data.CellSize, Data.CellSize, Data.CellSize);
            }
        }
    }
}
