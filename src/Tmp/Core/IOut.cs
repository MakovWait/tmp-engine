using System.Diagnostics;

namespace Tmp.Core;

public interface IOut<T> : IScalar<T>
{
    void Init(T value);
}

internal class Out<T> : IOut<T>
{
    public T Value { get; private set; }
    private bool _initialized;

    public void Init(T value)
    {
        Debug.Assert(!_initialized);
        Value = value;
        _initialized = true;
    }
}
