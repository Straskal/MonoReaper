﻿using System.Text.Json.Serialization;

namespace Adventure.Content.Pipeline.Ldtk
{
    public sealed class LdtkFieldInstance
    {
        [JsonPropertyName("__identifier")]
        public string Id { get; set; }

        [JsonPropertyName("__value")]
        public string Value { get; set; }
    }
}
