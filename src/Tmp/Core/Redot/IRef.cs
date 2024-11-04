namespace Tmp.Core.Redot;

public interface IRef<out T> : IScalar<T>;

public interface IRefMut<T> : IRef<T>
{
    void Set(T value);
}

internal class Ref<T>(T initial = default!) : IRefMut<T>
{
    private T _value = initial;
    
    public T Get()
    {
        return _value;
    }

    public void Set(T value)
    {
        _value = value;
    }
}
