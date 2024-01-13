using Adventure.Components;
using Adventure.Content;
using Adventure.Entities;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Adventure
{
    internal static class LevelLoader
    {
        public static readonly int PartitionCellSize = 64;

        public static List<Entity> LoadEntities(App game, string filename, string playerSpawnId = null)
        {
            playerSpawnId ??= "Default";
            var result = new List<Entity>();
            var data = game.Content.Load<LevelData>(filename);

            foreach (var entity in GetEntitiesFromLevelData(data, playerSpawnId))
            {
                result.Add(entity);
            }

            foreach (var tileMapEntity in GetTilemapEntitiesFromLevelData(data))
            {
                result.Add(tileMapEntity);
            }

            return result;
        }

        private static IEnumerable<Entity> GetEntitiesFromLevelData(LevelData levelData, string playerSpawnId)
        {
            var offset = new Vector2(levelData.Bounds.X, levelData.Bounds.Y);

            foreach (var entityData in levelData.Entities)
            {
                switch (entityData.Type)
                {
                    case "PlayerSpawn":
                        if (entityData.Fields.GetString("Id")?.Equals(playerSpawnId) == true)
                        {
                            yield return new Player()
                            {
                                Position = entityData.Position + offset
                            };
                        }
                        break;
                    case "Barrel":
                        yield return new Barrel()
                        {
                            Position = entityData.Position + offset
                        };
                        break;
                    case "FireballShooter":
                        yield return new EnemyFireballShooter()
                        {
                            Position = entityData.Position + offset
                        };
                        break;
                    case "LevelTrigger":
                        yield return new LevelTrigger(entityData)
                        {
                            Origin = Origin.TopLeft,
                            Position = entityData.Position + offset
                        };
                        break;
                    default:
                        continue;
                }
            }
        }

        private static IEnumerable<Entity> GetTilemapEntitiesFromLevelData(LevelData levelData)
        {
            var offset = new Vector2(levelData.Bounds.X, levelData.Bounds.Y);

            foreach (var tileLayerData in levelData.Layers)
            {
                var mapData = new Tilemap.MapData
                {
                    CellSize = tileLayerData.TileWidth,
                    CellsX = tileLayerData.TileWidth,
                    CellsY = tileLayerData.TileHeight,
                    TilesetFilePath = tileLayerData.TilesetPath.Substring(3),
                    Tiles = tileLayerData.Tiles.Select(tile => new Tilemap.TileInfo
                    {
                        Source = new Rectangle((int)tile.Source.X, (int)tile.Source.Y, tileLayerData.TileWidth, tileLayerData.TileWidth),
                        Position = tile.Position + offset
                    }).ToArray(),
                    IsSolid = true
                };

                yield return new Tilemap(mapData)
                {
                    Origin = Origin.TopLeft
                };
            }
        }
    }
}
