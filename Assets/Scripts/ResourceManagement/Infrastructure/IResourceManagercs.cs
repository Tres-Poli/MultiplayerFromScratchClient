using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ResourceManagement
{
    public interface IResourceManager
    {
        UniTask<T> LoadPrefab<T>(string key);
        UniTask<T> LoadConfig<T>(string key) where T : ScriptableObject;
        void Unload(object obj);
    }
}