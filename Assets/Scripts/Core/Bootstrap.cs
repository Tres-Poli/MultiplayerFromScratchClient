using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Leopotam.Ecs;
using Messages;
using Networking;
using ResourceManagement;
using Riptide.Utils;
using Systems;
using UI;
using UnityEngine;
using VContainer;

namespace Core
{
    public class Bootstrap : MonoBehaviour, IFixedController, IUpdateController
    {
        [SerializeField] private RectTransform _mainCanvas;
        
        public static EcsWorld World { get; private set; }
        private EcsSystems _updateSystems;
        private EcsSystems _fixedSystems;

        private IFinite _updateSubscription;
        private IFinite _fixedSubscription;
        
        private void Start()
        {
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        }
        
        private void OnDestroy() 
        {
            _updateSystems.Destroy();
            World.Destroy();
            
            _fixedSubscription.Finite();
            _updateSubscription.Finite();
        }

        [Inject]
        public async UniTaskVoid OnStartInject(IReadOnlyList<IInitialize> initializeInstances, IUiManager uiManager, ILoggerService logger, 
            IResourceManager resourceManager, ITickController tickController, IMessageRouter messageRouter, 
            IConnectionSyncManager connectionSyncManager, Camera cameraMain)
        {
            World = new EcsWorld ();
            _updateSystems = new EcsSystems(World)
                .Add(new NetworkSystem(logger, resourceManager, messageRouter, connectionSyncManager))
                .Add(new InputSystem(cameraMain, messageRouter))
                .Add(new CharacterSyncSystem(connectionSyncManager));
                

            _fixedSystems = new EcsSystems(World)
                .Add(new MoveSystem(messageRouter))
                .Add(new MoveReconciliationSystem(messageRouter));
            
            _updateSystems.Init();
            _fixedSystems.Init();
            
            await uiManager.Initialize(_mainCanvas);
            uiManager.AddScreen<ILoggerController>(ScreenType.Logger);

            for (int i = 0; i < initializeInstances.Count; i++)
            {
                await initializeInstances[i].Initialize();
            }

            _fixedSubscription = tickController.AddController((IFixedController)this);
            _updateSubscription = tickController.AddController((IUpdateController)this);
            
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void UpdateFixedController(float deltaTime)
        {
            _fixedSystems.Run();
        }

        public void UpdateController(float deltaTime)
        {
            _updateSystems.Run();
        }
    }
}