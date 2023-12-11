using VContainer;

namespace NetworkManagement
{
    public sealed class NetworkManagementScope
    {
        public NetworkManagementScope(IContainerBuilder builder)
        {
            builder.Register<INetworkManager, NetworkManager>(Lifetime.Singleton);
        }
    }
}