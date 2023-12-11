using VContainer;

namespace ResourceManagement
{
    public class ResourceManagementScope
    {
        public ResourceManagementScope(IContainerBuilder builder)
        {
            builder.Register<IResourceManager, ResourceManager>(Lifetime.Singleton);
        }
    }
}