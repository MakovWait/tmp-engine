namespace Tmp.Resource;

public interface IResourceLoader
{
    bool MatchPath(ResourcePath path);
}

public interface IResourceLoader<out T>
{
    T Load(ResourcePath path);
}

public interface IResourceLoader<out TResource, in TSettings> : IResourceLoader, IResourceLoader<TResource>
{
    TResource Load(ResourcePath path, TSettings settings);
}

public readonly struct NoSettings;
