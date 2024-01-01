using CharacterControllers;
using UnityEngine;

namespace Character
{
    public class SpawnManager : ISpawnManager
    {
        public void SpawnCharacter(CharacterView view)
        {
            view.transform.position = new Vector3(0f, 1f, 0f);
        }
    }
}