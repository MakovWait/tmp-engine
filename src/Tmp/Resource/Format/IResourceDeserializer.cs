using Tmp.Resource.Util;

namespace Tmp.Resource.Format;

public interface IResourceDeserializer
{
    bool MatchType(string type);
    
    T Deserialize<T>(ISerializeInput input, IResultMapper<T> resultMapper);
}

public interface IResourceDeserializer<out T> : IDeserialize<T>, IResourceDeserializer
{
}