using System;
using System.Collections.Generic;

namespace Adventure
{
    public class Trip
    {
        public int Time { get; set; }
        public int DistanceTraveled { get; set; }
        public int[] Path { get; set; } = Array.Empty<int>();
        public kBiome Biome { get; set; }
        public kWeather Weather { get; set; }
        public int Gold { get; set; }
        public int Food { get; set; }
        public Dictionary<string, string> Variables { get; set; } = new();

        public enum kBiome 
        {
            Woodland,
            Mountains,
            Swamp
        }

        public enum kWeather 
        {
            Sunny,
            Overcast,
            Rain,
            ThunderStorm,
            Snow,
            SnowStorm
        }
    }
}
