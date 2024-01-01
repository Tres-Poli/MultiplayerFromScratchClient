using System.Collections.Generic;
using CharacterControllers;
using Cysharp.Threading.Tasks;
using Input;
using Leopotam.Ecs;
using Messages;
using ResourceManagement;
using Riptide.Utils;
using Systems;
using UI;
using UnityEngine;
using VContainer;

namespace Core
{
    public class Bootstrap : MonoBehaviour, IUpdateController
    {
        [SerializeField] private RectTransform _mainCanvas;
        
        public static EcsWorld World { get; private set; }
        private EcsSystems _systems;

        private IFinite _updateSubscription;
        
        private void Start()
        {
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        }
        
        private void OnDestroy() 
        {
            _systems.Destroy();
            World.Destroy();
            
            _updateSubscription.Finite();
        }

        [Inject]
        public async UniTaskVoid OnStartInject(IReadOnlyList<IInitialize> initializeInstances, IUiManager uiManager, ILoggerService logger, 
            IResourceManager resourceManager, ITickController tickController, IMessageRouter messageRouter, IPlayerInput playerInput, 
            ICharacterFactory characterFactory)
        {
            World = new EcsWorld ();
            _systems = new EcsSystems(World)
                .Add(new MoveSystem(messageRouter, playerInput))
                .Add(new NetworkSystem(logger, resourceManager, messageRouter, characterFactory));
            
            _systems.Init();
            
            await uiManager.Initialize(_mainCanvas);
            uiManager.AddScreen<ILoggerController>(ScreenType.Logger);

            for (int i = 0; i < initializeInstances.Count; i++)
            {
                await initializeInstances[i].Initialize();
            }

            _updateSubscription = tickController.AddController(this);
        }

        public void UpdateController(float deltaTime)
        {
            _systems.Run();
        }
    }
}