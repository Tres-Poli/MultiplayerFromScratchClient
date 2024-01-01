using Cysharp.Threading.Tasks;

namespace Core
{
    public interface IInitialize
    {
        UniTask Initialize();
    }
}