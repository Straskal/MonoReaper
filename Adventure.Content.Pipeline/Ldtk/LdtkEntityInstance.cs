using System;
using System.Text.Json.Serialization;

namespace Adventure.Content.Pipeline.Ldtk
{
    public sealed class LdtkEntityInstance
    {
        [JsonPropertyName("__identifier")]
        public string Id { get; init; }

        [JsonPropertyName("px")]
        public int[] Position { get; init; }

        [JsonPropertyName("width")]
        public int Width { get; init; }

        [JsonPropertyName("height")]
        public int Height { get; init; }

        [JsonPropertyName("fieldInstances")]
        public LdtkFieldInstance[] FieldInstances { get; init; } = Array.Empty<LdtkFieldInstance>();
    }
}
