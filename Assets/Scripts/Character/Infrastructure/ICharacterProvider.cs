using System;
using UnityEngine;

namespace Character
{
    public interface ICharacterProvider
    {
        event Action OnCharacterSpawned;
        
        void CreateCharacter(ushort id, CharacterType characterType, Vector3 position);
        void RemoveCharacter(ushort id);
    }
}