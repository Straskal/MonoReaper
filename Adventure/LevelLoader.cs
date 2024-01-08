using Adventure.Components;
using Adventure.Content;
using Adventure.Entities;
using Engine;
using Engine.Extensions;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Adventure
{
    internal static class LevelLoader
    {
        public static readonly int PartitionCellSize = 64;

        public static Level LoadLevel(this App game, string filename, string playerSpawnId = null)
        {
            playerSpawnId ??= "Default";

            var levelData = game.Content.LoadWithoutCaching<LevelData>(filename);
            var level = new Level(game, PartitionCellSize, levelData.Width, levelData.Height);

            foreach (var entity in GetEntitiesFromLevelData(levelData, playerSpawnId))
            {
                level.Spawn(entity);
            }

            foreach (var tileMapEntity in GetTilemapEntitiesFromLevelData(levelData))
            {
                level.Spawn(tileMapEntity);
            }

            return level;
        }

        private static IEnumerable<Entity> GetEntitiesFromLevelData(LevelData levelData, string playerSpawnId)
        {
            foreach (var entityData in levelData.Entities)
            {
                switch (entityData.Type)
                {
                    case "PlayerSpawn":
                        if (entityData.Fields.GetString("Id")?.Equals(playerSpawnId) == true)
                        {
                            yield return new Player()
                            {
                                Position = entityData.Position
                            };
                        }
                        break;
                    case "Barrel":
                        yield return new Barrel()
                        {
                            Position = entityData.Position
                        };
                        break;
                    case "FireballShooter":
                        yield return new EnemyFireballShooter()
                        {
                            Position = entityData.Position
                        };
                        break;
                    case "LevelTrigger":
                        yield return new LevelTrigger(entityData)
                        {
                            Origin = Origin.TopLeft,
                            Position = entityData.Position
                        };
                        break;
                    default:
                        continue;
                }
            }
        }

        private static IEnumerable<Entity> GetTilemapEntitiesFromLevelData(LevelData levelData)
        {
            foreach (var tileLayerData in levelData.TileLayers)
            {
                var mapData = new Tilemap.MapData
                {
                    CellSize = tileLayerData.TileWidth,
                    CellsX = tileLayerData.TileWidth,
                    CellsY = tileLayerData.TileHeight,
                    TilesetFilePath = tileLayerData.TileSetRelativePath.Substring(3),
                    Tiles = tileLayerData.Tiles.Select(tile => new Tilemap.TileInfo
                    {
                        Source = new Rectangle((int)tile.Source.X, (int)tile.Source.Y, tileLayerData.TileWidth, tileLayerData.TileWidth),
                        Position = tile.Position
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
