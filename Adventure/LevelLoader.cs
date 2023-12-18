using System.Linq;
using Microsoft.Xna.Framework;
using Adventure.Components;
using Engine;
using Engine.Extensions;

namespace Adventure
{
    internal static class LevelLoader
    {
        public static Level LoadLevel(this App game, string filename, string spawnPoint = null)
        {
            var map = game.Content.LoadWithoutCaching<Content.Level>(filename);
            var level = new Level(game, 64, map.Width, map.Height);
            level.LoadEntities(map.Entities);
            foreach (var tileLayer in map.TileLayers)
            {
                level.LoadTileLayer(tileLayer);
            }
            return level;
        }

        private static void LoadEntities(this Level level, Content.Entity[] entities)
        {
            foreach (var entity in entities)
            {
                var spawned = new Entity
                {
                    // Unsure about supporting origin. Would everything as center be alright?
                    Origin = Origin.Center
                };

                switch (entity.Type)
                {
                    case "Player":
                        spawned.AddComponent(new Player());
                        break;
                    case "Barrel":
                        spawned.AddComponent(new Barrel());
                        break;
                    case "LevelTrigger":
                        spawned.AddComponent(new LevelTrigger("", 16, 16));
                        break;
                    default:
                        continue;
                }

                level.Spawn(spawned, entity.Position);
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
