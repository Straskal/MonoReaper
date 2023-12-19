using Microsoft.Xna.Framework.Content;

namespace Adventure.Content.Readers
{
    public class LevelDataReader : ContentTypeReader<LevelData>
    {
        protected override LevelData Read(ContentReader input, LevelData existingInstance)
        {
            var level = new LevelData
            {
                Name = input.ReadString(),
                Width = input.ReadInt32(),
                Height = input.ReadInt32(),
                Entities = new EntityData[input.ReadInt32()]
            };

            for (int i = 0; i < level.Entities.Length; i++)
            {
                level.Entities[i] = new EntityData
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
                    level.Entities[i].Fields.Add(input.ReadString(), input.ReadString());
                }
            }

            level.TileLayers = new TileLayerData[input.ReadInt32()];

            for (int i = 0; i < level.TileLayers.Length; i++)
            {
                level.TileLayers[i] = new TileLayerData
                {
                    IsSolid = input.ReadBoolean(),
                    TileWidth = input.ReadInt32(),
                    TileHeight = input.ReadInt32(),
                    TileSetRelativePath = input.ReadString(),
                    Tiles = new TileData[input.ReadInt32()]
                };

                for (int j = 0; j < level.TileLayers[i].Tiles.Length; j++)
                {
                    level.TileLayers[i].Tiles[j] = new TileData
                    {
                        Position = input.ReadVector2(),
                        Source = input.ReadVector2()
                    };
                }
            }

            return level;
        }
    }
}
