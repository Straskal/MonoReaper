using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure.Content
{
    public class LevelReader : ContentTypeReader<Level>
    {
        protected override Level Read(ContentReader input, Level existingInstance)
        {
            var level = new Level();
            level.Name = input.ReadString();
            level.Width = input.ReadInt32();
            level.Height = input.ReadInt32();
            level.Entities = new Entity[input.ReadInt32()];

            for (int i = 0; i < level.Entities.Length; i++) 
            {
                level.Entities[i] = new Entity();
                level.Entities[i].Name = input.ReadString();
                level.Entities[i].Type = input.ReadString();
                level.Entities[i].Position = input.ReadVector2();
            }

            level.TileLayers = new TileLayer[input.ReadInt32()];

            for (int i = 0; i < level.TileLayers.Length; i++)
            {
                level.TileLayers[i] = new TileLayer();
                level.TileLayers[i].IsSolid = input.ReadBoolean();
                level.TileLayers[i].TileWidth = input.ReadInt32();
                level.TileLayers[i].TileHeight = input.ReadInt32();
                level.TileLayers[i].TileSetRelativePath = input.ReadString();
                level.TileLayers[i].Tiles = new Tile[input.ReadInt32()];

                for (int j = 0; j < level.TileLayers[i].Tiles.Length; j++)
                {
                    level.TileLayers[i].Tiles[j] = new Tile();
                    level.TileLayers[i].Tiles[j].Position = input.ReadVector2();
                    level.TileLayers[i].Tiles[j].Source = input.ReadVector2();
                }
            }

            return level;
        }
    }
}
