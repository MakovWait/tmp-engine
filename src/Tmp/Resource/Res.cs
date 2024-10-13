using System.Diagnostics;

namespace Tmp.Resource;

internal sealed class Res<T>(
    T value,
    ResourcePath path,
    IReload<T> reload
) : IRes<T>
{
    private T _value = value;
    private bool _disposed;
    
    public ResourcePath Path { get; } = path;
    
    public void Reload()
    {
        Debug.Assert(!_disposed);
        CastInnerTo<IDisposable>()?.Dispose();
        _value = reload.Invoke();
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
    ResourcePath Path { get; }
    
    void Reload();
}

public interface IRes<out T> : IRes
{
    T Get();
}

public interface IReload<out T>
{
    T Invoke();
}

public class ReloadConst<T>(T value) : IReload<T>
{
    public T Invoke()
    {
        return value;
    }
}

public class ReloadDefault<TResource>(
    IResourcesSource subResources,
    IResourceLoader loader,
    ResourcePath path
) : IReload<TResource>
{
    public TResource Invoke()
    {
        return loader.Load<TResource>(path, subResources);
    }
}
