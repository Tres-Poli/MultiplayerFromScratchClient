using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Character
{
    public sealed class CharacterProvider : ICharacterProvider, IFinite
    {
        private readonly ICharacterFactory _factory;

        public event Action OnCharacterSpawned;

        private Dictionary<ushort, CharacterView> _charactersViewMap;

        public CharacterProvider(ICharacterFactory factory)
        {
            _factory = factory;
            _charactersViewMap = new Dictionary<ushort, CharacterView>(16);
        }

        public void CreateCharacter(ushort id, CharacterType characterType, Vector3 position)
        {
            if (_charactersViewMap.ContainsKey(id))
            {
                Debug.LogError($"Character with id {id} is already created");    
            }
            
            CharacterView view = _factory.CreateCharacter(id, characterType, position);
            _charactersViewMap[id] = view;
            OnCharacterSpawned?.Invoke();
        }

        public void RemoveCharacter(ushort id)
        {
            if (_charactersViewMap.TryGetValue(id, out CharacterView view))
            {
                UnityEngine.Object.Destroy(view);
            }
        }

        public void Finite()
        {
            OnCharacterSpawned = null;
        }
    }
}