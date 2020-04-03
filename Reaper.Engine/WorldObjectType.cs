using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Reaper.Engine
{
    [DataContract]
    public class WorldObjectType
    {
        [DataMember(Order = 0)]
        public string Name { get; set; } = string.Empty;

        [DataMember(Order = 10)]
        public int Width { get; set; } = 0;

        [DataMember(Order = 20)]
        public int Height { get; set; } = 0;

        [DataMember(Order = 30)]
        public Point Origin { get; set; } = Point.Zero;

        [DataMember(Order = 40)]
        public SpatialType SpatialType { get; set; } = SpatialType.Overlap;

        [DataMember(Order = 50)]
        public List<Behavior> Behaviors { get; set; } = new List<Behavior>();
    }
}
