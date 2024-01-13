using Microsoft.Xna.Framework.Content.Pipeline;
using System.Text.Json.Serialization;

namespace Adventure.Content.Pipeline.Ldtk
{
    public sealed class LdtkLayerInstance
    {
        [JsonPropertyName("__identifier")]
        public string Identifier { get; set; }

        [JsonPropertyName("__cWid")]
        public int CellWidth { get; set; }

        [JsonPropertyName("__cHei")]
        public int CellHeight { get; set; }

        [JsonPropertyName("__gridSize")]
        public int GridSize { get; set; }

        [JsonPropertyName("__tilesetRelPath")]
        public string TileSetRelativePath { get; set; }

        [JsonPropertyName("entityInstances")]
        public LdtkEntityInstance[] EntityInstances { get; set; }

        [JsonPropertyName("gridTiles")]
        public LdtkTileInstance[] GridTiles { get; set; }

        [JsonPropertyName("intGridCsv")]
        public int[] IntGridCsv { get; set; }
    }
}
