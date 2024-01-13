using System.Text.Json.Serialization;

namespace Adventure.Content.Pipeline.Ldtk
{
    public class LdtkTileInstance
    {
        [JsonPropertyName("__identifier")]
        public string Id { get; init; }

        [JsonPropertyName("px")]
        public int[] Position { get; init; }

        [JsonPropertyName("src")]
        public int[] Source { get; init; }
    }
}
