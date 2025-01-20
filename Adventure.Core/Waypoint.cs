using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Adventure.Core;

public enum WaypointType 
{
    Landmark,
    Settlement
}

public class Waypoint
{
    /// <summary>
    /// The type of waypoint.
    /// </summary>
    public WaypointType Type { get; set; }

    /// <summary>
    /// Waypoint position on the map.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// The paths that can be taken from this waypoint.
    /// </summary>
    public List<Path> Paths { get; set; }
}
