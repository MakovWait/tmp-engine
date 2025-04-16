namespace Tmp.Core;

public interface IRef<out T> : IScalar<T>;

public interface IRefMut<T> : IRef<T>
{
    new T Value { get; set; }
}

internal class Ref<T>(T initial = default!) : IRefMut<T>
{
    public T Value { get; set; } = initial;
}
