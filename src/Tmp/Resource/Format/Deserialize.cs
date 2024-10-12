using Tmp.Math;

namespace Tmp.Resource.Format;

public interface IDeserialize<out T>
{
    T From(ISerializeInput input);
}

public interface ISerializeInput
{
    Rect2 ReadRect2(string name);
    
    string ReadString(string name);

    Res<T> ReadSubRes<T>(string name);
}
