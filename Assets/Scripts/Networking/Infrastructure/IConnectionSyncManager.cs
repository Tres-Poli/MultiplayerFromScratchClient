using System;

namespace Networking
{
    public interface IConnectionSyncManager
    {
        event Action<ushort> OnClientDisconnected;
    }
}