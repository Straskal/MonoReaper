using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Adventure.Content.Pipeline.Ldtk
{
    [ContentProcessor(DisplayName = "LDTK Level")]
    public class LdtkLevelProcessor : ContentProcessor<LdtkLevel, LevelData>
    {
        public override LevelData Process(LdtkLevel input, ContentProcessorContext context)
        {
            return new LevelData
            {
                Name = input.Identifier,
                Bounds = GetBounds(input),
                Entities = GetEntities(input),
                Layers = GetLayers(input)
            };
        }

        private static Rectangle GetBounds(LdtkLevel input)
        {
            return new Rectangle(input.WorldX, input.WorldY, input.WorldY, input.Height);
        }

        private static EntityData[] GetEntities(LdtkLevel input)
        {
            return input.LayerInstances
                .Where(layer => IsEntityLayer(layer))
                .SelectMany(layer => GetLayerEntities(layer))
                .ToArray();
        }

        private static IEnumerable<EntityData> GetLayerEntities(LdtkLayerInstance layer)
        {
            return layer.EntityInstances.Select(entity => new EntityData
            {
                Name = entity.Id,
                Type = entity.Id,
                Width = entity.Width,
                Height = entity.Height,
                Position = new Vector2(entity.Position[0], entity.Position[1]),
                Fields = new EntityDataFields(entity.FieldInstances.ToDictionary(x => x.Id, x => ProcessFieldValue(x.Id, x.Value)))
            });
        }

        private static LayerData[] GetLayers(LdtkLevel input)
        {
            return input.LayerInstances
                .Where(layer => IsTileLayer(layer))
                .Select(layer => GetLayer(layer))
                .ToArray();
        }

        private static LayerData GetLayer(LdtkLayerInstance layer)
        {
            return new LayerData
            {
                IsSolid = true,
                TileWidth = layer.CellWidth,
                TileHeight = layer.CellHeight,
                TilesetPath = Path.ChangeExtension(layer.TileSetRelativePath, null),
                Tiles = GetLayerTiles(layer)
            };
        }

        private static TileData[] GetLayerTiles(LdtkLayerInstance layer)
        {
            return layer.GridTiles.Select(tile => new TileData
            {
                Position = new Vector2(tile.Position[0], tile.Position[1]),
                Source = new Vector2(tile.Source[0], tile.Source[1])
            }).ToArray();
        }

        private static bool IsEntityLayer(LdtkLayerInstance layer)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(layer.Identifier, "entities");
        }

        private static bool IsTileLayer(LdtkLayerInstance layer)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(layer.Identifier, "tiles");
        }

        private static string ProcessFieldValue(string name, string value)
        {
            // Remove .ldtkl extension from level paths for the content manager
            if (StringComparer.OrdinalIgnoreCase.Equals(name, "LevelPath"))
            {
                return Path.ChangeExtension(value, null);
            }

            return value;
        }
    }
}