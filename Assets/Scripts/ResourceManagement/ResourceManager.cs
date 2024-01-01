using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ResourceManagement
{
    internal sealed class ResourceManager : IResourceManager
    {
        private readonly ILoggerService _logger;

        public ResourceManager(ILoggerService logger)
        {
            _logger = logger;
        }
        
        public async UniTask<T> LoadPrefab<T>(string key)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key);
            await handle;

            if (handle.Result == null)
            {
                _logger.LogError("Desired object is NULL");
                return default;
            }
                
            if (handle.Result.TryGetComponent(out T typed))
            {
                return typed;
            }

            _logger.LogError("Cannot cast loaded object to desired type");
            return default;
        }

        public async UniTask<T> LoadConfig<T>(string key) where T : ScriptableObject
        {
            AsyncOperationHandle<ScriptableObject> handle = Addressables.LoadAssetAsync<ScriptableObject>(key);
            await handle;

            if (handle.Result == null)
            {
                _logger.LogError("Desired object is NULL");
                return null;
            }
                
            if (handle.Result is T typed)
            {
                return typed;
            }
            else
            {
                _logger.LogError("Cannot cast loaded object to desired type");
            }

            return default;
        }

        public void Unload(object obj)
        {
            Addressables.Release(obj);
        }
    }
}