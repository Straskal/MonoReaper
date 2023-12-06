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
    public class LdtkLevelProcessor : ContentProcessor<LdtkLevel, Level>
    {
        public override Level Process(LdtkLevel input, ContentProcessorContext context)
        {
            return new Level
            {
                Name = input.Identifier,
                Width = input.Width,
                Height = input.Height,
                Entities = ProcessEntities(input),
                TileLayers = ProcessTileLayers(input)
            };
        }

        private static Entity[] ProcessEntities(LdtkLevel input)
        {
            // TODO: Add warning if no entity layers.
            // TODO: Add warning when player entity is missing.
            return input.LayerInstances
                .Where(layer => StringComparer.OrdinalIgnoreCase.Equals(layer.Identifier, "entities"))
                .SelectMany(layer => layer.EntityInstances.Select(entity => new Entity
                {
                    Name = entity.Id,
                    Type = entity.Id,
                    Position = new Vector2(entity.Position[0], entity.Position[1])
                })).ToArray();
        }

        private static TileLayer[] ProcessTileLayers(LdtkLevel input)
        {
            return input.LayerInstances
                .Where(layer => StringComparer.OrdinalIgnoreCase.Equals(layer.Identifier, "tiles"))
                .Select(tileLayer => new TileLayer
                {
                    IsSolid = true,
                    TileWidth = tileLayer.CellWidth,
                    TileHeight = tileLayer.CellHeight,
                    TileSetRelativePath = Path.ChangeExtension(tileLayer.TileSetRelativePath, null),
                    Tiles = tileLayer.GridTiles.Select(tile => new Tile
                    {
                        Position = new Vector2(tile.Position[0], tile.Position[1]),
                        Source = new Vector2(tile.Source[0], tile.Source[1])
                    }).ToArray()
                }).ToArray();
        }
    }
}