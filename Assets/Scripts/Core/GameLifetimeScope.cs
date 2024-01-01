﻿using Character;
using CharacterControllers;
using Input;
using Messages;
using UI;
using ResourceManagement;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core
{
    internal sealed class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            GameObject tickControllerObject = new GameObject("TickController");
            TickController tickController = tickControllerObject.AddComponent<TickController>();
            builder.RegisterInstance<ITickController>(tickController);

            builder.RegisterInstance<Camera>(Camera.main);
            builder.Register<IPlayerInput, PlayerInput>(Lifetime.Singleton);
            builder.Register<ILoggerService, Logger>(Lifetime.Singleton);
            
            builder.Register<IResourceManager, ResourceManager>(Lifetime.Singleton);
            builder.Register<ICharacterFactory, CharacterFactory>(Lifetime.Singleton).As<IInitialize>();
            builder.Register<ISpawnManager, SpawnManager>(Lifetime.Singleton);
            
            // UI
            builder.Register<IUiFactory, LoggerFactory>(Lifetime.Scoped);
            builder.Register<IUiManager, UiManager>(Lifetime.Singleton);
            
            // Networking
            builder.Register<IMessageRouter, MessageRouter>(Lifetime.Singleton);
        }
    }
}