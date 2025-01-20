namespace Adventure.Core;

public class Player
{
    /// <summary>
    /// Player ID (1-4)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Player name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// True if the player is connected
    /// </summary>
    public bool IsConnected { get; set; }
}
