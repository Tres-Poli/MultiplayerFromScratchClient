using Common.UI;
using NetworkManagement;
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

            builder.Register<ILoggerService, Logger>(Lifetime.Singleton);
            
            new ResourceManagementScope(builder);
            new NetworkManagementScope(builder);
            new UiScope(builder);
        }
    }
}