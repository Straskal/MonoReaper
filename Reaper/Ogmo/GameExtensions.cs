using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Reaper.Behaviors.Common;
using Reaper.Engine;
using Reaper.Objects;
using Reaper.Ogmo.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AssetPaths = Reaper.Constants.AssetPaths;
using LayerNames = Reaper.Constants.Layers;
using ObjectDefinitions = Reaper.Objects.Constants;

namespace Reaper.Ogmo
{
    public static class GameExtensions
    {
        public static void LoadOgmoLayout(this MainGame game, string filename, string spawnPoint = null)
        {
            OgmoMap map;

            using (var sr = new StreamReader(filename))
                map = JsonConvert.DeserializeObject<OgmoMap>(sr.ReadToEnd());

            var layout = game.GetEmptyLayout(map.Values.SpatialCellSize, map.Width, map.Height);
            layout.View.OffsetX = map.Values.OffsetX;
            layout.View.OffsetY = map.Values.OffsetY;

            foreach (var layer in map.Layers)
            {
                switch (layer.Name) 
                {
                    case LayerNames.WorldObjects:
                        layout.LoadWorldObjectsLayer(spawnPoint ?? map.Values.EntrySpawnPoint, layer.Entities);
                        break;

                    case LayerNames.Solids:
                        layout.LoadSolidsLayer(layer);
                        break;

                    case LayerNames.Background:
                        layout.LoadSolidsLayer(layer, false);
                        break;
                }
            }

            game.ChangeLayout(layout);
        }

        private static void LoadWorldObjectsLayer(this Layout layout, string spawnPoint, List<OgmoEntity> entities)
        {
            // Filter out any spawn points that don't need to exist.
            if (HasManySpawnPoints(entities)) 
            {
                // If there are many spawn points and no default was given, just choose the first one.
                if (string.IsNullOrEmpty(spawnPoint)) 
                {
                    var firstSpawnPoint = entities.First(e => e.Name == ObjectDefinitions.Player);
                    entities.RemoveAll(e => e.Name == ObjectDefinitions.Player && e != firstSpawnPoint);
                }
                else 
                {
                    entities.RemoveAll(e => e.Name == ObjectDefinitions.Player && spawnPoint != e.Values.SpawnPointName);
                }
            }

            foreach (var entity in entities)
            {
                var definition = Definitions.Get(entity.Name);
                var worldObject = layout.Spawn(definition, new Vector2(entity.X, entity.Y));

                // Default properties for every world object.
                worldObject.IsMirrored = entity.FlippedX;
                worldObject.ZOrder = entity?.Values?.DrawOrder ?? 0;

                Loaders.Load(definition.Guid, worldObject, entity);
            }
        }

        private static bool HasManySpawnPoints(List<OgmoEntity> entities) 
        {
            return entities.Count(e => e.Name == "player") > 1;
        }

        private static void LoadSolidsLayer(this Layout layout, OgmoLayer layer, bool solid = true)
        {
            var tilemap = new WorldObjectDefinition();

            if (solid) tilemap.MakeSolid();

            tilemap.AddBehavior(wo => new TilemapBehavior(wo, new TilemapBehavior.MapData
            {
                CellSize = layer.GridCellHeight,
                CellsX = layer.GridCellsX,
                CellsY = layer.GridCellsX,
                TilesetFilePath = $"{AssetPaths.Tilesets}{layer.Tileset}",
                Tiles = layer.Data
            }));

            var tilemapWorldObject = layout.Spawn(tilemap, Vector2.Zero);
            tilemapWorldObject.ZOrder = solid ? -100 : -200;
        }
    }
}
