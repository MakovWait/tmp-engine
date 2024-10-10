namespace Tmp.Core.Redot;

public sealed class StateValue<T>(T initial) : IStateValue
{
    private T _value = initial;
    
    public T Get() => _value;
    
    public void Set(T value) => _value = value;
    
    public static implicit operator T(StateValue<T> self) => self._value;
}

public sealed class StateValueOptional<T> : IStateValue
{
    private T? _value;
    
    public T? Get() => _value;
    
    public void Set(T value) => _value = value;
    
    public static implicit operator T?(StateValueOptional<T> self) => self._value;
}

internal interface IStateValue
{
    
}
