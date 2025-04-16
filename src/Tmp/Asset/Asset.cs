using System.Diagnostics;

namespace Tmp.Asset;

// TODO find another way to avoid the namespace collision 😥
internal sealed class _Asset<T>(
    T value,
    AssetPath path,
    IReload<T> reload
) : IAsset<T>
{
    private T _value = value;
    private bool _disposed;
    private T _value1;

    public AssetPath Path { get; } = path;
    
    public void Reload()
    {
        Debug.Assert(!_disposed);
        CastInnerTo<IDisposable>()?.Dispose();
        _value = reload.Invoke();
    }
    
    public T Value
    {
        get
        {
            Debug.Assert(!_disposed);
            return _value;   
        }
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

public interface IAsset : IDisposable
{
    AssetPath Path { get; }
    
    void Reload();
}

public interface IAsset<out T> : IAsset
{
    T Value { get; }
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

public class ReloadDefault<TAsset>(
    IAssetsSource subAssets,
    IAssetLoader loader,
    AssetPath path
) : IReload<TAsset>
{
    public TAsset Invoke()
    {
        return loader.Load<TAsset>(path, subAssets);
    }
}
