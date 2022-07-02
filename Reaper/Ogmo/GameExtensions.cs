using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Reaper.Components;
using Reaper.Engine;
using Reaper.Engine.AABB;
using Reaper.Engine.Components;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AssetPaths = Reaper.Constants.AssetPaths;
using LayerNames = Reaper.Constants.Layers;

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
                    case "LayoutTransition":

                        spawned.AddComponent(new LevelTrigger(entity.Values.Level, entity.Values.SpawnPoint));
                        spawned.AddComponent(new Box(CollisionLayer.Overlap, 16, 16));
                        break;
                    case "PlayerSpawnPoint":
                        spawned.AddComponent(new Player());
                        spawned.AddComponent(new Body(12, 16));
                        spawned.AddComponent(new Sprite("art/player/player"));
                        spawned.AddComponent(new Animator(new[]
                        {
                            new Animator.Animation
                            {
                                Name = "idle",
                                ImageFilePath = "art/player/player",
                                Loop = true,
                                SecPerFrame = 0.1f,
                                Frames = new []
                                {
                                    new Rectangle(0, 0, 16, 16),
                                }
                            },
                            new Animator.Animation
                            {
                                Name = "walk_down",
                                ImageFilePath = "art/player/player",
                                SecPerFrame = 0.1f,
                                Loop = true,
                                Frames = new []
                                {
                                    new Rectangle(16 * 1, 0, 16, 16),
                                    new Rectangle(16 * 2, 0, 16, 16),
                                }
                            },
                            new Animator.Animation
                            {
                                Name = "walk_up",
                                ImageFilePath = "art/player/player",
                                SecPerFrame = 0.1f,
                                Loop = true,
                                Frames = new []
                                {
                                    new Rectangle(16 * 3, 0, 16, 16),
                                    new Rectangle(16 * 4, 0, 16, 16),
                                }
                            },
                            new Animator.Animation
                            {
                                Name = "walk_left",
                                ImageFilePath = "art/player/player",
                                SecPerFrame = 0.1f,
                                Loop = true,
                                Frames = new []
                                {
                                    new Rectangle(16 * 5, 0, 16, 16),
                                    new Rectangle(16 * 6, 0, 16, 16),
                                }
                            },
                            new Animator.Animation
                            {
                                Name = "walk_right",
                                ImageFilePath = "art/player/player",
                                SecPerFrame = 0.1f,
                                Loop = true,
                                Frames = new []
                                {
                                    new Rectangle(16 * 7, 0, 16, 16),
                                    new Rectangle(16 * 8, 0, 16, 16),
                                }
                            },
                        }));
                        break;
                    case "Blob":
                        spawned.AddComponent(new Body(12, 16));
                        spawned.AddComponent(new Sprite("art/player/player"));
                        spawned.AddComponent(new Animator(new[]
                        {
                            new Animator.Animation
                            {
                                Name = "idle",
                                ImageFilePath = "art/player/player",
                                Loop = true,
                                SecPerFrame = 0.1f,
                                Frames = new []
                                {
                                    new Rectangle(0, 0, 16, 16),
                                }
                            },
                            new Animator.Animation
                            {
                                Name = "walk_down",
                                ImageFilePath = "art/player/player",
                                SecPerFrame = 0.1f,
                                Loop = true,
                                Frames = new []
                                {
                                    new Rectangle(16 * 1, 0, 16, 16),
                                    new Rectangle(16 * 2, 0, 16, 16),
                                }
                            },
                            new Animator.Animation
                            {
                                Name = "walk_up",
                                ImageFilePath = "art/player/player",
                                SecPerFrame = 0.1f,
                                Loop = true,
                                Frames = new []
                                {
                                    new Rectangle(16 * 3, 0, 16, 16),
                                    new Rectangle(16 * 4, 0, 16, 16),
                                }
                            },
                            new Animator.Animation
                            {
                                Name = "walk_left",
                                ImageFilePath = "art/player/player",
                                SecPerFrame = 0.1f,
                                Loop = true,
                                Frames = new []
                                {
                                    new Rectangle(16 * 5, 0, 16, 16),
                                    new Rectangle(16 * 6, 0, 16, 16),
                                }
                            },
                            new Animator.Animation
                            {
                                Name = "walk_right",
                                ImageFilePath = "art/player/player",
                                SecPerFrame = 0.1f,
                                Loop = true,
                                Frames = new []
                                {
                                    new Rectangle(16 * 7, 0, 16, 16),
                                    new Rectangle(16 * 8, 0, 16, 16),
                                }
                            },
                        }));
                        break;
                }

                if (spawned != default(Entity))
                {
                    spawned.IsMirrored = entity.FlippedX;

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
