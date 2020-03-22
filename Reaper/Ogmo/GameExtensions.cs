using Reaper.Objects;
using Reaper.Engine;
using Reaper.Engine.Behaviors;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.IO;
using Reaper.Ogmo.Models;

using AssetPaths = Reaper.Constants.AssetPaths;
using LayerNames = Reaper.Constants.Layers;

namespace Reaper.Ogmo
{
    public static class GameExtensions
    {
        public static void LoadOgmoLayout(this IGame game, string filename)
        {
            OgmoMap map;

            using (var sr = new StreamReader(filename))
            {
                map = JsonConvert.DeserializeObject<OgmoMap>(sr.ReadToEnd());
            }

            var layout = game.GetEmptyLayout(map.Values.SpatialCellSize, map.Width, map.Height);
            int tmDrawLayer = -10;

            foreach (var layer in map.Layers)
            {
                if (layer.Data != null)
                {
                    layout.LoadTilemap(layer, tmDrawLayer--);
                }
                else if (layer.Entities != null)
                {
                    layout.LoadEntities(layer.Entities);
                }
            }

            game.ChangeLayout(layout);
        }

        private static void LoadTilemap(this Layout layout, OgmoLayer layer, int drawLayer) 
        {
            var tilemapDef = new WorldObjectDefinition();
            tilemapDef.AddBehavior(wo => new TilemapBehavior(wo, new TilemapBehavior.MapData
            {
                CellSize = layer.GridCellHeight,
                CellsX = layer.GridCellsX,
                CellsY = layer.GridCellsX,
                TilesetFilePath = $"{AssetPaths.Tilesets}{layer.Tileset}",
                Tiles = layer.Data
            }));

            if (layer.Name == LayerNames.Solids)
            {
                tilemapDef.MakeSolid();
            }

            var tm = layout.Spawn(tilemapDef, Vector2.Zero);
            tm.ZOrder = drawLayer;
        }

        private static void LoadEntities(this Layout layout, OgmoEntity[] entities)
        {
            foreach (var entity in entities)
            {
                var definition = Definitions.Get(entity.Name);
                var worldObject = layout.Spawn(definition, new Vector2(entity.X, entity.Y));

                // Default properties for every world object.
                worldObject.IsMirrored = entity.FlippedX;
                worldObject.ZOrder = entity?.Values.DrawOrder ?? 0;

                Loaders.Load(definition.Guid, worldObject, entity);
            }
        }
    }
}
