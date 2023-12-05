using Newtonsoft.Json;

namespace Ldtk
{
    public class LdtkLayerInstance
    {

        [JsonProperty("__identifier")]
        public string Identifier { get; set; }
        [JsonProperty("__cWid")]
        public int CellWidth { get; set; }
        [JsonProperty("__cHei")]
        public int CellHeight { get; set; }
        [JsonProperty("__gridSize")]
        public int GridSize { get; set; }
        [JsonProperty("__tilesetRelPath")]
        public string TileSetRelativePath { get; set; }
        public LdtkEntityInstance[] EntityInstances { get; set; }
        public LdtkTileInstance[] GridTiles { get; set; }
        public int[] IntGridCsv { get; set; }
    }
}
