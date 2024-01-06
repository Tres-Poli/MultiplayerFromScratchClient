using UnityEngine;

namespace Character
{
    public interface ICharacterFactory
    {
        CharacterView CreateCharacter(ushort id, CharacterType type, Vector3 position);
    }
}