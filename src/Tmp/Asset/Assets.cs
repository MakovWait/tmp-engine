using Tmp.Asset.Util;

namespace Tmp.Asset;

public interface IAssets : IDisposable, IAssetsSource
{
    public void AddLoader(IAssetLoader loader);

    public IAss<T> AddMem<T>(string name, T asset);
    
    public void Reload(AssetPath path);
    
    public IAss Load(AssetPath path);
}

// TODO stupid name
public interface IAssetsSource
{
    public IAss<T> Load<T>(AssetPath path);
}

public sealed class Assets : IAssets
{
    private readonly Dictionary<AssetPath, IAss> _assets = new();
    private readonly List<IAssetLoader> _loaders = [];

    public void AddLoader(IAssetLoader loader)
    {
        _loaders.Add(loader);
    }

    public IAss<T> AddMem<T>(string name, T asset)
    {
        AssetPath path = Path.Join("mem://", name);
        return RegisterAsset(
            path,
            new Ass<T>(
                asset,
                path,
                new ReloadConst<T>(asset)
            )
        );
    }

    public IAss<T> Load<T>(AssetPath path)
    {
        if (_assets.ContainsKey(path))
        {
            return GetLoaded<T>(path);
        }
        var asset = Load(path);
        return (IAss<T>)asset;
    }

    public IAss Load(AssetPath path)
    {
        if (_assets.ContainsKey(path))
        {
            return GetLoaded(path);
        }
        return Load(path, loader => new ResultMapperUntypedRes(this, path, loader));
    }

    private T Load<T>(AssetPath path, Func<IAssetLoader, IResultMapper<T>> mapperCtor)
    {
        if (path.IsMem)
        {
            throw new ArgumentException("The mem asset should be added manually first");
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

    private IAss GetLoaded(AssetPath path)
    {
        return _assets[path];
    }
    
    private IAss<T> GetLoaded<T>(AssetPath path)
    {
        var asset = _assets[path];
        return (IAss<T>)asset;
    }
    
    private T RegisterAsset<T>(AssetPath path, T asset) where T : IAss
    {
        _assets[path] = asset;
        return asset;
    }

    public void Reload(AssetPath path)
    {
        if (_assets.TryGetValue(path, out var asset))
        {
            asset.Reload();
        }
    }

    public void Dispose()
    {
        foreach (var asset in _assets.Values)
        {
            asset.Dispose();
        }
    }

    private class ResultMapperUntypedRes(Assets self, AssetPath path, IAssetLoader loader) : IResultMapper<IAss>
    {
        public IAss Map<T>(T value)
        {
            return self.RegisterAsset(path, new Ass<T>(
                value,
                path,
                new ReloadDefault<T>(self, loader, path)
            ));
        }
    }
}
