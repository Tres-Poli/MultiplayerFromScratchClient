using Leopotam.Ecs;
using ResourceManagement;
using Components;
using Configs;
using Core;
using Cysharp.Threading.Tasks;
using DataStructures;
using Systems;
using UnityEngine;

namespace Character
{
    public class CharacterFactory : ICharacterFactory, IInitialize
    {
        private const string PlayerPrefabKey = "Player";
        private const string PlayerConfigKey = "PlayerConfig";
        
        private readonly IResourceManager _resourceManager;
        private readonly ISpawnManager _spawnManager;
        private CharacterConfig _config;

        private CharacterView _characterPrefab;

        public CharacterFactory(IResourceManager resourceManager, ISpawnManager spawnManager)
        {
            _resourceManager = resourceManager;
            _spawnManager = spawnManager;
        }

        public async UniTask Initialize()
        {
            _config = await _resourceManager.LoadConfig<CharacterConfig>(PlayerConfigKey);
            _characterPrefab = await _resourceManager.LoadPrefab<CharacterView>(PlayerPrefabKey);
        }
        
        public CharacterView CreateCharacter(ushort id, CharacterType type, Vector3 position)
        {
            CharacterView charInstance = Object.Instantiate(_characterPrefab);
            EcsEntity entity = Bootstrap.World.NewEntity();
            SpeedComponent speedComponent = new SpeedComponent()
            {
                Speed = _config.Speed
            };

            BodyComponent bodyComponent = new BodyComponent()
            {
                Body = charInstance.Body
            };

            IdComponent idComponent = new IdComponent()
            {
                Id = id
            };
            

            entity.Replace(speedComponent);
            entity.Replace(bodyComponent);
            entity.Replace(idComponent);


            if (id == NetworkSystem.Client.Id)
            {
                PlayerInputComponent inputComponent = new PlayerInputComponent();
                MoveComponent moveComponent = new MoveComponent();
                entity.Replace(inputComponent);
                entity.Replace(moveComponent);
            }
            else
            {
                InterpolateMoveComponent interpolateMoveComponent = new InterpolateMoveComponent();
                interpolateMoveComponent.PositionsBuffer = new CircularBuffer<Vector3>(16, 2);
                entity.Replace(interpolateMoveComponent);
            }

            _spawnManager.SpawnCharacter(charInstance, position);
            return charInstance;
        }
    }
}