namespace Tmp.Core.Redot;

public sealed class ExportedVar<T>(T initial) : IExportedVar
{
    private T _value = initial;
    
    public T Get() => _value;
    
    public void Set(T value) => _value = value;
    
    public static implicit operator T(ExportedVar<T> self) => self._value;
}

public sealed class ExportedVarOptional<T> : IExportedVar
{
    private T? _value;
    
    public T? Get() => _value;
    
    public void Set(T value) => _value = value;
    
    public static implicit operator T?(ExportedVarOptional<T> self) => self._value;
}

internal interface IExportedVar
{
    
}
