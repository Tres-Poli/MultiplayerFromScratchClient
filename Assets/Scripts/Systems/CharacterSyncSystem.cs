using Components;
using Leopotam.Ecs;
using Networking;

namespace Systems
{
    public class CharacterSyncSystem : IEcsDestroySystem
    {
        private readonly IConnectionSyncManager _connectionSyncManager;
        private EcsFilter<IdComponent> _filter;
        
        public CharacterSyncSystem(IConnectionSyncManager connectionSyncManager)
        {
            _connectionSyncManager = connectionSyncManager;
            _connectionSyncManager.OnClientDisconnected += ClientDisconnected_Callback;
        }

        private void ClientDisconnected_Callback(ushort id)
        {
            for (int i = 0; i < _filter.GetEntitiesCount(); i++)
            {
                ref IdComponent idComponent = ref _filter.Get1(i);
                if (idComponent.Id == id)
                {
                    _filter.GetEntity(i).Destroy();
                }
            }
        }

        public void Destroy()
        {
            _connectionSyncManager.OnClientDisconnected -= ClientDisconnected_Callback;
        }
    }
}