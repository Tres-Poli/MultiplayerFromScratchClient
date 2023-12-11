using Common.UI;
using Common.UI.Controllers;
using Cysharp.Threading.Tasks;
using NetworkManagement;
using Riptide.Utils;
using UnityEngine;
using VContainer;

namespace Core
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private RectTransform _mainCanvas;
        
        private void Start()
        {
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        }

        [Inject]
        public async UniTaskVoid OnStartInject(INetworkManager networkManager, IUiManager uiManager, ILoggerService logger)
        {
            await uiManager.Initialize(_mainCanvas);
            uiManager.AddScreen<ILoggerController>(ScreenType.Logger);
            
            logger.Log("Test");
        }
    }
}