using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Reaper.Components;
using Core;
using Core.Collision;
using Core.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AssetPaths = Reaper.Constants.AssetPaths;
using LayerNames = Reaper.Constants.Layers;
using static Reaper.Constants;

namespace Reaper
{
    public static class GameExtensions
    {
        public static void LoadOgmoLayout(this App game, string filename, string spawnPoint = null)
        {
            OgmoMap map;

            using (var sr = new StreamReader(filename))
                map = JsonConvert.DeserializeObject<OgmoMap>(sr.ReadToEnd());

            var layout = new GameplayLevel(map.Values.SpatialCellSize, map.Width, map.Height);
            layout.Camera.OffsetX = map.Values.OffsetX;
            layout.Camera.OffsetY = map.Values.OffsetY;

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

            game.ChangeLevel(layout);
        }

        private static void LoadWorldObjectsLayer(this Level level, string spawnPoint, List<OgmoEntity> entities)
        {
            // Filter out any spawn points that don't need to exist.
            if (HasManySpawnPoints(entities))
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
                        spawned.AddComponent(new Box(16, 16, false));
                        break;
                    case "PlayerSpawnPoint":
                        spawned.AddComponent(new Player());
                        break;
                    case "Blob":
                        break;
                }

                if (spawned != default(Entity))
                {
                    level.Spawn(spawned, new Vector2(entity.X, entity.Y));
                }
            }
        }

        private static bool HasManySpawnPoints(List<OgmoEntity> entities)
        {
            return entities.Count(e => e.Name == "OverworldPlayerSpawnPoint") > 1;
        }

        private static void LoadSolidsLayer(this Level level, OgmoLayer layer, bool solid = true)
        {
            var entity = new Entity();

            var mapData = new Tilemap.MapData
            {
                CellSize = layer.GridCellHeight,
                CellsX = layer.GridCellsX,
                CellsY = layer.GridCellsX,
                TilesetFilePath = $"{AssetPaths.Tilesets}{layer.Tileset}",
                Tiles = layer.Data,
                IsSolid = solid
            };

            var tilemap = new Tilemap(mapData)
            {
                ZOrder = -100
            };

            entity.AddComponent(tilemap);

            level.Spawn(entity, Vector2.Zero);
        }
    }
}
