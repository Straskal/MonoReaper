using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Adventure.Server;

public class GameHub : Hub
{
    private static readonly ConcurrentDictionary<string, Game> games = [];
    private static readonly ConcurrentDictionary<string, string> gamesByConnectionId = [];
    private static readonly object recentRoomCodesLock = new();
    private static readonly HashSet<string> recentRoomCodes = [];
    private static readonly Queue<string> recentRoomCodesQueue = [];
    private static readonly Random random = new();

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();

        if (httpContext.Request.Query.TryGetValue("room", out var roomCode))
        {
            var attempts = 0;

            while (attempts < 100)
            {
                if (!(await TryAddPlayerToGameAsync(roomCode)))
                {
                    return;
                }

                attempts++;
            }

            Context.Abort();
        }
        else
        {
            var attempts = 0;

            while (attempts < 100)
            {
                roomCode = GenerateRoomCode();

                if (!string.IsNullOrWhiteSpace(roomCode))
                {
                    var game = new Game
                    {
                        Code = roomCode,
                        HostConnectionId = Context.ConnectionId
                    };

                    if (games.TryAdd(roomCode, game))
                    {
                        return;
                    }
                }

                attempts++;
            }
        }
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var attempts = 0;

        while (attempts < 100)
        {
            if (await TryRemovePlayerFromGameAsync()) 
            {
                return;
            }

            attempts++;
        }
    }

    private static string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        var code = new char[4];
        var attempts = 0;

        while (attempts < 100)
        {
            for (int i = 0; i < code.Length; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }

            var codeStr = new string(code);

            lock (recentRoomCodesLock)
            {
                if (recentRoomCodes.Add(codeStr))
                {
                    recentRoomCodesQueue.Enqueue(codeStr);

                    if (recentRoomCodesQueue.Count > 10_000)
                    {
                        recentRoomCodesQueue.Dequeue();
                    }

                    return codeStr;
                }
            }

            attempts++;
        }

        return null;
    }

    private async Task<bool> TryAddPlayerToGameAsync(string roomCode)
    {
        if (!games.TryGetValue(roomCode, out var sourceGame))
        {
            return false;
        }

        if (sourceGame.PlayerCount == Game.MaxPlayers)
        {
            return false;
        }

        var index = -1;

        for (int i = 0; i < sourceGame.PlayerConnectionIds.Length; i++)
        {
            if (sourceGame.PlayerConnectionIds[i] == null)
            {
                index = i;
                break;
            }
        }

        var updatedGame = new Game
        {
            Code = sourceGame.Code,
            HostConnectionId = sourceGame.HostConnectionId,
            PlayerConnectionIds = [.. sourceGame.PlayerConnectionIds],
            PlayerCount = sourceGame.PlayerCount
        };

        updatedGame.PlayerConnectionIds[index] = Context.ConnectionId;
        updatedGame.PlayerCount++;

        if (!games.TryUpdate(roomCode, updatedGame, sourceGame))
        {
            return false;
        }

        gamesByConnectionId[Context.ConnectionId] = roomCode;

        await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
        await Clients.Client(updatedGame.HostConnectionId).SendAsync("PlayerJoined", updatedGame.PlayerCount - 1);

        return true;
    }

    private async Task<bool> TryRemovePlayerFromGameAsync()
    {
        if (!gamesByConnectionId.TryGetValue(Context.ConnectionId, out var roomCode))
        {
            return true;
        }

        if (!games.TryGetValue(roomCode, out var sourceGame))
        {
            return true;
        }

        var index = -1;

        for (int i = 0; i < sourceGame.PlayerConnectionIds.Length; i++)
        {
            if (sourceGame.PlayerConnectionIds[i] == Context.ConnectionId)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            return true;
        }

        var updatedGame = new Game
        {
            Code = sourceGame.Code,
            HostConnectionId = sourceGame.HostConnectionId,
            PlayerConnectionIds = [.. sourceGame.PlayerConnectionIds],
        };

        updatedGame.PlayerConnectionIds[index] = null;
        updatedGame.PlayerCount--;

        if (!games.TryUpdate(roomCode, updatedGame, sourceGame))
        {
            return false;
        }

        gamesByConnectionId[Context.ConnectionId] = roomCode;

        await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
        await Clients.Client(updatedGame.HostConnectionId).SendAsync("PlayerJoined", updatedGame.PlayerCount - 1);

        return true;
    }
}
