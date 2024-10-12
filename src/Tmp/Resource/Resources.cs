namespace Tmp.Resource;

public interface IResources : IDisposable, IResourcesSource
{
    public void AddLoader(IResourceLoader loader);
    
    public void Reload(ResourcePath path);
}

// TODO stupid name
public interface IResourcesSource
{
    public Res<T> Load<T>(ResourcePath path);
}

public sealed class Resources : IResources
{
    private readonly Dictionary<ResourcePath, IRes> _resources = new();
    private readonly List<IResourceLoader> _loaders = [];

    public void AddLoader(IResourceLoader loader)
    {
        _loaders.Add(loader);
    }

    public Res<T> Load<T>(ResourcePath path)
    {
        if (_resources.ContainsKey(path))
        {
            return GetLoaded<T>(path);
        }
        foreach (var loader in _loaders)
        {
            if (loader.MatchPath(path))
            {
                return RegisterRes(
                    path,
                    new Res<T>(
                        loader.Load<T>(path, this),
                        path,
                        new ReloadDefault<T>(this, loader, path)
                    )
                );
            }
        }
        
        throw new Exception("Loader not found!!1");
    }

    private Res<T> GetLoaded<T>(ResourcePath path)
    {
        return (Res<T>)_resources[path];
    }
    
    private T RegisterRes<T>(ResourcePath path, T res) where T : IRes
    {
        _resources[path] = res;
        return res;
    }

    public void Reload(ResourcePath path)
    {
        if (_resources.TryGetValue(path, out var resource))
        {
            resource.Reload();
        }
    }

    public void Dispose()
    {
        foreach (var res in _resources.Values)
        {
            res.Dispose();
        }
    }
}
