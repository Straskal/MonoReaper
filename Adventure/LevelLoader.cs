using System.Linq;
using Microsoft.Xna.Framework;
using Adventure.Components;
using Engine;
using Engine.Extensions;

namespace Adventure
{
    internal static class LevelLoader
    {
        public static Level LoadLevel(this App game, string filename, string spawnPointId = null)
        {
            spawnPointId ??= "Default";
            var map = game.Content.LoadWithoutCaching<Content.Level>(filename);
            var level = new Level(game, 64, map.Width, map.Height);
            level.LoadEntities(map.Entities, spawnPointId);
            foreach (var tileLayer in map.TileLayers)
            {
                level.LoadTileLayer(tileLayer);
            }
            return level;
        }

        private static void LoadEntities(this Level level, Content.Entity[] entities, string spawnPointId)
        {
            foreach (var entity in entities)
            {
                switch (entity.Type)
                {
                    case "PlayerSpawn":
                        if (entity.Fields.GetString("Id")?.Equals(spawnPointId) == true) 
                        {
                            level.Spawn(new Player(), entity.Position);
                        }         
                        break;
                    case "Barrel":
                        level.Spawn(new Barrel(), entity.Position);
                        break;
                    case "LevelTrigger":
                        var levelTriggerEntity = new Entity(new LevelTrigger(entity.Width, entity.Height, entity.Fields))
                        {
                            Origin = Origin.TopLeft 
                        };
                        level.Spawn(levelTriggerEntity, entity.Position);
                        break;
                    default:
                        continue;
                }
            }
        }

        private static void LoadTileLayer(this Level level, Content.TileLayer layer, bool solid = true)
        {
            var mapData = new Tilemap.MapData
            {
                CellSize = layer.TileWidth,
                CellsX = layer.TileWidth,
                CellsY = layer.TileHeight,
                TilesetFilePath = layer.TileSetRelativePath.Substring(3),
                Tiles = layer.Tiles.Select(tile => new Tilemap.TileInfo
                {
                    Source = new Rectangle((int)tile.Source.X, (int)tile.Source.Y, layer.TileWidth, layer.TileWidth),
                    Position = tile.Position
                }).ToArray(),
                IsSolid = solid
            };

            var entity = new Entity(new Tilemap(mapData) { ZOrder = -100 })
            {
                Origin = Origin.TopLeft
            };
            level.Spawn(entity, Vector2.Zero);
        }
    }
}
