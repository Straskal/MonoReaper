namespace Adventure.Server;

public class Game : IEquatable<Game>
{
    public const int MaxPlayers = 4;

    public string Code { get; set; }
    public string HostConnectionId { get; set; }
    public string[] PlayerConnectionIds { get; set; } = new string[MaxPlayers];
    public int PlayerCount { get; set; }

    public bool Equals(Game other)
    {
        if (other == null) 
        {
            return false;
        }

        if (Code != other.Code) 
        {
            return false;
        }

        if (HostConnectionId != other.HostConnectionId) 
        {
            return false;
        }

        if (PlayerCount != other.PlayerCount) 
        {
            return false;
        }

        for (int i = 0; i < PlayerConnectionIds.Length; i++) 
        {
            if (PlayerConnectionIds[i] != other.PlayerConnectionIds[i]) 
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Game);
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }
}
