namespace Adventure.Core;

public class Path
{
    /// <summary>
    /// The waypoint that the path leads to.
    /// </summary>
    public Waypoint Waypoint { get; set; }

    /// <summary>
    /// The distance to the waypoint.
    /// </summary>
    public float Distance { get; set; }
}
