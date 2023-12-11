using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using ResourceManagement;
using UnityEngine;

namespace Common.UI
{
    internal sealed class UiManager : IUiManager
    {
        private const string ScreenConfig = "ScreenConfig";
        
        private readonly ILoggerService _loggerService;
        private readonly IResourceManager _resourceManager;
        private readonly Dictionary<ScreenType, IUiFactory> _factoryMap;

        private ScreenConfig _config;
        private RectTransform _mainCanvas;
        
        public UiManager(IReadOnlyList<IUiFactory> factories, ILoggerService loggerService, IResourceManager resourceManager)
        {
            _loggerService = loggerService;
            _resourceManager = resourceManager;
            _factoryMap = new Dictionary<ScreenType, IUiFactory>();

            for (int i = 0; i < factories.Count; i++)
            {
                IUiFactory currFactory = factories[i];
                _factoryMap.Add(currFactory.ScreenType, currFactory);
            }
        }

        public async UniTask Initialize(RectTransform canvas)
        {
            _mainCanvas = canvas;
            _config = await _resourceManager.LoadConfig<ScreenConfig>(ScreenConfig);
        }
        
        public T AddScreen<T>(ScreenType type) where T : IUiController
        {
            if (_config == null)
            {
                _loggerService.LogError($"Cannot add screen ({type}) before config is loaded");
            }
            
            if (_factoryMap.TryGetValue(type, out IUiFactory factory) && _config.UiPrefabs.TryGetValue(type, out GameObject prefab))
            {
                IUiController controller = factory.AddUiScreen(_mainCanvas, prefab);
                if (controller is T concreteController)
                {
                    return concreteController;
                }

                _loggerService.LogError($"Invalid cast for UI type {type}");
                controller.Finite();
            }

            _loggerService.LogError($"No factory or prefab for UI type {type}");
            return default;
        }
    }
}