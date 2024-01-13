using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Adventure.Content.Readers
{
    public class LevelDataReader : ContentTypeReader<LevelData>
    {
        protected override LevelData Read(ContentReader input, LevelData existingInstance)
        {
            var name = input.ReadString();
            var bounds = new Rectangle(input.ReadInt32(), input.ReadInt32(), input.ReadInt32(), input.ReadInt32());
            var entities = new EntityData[input.ReadInt32()];

            for (int i = 0; i < entities.Length; i++)
            {
                entities[i] = new EntityData
                {
                    Name = input.ReadString(),
                    Type = input.ReadString(),
                    Position = input.ReadVector2(),
                    Width = input.ReadInt32(),
                    Height = input.ReadInt32()
                };

                var fieldCount = input.ReadInt32();

                for (int j = 0; j < fieldCount; j++)
                {
                    entities[i].Fields.Add(input.ReadString(), input.ReadString());
                }
            }

            var layers = new LayerData[input.ReadInt32()];

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new LayerData
                {
                    IsSolid = input.ReadBoolean(),
                    TileWidth = input.ReadInt32(),
                    TileHeight = input.ReadInt32(),
                    TilesetPath = input.ReadString(),
                    Tiles = new TileData[input.ReadInt32()]
                };

                for (int j = 0; j < layers[i].Tiles.Length; j++)
                {
                    layers[i].Tiles[j] = new TileData
                    {
                        Position = input.ReadVector2(),
                        Source = input.ReadVector2()
                    };
                }
            }

            return new LevelData
            {
                Name = name,
                Bounds = bounds,
                Entities = entities,
                Layers = layers
            };
        }
    }
}
