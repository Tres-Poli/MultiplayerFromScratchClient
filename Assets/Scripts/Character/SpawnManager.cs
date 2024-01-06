using UnityEngine;

namespace Character
{
    public class SpawnManager : ISpawnManager
    {
        public void SpawnCharacter(CharacterView view, Vector3 positions)
        {
            view.transform.position = positions;
        }
    }
}