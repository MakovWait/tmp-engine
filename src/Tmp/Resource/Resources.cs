using Tmp.Resource.Util;

namespace Tmp.Resource;

public interface IResources : IDisposable, IResourcesSource
{
    public void AddLoader(IResourceLoader loader);

    public IRes<T> AddDyn<T>(string name, T resource);
    
    public void Reload(ResourcePath path);
    
    public IRes Load(ResourcePath path);
}

// TODO stupid name
public interface IResourcesSource
{
    public IRes<T> Load<T>(ResourcePath path);
}

public sealed class Resources : IResources
{
    private readonly Dictionary<ResourcePath, IRes> _resources = new();
    private readonly List<IResourceLoader> _loaders = [];

    public void AddLoader(IResourceLoader loader)
    {
        _loaders.Add(loader);
    }

    public IRes<T> AddDyn<T>(string name, T resource)
    {
        ResourcePath path = Path.Join("dyn://", name);
        return RegisterRes(
            path,
            new Res<T>(
                resource,
                path,
                new ReloadConst<T>(resource)
            )
        );
    }

    public IRes<T> Load<T>(ResourcePath path)
    {
        if (_resources.ContainsKey(path))
        {
            return GetLoaded<T>(path);
        }
        var res = Load(path);
        return (IRes<T>)res;
    }

    public IRes Load(ResourcePath path)
    {
        if (_resources.ContainsKey(path))
        {
            return GetLoaded(path);
        }
        return Load(path, loader => new ResultMapperUntypedRes(this, path, loader));
    }

    private T Load<T>(ResourcePath path, Func<IResourceLoader, IResultMapper<T>> mapperCtor)
    {
        if (path.IsDyn)
        {
            throw new ArgumentException("The dyn resource should be added manually first");
        }
        foreach (var loader in _loaders)
        {
            if (loader.MatchPath(path))
            {
                return loader.Load(path, this, mapperCtor(loader));
            }
        }
        throw new Exception("Loader not found!!1");
    }

    private IRes GetLoaded(ResourcePath path)
    {
        return _resources[path];
    }
    
    private IRes<T> GetLoaded<T>(ResourcePath path)
    {
        var res = _resources[path];
        return (IRes<T>)res;
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

    private class ResultMapperUntypedRes(Resources self, ResourcePath path, IResourceLoader loader) : IResultMapper<IRes>
    {
        public IRes Map<T>(T value)
        {
            return self.RegisterRes(path, new Res<T>(
                value,
                path,
                new ReloadDefault<T>(self, loader, path)
            ));
        }
    }
}
