using System.Diagnostics;

namespace Tmp.Resource;

public sealed class Res<T>(
    T value,
    ResourcePath? path,
    IReloadSource<T> reloadSource
) : IRes
{
    private T _value = value;
    private bool _disposed;
    
    public ResourcePath? Path { get; } = path;
    
    public void Reload()
    {
        Debug.Assert(!_disposed);
        CastInnerTo<IDisposable>()?.Dispose();
        _value = reloadSource.Reload();
    }
    
    public T Get()
    {
        Debug.Assert(!_disposed);
        return _value;
    }

    public void Dispose()
    {
        Debug.Assert(!_disposed);
        _disposed = true;
        CastInnerTo<IDisposable>()?.Dispose();
    }

    private TCastTarget? CastInnerTo<TCastTarget>()
    {
        return _value is TCastTarget y ? y : default;
    }
}

public interface IRes : IDisposable
{
    void Reload();
}

public interface IReloadSource<out T>
{
    T Reload();
}

public class ReloadConst<T>(T value) : IReloadSource<T>
{
    public T Reload()
    {
        return value;
    }
}

public class ReloadDefault<TResource>(
    IResourceLoader<TResource> loader,
    ResourcePath path
) : IReloadSource<TResource>
{
    public TResource Reload()
    {
        return loader.Load(path);
    }
}

public class ReloadDefault<TResource, TSettings>(
    IResourceLoader<TResource, TSettings> loader,
    ResourcePath path,
    TSettings settings
) : IReloadSource<TResource>
{
    public TResource Reload()
    {
        return loader.Load(path, settings);
    }
}