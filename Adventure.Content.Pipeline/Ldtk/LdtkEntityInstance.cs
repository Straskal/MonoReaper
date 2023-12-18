﻿using System;
using System.Text.Json.Serialization;

namespace Adventure.Content.Pipeline.Ldtk
{
    public sealed class LdtkEntityInstance
    {
        [JsonPropertyName("__identifier")]
        public string Id { get; set; }

        [JsonPropertyName("px")]
        public int[] Position { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("fieldInstances")]
        public FieldInstance[] FieldInstances { get; set; } = Array.Empty<FieldInstance>();
    }
}
