using Character;
using Networking;
using Riptide;
using UnityEngine;

namespace MessageStructs
{
    public struct CharactersSyncMessage : IMessageSerializable
    {
        public CharacterSyncElem[] Characters;
        public int Size;
        
        public void Serialize(Message message)
        {
            message.AddInt(Size);
            for (int i = 0; i < Size; i++)
            {
                CharacterSyncElem character = Characters[i];
                message.AddUShort(character.Id);
                message.AddUShort((ushort)character.Type);
                message.AddFloat(character.Position.x);
                message.AddFloat(character.Position.y);
                message.AddFloat(character.Position.z);
            }
        }

        public void Deserialize(Message message)
        {
            Size = message.GetInt();
            Characters = new CharacterSyncElem[Size];
            for (int i = 0; i < Size; i++)
            {
                ushort id = message.GetUShort();
                CharacterType type = (CharacterType)message.GetUShort();
                Vector3 position = new Vector3(message.GetFloat(), message.GetFloat(), message.GetFloat());
                CharacterSyncElem character = new CharacterSyncElem(id, type, position);
                Characters[i] = character;
            }
        }
    }
}