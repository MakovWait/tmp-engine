using Tmp.Resource.Util;

namespace Tmp.Resource;

public interface IResourceLoader
{
    bool MatchPath(ResourcePath path);

    T Load<T>(ResourcePath path, IResourcesSource subResources, IResultMapper<T> target);
    
    T Load<T>(ResourcePath path, IResourcesSource subResources)
    {
        return Load(path, subResources, ResultMapper<T>.AsIs);
    }
}
