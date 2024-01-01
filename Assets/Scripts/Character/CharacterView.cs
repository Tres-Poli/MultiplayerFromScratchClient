using UnityEngine;

namespace Character
{
    public class CharacterView : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody Body { get; private set; }
    }
}