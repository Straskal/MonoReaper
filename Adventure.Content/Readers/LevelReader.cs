using Microsoft.Xna.Framework.Content;

namespace Adventure.Content.Readers
{
    public class LevelReader : ContentTypeReader<Level>
    {
        protected override Level Read(ContentReader input, Level existingInstance)
        {
            var level = new Level
            {
                Name = input.ReadString(),
                Width = input.ReadInt32(),
                Height = input.ReadInt32(),
                Entities = new Entity[input.ReadInt32()]
            };

            for (int i = 0; i < level.Entities.Length; i++)
            {
                level.Entities[i] = new Entity
                {
                    Name = input.ReadString(),
                    Type = input.ReadString(),
                    Position = input.ReadVector2()
                };
            }

            level.TileLayers = new TileLayer[input.ReadInt32()];

            for (int i = 0; i < level.TileLayers.Length; i++)
            {
                level.TileLayers[i] = new TileLayer
                {
                    IsSolid = input.ReadBoolean(),
                    TileWidth = input.ReadInt32(),
                    TileHeight = input.ReadInt32(),
                    TileSetRelativePath = input.ReadString(),
                    Tiles = new Tile[input.ReadInt32()]
                };

                for (int j = 0; j < level.TileLayers[i].Tiles.Length; j++)
                {
                    level.TileLayers[i].Tiles[j] = new Tile
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
