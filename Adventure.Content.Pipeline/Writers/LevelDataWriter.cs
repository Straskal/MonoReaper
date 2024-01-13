using Adventure.Content.Readers;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;

namespace Adventure.Content.Pipeline.Writers
{
    [ContentTypeWriter]
    public class LevelDataWriter : ContentTypeWriter<LevelData>
    {
        protected override void Write(ContentWriter output, LevelData value)
        {
            try
            {
                output.Write(value.Name);
                output.Write(value.Bounds.X);
                output.Write(value.Bounds.Y);
                output.Write(value.Bounds.Width);
                output.Write(value.Bounds.Height);
                output.Write(value.Entities.Length);

                foreach (var entity in value.Entities)
                {
                    output.Write(entity.Name);
                    output.Write(entity.Type);
                    output.Write(entity.Position);
                    output.Write(entity.Width);
                    output.Write(entity.Height);
                    output.Write(entity.Fields.Count);

                    foreach (var kvp in entity.Fields)
                    {
                        output.Write(kvp.Key);
                        output.Write(kvp.Value);
                    }
                }

                output.Write(value.Layers.Length);

                foreach (var tileLayer in value.Layers)
                {
                    output.Write(tileLayer.IsSolid);
                    output.Write(tileLayer.TileWidth);
                    output.Write(tileLayer.TileHeight);
                    output.Write(tileLayer.TilesetPath);
                    output.Write(tileLayer.Tiles.Length);

                    foreach (var tile in tileLayer.Tiles)
                    {
                        output.Write(tile.Position);
                        output.Write(tile.Source);
                    }
                }
            }
            catch (Exception e) 
            {
                throw new Exception(value.Name + " -------------------- " + e.ToString());
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(LevelData).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(LevelDataReader).AssemblyQualifiedName;
        }
    }
}
