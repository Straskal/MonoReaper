using Reaper.Objects;
using Reaper.Engine;
using Reaper.Engine.Behaviors;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.IO;
using System;

namespace Reaper.Ogmo
{
    public static class OgmoExtensions
    {
        public static void LoadOgmoLayout(this IGame game, string filename)
        {
            OgmoMap map;

            using (StreamReader r = new StreamReader(filename))
            {
                string json = r.ReadToEnd();
                map = JsonConvert.DeserializeObject<OgmoMap>(json);
            }

            var layout = game.GetEmptyLayout(map.Values.SpatialCellSize, map.Width, map.Height);

            int tmDrawLayer = -10;

            foreach (var layer in map.Layers)
            {
                if (layer.Data != null)
                {
                    var data = new TilemapBehavior.MapData
                    {
                        CellSize = layer.GridCellHeight,
                        CellsX = layer.GridCellsX,
                        CellsY = layer.GridCellsX,
                        TilesetFilePath = "art/tilesets/" + layer.Tileset,
                        Tiles = layer.Data
                    };

                    var tilemapDef = new WorldObjectDefinition();

                    if (layer.Name == "middleground") 
                    {
                        tilemapDef.MakeSolid();
                    }

                    tilemapDef.AddBehavior(wo => new TilemapBehavior(wo, data));

                    var tm = layout.Spawn(tilemapDef, Vector2.Zero);
                    tm.ZOrder = tmDrawLayer--;
                }
                else if (layer.Entities != null)
                {
                    foreach (var entity in layer.Entities)
                    {
                        var wo = layout.Spawn(Definitions.Get(entity), new Vector2(entity.X, entity.Y));

                        wo.IsMirrored = entity.FlippedX;
                        wo.ZOrder = entity?.Values.DrawOrder ?? 0;

                        switch (entity.Name) 
                        {
                            case "transition":
                                if (string.IsNullOrWhiteSpace(entity.Values.Level))
                                    throw new ArgumentException("Level transitions must be provided with a layout to load");

                                wo.GetBehavior<LevelTransitionBehavior>().Level = entity.Values.Level;
                                break;
                        }
                    }
                }
            }

            game.ChangeLayout(layout);
        }
    }
}
