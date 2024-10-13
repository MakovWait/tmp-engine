using Tmp.Math;
using Tommy;

namespace Tmp.Resource.Format.Text;

public class ResourceLoaderText : IResourceLoader
{
    private readonly List<IResourceDeserializer> _deserializers = [];

    public void AddDeserializer(IResourceDeserializer deserializer)
    {
        _deserializers.Add(deserializer);
    }
    
    public bool MatchPath(ResourcePath path)
    {
        return path.Extension == "gres";
    }

    public T Load<T>(ResourcePath path, IResourcesSource subResources)
    {
        using var reader = File.OpenText(path.FilePath);
        var table = TOML.Parse(reader);
        var node = path.HasLabel ? table["subres"][path.Label] : table["self"];
        var type = node["__type__"];
        foreach (var deserializer in _deserializers)
        {
            if (deserializer.MatchType(type) && deserializer is IResourceDeserializer<T> deserialize)
            {
                return deserialize.From(new SerializeInputToml(subResources, node));
            }
        }
        throw new Exception($"Can't deserialize {path}");
    }
}

public class SerializeInputToml(IResourcesSource resources, TomlNode root) : ISerializeInput
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

    public Res<T> ReadSubRes<T>(string name)
    {
        var loc = root[name];
        return resources.Load<T>(loc["path"].AsString.Value);
    }
}
