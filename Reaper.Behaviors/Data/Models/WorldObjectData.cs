using Reaper.Engine;
using System.Collections.Generic;

namespace Reaper.Gameplay.Data.Models
{
    public class WorldObjectData
    {
        public string Name { get; set; }
        public SpatialType Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public List<Behavior> Behaviors { get; set; }
    }
}
