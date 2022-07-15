using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Reaper.Components;
using Core;
using Core.Collision;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AssetPaths = Reaper.Constants.AssetPaths;
using LayerNames = Reaper.Constants.Layers;

namespace Reaper
{
    public static class OgmoLoad
    {
        public static void LoadOgmoLevel(this App game, string filename, string spawnPoint = null)
        {
            OgmoMap map;

            using (var sr = new StreamReader(filename))
                map = JsonConvert.DeserializeObject<OgmoMap>(sr.ReadToEnd());

            var level = new GameplayLevel(map.Values.SpatialCellSize, map.Width, map.Height);

            foreach (var layer in map.Layers)
            {
                switch (layer.Name)
                {
                    case LayerNames.Entities:
                        level.LoadEntities(spawnPoint ?? map.Values.EntrySpawnPoint, layer.Entities);
                        break;

                    case LayerNames.Solids:
                        level.LoadTileLayer(layer);
                        break;

                    case LayerNames.Background:
                        level.LoadTileLayer(layer, solid: false);
                        break;
                }
            }

            game.ChangeLevel(level);
        }

        private static void LoadEntities(this Level level, string spawnPoint, List<OgmoEntity> entities)
        {
            if (entities.Count(e => e.Name == "OverworldPlayerSpawnPoint") > 1)
            {
                // If there are many spawn points and no default was given, just choose the first one.
                if (string.IsNullOrEmpty(spawnPoint))
                {
                    var firstSpawnPoint = entities.First(e => e.Name == "OverworldPlayerSpawnPoint");

                    entities.RemoveAll(e => e.Name == "OverworldPlayerSpawnPoint" && e != firstSpawnPoint);
                }
                else
                {
                    entities.RemoveAll(e => e.Name == "OverworldPlayerSpawnPoint" && spawnPoint != e.Values.SpawnPointName);
                }
            }

            foreach (var entity in entities)
            {
                var spawned = new Entity()
                {
                    // Unsure about supporting origin. Would everything as center be alright?
                    Origin = Origin.BottomCenter
                };

                switch (entity.Name)
                {
                    case "Door":
                        spawned.AddComponent(new Door());
                        break;
                    case "Barrel":
                        spawned.AddComponent(new Barrel());
                        break;
                    case "LayoutTransition":
                        spawned.AddComponent(new LevelTrigger(entity.Values.Level, entity.Values.SpawnPoint));

                        // Data driven level trigger sizes should be supported.
                        spawned.AddComponent(new Box(16, 16, false));
                        break;
                    case "PlayerSpawnPoint":
                        spawned.AddComponent(new Player());
                        break;
                    case "Blob":
                        break;
                    default:
                        continue;
                }

                level.Spawn(spawned, new Vector2(entity.X, entity.Y));
            }
        }

        private static void LoadTileLayer(this Level level, OgmoLayer layer, bool solid = true)
        {
            var mapData = new Tilemap.MapData
            {
                CellSize = layer.GridCellHeight,
                CellsX = layer.GridCellsX,
                CellsY = layer.GridCellsX,
                TilesetFilePath = $"{AssetPaths.Tilesets}{layer.Tileset}",
                Tiles = layer.Data,
                IsSolid = solid
            };

            var entity = new Entity();
            var tilemap = new Tilemap(mapData)
            {
                ZOrder = -100
            };

            entity.AddComponent(tilemap);

            level.Spawn(entity, Vector2.Zero);
        }
    }
}
