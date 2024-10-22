using Tmp.Asset.Util;
using Tmp.Math;
using Tommy;

namespace Tmp.Asset.Format.Text;

public class AssetLoaderText : IAssetLoader
{
    private readonly List<IAssetDeserializer> _deserializers = [];

    public void AddDeserializer(IAssetDeserializer deserializer)
    {
        _deserializers.Add(deserializer);
    }
    
    public bool MatchPath(AssetPath path)
    {
        return path.Extension == "jass";
    }

    public T Load<T>(AssetPath path, IAssetsSource subAssets, IResultMapper<T> resultMapper)
    {
        using var reader = File.OpenText(path.FilePath);
        var table = TOML.Parse(reader);
        var node = path.HasLabel ? table["subres"][path.Label] : table["self"];
        var type = node["__type__"];
        foreach (var deserializer in _deserializers)
        {
            if (deserializer.MatchType(type))
            {
                var input = new SerializeInputToml(subAssets, node);
                return deserializer.Deserialize(input, resultMapper);
            }
        }
        throw new Exception($"Can't deserialize {path}");
    }
}

public class SerializeInputToml(IAssetsSource assets, TomlNode root) : ISerializeInput
{
    public Rect2 ReadRect2(string name)
    {
        var loc = root[name];
        return new Rect2(
            (float)loc["x"].AsFloat.Value,
            (float)loc["y"].AsFloat.Value,
            (float)loc["w"].AsFloat.Value,
            (float)loc["h"].AsFloat.Value
        );
    }

    public string ReadString(string name)
    {
        var loc = root[name];
        return loc.AsString.Value;
    }

    public IAsset<T> ReadSubRes<T>(string name)
    {
        var loc = root[name];
        return assets.Load<T>(loc["path"].AsString.Value);
    }
}
