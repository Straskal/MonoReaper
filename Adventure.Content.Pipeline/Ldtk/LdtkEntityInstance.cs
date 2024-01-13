using System;
using System.Text.Json.Serialization;

namespace Adventure.Content.Pipeline.Ldtk
{
    public sealed class LdtkEntityInstance
    {
        [JsonPropertyName("__identifier")]
        public string Id { get; set; }

        [JsonPropertyName("px")]
        public int[] Position { get; init; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("fieldInstances")]
        public LdtkFieldInstance[] FieldInstances { get; set; } = Array.Empty<LdtkFieldInstance>();
    }
}
