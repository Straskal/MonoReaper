using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Reaper.Objects;
using Reaper.Engine;
using Reaper.Engine.Behaviors;
using Reaper.Tiled.Models;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Constants = Reaper.Tiled.TiledConstants;

namespace Reaper.Tiled.Extensions
{
    public static class TiledExtensions
    {
        public static void LoadTiledLayout(this IGame game, string fileName) 
        {
            LoadTiledMap(fileName, out var map, out var tilesets, out var templates);

            var layout = game.GetEmptyLayout(256, map.Width * map.TileWidth, map.Height * map.TileHeight);

            // Draw each layer under the last, starting from -10.
            int tmDrawLayer = -10;

            for (int i = 0; i < map.Layers.Length; i++)
            {
                var layer = map.Layers[i];

                switch (layer.Type) 
                {
                    // Spawn tilemap from tile layer
                    case Constants.LayerTypes.Tile:
                        {
                            var data = new TilemapBehavior.MapData
                            {
                                CellSize = map.TileWidth,
                                CellsX = layer.Width,
                                CellsY = layer.Height,
                                TilesetFilePath = "Art/Tilesets/" + tilesets[i].Name,
                                Tiles = layer.Data
                            };

                            var tilemapDef = new WorldObjectDefinition().MakeSolid();
                            tilemapDef.AddBehavior(wo => new TilemapBehavior(wo, data));

                            var tilemap = layout.Spawn(tilemapDef, new Vector2(layer.X, layer.Y));
                            tilemap.ZOrder = tmDrawLayer--;
                        }
                        break;
                    // Spawn objects from object layer
                    case Constants.LayerTypes.Object:
                        foreach (var obj in layer.Objects) 
                        {
                            var template = templates[obj.Template];
                            layout.Spawn(Definitions.Get(template.Object.Name.ToLower()), new Vector2(obj.X, obj.Y));
                        }
                        break;
                }
            }

            game.ChangeLayout(layout);
        }

        private static void LoadTiledMap(
            string fileName,
            out TiledMap map, 
            out TiledTileset[] tilesets,
            out Dictionary<string, TiledTemplate> templates)
        {
            map = ReadMapFile<TiledMap>(fileName);
            tilesets = ReadTilesets(map);
            templates = ReadTemplates(map);
        }

        private static T ReadMapFile<T>(string fileName)
        {
            using (var sr = new StreamReader(fileName))
            {
                return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
            }
        }

        private static TiledTileset[] ReadTilesets(TiledMap map) 
        {
            var tilesets = new List<TiledTileset>();

            foreach (var tilesetInfo in map.Tilesets)
            {
                tilesets.Add(ReadMapFile<TiledTileset>("./Content/Levels/" + tilesetInfo.Source));
            }

            return tilesets.ToArray();
        }

        private static Dictionary<string, TiledTemplate> ReadTemplates(TiledMap map)
        {
            var templates = new Dictionary<string, TiledTemplate>();

            var templatePaths = map.Layers.SelectMany(l => l.Objects ?? Enumerable.Empty<TiledObject>()).Select(o => o.Template).Distinct();

            foreach (var path in templatePaths) 
            {
                if (!templates.ContainsKey(path))
                {
                    templates.Add(path, ReadMapFile<TiledTemplate>("./Content/Levels/" + path));
                }
            }

            return templates;
        }
    }
}
