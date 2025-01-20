using Microsoft.AspNetCore.SignalR.Client;
using System.Threading;
using System.Threading.Tasks;

namespace Adventure.Core;

public class NetworkContext
{
    private readonly HubConnection _connection;

    public NetworkContext() 
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("")
            .WithAutomaticReconnect()
            .Build();

        // Set up message handlers
    }

    public bool IsConnected { get; private set; }

    public async Task ConnectAsync(CancellationToken cancellationToken = default) 
    {
        await _connection.StartAsync(cancellationToken).ConfigureAwait(true);

        IsConnected = true;
    }

    public void Send() 
    {

    }
}
