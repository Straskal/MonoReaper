using System.Threading.Tasks;
using System.Threading;

namespace Adventure.Core;

public interface INetManager
{
    Task ConnectAsync(CancellationToken cancellationToken = default);

    void Send();
}
