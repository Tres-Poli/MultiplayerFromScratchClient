using Character;
using UnityEngine;

namespace Character
{
    public interface ISpawnManager
    {
        void SpawnCharacter(CharacterView view, Vector3 position);
    }
}