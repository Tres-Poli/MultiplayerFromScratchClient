using Common.UI.Logger;
using VContainer;

namespace Common.UI
{
    public class UiScope
    {
        public UiScope(IContainerBuilder builder)
        {
            builder.Register<IUiFactory, LoggerFactory>(Lifetime.Scoped);
            
            builder.Register<IUiManager, UiManager>(Lifetime.Singleton);
        }
    }
}