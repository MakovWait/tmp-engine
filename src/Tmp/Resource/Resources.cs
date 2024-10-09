using System.Diagnostics;

namespace Tmp.Resource;

public interface IResources : IDisposable
{
    public void AddLoader(IResourceLoader loader);
    
    public Res<T> Load<T>(ResourcePath path);
    
    public Res<TResource> Load<TResource, TSettings>(ResourcePath path, TSettings settings);
    
    public void Reload(ResourcePath path);
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
        Debug.Assert(!_resources.ContainsKey(path));
        foreach (var loader in _loaders)
        {
            if (loader.MatchPath(path) && loader is IResourceLoader<T> foundLoader)
            {
                return RegisterRes(
                    path,
                    new Res<T>(
                        foundLoader.Load(path),
                        path,
                        new ReloadDefault<T>(foundLoader, path)
                    )
                );
            }
        }
        
        throw new Exception("Loader not found!!1");
    }

    public Res<TResource> Load<TResource, TSettings>(ResourcePath path, TSettings settings)
    {
        Debug.Assert(!_resources.ContainsKey(path));
        foreach (var loader in _loaders)
        {
            if (loader.MatchPath(path) && loader is IResourceLoader<TResource, TSettings> foundLoader)
            {
                return RegisterRes(
                    path,
                    new Res<TResource>(
                        foundLoader.Load(path),
                        path,
                        new ReloadDefault<TResource, TSettings>(foundLoader, path, settings)
                    )
                );
            }
        }
        
        throw new Exception("Loader not found!!1");
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
