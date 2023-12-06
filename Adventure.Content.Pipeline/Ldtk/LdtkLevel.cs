using System.Text.Json.Serialization;

namespace Adventure.Content.Pipeline.Ldtk
{
    public class LdtkLevel
    {
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        [JsonPropertyName("pxWid")]
        public int Width { get; set; }

        [JsonPropertyName("pxHei")]
        public int Height { get; set; }

        [JsonPropertyName("layerInstances")]
        public LdtkLayerInstance[] LayerInstances { get; set; }
    }
}
