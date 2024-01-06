using Character;
using UnityEngine;

namespace Networking
{
    public struct CharacterSyncElem
    {
        public CharacterSyncElem(ushort id, CharacterType type, Vector3 position)
        {
            Id = id;
            Type = type;
            Position = position;
        }
        
        public ushort Id;
        public CharacterType Type;
        public Vector3 Position;
    }
}