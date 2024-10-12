namespace Tmp.Resource.Format;

public interface IResourceDeserializer
{
    bool MatchType(string type);
}

public interface IResourceDeserializer<out T> : IDeserialize<T>, IResourceDeserializer
{
}