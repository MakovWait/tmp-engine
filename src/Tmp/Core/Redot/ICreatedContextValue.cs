namespace Tmp.Core.Redot;

public interface ICreatedContextValue
{
    
}

public interface ICreatedContextValue<T>
{
    T Get();

    void Replace(T val);
}