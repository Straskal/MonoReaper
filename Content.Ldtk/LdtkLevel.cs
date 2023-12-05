using Newtonsoft.Json;

namespace Ldtk
{
    public class LdtkLevel
    {
        public string Identifier { get; set; }

        [JsonProperty("pxWid")]
        public int Width { get; set; }

        [JsonProperty("pxHei")]
        public int Height { get; set; }

        public LdtkLayerInstance[] LayerInstances { get; set; }
    }
}
