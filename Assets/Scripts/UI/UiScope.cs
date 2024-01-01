using VContainer;

namespace UI
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