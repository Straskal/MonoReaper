﻿using System.Text.Json.Serialization;

namespace Adventure.Content.Pipeline
{
    public sealed class LdtkEntityInstance
    {
        [JsonPropertyName("__identifier")]
        public string Id { get; set; }

        [JsonPropertyName("px")]
        public int[] Position { get; set; }
    }
}