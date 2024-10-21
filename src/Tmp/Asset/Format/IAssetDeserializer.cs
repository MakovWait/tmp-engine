using Tmp.Asset.Util;

namespace Tmp.Asset.Format;

public interface IAssetDeserializer
{
    bool MatchType(string type);
    
    T Deserialize<T>(ISerializeInput input, IResultMapper<T> resultMapper);
}

public interface IAssetDeserializer<out T> : IDeserialize<T>, IAssetDeserializer
{
}