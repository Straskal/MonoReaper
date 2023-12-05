using Newtonsoft.Json;

namespace Ldtk
{
    public class LdtkEntityInstance
    {
        [JsonProperty("__identifier")]
        public string Id { get; set; }

        [JsonProperty("px")]
        public int[] Position { get; set; }
    }
}
