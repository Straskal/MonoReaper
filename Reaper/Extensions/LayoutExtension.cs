﻿using Reaper.Objects;
using Reaper.Engine;
using Reaper.Engine.Builtins;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Reaper;
using System.IO;

namespace Reaper.Extensions
{
    public static class LayoutExtension
    {
        public static void LoadOgmoLayout(this IGame game, string filename)
        {
            OgmoMap map;

            using (StreamReader r = new StreamReader(filename))
            {
                string json = r.ReadToEnd();
                map = JsonConvert.DeserializeObject<OgmoMap>(json);
            }

            var layout = game.GetEmptyLayout(32, map.Width, map.Height);

            foreach (var layer in map.Layers)
            {
                if (layer.Data != null)
                {
                    var data = new TilemapBehavior.MapData
                    {
                        CellSize = layer.GridCellHeight,
                        CellsX = layer.GridCellsX,
                        CellsY = layer.GridCellsX,
                        TilesetFilePath = layer.Tileset,
                        Tiles = layer.Data
                    };

                    var tilemapDef = new WorldObjectDefinition(0, 0);
                    tilemapDef.AddBehavior(wo => new TilemapBehavior(wo, data));

                    layout.Spawn(tilemapDef, Vector2.Zero);
                }
                else if (layer.Entities != null)
                {
                    foreach (var entity in layer.Entities)
                    {
                        layout.Spawn(Definitions.Get(entity), new Vector2(entity.X, entity.Y));
                    }
                }
            }

            game.ChangeLayout(layout);
        }
    }
}
