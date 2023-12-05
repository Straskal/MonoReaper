using System.Text.Json.Serialization;

namespace Adventure.Content.Pipeline
{
    public class LdtkTileInstance
    {
        [JsonPropertyName("__identifier")]
        public string Id { get; set; }

        [JsonPropertyName("px")]
        public int[] Position { get; set; }

        [JsonPropertyName("src")]
        public int[] Source { get; set; }
    }
}
