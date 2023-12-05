using Newtonsoft.Json;

namespace Ldtk
{
    public class LdtkTileInstance
    {
        [JsonProperty("__identifier")]
        public string Id { get; set; }

        [JsonProperty("px")]
        public int[] Position { get; set; }

        [JsonProperty("src")]
        public int[] Source { get; set; }
    }
}
