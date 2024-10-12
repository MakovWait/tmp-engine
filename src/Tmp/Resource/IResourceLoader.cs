namespace Tmp.Resource;

public interface IResourceLoader
{
    bool MatchPath(ResourcePath path);
    
    T Load<T>(ResourcePath path, IResourcesSource subResources);
}
