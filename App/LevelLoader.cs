using Core;
using System.Linq;
using Microsoft.Xna.Framework;
using Reaper;
using Reaper.Components;

namespace Ldtk
{
    internal static class LevelLoader
    {
        public static void LoadLevel(this Core.App game, string filename, string spawnPoint = null) 
        {
            var map = game.Content.Load<Adventure.Content.Level>(filename);
            var level = new GameplayLevel(50, map.Width, map.Height);
            level.LoadEntities(map.Entities);
            foreach (var tileLayer in map.TileLayers) 
            {
                level.LoadTileLayer(tileLayer);
            }
            game.Content.UnloadAsset(filename);
            game.ChangeLevel(level);
        }

        private static void LoadEntities(this Level level, Adventure.Content.Entity[] entities)
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
                    default:
                        continue;
                }

                level.Spawn(spawned, entity.Position);
            }
        }

        private static void LoadTileLayer(this Level level, Adventure.Content.TileLayer layer, bool solid = true)
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
