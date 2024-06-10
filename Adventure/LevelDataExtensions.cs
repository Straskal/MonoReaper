using Adventure.Content;
using Adventure.Entities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Adventure
{
    internal static class LevelDataExtensions
    {
        public static List<Entity> GetEntities(this LevelData levelData)
        {
            var result = new List<Entity>();

            foreach (var entity in GetEntitiesFromLevelData(levelData))
            {
                result.Add(entity);
            }

            //foreach (var tileMapEntity in GetTilemapEntitiesFromLevelData(levelData))
            //{
            //    result.Add(tileMapEntity);
            //}

            return result;
        }

        private static IEnumerable<Entity> GetEntitiesFromLevelData(LevelData levelData)
        {
            var levelOffset = new Vector2(levelData.Bounds.X, levelData.Bounds.Y);

            foreach (var entityData in levelData.Entities)
            {
                Entity result = null;

                switch (entityData.Type)
                {
                    case "PlayerSpawn":
                        result = new TopDownPlayer()
                        {
                            Position = entityData.Position + levelOffset
                        };
                        break;
                }

                if (result != null) 
                {
                    yield return result;
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

                yield return new Tilemap 
                {
                    Data = mapData
                };
            }
        }
    }
}
