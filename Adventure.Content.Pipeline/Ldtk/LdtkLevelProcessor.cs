using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Adventure.Content.Pipeline.Ldtk
{
    /// <summary>
    /// Processes an LdtkLevel and maps it to a content Level.
    /// </summary>
    [ContentProcessor(DisplayName = "LDTK Level")]
    public class LdtkLevelProcessor : ContentProcessor<LdtkLevel, LevelData>
    {
        public override LevelData Process(LdtkLevel input, ContentProcessorContext context)
        {
            return new LevelData
            {
                Name = input.Identifier,
                Width = input.Width,
                Height = input.Height,
                Entities = ProcessEntities(input),
                TileLayers = ProcessTileLayers(input)
            };
        }

        private static EntityData[] ProcessEntities(LdtkLevel input)
        {
            // TODO: Add warning if no entity layers.
            // TODO: Add warning when player entity is missing.
            return input.LayerInstances
                .Where(layer => StringComparer.OrdinalIgnoreCase.Equals(layer.Identifier, "entities"))
                .SelectMany(layer => layer.EntityInstances.Select(entity => new EntityData
                {
                    Name = entity.Id,
                    Type = entity.Id,
                    Width = entity.Width,
                    Height = entity.Height,
                    Position = new Vector2(entity.Position[0], entity.Position[1]),
                    Fields = new EntityDataFields(entity.FieldInstances.ToDictionary(x => x.Id, x => ProcessFieldValue(x.Id, x.Value)))
                })).ToArray();
        }

        private static TileLayerData[] ProcessTileLayers(LdtkLevel input)
        {
            return input.LayerInstances
                .Where(layer => IsTileLayer(layer))
                .Select(tileLayer => new TileLayerData
                {
                    IsSolid = true,
                    TileWidth = tileLayer.CellWidth,
                    TileHeight = tileLayer.CellHeight,
                    TileSetRelativePath = Path.ChangeExtension(tileLayer.TileSetRelativePath, null),
                    Tiles = tileLayer.GridTiles.Select(tile => new TileData
                    {
                        Position = new Vector2(tile.Position[0], tile.Position[1]),
                        Source = new Vector2(tile.Source[0], tile.Source[1])
                    }).ToArray()
                }).ToArray();
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