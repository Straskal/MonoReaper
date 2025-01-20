using System;
using System.Threading.Tasks;

namespace Adventure.Core;

public class Session
{
    public const int MaxPlayers = 4;

    private readonly NetworkContext networkContext;

    public Session(NetworkContext netManager) 
    {
        networkContext = netManager ?? throw new ArgumentNullException(nameof(netManager));
    }

    /// <summary>
    /// Players who are connected or were connected at some point
    /// </summary>
    public Player[] Players = new Player[MaxPlayers];

    /// <summary>
    /// The session's current scene.
    /// </summary>
    public Scene CurrentScene { get; private set; }

    public Task ConnectAsync() 
    {
        return networkContext.ConnectAsync();
    }

    public void Update() 
    {
    }

    public void Draw() 
    {
    }

    public void ChangeState(Scene state) 
    {
    }
}
