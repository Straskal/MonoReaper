using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using System.IO;
using System.Linq;

namespace Adventure.Content.Pipeline
{
    [ContentTypeWriter]
    public class LdtkLevelWriter : ContentTypeWriter<LdtkLevel>
    {
        protected override void Write(ContentWriter output, LdtkLevel value)
        {
            output.Write(value.Identifier);
            output.Write(value.Width);
            output.Write(value.Height);

            var entityLayer = value.LayerInstances.FirstOrDefault(layer => StringComparer.OrdinalIgnoreCase.Equals(layer.Identifier, "entities"));

            if (entityLayer != null) 
            {
                output.Write(entityLayer.EntityInstances.Length);

                foreach (var entity in entityLayer.EntityInstances)
                {
                    output.Write(entity.Id);
                    output.Write(entity.Id);
                    output.Write(new Vector2(entity.Position[0], entity.Position[1]));
                }
            }

            var tileLayers = value.LayerInstances.Where(layer => StringComparer.OrdinalIgnoreCase.Equals(layer.Identifier, "tiles"));

            output.Write(tileLayers.Count());

            foreach (var tileLayer in tileLayers)
            {
                output.Write(true); // IsSolid
                output.Write(tileLayer.CellWidth);
                output.Write(tileLayer.CellHeight);
                output.Write(Path.ChangeExtension(tileLayer.TileSetRelativePath, null));
                output.Write(tileLayer.GridTiles.Length);

                foreach (var tile in tileLayer.GridTiles) 
                {
                    output.Write(new Vector2(tile.Position[0], tile.Position[1]));
                    output.Write(new Vector2(tile.Source[0], tile.Source[1]));
                }
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Level).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(LevelReader).AssemblyQualifiedName;
        }
    }
}
