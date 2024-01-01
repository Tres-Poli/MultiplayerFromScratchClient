using Cysharp.Threading.Tasks;

namespace CharacterControllers
{
    public interface ICharacterFactory
    {
        UniTaskVoid CreateCharacter(ushort id);
    }
}