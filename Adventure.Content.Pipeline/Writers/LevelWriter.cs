using Adventure.Content.Readers;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Adventure.Content.Pipeline.Writers
{
    /// <summary>
    /// Writes a Level to xnb format.
    /// </summary>
    [ContentTypeWriter]
    public class LevelWriter : ContentTypeWriter<Level>
    {
        protected override void Write(ContentWriter output, Level value)
        {
            output.Write(value.Name);
            output.Write(value.Width);
            output.Write(value.Height);
            output.Write(value.Entities.Length);

            foreach (var entity in value.Entities)
            {
                output.Write(entity.Name);
                output.Write(entity.Type);
                output.Write(entity.Position);
            }

            output.Write(value.TileLayers.Length);

            foreach (var tileLayer in value.TileLayers)
            {
                output.Write(tileLayer.IsSolid);
                output.Write(tileLayer.TileWidth);
                output.Write(tileLayer.TileHeight);
                output.Write(tileLayer.TileSetRelativePath);
                output.Write(tileLayer.Tiles.Length);

                foreach (var tile in tileLayer.Tiles)
                {
                    output.Write(tile.Position);
                    output.Write(tile.Source);
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
