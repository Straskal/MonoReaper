using System.Text.Json.Serialization;

namespace Adventure.Content.Pipeline.Ldtk
{
    public sealed class LdtkLayerInstance
    {
        [JsonPropertyName("__identifier")]
        public string Identifier { get; init; }

        [JsonPropertyName("__cWid")]
        public int CellWidth { get; init; }

        [JsonPropertyName("__cHei")]
        public int CellHeight { get; init; }

        [JsonPropertyName("__gridSize")]
        public int GridSize { get; init; }

        [JsonPropertyName("__tilesetRelPath")]
        public string TileSetRelativePath { get; init; }

        [JsonPropertyName("entityInstances")]
        public LdtkEntityInstance[] EntityInstances { get; init; }

        [JsonPropertyName("gridTiles")]
        public LdtkTileInstance[] GridTiles { get; init; }

        [JsonPropertyName("intGridCsv")]
        public int[] IntGridCsv { get; init; }
    }
}
