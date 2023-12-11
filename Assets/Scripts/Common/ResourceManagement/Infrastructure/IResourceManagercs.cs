using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ResourceManagement
{
    public interface IResourceManager
    {
        UniTask<T> LoadConfig<T>(string key) where T : ScriptableObject;
        void Unload(object obj);
    }
}